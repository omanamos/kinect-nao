using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MotionRecorder;
using KinectInterface;
using DataStore;
using System.Diagnostics;

namespace MotionRecorder
{
    public class KinectSequencer
    {
        // Kinect
        IKinect kinect;
        // Our visualizer thread
        SkeletonVisualizer viz = new SkeletonVisualizer();
        // Our timer for creating timestamps
        Stopwatch stopwatch = new Stopwatch();
        // The contents of our current recording
        public List<Tuple<long, HumanSkeleton>> currentRecordingSequence = new List<Tuple<long, HumanSkeleton>>();

        long maxTimeWindow = 5000; // 5 seconds

        public bool SequenceAvailable { get; private set; }
        
        public KinectSequencer()
        {
            kinect = new RealKinect();
            kinect.NewData += new NewDataEventHandler(kinect_NewData);
            stopwatch.Start();
            SequenceAvailable = false;
        }

        void kinect_NewData(object sender, NewDataEventArgs e)
        {
            HumanSkeleton skel = new HumanSkeleton(arrayToList(e.data));
            viz.showSkeleton(skel);
            lock (currentRecordingSequence)
            {
                if (currentRecordingSequence.Count == 0)
                {
                    currentRecordingSequence.Add(Tuple.Create(stopwatch.ElapsedMilliseconds, skel));
                }
                else
                {
                    long startMs = currentRecordingSequence[0].Item1;
                    long endMs = stopwatch.ElapsedMilliseconds;
                    long msDelta = (endMs - startMs);
                    if (msDelta > maxTimeWindow)
                    {
                        // We need to remove elements from the front until we are within the delta
                        while ((endMs - startMs) > maxTimeWindow && currentRecordingSequence.Count > 0)
                        {
                            currentRecordingSequence.RemoveAt(0);
                            if (currentRecordingSequence.Count > 0)
                                startMs = currentRecordingSequence[0].Item1;
                        }
                    }
                    currentRecordingSequence.Add(Tuple.Create(endMs, skel));
                    startMs = currentRecordingSequence[0].Item1;
                    msDelta = (endMs - startMs);
                    if (msDelta > maxTimeWindow / 2 && currentRecordingSequence.Count > (maxTimeWindow / 30) / 2)
                    {
                        // We have a valid capture if we have at least half our max time window
                        SequenceAvailable = true;
                    }
                }
            }
        }

        public ActionSequence<HumanSkeleton> getLatestSequence()
        {
            if (SequenceAvailable)
            {
                SequenceAvailable = false;
                lock (currentRecordingSequence)
                {
                    return new ActionSequence<HumanSkeleton>(
                        new List<HumanSkeleton>(currentRecordingSequence.Select(e => e.Item2)));
                }
            }
            else
            {
                return null;
            }
        }

        public void clearSequenceBuffer()
        {
            lock (currentRecordingSequence)
            {
                currentRecordingSequence.Clear();
            }
        }

        public void stop()
        {
            viz.stop();
        }

        private List<double[]> arrayToList(double[,] arry)
        {
            List<double[]> lst = new List<double[]>();
            for (int i = 0; i < arry.GetLength(0); i++)
            {
                double[] d = { arry[i, 0], arry[i, 1], arry[i, 2] };
                lst.Add(d);
            }
            return lst;
        }
    }
}
