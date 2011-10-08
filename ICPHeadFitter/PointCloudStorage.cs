using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Emgu.CV;

namespace ICPHeadFitter
{
    class PointCloudStorage
    {
        public static List<double[]> deserialize(string filename)
        {
            List<double[]> points = new List<double[]>();
            using (StreamReader file = new StreamReader(filename))
            {
                while (!file.EndOfStream)
                {
                    string line = file.ReadLine();
                    // Line format: id x y z group
                    string[] toks = line.Split(' ');
                    double x, y, z;
                    Double.TryParse(toks[1], out x);
                    Double.TryParse(toks[2], out y);
                    Double.TryParse(toks[3], out z);
                    double[] point = { x, y, z };
                    points.Add(point);
                }
            }
            return points;
        }

        public static void serialize(string filename, Matrix<double> points)
        {
            using (StreamWriter writer = new StreamWriter(filename))
            {
                for (int i = 0; i < points.Rows; i++)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(i);
                    sb.Append(" ");
                    sb.Append(points[i, 0]);
                    sb.Append(" ");
                    sb.Append(points[i, 1]);
                    sb.Append(" ");
                    sb.Append(points[i, 2]);
                    sb.Append(" ");
                    sb.Append(1); // Group ID
                    writer.WriteLine(sb.ToString());
                }
            }
        }
    }
}
