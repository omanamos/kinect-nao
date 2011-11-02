using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

using Microsoft.Research.Kinect.Nui;

namespace DataStore
{
    public class AngleConverter
    {
        private const float PI = 3.14159f;
        private HumanSkeleton hk;
        private float[] angles;

        public AngleConverter(HumanSkeleton hk)
        {
            this.hk = hk;
            this.angles = new float[(int)NaoJointAngle.count];
            getArmAngles();
        }

        // The maximum and minum angles are limited by Nao's joints limitation
        private void getArmAngles() 
        {
            // Get Joint positisions
            Joint hipCenter = hk.getJoint(JointID.HipCenter);

            Joint elbowR = hk.getJoint(JointID.ElbowRight);
            Joint shoulderR = hk.getJoint(JointID.ShoulderRight);
            Joint wristR = hk.getJoint(JointID.WristRight);

            Joint elbowL = hk.getJoint(JointID.ElbowLeft);
            Joint shoulderL = hk.getJoint(JointID.ShoulderLeft);
            Joint wristL = hk.getJoint(JointID.WristLeft);


            /* 
             * Shoulder
             */
            // Shoulder Roll
            Vector3D RShoulderToElbow = new Vector3D(shoulderR.x - elbowR.x, shoulderR.y - elbowR.y, shoulderR.z - elbowR.z);
            float RShoulderRollAng;
            if (RShoulderToElbow.Y >= 0)
                RShoulderRollAng = (float)Math.Atan2(RShoulderToElbow.X, RShoulderToElbow.Y);
            else
                RShoulderRollAng = (float)Math.Atan2(RShoulderToElbow.X, -1 * RShoulderToElbow.Y);
            if (RShoulderRollAng > 0)
                RShoulderRollAng = 0;
            this.angles[(int)NaoJointAngle.RShoulderRoll] = RShoulderRollAng;

            Vector3D LShoulderToElbow = new Vector3D(shoulderL.x - elbowL.x, shoulderL.y - elbowL.y, shoulderL.z - elbowL.z);
            float LShoulderRollAng;
            if (LShoulderToElbow.Y >= 0)
                LShoulderRollAng = (float)Math.Atan2(LShoulderToElbow.X, LShoulderToElbow.Y);
            else
                LShoulderRollAng = (float)Math.Atan2(LShoulderToElbow.X, -1 * LShoulderToElbow.Y);
            if (LShoulderRollAng < 0)
                LShoulderRollAng = 0;
            this.angles[(int)NaoJointAngle.LShoulderRoll] = LShoulderRollAng;

            // Shoulder Pitch
            float LShoulderPitchAng = (float)Math.Atan2(LShoulderToElbow.Y, LShoulderToElbow.Z);
            this.angles[(int)NaoJointAngle.LShoulderPitch] = LShoulderPitchAng;

            float RShoulderPitchAng = (float)Math.Atan2(RShoulderToElbow.Y, RShoulderToElbow.Z);
            this.angles[(int)NaoJointAngle.RShoulderPitch] = RShoulderPitchAng;
            

            /* 
             * Elbow
             */
            // Elbow Roll
            Vector3D RElbowToWrist = new Vector3D(elbowR.x - wristR.x, elbowR.y - wristR.y, elbowR.z - wristR.z);
            float RElbowRollAng = toRadian((float)Vector3D.AngleBetween(RShoulderToElbow, RElbowToWrist));
            if (RElbowRollAng >= 1.57)
                RElbowRollAng = 1.57f;
            else if (RElbowRollAng <= 0)
                RElbowRollAng = 0;
            this.angles[(int)NaoJointAngle.RElbowRoll] = RElbowRollAng;

            Vector3D LElbowToWrist = new Vector3D(elbowL.x - wristL.x, elbowL.y - wristL.y, elbowL.z - wristL.z);
            float LElbowRollAng = toRadian((float)Vector3D.AngleBetween(LShoulderToElbow, LElbowToWrist)) * -1;
            if (LElbowRollAng <= -1.57)
                LElbowRollAng = -1.57f;
            else if (LElbowRollAng >= 0)
                LElbowRollAng = 0;
            this.angles[(int)NaoJointAngle.LElbowRoll] = LElbowRollAng;

            // Elbow Yaw
            Vector3D shoulderLTohipCenter = new Vector3D(shoulderL.x - hipCenter.x, shoulderL.y - hipCenter.y, shoulderL.z - hipCenter.z);
            Vector3D LCrossCenterArm = Vector3D.CrossProduct(shoulderLTohipCenter, LShoulderToElbow);
            Vector3D LCrossArms = Vector3D.CrossProduct(LShoulderToElbow, LElbowToWrist);
            float LElbowYawAng = toRadian((float)Vector3D.AngleBetween(LCrossCenterArm, LCrossArms))- PI * 3 / 4;
            if (LElbowYawAng > 1.9f)
                LElbowYawAng = 1.9f;
            else if (LElbowYawAng < -1.9f)
                LElbowYawAng = -1.9f;
            this.angles[(int)NaoJointAngle.LElbowYaw] = LElbowYawAng;

            Vector3D shoulderRTohipCenter = new Vector3D(shoulderR.x - hipCenter.x, shoulderR.y - hipCenter.y, shoulderR.z - hipCenter.z);
            Vector3D RCrossCenterArm = Vector3D.CrossProduct(shoulderRTohipCenter, RShoulderToElbow);
            Vector3D RCrossArms = Vector3D.CrossProduct(RShoulderToElbow, RElbowToWrist);
            float RElbowYawAng = -1 * (toRadian((float)Vector3D.AngleBetween(RCrossCenterArm, RCrossArms)) - PI * 3 / 4);
            if (RElbowYawAng > 1.9f)
                RElbowYawAng = 1.9f;
            else if (LElbowYawAng < -1.9f)
                RElbowYawAng = -1.9f;
            this.angles[(int)NaoJointAngle.RElbowYaw] = RElbowYawAng;
        }

