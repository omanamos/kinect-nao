using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NaoKinectTest.HumanModel;
using System.Runtime.Serialization;

namespace DataStore
{
    [Serializable]
    public class NaoSkeleton : ISerializable
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
            this.Position = position;

            this.LeftShoulder = leftShoulder;
            this.RightShoulder = rightShoulder;

            this.LeftWrist = leftWrist;
            this.RightWrist = rightWrist;

            this.LeftElbow = leftElbow;
            this.RightElbow = rightElbow;

            this.LeftHand = leftHand;
            this.RightHand = rightHand;
        }

        public NaoSkeleton(SerializationInfo info, StreamingContext ctxt)
        {
            this.Position = (NaoPosition)info.GetValue("Position", typeof(NaoPosition));

            this.LeftShoulder = (NaoShoulder)info.GetValue("LeftShoulder", typeof(NaoShoulder));
            this.RightShoulder = (NaoShoulder)info.GetValue("RightShoulder", typeof(NaoShoulder));

            this.LeftWrist = (NaoWrist)info.GetValue("LeftWrist", typeof(NaoWrist));
            this.RightWrist = (NaoWrist)info.GetValue("RightWrist", typeof(NaoWrist));

            this.LeftElbow = (NaoElbow)info.GetValue("LeftElbow", typeof(NaoElbow));
            this.RightElbow = (NaoElbow)info.GetValue("RightElbow", typeof(NaoElbow));

            this.LeftHand = (NaoHand)info.GetValue("LeftHand", typeof(NaoHand));
            this.RightHand = (NaoHand)info.GetValue("RightHand", typeof(NaoHand));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Position", Position);

            info.AddValue("LeftShoulder", LeftShoulder);
            info.AddValue("RightShoulder", RightShoulder);

            info.AddValue("LeftWrist", LeftWrist);
            info.AddValue("RightWrist", RightWrist);

            info.AddValue("LeftElbow", LeftElbow);
            info.AddValue("RightElbow", RightElbow);

            info.AddValue("LeftHand", LeftHand);
            info.AddValue("RightHand", RightHand);
        }
    }
}
