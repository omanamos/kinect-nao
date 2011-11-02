using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataStore
{
    public class NaoElbow
    {
        public static readonly double YAW_MIN = -2.0856;
        public static readonly double YAW_MAX = 2.0856;
        public static readonly double YAW_RANGE = YAW_MAX - YAW_MIN;

        public static readonly double ROLL_MIN = 0.0349;
        public static readonly double ROLL_MAX = 1.5446;
        public static readonly double ROLL_RANGE = ROLL_MAX - ROLL_MIN;

        public double Yaw { get; private set; }
        public double Roll { get; private set; }

        private NaoElbow(){}

        public NaoElbow(double yaw, double roll)
        {
            if (Util.isWithinRange(yaw, YAW_MIN, YAW_MAX))
            {
                this.Yaw = yaw;
            }
            else
            {
                throw new ArgumentException(yaw + " is outside of range (" + YAW_MIN + ", " + YAW_MAX + ")");
            }

            if (Util.isWithinRange(roll, ROLL_MIN, ROLL_MAX))
            {
                this.Roll = roll;
            }
            else
            {
                throw new ArgumentException(roll + " is outside of range (" + ROLL_MIN + ", " + ROLL_MAX + ")");
            }
        }
    }
}
