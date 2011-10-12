using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NaoKinectTest.HumanModel;

namespace NaoKinectTest
{
    class NaoElbow
    {
        public static readonly double YAW_MIN = -2.0857;
        public static readonly double YAW_MAX = 2.0857;
        public static readonly double YAW_RANGE = YAW_MAX - YAW_MIN;

        public static readonly double ROLL_MIN = -1.3265;
        public static readonly double ROLL_MAX = 0.3142;
        public static readonly double ROLL_RANGE = ROLL_MAX - ROLL_MIN;

        private double yaw;
        private double roll;

        private NaoElbow(){}

        public NaoElbow(double humanYaw, double humanRoll)
        {
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
            return (humanPitch - HumanElbow.YAW_MIN / HumanElbow.YAW_RANGE) * YAW_RANGE;
        }

        private double scaleRoll(double humanRoll)
        {
            return (humanRoll - HumanShoulder.PITCH_MIN / HumanShoulder.PITCH_RANGE) * ROLL_RANGE;
        }
    }
}
