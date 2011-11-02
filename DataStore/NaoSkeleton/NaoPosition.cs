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

        public NaoPosition(SerializationInfo info, StreamingContext ctxt)
        {
            this.X = (float)info.GetValue("X", typeof(float));
            this.Y = (float)info.GetValue("Y", typeof(float));
            this.Z = (float)info.GetValue("Z", typeof(float));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("X", X);
            info.AddValue("Y", Y);
            info.AddValue("Z", Z);
        }
    }
}
