using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

using Microsoft.Research.Kinect.Nui;

using Emgu.CV;

namespace NaoKinectTest.HumanModel
{
    class HumanSkeleton
    {
        private JointsCollection kinectJoints;
        private enum sides {LEFT, RIGHT};
        private enum side_joints { SHOULDER, ELBOW, WRIST };

        private HumanShoulder leftShoulder;
        private HumanShoulder rightShoulder;
        private HumanElbow leftElbow;
        private HumanElbow rightElbow;

        // Initialize a new HumanSkeleton model with the given observed joints
        public HumanSkeleton(JointsCollection kinectJoints)
        {
            this.kinectJoints = kinectJoints;
            Joint shoulderCenter = kinectJoints[JointID.ShoulderCenter];
            Joint spine = kinectJoints[JointID.Spine];
            leftShoulder = new HumanShoulder(shoulderCenter, spine, 
                kinectJoints[JointID.ShoulderLeft], 
                kinectJoints[JointID.ElbowLeft],
                kinectJoints[JointID.WristLeft]);

            LeftShoulderPitch = leftShoulder.Pitch;
            LeftShoulderYaw = leftShoulder.Yaw;
            LeftShoulderRoll = leftShoulder.Roll;

            rightShoulder = new HumanShoulder(shoulderCenter, spine,
                kinectJoints[JointID.ShoulderRight],
                kinectJoints[JointID.ElbowRight],
                kinectJoints[JointID.WristRight]);

            RightShoulderPitch = rightShoulder.Pitch;
            RightShoulderYaw = rightShoulder.Yaw;
            RightShoulderRoll = rightShoulder.Roll;

            leftElbow = new HumanElbow(
                kinectJoints[JointID.ShoulderLeft], 
                kinectJoints[JointID.ElbowLeft], 
                kinectJoints[JointID.WristLeft]);

            LeftElbowRoll = leftElbow.Yaw;

            rightElbow = new HumanElbow(
                kinectJoints[JointID.ShoulderRight],
                kinectJoints[JointID.ElbowRight],
                kinectJoints[JointID.WristRight]);

            RightElbowRoll = rightElbow.Yaw;
        }

        private JointID getJointIdForSide(sides side, side_joints joint)
        {
            switch (side)
            {
                case sides.LEFT:
                    switch (joint)
                    {
                        case side_joints.ELBOW:
                            return JointID.ElbowLeft;
                        case side_joints.SHOULDER:
                            return JointID.ShoulderLeft;
                        case side_joints.WRIST:
                            return JointID.WristLeft;
                        default:
                            // Invalid
                            return JointID.Count;
                    }
                case sides.RIGHT:
                {
                    switch (joint)
                    {
                        case side_joints.ELBOW:
                            return JointID.ElbowRight;
                        case side_joints.SHOULDER:
                            return JointID.ShoulderRight;
                        case side_joints.WRIST:
                            return JointID.WristRight;
                        default:
                            // Invalid
                            return JointID.Count;
                    }
                }
                default:
                    return JointID.Count;
            }
            
        }

        public double LeftShoulderPitch { get; set; }

        public double RightShoulderPitch { get; set; }

        public double RightShoulderYaw { get; set; }

        public double LeftShoulderYaw { get; set; }

        public double RightShoulderRoll { get; set; }

        public double LeftShoulderRoll { get; set; }

        public double LeftElbowRoll { get; set; }

        public double RightElbowRoll { get; set; }
    }
}
