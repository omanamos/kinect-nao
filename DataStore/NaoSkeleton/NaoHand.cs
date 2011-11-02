using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataStore
{
    [Serializable]
    public class NaoHand : ISerializable
    {
        private bool opened;

        private NaoHand() { }

        public NaoHand(bool opened)
        {
            this.opened = opened;
        }

        public bool isOpen()
        {
            return this.opened;
        }

        public NaoHand(SerializationInfo info, StreamingContext ctxt)
        {
            this.opened = (bool)info.GetValue("Opened", typeof(bool));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Opened", opened);
        }
    }
}
