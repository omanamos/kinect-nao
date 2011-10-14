using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NaoKinectTest.HumanModel;

namespace NaoKinectTest
{
    class NaoElbow
    {
        public static readonly double YAW_MIN = -2.0856;
        public static readonly double YAW_MAX = 2.0856;
        public static readonly double YAW_RANGE = YAW_MAX - YAW_MIN;

        public double ROLL_MIN = 0.0349;
        public double ROLL_MAX = 1.5446;
        public double ROLL_RANGE;

        private double yaw;
        private double roll;

        private NaoElbow(){}

        public NaoElbow(double humanYaw, double humanRoll, bool leftSide)
        {
            if (leftSide)
            {
                ROLL_MIN = -ROLL_MAX;
                ROLL_MAX = -ROLL_MIN;

            }
            else
            {
                humanRoll = -humanRoll;
            }
            ROLL_RANGE = ROLL_MAX - ROLL_MIN;
            this.yaw = scalePitch(humanYaw);
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

        private double scalePitch(double humanPitch)
        {
            return Util.clamp(((humanPitch - HumanShoulder.PITCH_MIN) / HumanShoulder.PITCH_RANGE) * YAW_RANGE + YAW_MIN, YAW_MIN, YAW_MAX);
        }

        private double scaleRoll(double humanRoll)
        {
            return Util.clamp(((humanRoll - HumanElbow.YAW_MIN) / HumanElbow.YAW_RANGE) * ROLL_RANGE + ROLL_MIN, ROLL_MIN, ROLL_MAX);
        }
    }
}
