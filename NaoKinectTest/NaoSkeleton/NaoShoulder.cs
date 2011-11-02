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
        private bool leftSide;

        public double Pitch { get; private set; }
        public double Roll { get; private set; }

        private NaoShoulder(){}

        public NaoShoulder(double humanPitch, double humanRoll, bool leftSide)
        {
            this.leftSide = leftSide;
            if (leftSide)
            {
                ROLL_MIN = -0.3142;
                ROLL_MAX = 1.3265;
            }
            ROLL_RANGE = ROLL_MAX - ROLL_MIN;
            Pitch = scalePitch(humanPitch);
            Roll = scaleRoll(humanRoll);
        }

        public double scalePitch(double humanPitch)
        {
            Console.Out.WriteLine("Pitch: " + humanPitch);
            return Util.clamp(((humanPitch - HumanShoulder.YAW_MIN) / HumanShoulder.YAW_RANGE) * PITCH_RANGE + PITCH_MIN, PITCH_MIN, PITCH_MAX);
        }

        public double scaleRoll(double humanRoll)
        {
            double rtn = ((humanRoll - HumanShoulder.PITCH_MIN) / HumanShoulder.PITCH_RANGE) * ROLL_RANGE + ROLL_MIN;
            if (leftSide)
            {
                rtn += 0;
            }
            else
            {
                rtn = -rtn - Math.PI;
            }
            //return Util.clamp(rtn, ROLL_MIN, ROLL_MAX);(leftSide ? 1 : -1) * 
            return Util.clamp(((humanRoll - HumanShoulder.PITCH_MIN) / HumanShoulder.PITCH_RANGE) * ROLL_RANGE + ROLL_MIN, ROLL_MIN, ROLL_MAX);
        }
    }
}
