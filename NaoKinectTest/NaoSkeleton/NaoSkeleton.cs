using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NaoKinectTest.HumanModel;

namespace NaoKinectTest
{
    class NaoSkeleton
    {
        public NaoShoulder LeftShoulder { get; private set; }
        public NaoShoulder RightShoulder { get; private set; }
        public NaoWrist LeftWrist { get; private set; }
        public NaoWrist RightWrist { get; private set; }
        public NaoElbow LeftElbow { get; private set; }
        public NaoElbow RightElbow { get; private set; }
        public NaoHand LeftHand { get; private set; }
        public NaoHand RightHand { get; private set; }

        private NaoSkeleton() {}

        public NaoSkeleton(HumanSkeleton skeleton)
        {
            this.LeftShoulder = new NaoShoulder(skeleton.LeftShoulderYaw, skeleton.LeftShoulderPitch, true);
            this.RightShoulder = new NaoShoulder(skeleton.RightShoulderYaw, -skeleton.RightShoulderPitch, false);

            this.LeftWrist = new NaoWrist(0.0);
            this.RightWrist = new NaoWrist(0.0);

            this.LeftElbow = new NaoElbow(skeleton.LeftShoulderRoll, skeleton.LeftElbowYaw, true);
            this.RightElbow = new NaoElbow(skeleton.RightShoulderRoll, skeleton.RightElbowYaw, false);

            this.LeftHand = new NaoHand(true);
            this.RightHand = new NaoHand(true);
        }
    }
}
