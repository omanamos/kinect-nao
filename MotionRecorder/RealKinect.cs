using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;

namespace MotionRecorder
{
    public class RealKinect : IKinect
    {
        public event NewDataEventHandler NewData;

        Runtime nui;
        public readonly static Dictionary<int, JointID> JOINT_ORDERING = initOrdering();

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

            nui.SkeletonEngine.SmoothParameters = parameters;

            nui.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(nui_SkeletonFrameReady);
            //nui.VideoFrameReady += new EventHandler<ImageFrameReadyEventArgs>(nui_VideoFrameReady);
            nui.NuiCamera.ElevationAngle = 8;
        }

        private static Dictionary<int, JointID> initOrdering()
        {
            Dictionary<int, JointID> rtn = new Dictionary<int, JointID>();
            rtn[0] = JointID.FootRight;
            rtn[1] = JointID.AnkleRight;
            rtn[2] = JointID.KneeRight;
            rtn[3] = JointID.HipRight;
            rtn[4] = JointID.HipCenter;
            rtn[5] = JointID.Spine;
            rtn[6] = JointID.ShoulderCenter;
            rtn[7] = JointID.ShoulderRight;
            rtn[8] = JointID.ElbowRight;
            rtn[9] = JointID.WristRight;
            rtn[10] = JointID.HandRight;
            rtn[11] = JointID.Head;
            rtn[12] = JointID.HandLeft;
            rtn[13] = JointID.WristLeft;
            rtn[14] = JointID.ElbowLeft;
            rtn[15] = JointID.ShoulderLeft;
            rtn[16] = JointID.HipLeft;
            rtn[17] = JointID.KneeLeft;
            rtn[18] = JointID.AnkleLeft;
            rtn[19] = JointID.FootLeft;
            return rtn;
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
                    Vector position = skeleton.Joints[JOINT_ORDERING[i]].Position;
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
