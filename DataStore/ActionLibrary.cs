using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataStore
{
    public class ActionLibrary
    {
        private Dictionary<string, ActionSequence<NaoSkeleton>> actions;

        public ActionLibrary()
        {
            this.actions = new Dictionary<string, ActionSequence<NaoSkeleton>>();
        }

        public void mapAction(string name, ActionSequence<NaoSkeleton> seq)
        {
            this.actions[name] = seq;
        }

        public ActionSequence<NaoSkeleton> getSequence(string name)
        {
            if (!this.actions.ContainsKey(name))
            {
                throw new ArgumentException(name + " is an unknown action.");
            }
            return this.actions[name];
        }

        public static ActionLibrary load(String path)
        {
            // TODO(johnson): load from a file
            return null;
        }

        public void save(String path)
        {
            // TODO(johnson): save to a file
        }
    }
}
