using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataStore
{
    [Serializable]
    public class NaoShoulder : ISerializable
    {
        public static readonly double PITCH_MIN = -2.0856;
        public static readonly double PITCH_MAX = 2.0856;
        public static readonly double PITCH_RANGE = PITCH_MAX - PITCH_MIN;

        public readonly double ROLL_MIN;
        public readonly double ROLL_MAX;
        public readonly double ROLL_RANGE;

        public double Pitch { get; private set; }
        public double Roll { get; private set; }

        private NaoShoulder(){}

        public NaoShoulder(double pitch, double roll, bool leftSide)
        {
            if (leftSide)
            {
                ROLL_MIN = -0.3142;
                ROLL_MAX = 1.3265;
            }
            else
            {
                ROLL_MIN = -1.3265;
                ROLL_MAX = 0.3142;
            }
            ROLL_RANGE = ROLL_MAX - ROLL_MIN;

            if (Util.isWithinRange(pitch, PITCH_MIN, PITCH_MAX))
            {
                this.Pitch = pitch;
            }
            else
            {
                throw new ArgumentException(pitch + " is outside of range (" + PITCH_MIN + ", " + PITCH_MAX + ")");
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

        public NaoShoulder(SerializationInfo info, StreamingContext ctxt)
        {
            this.Pitch = (double)info.GetValue("Pitch", typeof(double));
            this.Roll = (double)info.GetValue("Roll", typeof(double));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Pitch", Pitch);
            info.AddValue("Roll", Roll);
        }
    }
}
