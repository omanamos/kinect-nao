using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataStore
{
    public class NaoWrist
    {
        public static readonly double YAW_MIN = -1.8238;
        public static readonly double YAW_MAX = 1.8238;
        public static readonly double YAW_RANGE = YAW_MAX - YAW_MIN;

        public double Yaw { get; private set; }

        private NaoWrist(){}

        public NaoWrist(double yaw)
        {
            if (Util.isWithinRange(yaw, YAW_MIN, YAW_MAX))
            {
                this.Yaw = yaw;
            }
            else
            {
                throw new ArgumentException(yaw + " is outside of range (" + YAW_MIN + ", " + YAW_MAX + ")");
            }
        }
    }
}
