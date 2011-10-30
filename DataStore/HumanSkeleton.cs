using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Research.Kinect.Nui;

using Emgu.CV;

using MotionRecorder;

namespace NaoKinectTest.HumanModel
{
    public class HumanSkeleton
    {
        private Dictionary<JointID, Joint> joints;

        // Initialize a new HumanSkeleton model with the given observed joints
        public HumanSkeleton(string[] line)
        {
            this.joints = new Dictionary<JointID, Joint>();
            for (int i = 0; i < (line.Length - 1) / 3; i++)
            {
                joints.Add(RealKinect.JOINT_ORDERING[i], new Joint(double.Parse(line[1 + 3 * i]), 
                        double.Parse(line[2 + 3 * i]), double.Parse(line[3 + 3 * i])));
            }
        }

        public Joint getJoint(JointID id)
        {
            return this.joints[id];
        }
    }

    public class Joint
    {
        public readonly double x;
        public readonly double y;
        public readonly double z;

        public Joint(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}
