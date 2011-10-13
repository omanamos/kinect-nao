using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Aldebaran.Proxies;


namespace NaoKinectTest
{
    class NaoController
    {
        private static readonly float SPEED = 0.1f;

        private MotionProxy proxy;
        private NaoSkeleton lastUpdate;

        public NaoController(string ip)
        {
            lastUpdate = null;
            proxy = new MotionProxy(ip, 9559);
            proxy.stiffnessInterpolation("Head", 1.0f, 1.0f);
        }

        public void update(NaoSkeleton skeleton)
        {
            //Shoulder Angles
            proxy.setAngles("LShoulderPitch", skeleton.LeftShoulder.Pitch, SPEED);
            proxy.setAngles("RShoulderPitch", skeleton.RightShoulder.Pitch, SPEED);
            proxy.setAngles("LShoulderRoll", skeleton.LeftShoulder.Roll, SPEED);
            proxy.setAngles("RShoulderRoll", skeleton.RightShoulder.Roll, SPEED);

            //Elbow Angles
            proxy.setAngles("LElbowYaw", skeleton.LeftElbow.getYaw(), SPEED);
            proxy.setAngles("RElbowYaw", skeleton.RightElbow.getYaw(), SPEED);
            proxy.setAngles("LElbowRoll", skeleton.LeftElbow.getRoll(), SPEED);
            proxy.setAngles("RElbowRoll", skeleton.RightElbow.getRoll(), SPEED);

            //Wrist Angles
            proxy.setAngles("LWristYaw", skeleton.LeftWrist.getYaw(), SPEED);
            proxy.setAngles("RWristYaw", skeleton.RightWrist.getYaw(), SPEED);

            //Hand Angles
            proxy.setAngles("LHand", skeleton.LeftHand.isOpen(), SPEED);
            proxy.setAngles("RHand", skeleton.RightHand.isOpen(), SPEED);
        }
    }
}
