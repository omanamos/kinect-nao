using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Research.Kinect.Nui;
using KinectInterface;

using DataStore;

namespace DataStore
{
    public class HumanSkeleton : ISkeleton
    {
        private Dictionary<JointID, Joint> joints;
        public double[][] JointPositions {get; private set;}
        
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
            JointPositions = new double[RealKinect.JOINT_ORDERING.Count][];
            string[] toks = line.Split();
            for (int i = 0; i < RealKinect.JOINT_ORDERING.Count; i++)
            {
                JointPositions[i] = new double[3];
                double x = double.Parse(toks[1 + 3 * i]);
                double y = double.Parse(toks[2 + 3 * i]);
                double z = double.Parse(toks[3 + 3 * i]);
                JointPositions[i][0] = x;
                JointPositions[i][1] = y;
                JointPositions[i][2] = z;
                joints.Add(RealKinect.JOINT_ORDERING[i], new Joint(x, y, z));
            }
        }

        /// <summary>
        /// Creates a new HumanSkeleton from a flattened array of joint positions
        /// </summary>
        /// <param name="jointPositions"></param>
        public HumanSkeleton(double[] jointPositions)
        {
            this.joints = new Dictionary<JointID, Joint>();
            JointPositions = new double[RealKinect.JOINT_ORDERING.Count][];
            for (int i = 0; i < RealKinect.JOINT_ORDERING.Count; i++)
            {
                JointPositions[i] = new double[3];
                double x = jointPositions[3 * i];
                double y = jointPositions[1 + 3 * i];
                double z = jointPositions[2 + 3 * i];
                JointPositions[i][0] = x;
                JointPositions[i][1] = y;
                JointPositions[i][2] = z;
                joints.Add(RealKinect.JOINT_ORDERING[i], new Joint(x, y, z));
            }
        }

        /// <summary>
        /// Creates a HumanSkeleton from a list of joint positions
        /// </summary>
        /// <param name="jointPositions"></param>
        public HumanSkeleton(List<double[]> jointPositions)
        {
            this.joints = new Dictionary<JointID, Joint>();
            JointPositions = new double[RealKinect.JOINT_ORDERING.Count][];
            for (int i = 0; i < RealKinect.JOINT_ORDERING.Count; i++)
            {
                JointPositions[i] = new double[3];
                double x = jointPositions[i][0];
                double y = jointPositions[i][1];
                double z = jointPositions[i][2];
                JointPositions[i][0] = x;
                JointPositions[i][1] = y;
                JointPositions[i][2] = z;
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
            {
                AngleConverter ac = new AngleConverter(this);
                return Util.toDoubleArray(ac.getAngles());
            }
            else
                return new double[] {
                    joints[JointID.HipCenter].x, 
                    joints[JointID.HipCenter].y, 
                    joints[JointID.HipCenter].z};
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
