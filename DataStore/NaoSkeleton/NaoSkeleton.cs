using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataStore
{
    public class NaoSkeleton : ISkeleton
    {
        public NaoPosition Position { get; private set; }
        public NaoShoulder LeftShoulder { get; private set; }
        public NaoShoulder RightShoulder { get; private set; }
        public NaoWrist LeftWrist { get; private set; }
        public NaoWrist RightWrist { get; private set; }
        public NaoElbow LeftElbow { get; private set; }
        public NaoElbow RightElbow { get; private set; }
        public NaoHand LeftHand { get; private set; }
        public NaoHand RightHand { get; private set; }

        private NaoSkeleton() {}

        public NaoSkeleton(NaoPosition position,
                NaoShoulder leftShoulder, NaoShoulder rightShoulder,
                NaoWrist leftWrist, NaoWrist rightWrist,
                NaoElbow leftElbow, NaoElbow rightElbow,
                NaoHand leftHand, NaoHand rightHand)
        {
            this.LeftShoulder = leftShoulder;
            this.RightShoulder = rightShoulder;

            this.LeftWrist = leftWrist;
            this.RightWrist = rightWrist;

            this.LeftElbow = leftElbow;
            this.RightElbow = rightElbow;

            this.LeftHand = leftHand;
            this.RightHand = rightHand;
        }

        public double[] toArray(bool useJointVals)
        {
            throw new NotImplementedException();
        }
    }
}
