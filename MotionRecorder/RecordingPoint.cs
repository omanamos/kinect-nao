using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace MotionRecorder
{
    public class RecordingPoint
    {
        public RecordingPoint(long time, double[,] data)
        {
            Time = time;
            Data = data;
        }

        public long Time { get; set; }

        public double[,] Data { get; set; }
    }
}
