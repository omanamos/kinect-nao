using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Research.Kinect.Nui;

using MotionRecorder;
using DataStore;

namespace NaoKinectTest.HumanModel
{
    public class HumanSkeleton : ISkeleton
    {
        private Dictionary<JointID, Joint> joints;
        private double[][] arry;
        
        // 
        /// <summary>
        /// Initialize a new HumanSkeleton model with the given observed joints
        /// </summary>
        /// <param name="line">A line of the format:
        /// timestamp j1x j1y j1z .... j20x j20y j20z
        /// </param>
        public HumanSkeleton(string line)
        {
            this.joints = new Dictionary<JointID, Joint>();
            arry = new double[RealKinect.JOINT_ORDERING.Count][];
            string[] toks = line.Split();
            for (int i = 0; i < RealKinect.JOINT_ORDERING.Count; i++)
            {
                arry[i] = new double[3];
                double x = double.Parse(toks[1 + 3 * i]);
                double y = double.Parse(toks[2 + 3 * i]);
                double z = double.Parse(toks[3 + 3 * i]);
                arry[i][0] = x;
                arry[i][1] = y;
                arry[i][2] = z;
                joints.Add(RealKinect.JOINT_ORDERING[i], new Joint(x, y, z));
            }
        }

        public Joint getJoint(JointID id)
        {
            return this.joints[id];
        }

        public double[] toArray(bool useJointvals)
        {
            // TODO: convert to joint vals if needed
            if (useJointvals)
                return null;
            else
                // JointID.HipCenter. Update this if RealKinect.cs changes
                return arry[4];
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
