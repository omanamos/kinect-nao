using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Aldebaran.Proxies;


namespace NaoKinectTest
{
    class NaoController
    {
        private static sealed float SPEED = 0.1f;

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
            proxy.setAngles("LShoulderPitch", skeleton.getLeftShoulder().getPitch(), SPEED);
            proxy.setAngles("RShoulderPitch", skeleton.getRightShoulder().getPitch(), SPEED);
            proxy.setAngles("LShoulderRoll", skeleton.getLeftShoulder().getRoll(), SPEED);
            proxy.setAngles("RShoulderRoll", skeleton.getRightShoulder().getRoll(), SPEED);

            //Elbow Angles
            proxy.setAngles("LElbowYaw", skeleton.getLeftElbow().getYaw(), SPEED);
            proxy.setAngles("RElbowYaw", skeleton.getRightElbow().getYaw(), SPEED);
            proxy.setAngles("LElbowRoll", skeleton.getLeftElbow().getRoll(), SPEED);
            proxy.setAngles("RElbowRoll", skeleton.getRightElbow().getRoll(), SPEED);

            //Wrist Angles
            proxy.setAngles("LWristYaw", skeleton.getLeftWrist().getYaw(), SPEED);
            proxy.setAngles("RWristYaw", skeleton.getRightWrist().getYaw(), SPEED);

            //Hand Angles
            proxy.setAngles("LHand", skeleton.getLeftHand().isOpen(), SPEED);
            proxy.setAngles("RHand", skeleton.getRightHand().isOpen(), SPEED);
        }
    }
}
