using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NaoKinectTest.HumanModel;

namespace NaoKinectTest
{
    class NaoSkeleton
    {
        private NaoShoulder leftShoulder;
        private NaoShoulder rightShoulder;
        private NaoWrist leftWrist;
        private NaoWrist rightWrist;
        private NaoElbow leftElbow;
        private NaoElbow rightElbow;
        private NaoHand leftHand;
        private NaoHand rightHand;

        private NaoSkeleton() {}

        public NaoSkeleton(HumanSkeleton skeleton)
        {

        }
    }
}
