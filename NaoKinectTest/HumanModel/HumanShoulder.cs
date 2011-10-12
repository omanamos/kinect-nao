using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using Microsoft.Research.Kinect.Nui;

namespace NaoKinectTest.HumanModel
{
    class HumanShoulder
    {
        // TODO: Change these values
        public static readonly double PITCH_MIN = -2.0857;
        public static readonly double PITCH_MAX = 2.0857;
        public static readonly double PITCH_RANGE = PITCH_MAX - PITCH_MIN;

        // TODO: Change these values
        public static readonly double YAW_MIN = -2.0857;
        public static readonly double YAW_MAX = 2.0857;
        public static readonly double YAW_RANGE = YAW_MAX - YAW_MIN;

        // TODO: Change these values
        public static readonly double ROLL_MIN = -1.3265;
        public static readonly double ROLL_MAX = 0.3142;
        public static readonly double ROLL_RANGE = ROLL_MAX - ROLL_MIN;

        public HumanShoulder(Joint shoulderCenter, Joint spine, Joint shoulder, Joint elbow)
        {
            // Compute the roll, pitch and yaw of the shoulder
            compute(shoulderCenter, spine, shoulder, elbow);
        }

        private void compute(Joint shoulderCenter, Joint spine, Joint shoulder, Joint elbow)
        {
            Vector3D center = Util.vectorFromJoint(shoulderCenter);
            Vector3D spineVec = Util.vectorFromJoint(spine);
            Vector3D centerToSpine = spineVec - center;

            Vector3D shoulderVec = Util.vectorFromJoint(shoulder);
            Vector3D centerToShoulder = shoulderVec - center;

            Vector3D chestPlaneNormal = Vector3D.CrossProduct(centerToShoulder, centerToSpine);

            Vector3D elbowVec = Util.vectorFromJoint(elbow);
            Vector3D shoulderToElbow = elbowVec - shoulderVec;

            Vector3D pitchVector = Util.projectToPlane(chestPlaneNormal, shoulderToElbow);

            Vector3D yawVector = Util.projectToPlane(centerToSpine, shoulderToElbow);

            double shoulderPitch = Vector3D.AngleBetween(pitchVector, centerToSpine);
            // AngleBetween returns between [0,180] so we need to add the sign appropriately
            if (Vector3D.DotProduct(pitchVector, centerToShoulder) < 0)
            {
                shoulderPitch = -shoulderPitch;
            }
            
            double shoulderYaw = Vector3D.AngleBetween(yawVector, centerToShoulder);
            if (Vector3D.DotProduct(yawVector, chestPlaneNormal) > 0)
            {
                shoulderYaw = -shoulderYaw;
            }

            Pitch = shoulderPitch;
            Yaw = shoulderYaw;
        }

        public double Yaw { get; set; }

        public double Pitch { get; set; }
    }
}
