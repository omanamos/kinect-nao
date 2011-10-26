using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NaoKinectTest.HumanModel;

namespace NaoKinectTest
{
    class NaoElbow
    {
        public double YAW_MIN = -2.0856;
        public double YAW_MAX = 2.0856;
        public double YAW_RANGE;

        public double ROLL_MIN = 0.0349;
        public double ROLL_MAX = 1.5446;
        public double ROLL_RANGE;

        private double yaw;
        private double roll;
        private bool leftSide;

        private NaoElbow(){}

        public NaoElbow(double humanYaw, double humanRoll, bool leftSide)
        {
            this.leftSide = leftSide;
            if (leftSide)
            {
                double tmp = ROLL_MIN;
                ROLL_MIN = -ROLL_MAX;
                ROLL_MAX = -tmp;
            }
            else
            {
                humanRoll = Math.Abs(humanRoll - Math.PI);
                double tmp = YAW_MIN;
                YAW_MIN = -YAW_MAX;
                YAW_MAX = -tmp;
            }
            YAW_RANGE = YAW_MAX - YAW_MIN;
            ROLL_RANGE = ROLL_MAX - ROLL_MIN;
            this.yaw = scaleYaw(humanYaw);
            this.roll = scaleRoll(humanRoll);
        }

        public double getYaw()
        {
            return this.yaw;
        }

        public double getRoll()
        {
            return this.roll;
        }

        private double scaleYaw(double humanYaw)
        {
            double rtn = Util.clamp(((humanYaw - HumanShoulder.ROLL_MIN) / HumanShoulder.ROLL_RANGE) * YAW_RANGE + YAW_MIN, YAW_MIN, YAW_MAX);
            return leftSide ? rtn : -rtn;
        }

        private double scaleRoll(double humanRoll)
        {
            return Util.clamp(((humanRoll - HumanElbow.YAW_MIN) / HumanElbow.YAW_RANGE) * ROLL_RANGE + ROLL_MIN, ROLL_MIN, ROLL_MAX);
        }
    }
}
