using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NaoKinectTest.HumanModel;

namespace NaoKinectTest
{
    public class NaoShoulder
    {
        public static readonly double PITCH_MIN = -2.0856;
        public static readonly double PITCH_MAX = 2.0856;
        public static readonly double PITCH_RANGE = PITCH_MAX - PITCH_MIN;

        public double ROLL_MIN = -1.3265;
        public double ROLL_MAX = 0.3142;
        public double ROLL_RANGE;

        public double Pitch { get; private set; }
        public double Roll { get; private set; }

        private NaoShoulder(){}

        public NaoShoulder(double humanPitch, double humanRoll, bool leftSide)
        {
            if (leftSide)
            {
                ROLL_MIN = -0.3142;
                ROLL_MAX = 1.3265;

                humanPitch = -humanPitch;
            }
            ROLL_RANGE = ROLL_MAX - ROLL_MIN;
            Pitch = scalePitch(humanPitch);
            Roll = scaleRoll(humanRoll);
        }

        public double scalePitch(double humanPitch)
        {
            Console.Out.WriteLine("Pitch: " + humanPitch);
            return Util.clamp(((humanPitch - HumanShoulder.PITCH_MIN) / HumanShoulder.PITCH_RANGE) * PITCH_RANGE + PITCH_MIN, PITCH_MIN, PITCH_MAX);
        }

        public double scaleRoll(double humanRoll)
        {
            return Util.clamp(((-humanRoll - HumanShoulder.ROLL_MIN) / HumanShoulder.ROLL_RANGE) * ROLL_RANGE + ROLL_MIN, ROLL_MIN, ROLL_MAX);
        }
    }
}
