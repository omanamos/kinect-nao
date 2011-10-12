using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using Microsoft.Research.Kinect.Nui;

namespace NaoKinectTest.HumanModel
{
    class Util
    {
        public static Vector3D projectToPlane(Vector3D planeNormal, Vector3D v)
        {
            return v - (Vector3D.DotProduct(v, planeNormal) * planeNormal);
        }

        public static Vector3D vectorFromJoint(JointID id, JointsCollection kinectJoints)
        {
            Vector3D v = new Vector3D();
            v.X = kinectJoints[id].Position.X;
            v.Y = kinectJoints[id].Position.Y;
            v.Z = kinectJoints[id].Position.Z;
            return v;
        }

        public static Vector3D vectorFromJoint(Joint j)
        {
            Vector3D v = new Vector3D();
            v.X = j.Position.X;
            v.Y = j.Position.Y;
            v.Z = j.Position.Z;
            return v;
        }
    }
}
