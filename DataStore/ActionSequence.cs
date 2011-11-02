using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataStore
{
    public class ActionSequence<E>
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
    }
}
