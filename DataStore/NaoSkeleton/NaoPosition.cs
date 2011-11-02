using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataStore
{
    public class NaoPosition
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }

        private NaoPosition() { }

        public NaoPosition(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
    }
}
