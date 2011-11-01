using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataStore
{
    public class ActionLibrary
    {
        private Dictionary<String, ActionSequence> actions;

        public ActionLibrary()
        {
            this.actions = new Dictionary<string, ActionSequence>();
        }

        public void mapAction(string name, ActionSequence seq)
        {
            this.actions[name] = seq;
        }

        public ActionSequence getSequence(string name)
        {
            if (!this.actions.ContainsKey(name))
            {
                throw new ArgumentException(name + " is an unknown action.");
            }
            return this.actions[name];
        }
    }
}
