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
        public static readonly float PITCH_MIN = -2.0856f;
        public static readonly float PITCH_MAX = 2.0856f;
        public static readonly float PITCH_RANGE = PITCH_MAX - PITCH_MIN;

        public readonly float ROLL_MIN;
        public readonly float ROLL_MAX;
        public readonly float ROLL_RANGE;

        public float Pitch { get; private set; }
        public float Roll { get; private set; }

        private NaoShoulder(){}

        public NaoShoulder(float pitch, float roll, bool leftSide)
        {
            if (leftSide)
            {
                ROLL_MIN = 0.3142f;
                ROLL_MAX = 1.3265f;
            }
            else
            {
                ROLL_MIN = -1.3265f;
                ROLL_MAX = -0.3142f;
            }
            ROLL_RANGE = ROLL_MAX - ROLL_MIN;

            // TODO: find out why we need these -> shouldn't if AngleConverter is working
            roll = Util.clamp(roll, ROLL_MIN, ROLL_MAX);
            pitch = Util.clamp(pitch, PITCH_MIN, PITCH_MAX);

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
            this.Pitch = (float)info.GetValue("Pitch", typeof(float));
            this.Roll = (float)info.GetValue("Roll", typeof(float));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Pitch", Pitch);
            info.AddValue("Roll", Roll);
        }
    }
}
