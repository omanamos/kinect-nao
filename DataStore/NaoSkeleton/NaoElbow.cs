using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataStore
{
    [Serializable]
    public class NaoElbow : ISerializable
    {
        public static readonly float YAW_MIN = -2.0856f;
        public static readonly float YAW_MAX = 2.0856f;
        public static readonly float YAW_RANGE = YAW_MAX - YAW_MIN;

        public static readonly float ROLL_MIN = 0.0349f;
        public static readonly float ROLL_MAX = 1.5446f;
        public static readonly float ROLL_RANGE = ROLL_MAX - ROLL_MIN;

        public float Yaw { get; private set; }
        public float Roll { get; private set; }

        private NaoElbow() { }

        public NaoElbow(float yaw, float roll, bool leftSide)
        {
            if (Util.isWithinRange(yaw, YAW_MIN, YAW_MAX))
            {
                this.Yaw = yaw;
            }
            else
            {
                throw new ArgumentException(yaw + " is outside of range (" + YAW_MIN + ", " + YAW_MAX + ")");
            }

            float roll_min;
            float roll_max;
            if (leftSide)
            {
                roll_min = -1 * ROLL_MAX;
                roll_max = -1 * ROLL_MIN;
            }
            else
            {
                roll_min = ROLL_MIN;
                roll_max = ROLL_MAX;
            }
            roll = Util.clamp(roll, roll_min, roll_max);
            if (Util.isWithinRange(roll, roll_min, roll_max))
            {
                this.Roll = roll;
            }
            else
            {
                throw new ArgumentException(roll + " is outside of range (" + roll_min + ", " + roll_max + ")");
            }
        }

        public NaoElbow(SerializationInfo info, StreamingContext ctxt)
        {
            this.Yaw = (float)info.GetValue("Yaw", typeof(float));
            this.Roll = (float)info.GetValue("Roll", typeof(float));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Yaw", Yaw);
            info.AddValue("Roll", Roll);
        }
    }
}
