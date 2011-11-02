using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataStore
{
    [Serializable]
    public class ActionSequence<E> : ISerializable
    {
        private List<E> states;
        private int cur;

        private ActionSequence() { }

        public ActionSequence(List<E> states)
        {
            this.cur = -1;
            this.states = states;
        }

        public bool hasNext()
        {
            return this.cur != states.Count;
        }

        public E current()
        {
            return this.states[this.cur];
        }
        
        public E next()
        {
            this.cur++;
            return this.states[this.cur];
        }

        public void reset()
        {
            this.cur = -1;
        }

        public ActionSequence(SerializationInfo info, StreamingContext ctxt)
        {
            this.states = (List<E>)info.GetValue("states", typeof(List<E>));
            this.cur = (int)info.GetValue("cur", typeof(int));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("states", states);
            info.AddValue("cur", cur);
        }
    }
}
