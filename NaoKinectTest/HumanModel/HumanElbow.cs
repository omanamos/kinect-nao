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
        public static readonly double YAW_MIN = -2.0857;
        public static readonly double YAW_MAX = 2.0857;
        public static readonly double YAW_RANGE = YAW_MAX - YAW_MIN;

        private double yaw;

        public HumanElbow(Joint shoulder, Joint elbow, Joint wrist)
        {
            compute(shoulder, elbow, wrist);
        }

        private void compute(Joint shoulder, Joint elbow, Joint wrist)
        {
            // TODO: fill this in
        }
    }
}
