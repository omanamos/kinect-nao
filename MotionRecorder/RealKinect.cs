using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;

namespace MotionRecorder
{
    class RealKinect : IKinect
    {
        public event NewDataEventHandler NewData;

        Runtime nui;
        Dictionary<int, JointID> jointOrdering;
        public RealKinect()
        {
            nui = new Runtime();
            nui.Initialize(RuntimeOptions.UseColor | RuntimeOptions.UseSkeletalTracking);

            //Must set to true and set after call to Initialize
            nui.SkeletonEngine.TransformSmooth = true;

            //Use to transform and reduce jitter
            var parameters = new TransformSmoothParameters
            {
                Smoothing = 1.0f,
                Correction = 0.1f,
                Prediction = 0.1f,
                JitterRadius = 0.05f,
                MaxDeviationRadius = 0.05f
            };

            jointOrdering = new Dictionary<int, JointID>();
            jointOrdering[0] = JointID.FootRight;
            jointOrdering[1] = JointID.AnkleRight;
            jointOrdering[2] = JointID.KneeRight;
            jointOrdering[3] = JointID.HipRight;
            jointOrdering[4] = JointID.HipCenter;
            jointOrdering[5] = JointID.Spine;
            jointOrdering[6] = JointID.ShoulderCenter;
            jointOrdering[7] = JointID.ShoulderRight;
            jointOrdering[8] = JointID.ElbowRight;
            jointOrdering[9] = JointID.WristRight;
            jointOrdering[10] = JointID.HandRight;
            jointOrdering[11] = JointID.Head;
            jointOrdering[12] = JointID.HandLeft;
            jointOrdering[13] = JointID.WristLeft;
            jointOrdering[14] = JointID.ElbowLeft;
            jointOrdering[15] = JointID.ShoulderLeft;
            jointOrdering[16] = JointID.HipLeft;
            jointOrdering[17] = JointID.KneeLeft;
            jointOrdering[18] = JointID.AnkleLeft;
            jointOrdering[19] = JointID.FootLeft;

            nui.SkeletonEngine.SmoothParameters = parameters;

            nui.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(nui_SkeletonFrameReady);
            //nui.VideoFrameReady += new EventHandler<ImageFrameReadyEventArgs>(nui_VideoFrameReady);
            nui.NuiCamera.ElevationAngle = 8;
        }

        void nui_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            SkeletonFrame allSkeletons = e.SkeletonFrame;

            //get the first tracked skeleton
            SkeletonData skeleton = (from s in allSkeletons.Skeletons
                                     where s.TrackingState == SkeletonTrackingState.Tracked
                                     select s).FirstOrDefault();
            if (skeleton != null)
            {
                double[,] data = new double[20, 3];
                for (int i = 0; i < 20; i++)
                {
                    Vector position = skeleton.Joints[jointOrdering[i]].Position;
                    data[i, 0] = position.X;
                    data[i, 1] = position.Y;
                    data[i, 2] = position.Z;
                }

                OnNewData(new NewDataEventArgs(data));
            }
        }

        private void OnNewData(NewDataEventArgs newDataEventArgs)
        {
            if (NewData != null)
                NewData(this, newDataEventArgs);
        }




        public void start()
        {
        }

        public void stop()
        {
        }
    }
}
