using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using icp_net;

namespace ICPHeadFitter
{
    class Program
    {
        static void Main(string[] args)
        {
            List<double[]> head_pts = PointCloudStorage.deserialize("model_pts");
            List<double[]> scene_pts = PointCloudStorage.deserialize("scene_pts");

            double[,] M = new double[head_pts.Count, 3];
            for (int i = 0; i < head_pts.Count; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    M[i, j] = head_pts[i][j];
                }
            }

            double[,] T = new double[scene_pts.Count, 3];
            for (int i = 0; i < scene_pts.Count; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    T[i, j] = scene_pts[i][j];
                }
            }

            ManagedICP icp = new ManagedICP(M, head_pts.Count, 3);

            double[,] Rot = new double[3, 3];
            /*Rot[0, 0] = 0.707;
            Rot[1, 1] = 0.707;
            Rot[0, 1] = -0.707;
            Rot[1, 0] = 0.707;
            Rot[2, 2] = 1.0;
             * */
            Rot[0, 0] = 0;
            Rot[1, 1] = 0;
            Rot[0, 1] = -1;
            Rot[1, 0] = 1;
            Rot[2, 2] = 1.0;
            double[] trans = new double[3];
            icp.fit(T, scene_pts.Count, Rot, trans, -1);
            Console.WriteLine("Rotation:");
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine(Rot[i, 0] + " " + Rot[i, 1] + " " + Rot[i, 2]);
            }
            int b = 5;
        }
    }
}
