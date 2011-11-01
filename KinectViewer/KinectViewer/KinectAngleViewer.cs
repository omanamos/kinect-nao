using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KinectViewer
{
    class KinectAngleViewer : KinectViewer
    {

        protected override void updateSkeleton(SkeletonData skeleton)
        {
            Vector3 elbowRight     = getLoc(skeleton.Joints[JointID.ElbowRight]),
                    handRight      = getLoc(skeleton.Joints[JointID.HandRight]),
                    shoulderRight  = getLoc(skeleton.Joints[JointID.ShoulderRight]),
                    shoulderLeft   = getLoc(skeleton.Joints[JointID.ShoulderLeft]),
                    shoulderCenter = getLoc(skeleton.Joints[JointID.ShoulderCenter]),
                    spine          = getLoc(skeleton.Joints[JointID.Spine]),
                    elbowLeft      = getLoc(skeleton.Joints[JointID.ElbowLeft]);

            lines.Clear();
            // ...
        }
        
        private void debugReferenceFrame(String str, Matrix m, float sz)
        {
            debugReferenceFrame(str, m, sz, m.Translation);
        }

        private void debugReferenceFrame(String str, Matrix m, float sz, Vector3 origin)
        {
            lines.Add(new LabelledVector(origin, origin + m.Right * sz, Color.Black, str));
            lines.Add(new LabelledVector(origin, origin + m.Up * sz, Color.Green, ""));
            lines.Add(new LabelledVector(origin, origin + m.Forward * sz, Color.Blue, ""));
        }
    }
}
