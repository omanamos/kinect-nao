using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MotionRecorder
{
    class MotionRecording
    {
        private List<RecordingPoint> points;
        private Dictionary<long, RecordingPoint> timestampMap;
        private string name;
        
        public MotionRecording(List<RecordingPoint> points, string name)
        {
            this.points = new List<RecordingPoint>(points);
            this.name = name;
            timestampMap = new Dictionary<long, RecordingPoint>(points.Count);
            foreach (RecordingPoint p in points)
            {
                timestampMap[p.Time] = p;
            }
        }

        public RecordingPoint getPointFromTimestamp(long timestamp)
        {
            return timestampMap[timestamp];
        }

        public static MotionRecording loadFromFile(string filename)
        {
            List<RecordingPoint> points = new List<RecordingPoint>();
            string name = Path.GetFileNameWithoutExtension(filename);
            using (StreamReader r = new StreamReader(filename))
            {
                while (!r.EndOfStream)
                {
                    string line = r.ReadLine();
                    string[] toks = line.Split(' ');
                    long time = long.Parse(toks[0]);
                    double[,] pts = new double[toks.Length - 1, 3];
                    int numPts = (toks.Length - 1) / 3;
                    for (int idx = 0; idx < numPts; idx++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            pts[idx, j] = double.Parse(toks[1 + 3 * idx + j]);
                        }
                    }
                    RecordingPoint rp = new RecordingPoint(time, pts);
                    points.Add(rp);
                }
            }
            return new MotionRecording(points, name);
        }

        public void saveToFile(string filename)
        {
            using (StreamWriter s = new StreamWriter(filename))
            {
                foreach (RecordingPoint r in points)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(r.Time);
                    for (int i = 0; i < r.Data.GetLength(0); i++)
                    {
                        sb.Append(" ");
                        sb.Append(r.Data[i, 0]);
                        sb.Append(" ");
                        sb.Append(r.Data[i, 1]);
                        sb.Append(" ");
                        sb.Append(r.Data[i, 2]);
                    }
                    s.WriteLine(sb.ToString());
                }
            }
        }

        public List<long> getPointTimestamps()
        {
            return timestampMap.Keys.ToList();
        }

        public override string ToString()
        {
            return name;
        }

    }
}
