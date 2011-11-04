using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataStore;
using System.Timers;
using MotionRecorder;

namespace Recognizer
{
    public class HumanActionRecognizer
    {
        double RADIAN_THRESHOLD = 0.2;
        double METER_THRESHOLD = 0.01;
        Timer t;
        HMMRecognizer rec;

        // start thread creating actionSequences
        KinectSequencer sequencer;
        private  bool waitingForStableSequence = true;

        public HumanActionRecognizer(string path, int pollInterval = 33)
        {
            rec = new HMMRecognizer(path);

            sequencer = new KinectSequencer();

            t = new Timer(pollInterval);
            t.Elapsed += new ElapsedEventHandler(t_Elapsed);
        }

        private void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            recognize();
        }

        private void recognize()
        {
            // if there is a sequence available
            if (sequencer.SequenceAvailable)
            {
                // grab the latest sequence
                ActionSequence<HumanSkeleton> seq = sequencer.getLatestSequence();
                if (waitingForStableSequence)
                {
                    bool stable = Util.isStableSequence(seq.toArray(), METER_THRESHOLD); // 1cm deviation
                    stable = stable && Util.isStableSequence(seq.toArray(true), RADIAN_THRESHOLD);
                    // If we have stood still for the time window,
                    if (stable)
                    {
                        Console.WriteLine("*********************************stable******");
                        waitingForStableSequence = false;
                        if (RecordingReady != null)
                            RecordingReady(this, new EventArgs());
                    }
                }
                else
                {
                    if (!recording)
                    {
                        // start recording once we get an unstable sequence
                        bool stable = Util.isStableSequence(seq.toArray(), METER_THRESHOLD);
                        stable = stable && Util.isStableSequence(seq.toArray(true), RADIAN_THRESHOLD);
                        if (!stable)
                        {
                            // Clear the stable data and begin recording the motion
                            sequencer.clearSequenceBuffer();
                            recording = true;
                            if (RecordingStart != null)
                                RecordingStart(this, new EventArgs());
                        }
                    } 
                    else
                    {                          
                        // perform recognition
                        Tuple<string, double> guess = rec.recognizeAction(seq);
                        Console.WriteLine("Action: " + guess.Item1 + " L: " + guess.Item2);
                        if (Recognition != null)
                            Recognition(this, new RecognitionEventArgs(guess.Item1, guess.Item2));
                        waitingForStableSequence = true;
                        recording = false;
                        if (RecordingStop != null)
                            RecordingStop(this, new EventArgs());
                    }
                }
            }
        }

        public void start()
        {
            t.Start();
        }

        public void stop()
        {
            t.Stop();
        }

        public void exit()
        {
            sequencer.stop();
        }

        public delegate void RecordingEventHandler(object sender, EventArgs e);
        public delegate void RecognitionEventHandler(object sender, RecognitionEventArgs e);

        public event RecordingEventHandler RecordingReady;
        public event RecordingEventHandler RecordingStart;
        public event RecordingEventHandler RecordingStop;
        public event RecognitionEventHandler Recognition;
        private bool recording;
    }

    public class RecognitionEventArgs : EventArgs
    {
        public string Class { get; private set; }
        public double Likelihood { get; private set; }

        public RecognitionEventArgs(string cls, double l)
        {
            Class = cls;
            Likelihood = l;
        }
    }
}
