using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataStore
{
    [Serializable]
    public class NaoWrist : ISerializable
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

        public NaoWrist(SerializationInfo info, StreamingContext ctxt)
        {
            this.Yaw = (double)info.GetValue("Yaw", typeof(double));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Yaw", Yaw);
        }
    }
}