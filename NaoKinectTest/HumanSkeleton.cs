using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

using Microsoft.Research.Kinect.Nui;

using Emgu.CV;

namespace NaoKinectTest
{
    class HumanSkeleton
    {
        private JointsCollection kinectJoints;
        private enum sides {LEFT, RIGHT};
        private enum side_joints { SHOULDER, ELBOW, WRIST };
        
        // Initialize a new HumanSkeleton model with the given observed joints
        public HumanSkeleton(JointsCollection kinectJoints)
        {
            this.kinectJoints = kinectJoints;
            computeShoulderAngles(sides.LEFT);
            computeShoulderAngles(sides.RIGHT);
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

        private void computeShoulderAngles(sides side)
        {
            Vector3D center = vectorFromJoint(JointID.ShoulderCenter);
            Vector3D spine = vectorFromJoint(JointID.Spine);
            Vector3D centerToSpine = spine - center;

            Vector3D shoulder = vectorFromJoint(getJointIdForSide(side, side_joints.SHOULDER));
            Vector3D centerToShoulder = shoulder - center;
            
            Vector3D chestPlaneNormal = Vector3D.CrossProduct(centerToShoulder, centerToSpine);

            Vector3D elbow = vectorFromJoint(getJointIdForSide(side, side_joints.ELBOW));
            Vector3D shoulderToElbow = elbow - shoulder;

            Vector3D pitchVector = projectToPlane(chestPlaneNormal, shoulderToElbow);

            Vector3D yawVector = projectToPlane(centerToSpine, shoulderToElbow);
            
            double shoulderPitch = Vector3D.AngleBetween(pitchVector, centerToSpine);
            Console.WriteLine("ShoulderPitch: " + shoulderPitch);
            // AngleBetween returns between [0,180] so we need to add the sign appropriately
            if (Vector3D.DotProduct(pitchVector, centerToShoulder) < 0)
            {
                shoulderPitch = -shoulderPitch;
            }
            Console.WriteLine("SignedShoulderPitch: " + shoulderPitch);
            Console.WriteLine("AddedShoulderPitch: " + shoulderPitch);
            double shoulderYaw = Vector3D.AngleBetween(yawVector, centerToShoulder);
            if (Vector3D.DotProduct(yawVector, chestPlaneNormal) > 0)
            {
                shoulderYaw = -shoulderYaw;
            }

            /* Shoulder Roll
            // We will rotate the spineToCenter (up) vector to be perpendicular to the shoulderToElbow frame
            Vector3D spineToCenter = -centerToSpine;
            

            Quaternion q = new Quaternion(rotationAxis, rotationAngle);
            Matrix3D m = Matrix3D.Identity;
            m.Rotate(q);
            Vector3D upInUpperArmFrame = Vector3D.Multiply(spineToCenter, m);
            */

            switch (side)
            {
                case sides.LEFT:
                    LeftShoulderPitch = shoulderPitch;
                    LeftShoulderYaw = shoulderYaw;
                    break;
                case sides.RIGHT:
                    RightShoulderPitch = shoulderPitch;
                    RightShoulderYaw = shoulderYaw;
                    break;
            }

        }

        private Vector3D projectToPlane(Vector3D planeNormal, Vector3D v)
        {
            return v - (Vector3D.DotProduct(v, planeNormal) * planeNormal);
        }

        private Vector3D vectorFromJoint(JointID id)
        {
            Vector3D v = new Vector3D();
            v.X = kinectJoints[id].Position.X;
            v.Y = kinectJoints[id].Position.Y;
            v.Z = kinectJoints[id].Position.Z;
            return v;
        }

        private void computeShoulderYaw()
        {

        }

        private void computeShoulderRoll()
        {

        }

        private void computeElbowPitch()
        {
        }

        public double LeftShoulderPitch { get; set; }

        public double RightShoulderPitch { get; set; }

        public double RightShoulderYaw { get; set; }

        public double LeftShoulderYaw { get; set; }
    }
}
