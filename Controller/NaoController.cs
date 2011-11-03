using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using Aldebaran.Proxies;
using DataStore;


namespace Controller
{
    public class NaoController
    {
        private readonly string ip;
        private static readonly float SPEED = 0.2f;
        
        private MotionProxy proxy;
        private NaoSkeleton lastUpdate;
        private TextToSpeechProxy tts;

        public NaoController(string ip)
        {
            try
            {
                this.ip = ip;
                lastUpdate = null;
                proxy = new MotionProxy(ip, 9559);
                this.setStiffness(1.0f);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("HeadPanning.Connect exception: " + e);
            }
        }

        public void exit()
        {
            this.setStiffness(0.0f);
        }

        private void setStiffness(float stiffness)
        {
            float time = 1.0f;
            String[] joints = {"HeadYaw", "HeadPitch",
                                  "LShoulderPitch", "RShoulderPitch",
                                  "LShoulderRoll", "RShoulderRoll",
                                  "LElbowYaw", "RElbowYaw",
                                  "LElbowRoll", "RElbowRoll",
                                  "RHipYawPitch", "LHipYawPitch",
                                  "LHipPitch", "RHipPitch",
                                  "LKneePitch", "RKneePitch",
                                  "LAnklePitch", "RAnklePitch",
                                  "LHipRoll", "RHipRoll",
                                  "LAnkleRoll", "RAnkleRoll"};
            float[] stiff = {stiffness, stiffness, stiffness, stiffness, stiffness, stiffness,
                                stiffness, stiffness, stiffness, stiffness, stiffness, stiffness,
                                stiffness, stiffness, stiffness, stiffness, stiffness, stiffness,
                                stiffness, stiffness, stiffness, stiffness};
            float[] times = {time, time, time, time, time, time,
                                time, time, time, time, time, time,
                                time, time, time, time, time, time,
                                time, time, time, time};

            proxy.stiffnessInterpolation(joints, stiff, times);
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
            proxy.stiffnessInterpolation("RAnkleRoll", stiffness, time);*/
        }

        public void update(NaoSkeleton skeleton)
        {
            // Given 0 to rotational angle for now
            NaoPosition nao_pos = skeleton.Position;
            NaoPosition nao_pos_prv = lastUpdate.Position;
            proxy.walkTo(nao_pos.X - nao_pos_prv.X, nao_pos.Y - nao_pos_prv.Y, 0.0f);
            
            String[] angleTypes = {"LShoulderPitch", "RShoulderPitch", 
                                      "LShoulderRoll", "RShoulderRoll", 
                                      "LElbowYaw", "RElbowYaw", 
                                      "LElbowRoll", "RElbowRoll"};
            float[] angles = {skeleton.LeftShoulder.Pitch, skeleton.RightShoulder.Pitch, 
                                 skeleton.LeftShoulder.Roll, skeleton.RightShoulder.Roll, 
                                 skeleton.LeftElbow.Yaw, skeleton.RightElbow.Yaw, 
                                 skeleton.LeftElbow.Roll, skeleton.RightElbow.Roll};

            proxy.angleInterpolationWithSpeed(angleTypes, angles, SPEED);
            /*
            //Shoulder Angles
            proxy.setAngles("LShoulderPitch", (float)skeleton.LeftShoulder.Pitch, SPEED);
            proxy.setAngles("RShoulderPitch", (float)skeleton.RightShoulder.Pitch, SPEED);
            proxy.setAngles("LShoulderRoll", (float)skeleton.LeftShoulder.Roll, SPEED);
            proxy.setAngles("RShoulderRoll", (float)skeleton.RightShoulder.Roll, SPEED);

            //Elbow Angles
            proxy.setAngles("LElbowYaw", (float)skeleton.LeftElbow.Yaw, SPEED);
            proxy.setAngles("RElbowYaw", (float)skeleton.RightElbow.Yaw, SPEED);
            proxy.setAngles("LElbowRoll", (float)skeleton.LeftElbow.Roll, SPEED);
            proxy.setAngles("RElbowRoll", (float)skeleton.RightElbow.Roll, SPEED);*/

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

        public void speak(String context)
        {
            tts = new TextToSpeechProxy(this.ip, 9559);
            tts.say(context);
        }
    }
}
