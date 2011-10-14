using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Aldebaran.Proxies;


namespace NaoKinectTest
{
    class NaoController
    {
        private static readonly float SPEED = 0.2f;

        private MotionProxy proxy;
        private NaoSkeleton lastUpdate;

        public NaoController(string ip)
        {
            try
            {
                lastUpdate = null;
                proxy = new MotionProxy(ip, 9559);
                proxy.stiffnessInterpolation("Head", 1.0f, 1.0f);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("HeadPanning.Connect exception: " + e);
            }
        }

        public void update(NaoSkeleton skeleton)
        {
            //Shoulder Angles
            proxy.setAngles("LShoulderPitch", (float)skeleton.LeftShoulder.Pitch, SPEED);
            proxy.setAngles("RShoulderPitch", (float)skeleton.RightShoulder.Pitch, SPEED);
            proxy.setAngles("LShoulderRoll", (float)skeleton.LeftShoulder.Roll, SPEED);
            proxy.setAngles("RShoulderRoll", (float)skeleton.RightShoulder.Roll, SPEED);
            //proxy.setAngles("LShoulderRoll", (float)1.3264, SPEED);
            //proxy.setAngles("RShoulderRoll", (float)-1.3264, SPEED);
            //proxy.setAngles("LShoulderPitch", (float)0, SPEED);
            //proxy.setAngles("RShoulderPitch", (float)0, SPEED);

            //Elbow Angles
            //proxy.setAngles("LElbowYaw", (float)skeleton.LeftElbow.getYaw(), SPEED);
            //proxy.setAngles("RElbowYaw", (float)skeleton.RightElbow.getYaw(), SPEED);
            //proxy.setAngles("LElbowRoll", (float)skeleton.LeftElbow.getRoll(), SPEED);
            //proxy.setAngles("RElbowRoll", (float)skeleton.RightElbow.getRoll(), SPEED);

            proxy.setAngles("LElbowYaw", (float)0.0, SPEED);
            proxy.setAngles("RElbowYaw", (float)0.0, SPEED);
            proxy.setAngles("LElbowRoll", (float)-0.04, SPEED);
            proxy.setAngles("RElbowRoll", (float)0.04, SPEED);


            //Shoulder Angles
            Console.Out.WriteLine("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
            Console.Out.WriteLine("LShoulderPitch: " + (float)skeleton.LeftShoulder.Pitch);
            Console.Out.WriteLine("RShoulderPitch: " + (float)skeleton.RightShoulder.Pitch);
            Console.Out.WriteLine("LShoulderRoll: " + (float)skeleton.LeftShoulder.Roll);
            Console.Out.WriteLine("RShoulderRoll: " + (float)skeleton.RightShoulder.Roll);

            //Elbow Angles
            
            Console.Out.WriteLine("LElbowYaw: " + (float)skeleton.LeftElbow.getYaw());
            Console.Out.WriteLine("RElbowYaw: " + (float)skeleton.RightElbow.getYaw());
            Console.Out.WriteLine("LElbowRoll: " + (float)skeleton.LeftElbow.getRoll());
            Console.Out.WriteLine("RElbowRoll: " + (float)skeleton.RightElbow.getRoll());
             
            //Wrist Angles
            //proxy.setAngles("LWristYaw", (float)skeleton.LeftWrist.getYaw(), SPEED);
            //proxy.setAngles("RWristYaw", (float)skeleton.RightWrist.getYaw(), SPEED);

            //Hand Angles
            //proxy.setAngles("LHand", (float)skeleton.LeftHand.isOpen(), SPEED);
            //proxy.setAngles("RHand", (float)skeleton.RightHand.isOpen(), SPEED);
        }
    }
}