        private float toRadian(float degree)
        {
            return degree / 180 * PI;
        }

        // return the a NaoSkeleton with given joints
        public NaoSkeleton getNaoSkeleton()
        {
            Joint center = hk.getJoint(JointID.HipCenter);
            NaoPosition position = new NaoPosition(center.x, center.y, center.z);

            NaoElbow LElbow = new NaoElbow(this.angles[(int)NaoJointAngle.LElbowYaw], this.angles[(int)NaoJointAngle.LElbowRoll], true);
            NaoElbow RElbow = new NaoElbow(this.angles[(int)NaoJointAngle.RElbowYaw], this.angles[(int)NaoJointAngle.RElbowRoll], false);

            NaoShoulder LShoulder = new NaoShoulder(this.angles[(int)NaoJointAngle.LShoulderPitch], this.angles[(int)NaoJointAngle.LShoulderRoll], true);
            NaoShoulder RShoulder = new NaoShoulder(this.angles[(int)NaoJointAngle.RShoulderPitch], this.angles[(int)NaoJointAngle.RShoulderRoll], false);

            NaoWrist LWrist = new NaoWrist(0);
            NaoWrist RWrist = new NaoWrist(0);

            NaoHand LHand = new NaoHand(true);
            NaoHand RHand = new NaoHand(true);

            return new NaoSkeleton(position, LShoulder, RShoulder, LWrist, RWrist, LElbow, RElbow, LHand, RHand);
        }

        // return the angles of joints, use enum NajointAngle to get the corresponding values
        public float[] getAngles()
        {
            float[] ang = new float[(int)NaoJointAngle.count];
            for (int i = 0; i < (int)NaoJointAngle.count; i++)
            {
                ang[i] = this.angles[i];
            }
            return ang;
        }

        public enum NaoJointAngle
        {
            LShoulderPitch,
            RShoulderPitch,
            LShoulderRoll,
            RShoulderRoll,
            LElbowRoll,
            RElbowRoll,
            LElbowYaw,
            RElbowYaw,
            count,
        }
    }
}
