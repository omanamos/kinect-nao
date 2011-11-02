using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataStore
{
    public class ActionLibrary<T> where T : ISkeleton
    {
        private Dictionary<String, ActionSequence<T>> actions;

        public ActionLibrary()
        {
            this.actions = new Dictionary<string, ActionSequence<T>>();
        }

        public void mapAction(string name, ActionSequence<T> seq)
        {
            this.actions[name] = seq;
        }

        public ActionSequence<T> getSequence(string name)
        {
            if (!this.actions.ContainsKey(name))
            {
                throw new ArgumentException(name + " is an unknown action.");
            }
            return this.actions[name];
        }
    }
}
