using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataStore
{
    [Serializable]
    public class NaoPosition : ISerializable
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

        public NaoPosition(SerializationInfo info, StreamingContext ctxt)
        {
            this.X = (double)info.GetValue("X", typeof(double));
            this.Y = (double)info.GetValue("Y", typeof(double));
            this.Z = (double)info.GetValue("Z", typeof(double));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("X", X);
            info.AddValue("Y", Y);
            info.AddValue("Z", Z);
        }
    }
}
