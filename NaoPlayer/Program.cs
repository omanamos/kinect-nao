using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using DataStore;
using Aldebaran.Proxies;

using Controller;

namespace NaoPlayer
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, string> actions = new Dictionary<string, string>();
            actions["raise the roof"] = "raisetheroof";
            actions["walk forward"] = "walkforward";
            actions["walk left"] = "walkleft";
            actions["wave right"] = "waveright";
            actions["wave left"] = "waveleft";
            string filename = "Z:/dev/kinect-nao/recordings/";
            ActionLibrary lib = new ActionLibrary();

            foreach (string actionName in actions.Keys)
            {
                List<HumanSkeleton> seq = new List<HumanSkeleton>();
                using (StreamReader s = new StreamReader(filename + actions[actionName] + "/"
                    + actionName + "1.rec"))
                {
                    while (!s.EndOfStream)
                    {
                        seq.Add(new HumanSkeleton(s.ReadLine()));
                    }
                }

                //List<float[]> naoSeq = new List<float[]>();
                ActionSequence<NaoSkeleton> naoSeq = new ActionSequence<NaoSkeleton>();
                foreach (HumanSkeleton h in seq)
                {
                    AngleConverter ac = new AngleConverter(h);
                    naoSeq.append(ac.getNaoSkeleton());
                    //naoSeq.Add(ac.getAngles());
                }

                lib.appendToCache(naoSeq);
                lib.setCachedName(actionName);
                lib.saveCache();
                //bool isJointAction = true; // false if is a walking action
                //ExecuteOnNao(naoSeq, isJointAction);
            }
            lib.save(MainController.ACTION_LIB_PATH);
        }

        static void ExecuteOnNao(List<float[]> naoSeq, bool isJointAction)
        {
            MotionProxy mp = new MotionProxy("127.0.0.1", 9559);
            // do stuff with naoskeletons and real NAO
            foreach (float[] act in naoSeq) {

                float LShoulderRoll = act[(int)AngleConverter.NaoJointAngle.LShoulderRoll];
                float LShoulderPitch = act[(int)AngleConverter.NaoJointAngle.LShoulderPitch];
                float LElbowYaw = act[(int)AngleConverter.NaoJointAngle.LElbowYaw];
                float LElbowRoll = act[(int)AngleConverter.NaoJointAngle.LElbowRoll];
                Console.WriteLine("LShoulderRoll: " + LShoulderRoll);
                Console.WriteLine("LShoulderPitch: " + LShoulderPitch);
 /*               mp.setAngles("LElbowYaw", LElbowYaw, 0.4f);
                mp.setAngles("LElbowRoll", LElbowRoll, 0.4f);
                mp.setAngles("LShoulderRoll", LShoulderRoll, 0.4f);
                mp.setAngles("LShoulderPitch", LShoulderPitch, 0.4f);*/
                String[] joints = new String[(int)AngleConverter.NaoJointAngle.count];
                joints[(int)AngleConverter.NaoJointAngle.LElbowRoll] = "LElbowRoll";
                joints[(int)AngleConverter.NaoJointAngle.LElbowYaw] = "LElbowYaw";
                joints[(int)AngleConverter.NaoJointAngle.LShoulderPitch] = "LShoulderPitch";
                joints[(int)AngleConverter.NaoJointAngle.LShoulderRoll] = "LShoulderRoll";
                joints[(int)AngleConverter.NaoJointAngle.RElbowRoll] = "RElbowRoll";
                joints[(int)AngleConverter.NaoJointAngle.RElbowYaw] = "RElbowYaw";
                joints[(int)AngleConverter.NaoJointAngle.RShoulderPitch] = "RShoulderPitch";
                joints[(int)AngleConverter.NaoJointAngle.RShoulderRoll] = "RShoulderRoll";

                mp.angleInterpolationWithSpeed(joints, act, 0.3f);
                System.Threading.Thread.Sleep(50);
            }
            Console.Read();
        }

    }
}
