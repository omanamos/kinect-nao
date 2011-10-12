using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NaoKinectTest.HumanModel;

namespace NaoKinectTest
{
    class NaoShoulder
    {
        public static readonly double PITCH_MIN = -2.0857;
        public static readonly double PITCH_MAX = 2.0857;
        public static readonly double PITCH_RANGE = PITCH_MAX - PITCH_MIN;

        public static readonly double ROLL_MIN = -1.3265;
        public static readonly double ROLL_MAX = 0.3142;
        public static readonly double ROLL_RANGE = ROLL_MAX - ROLL_MIN;

        private double pitch;
        private double roll;

        private NaoShoulder(){}

        public NaoShoulder(double humanPitch, double humanRoll)
        {
            this.pitch = scalePitch(humanPitch);
            this.roll = scaleRoll(humanRoll);
        }

        public double getPitch()
        {
            return this.pitch;
        }

        public double getRoll()
        {
            return this.roll;
        }

        private double scalePitch(double humanPitch)
        {
            return (humanPitch - HumanShoulder.PITCH_MIN / HumanShoulder.PITCH_RANGE) * PITCH_RANGE;
        }

        private double scaleRoll(double humanRoll)
        {
            return (humanRoll - HumanShoulder.ROLL_MIN / HumanShoulder.ROLL_RANGE) * PITCH_RANGE;
        }
    }
}
