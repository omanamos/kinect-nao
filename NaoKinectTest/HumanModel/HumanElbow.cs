using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using Microsoft.Research.Kinect.Nui;

namespace NaoKinectTest.HumanModel
{
    class HumanElbow
    {
        // TODO: Change these values
        public static readonly double YAW_MIN = 0.8;
        public static readonly double YAW_MAX = 3.14;
        public static readonly double YAW_RANGE = YAW_MAX - YAW_MIN;

        private double yaw;

        public HumanElbow(Joint shoulder, Joint elbow, Joint wrist)
        {
            compute(shoulder, elbow, wrist);
        }

        private void compute(Joint shoulder, Joint elbow, Joint wrist)
        {
            Vector3D shoulderVec = Util.vectorFromJoint(shoulder);
            Vector3D elbowVec = Util.vectorFromJoint(elbow);
            Vector3D wristVec = Util.vectorFromJoint(wrist);

            Vector3D elbowToShoulder = shoulderVec - elbowVec;
            Vector3D elbowToWrist = wristVec - elbowVec;

            Yaw = Util.degToRad(Vector3D.AngleBetween(elbowToWrist, elbowToShoulder));
        }

        public double Yaw { get; set; }
    }
}
