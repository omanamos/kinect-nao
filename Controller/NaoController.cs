using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Aldebaran.Proxies;
using DataStore;


namespace Controller
{
    public class NaoController
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
                float stiffness = 1.0f;
                float time = 1.0f;
                /*
                proxy.stiffnessInterpolation("Head", stiffness, time);
                proxy.stiffnessInterpolation("LShoulderPitch", stiffness, time);
                proxy.stiffnessInterpolation("RShoulderPitch", stiffness, time);
                proxy.stiffnessInterpolation("LShoulderRoll", stiffness, time);
                proxy.stiffnessInterpolation("RShoulderRoll", stiffness, time);
                proxy.stiffnessInterpolation("LElbowYaw", stiffness, time);
                proxy.stiffnessInterpolation("RElbowYaw", stiffness, time);
                proxy.stiffnessInterpolation("LElbowRoll", stiffness, time);
                proxy.stiffnessInterpolation("RElbowRoll", stiffness, time);

                proxy.stiffnessInterpolation("RHipYawPitch", stiffness, time);
                proxy.stiffnessInterpolation("LHipYawPitch", stiffness, time);
                proxy.stiffnessInterpolation("LHipPitch", stiffness, time);
                proxy.stiffnessInterpolation("RHipPitch", stiffness, time);
                proxy.stiffnessInterpolation("LKneePitch", stiffness, time);
                proxy.stiffnessInterpolation("RKneePitch", stiffness, time);
                proxy.stiffnessInterpolation("LAnklePitch", stiffness, time);
                proxy.stiffnessInterpolation("RAnklePitch", stiffness, time);
                proxy.stiffnessInterpolation("LHipRoll", stiffness, time);
                proxy.stiffnessInterpolation("RHipRoll", stiffness, time);
                proxy.stiffnessInterpolation("LAnkleRoll", stiffness, time);
                proxy.stiffnessInterpolation("RAnkleRoll", stiffness, time);
                 * */
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("HeadPanning.Connect exception: " + e);
            }
        }

        public void update(NaoSkeleton skeleton)
        {
            // TODO(namos): add in code to make the NAO walk

            //Shoulder Angles
            proxy.setAngles("LShoulderPitch", (float)skeleton.LeftShoulder.Pitch, SPEED);
            proxy.setAngles("RShoulderPitch", (float)skeleton.RightShoulder.Pitch, SPEED);
            proxy.setAngles("LShoulderRoll", (float)skeleton.LeftShoulder.Roll, SPEED);
            proxy.setAngles("RShoulderRoll", (float)skeleton.RightShoulder.Roll, SPEED);

            //Elbow Angles
            proxy.setAngles("LElbowYaw", (float)skeleton.LeftElbow.Yaw, SPEED);
            proxy.setAngles("RElbowYaw", (float)skeleton.RightElbow.Yaw, SPEED);
            proxy.setAngles("LElbowRoll", (float)skeleton.LeftElbow.Roll, SPEED);
            proxy.setAngles("RElbowRoll", (float)skeleton.RightElbow.Roll, SPEED);

            //Shoulder Angles
            Console.Out.WriteLine("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
            Console.Out.WriteLine("LShoulderPitch: " + (float)skeleton.LeftShoulder.Pitch);
            Console.Out.WriteLine("RShoulderPitch: " + (float)skeleton.RightShoulder.Pitch);
            Console.Out.WriteLine("LShoulderRoll: " + (float)skeleton.LeftShoulder.Roll);
            Console.Out.WriteLine("RShoulderRoll: " + (float)skeleton.RightShoulder.Roll);

            //Elbow Angles
            Console.Out.WriteLine("LElbowYaw: " + (float)skeleton.LeftElbow.Yaw);
            Console.Out.WriteLine("RElbowYaw: " + (float)skeleton.RightElbow.Yaw);
            Console.Out.WriteLine("LElbowRoll: " + (float)skeleton.LeftElbow.Roll);
            Console.Out.WriteLine("RElbowRoll: " + (float)skeleton.RightElbow.Roll);
             
            //Wrist Angles
            //proxy.setAngles("LWristYaw", (float)skeleton.LeftWrist.getYaw(), SPEED);
            //proxy.setAngles("RWristYaw", (float)skeleton.RightWrist.getYaw(), SPEED);

            //Hand
            //proxy.setAngles("LHand", (float)skeleton.LeftHand.isOpen(), SPEED);
            //proxy.setAngles("RHand", (float)skeleton.RightHand.isOpen(), SPEED);
        }
    }
}
