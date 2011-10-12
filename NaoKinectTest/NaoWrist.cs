using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NaoKinectTest
{
    class NaoWrist
    {
        public static sealed double YAW_MIN = -1.8238;
        public static sealed double YAW_MAX = 1.8238;
        public static sealed double YAW_RANGE = YAW_MAX - YAW_MIN;

        private double yaw;

        private NaoWrist(){}

        public NaoWrist(double humanYaw)
        {
            this.yaw = scaleYaw(humanYaw);
        }

        public double getYaw()
        {
            return this.getYaw();
        }

        private double scaleYaw(double humanYaw)
        {
            return (humanYaw - HumanShoulder.YAW_MIN / HumanShoulder.YAW_RANGE) * YAW_RANGE;
        }
    }
}
