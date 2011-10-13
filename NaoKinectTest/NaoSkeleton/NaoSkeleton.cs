using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NaoKinectTest.HumanModel;

namespace NaoKinectTest
{
    class NaoSkeleton
    {
        private NaoShoulder leftShoulder { get; }
        private NaoShoulder rightShoulder { get; }
        private NaoWrist leftWrist { get; }
        private NaoWrist rightWrist { get; }
        private NaoElbow leftElbow { get; }
        private NaoElbow rightElbow { get; }
        private NaoHand leftHand { get; }
        private NaoHand rightHand { get; }

        private NaoSkeleton() {}

        public NaoSkeleton(HumanSkeleton skeleton)
        {
            
        }

        public NaoShoulder getLeftShoulder()
        {
            return this.leftShoulder;
        }

        public NaoShoulder getRightShoulder()
        {
            return this.rightShoulder;
        }

        public NaoWrist getLeftWrist()
        {
            return this.leftWrist;
        }

        public NaoWrist getRightWrist()
        {
            return this.rightWrist;
        }

        public NaoElbow getLeftElbow()
        {
            return this.leftElbow;
        }

        public NaoElbow getRightElbow()
        {
            return this.rightElbow;
        }

        public NaoHand getLeftHand()
        {
            return this.leftHand;
        }

        public NaoHand getRightHand()
        {
            return this.rightHand;
        }
    }
}
