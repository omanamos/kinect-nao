using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using Microsoft.Research.Kinect.Nui;

namespace NaoKinectTest.HumanModel
{
    class HumanShoulder
    {
        // TODO: Change these values
        public static readonly double PITCH_MIN = 0.3;
        public static readonly double PITCH_MAX = 2.7;
        public static readonly double PITCH_RANGE = PITCH_MAX - PITCH_MIN;

        // TODO: Change these values
        public static readonly double YAW_MIN = -1.1;
        public static readonly double YAW_MAX = 1.8;
        public static readonly double YAW_RANGE = YAW_MAX - YAW_MIN;

        // TODO: Change these values
        public static readonly double ROLL_MIN = 0.2;
        public static readonly double ROLL_MAX = 2.4;
        public static readonly double ROLL_RANGE = ROLL_MAX - ROLL_MIN;
        private bool _leftSide;

        public HumanShoulder(Joint shoulderCenter, Joint spine, Joint shoulder, Joint elbow, Joint wrist, bool leftSide)
        {
            // Compute the roll, pitch and yaw of the shoulder
            _leftSide = leftSide;
            compute(shoulderCenter, spine, shoulder, elbow, wrist);
        }

        private void compute(Joint shoulderCenter, Joint spine, Joint shoulder, Joint elbow, Joint wrist)
        {
            Vector3D center = Util.vectorFromJoint(shoulderCenter);
            Vector3D spineVec = Util.vectorFromJoint(spine);
            Vector3D centerToSpine = spineVec - center;
            centerToSpine.Normalize();

            Vector3D shoulderVec = Util.vectorFromJoint(shoulder);
            Vector3D centerToShoulder = shoulderVec - center;
            centerToShoulder.Normalize();

            

            Vector3D chestPlaneNormal = Vector3D.CrossProduct(centerToShoulder, -centerToSpine);


            Vector3D elbowVec = Util.vectorFromJoint(elbow);
            Vector3D shoulderToElbow = elbowVec - shoulderVec;
            shoulderToElbow.Normalize();

            Vector3D naoPitchPlaneNormal = Vector3D.CrossProduct(shoulderToElbow, centerToShoulder);

            Vector3D pitchVector = Util.projectToPlane(naoPitchPlaneNormal, shoulderToElbow);

            Vector3D oldPitchVector = Util.projectToPlane(chestPlaneNormal, shoulderToElbow);
            double oldShoulderPitch = Vector3D.AngleBetween(oldPitchVector, centerToSpine);
            if (Vector3D.DotProduct(oldPitchVector, centerToShoulder) < 0)
            {
                Console.WriteLine("Inverting shoulder pitch");
                oldShoulderPitch = -oldShoulderPitch;
            }
            Vector3D naoYawPlaneNormal = centerToShoulder;


            Vector3D yawVector = Util.projectToPlane(naoYawPlaneNormal, shoulderToElbow);

            Vector3D wristVec = Util.vectorFromJoint(wrist);
            Vector3D elbowToWrist = wristVec - elbowVec;

            Vector3D rollVector = Util.projectToPlane(shoulderToElbow, elbowToWrist);

            double shoulderYaw = 0;
            if (yawVector.Length > 0.5)
            {
                if (_leftSide)
                {
                    shoulderYaw = Vector3D.AngleBetween(yawVector, chestPlaneNormal);
                }
                else
                {
                    shoulderYaw = Vector3D.AngleBetween(yawVector, -chestPlaneNormal);
                }

                if (Vector3D.DotProduct(yawVector, centerToSpine) < 0)
                {
                    Console.WriteLine("Inverting shoulder yaw");
                    shoulderYaw = 2 * Math.PI - shoulderYaw;
                }
                prevYaw = Yaw;
                Yaw = Util.degToRad(shoulderYaw);
            }
            else
            {
                Console.WriteLine("yaw too small, ignoring");
            }


            AxisAngleRotation3D YawRotation = new AxisAngleRotation3D(
                centerToShoulder, shoulderYaw);
            Transform3D tr = new RotateTransform3D(YawRotation);
            Vector3D downInPitchFrame = tr.Transform(centerToSpine);

            double shoulderPitch = Vector3D.AngleBetween(pitchVector, downInPitchFrame);
            /*
            if (_leftSide)
            {
                shoulderPitch = Vector3D.AngleBetween(pitchVector, downInPitchFrame);
            }
            else
            {
                shoulderPitch = Vector3D.AngleBetween(pitchVector, -downInPitchFrame);
            }*/
            // AngleBetween returns between [0,180] so we need to add the sign appropriately
            if (Vector3D.DotProduct(pitchVector, centerToShoulder) < 0)
            {
                Console.WriteLine("Inverting shoulder pitch");
                shoulderPitch = -shoulderPitch;
            }
            
            //TODO this is very wrong
            AxisAngleRotation3D aar = new AxisAngleRotation3D(
                chestPlaneNormal, 90 - oldShoulderPitch);
            Transform3D t = new RotateTransform3D(aar);
            Vector3D upInShoulderFrame = t.Transform(-centerToSpine);

            double abt = Vector3D.AngleBetween(upInShoulderFrame, shoulderToElbow);
            Console.WriteLine("Abt:" + abt + " shoulderPitch" + shoulderPitch);

            double shoulderRoll = Vector3D.AngleBetween(rollVector, upInShoulderFrame);
            
            Pitch = Util.degToRad(shoulderPitch);
            
            Roll = Util.degToRad(shoulderRoll);
        }

        public double Yaw { get; set; }
        private double prevYaw = 0;
        public double Pitch { get; set; }

        public double Roll { get; set; }
    }
}
