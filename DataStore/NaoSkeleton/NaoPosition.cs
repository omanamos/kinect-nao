using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataStore
{
    public class NaoPosition
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }

        private NaoPosition() { }

        public NaoPosition(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
    }
}
