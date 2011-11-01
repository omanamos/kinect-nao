using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataStore
{
    public class ActionSequence
    {
        private List<NaoSkeleton> states;
        private int cur;

        private ActionSequence() { }

        public ActionSequence(List<NaoSkeleton> states)
        {
            this.cur = -1;
            this.states = states;
        }

        public bool hasNext()
        {
            return this.cur != states.Count;
        }

        public NaoSkeleton current()
        {
            return this.states[this.cur];
        }
        
        public NaoSkeleton next()
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
