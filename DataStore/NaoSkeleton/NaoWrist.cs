using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataStore
{
    public class NaoWrist
    {
        public static readonly float YAW_MIN = -1.8238f;
        public static readonly float YAW_MAX = 1.8238f;
        public static readonly float YAW_RANGE = YAW_MAX - YAW_MIN;

        public float Yaw { get; private set; }

        private NaoWrist(){}

        public NaoWrist(float yaw)
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
