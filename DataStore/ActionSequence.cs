using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataStore
{
    public class ActionSequence<T> where T : ISkeleton
    {
        private List<T> states;
        private int cur;

        private ActionSequence() { }

        public ActionSequence(List<T> states)
        {
            this.cur = -1;
            this.states = states;
        }

        public bool hasNext()
        {
            return this.cur != states.Count;
        }

        public T current()
        {
            return this.states[this.cur];
        }

        public T next()
        {
            this.cur++;
            return this.states[this.cur];
        }

        public void reset()
        {
            this.cur = -1;
        }

        /// <summary>
        /// Returns a TxD matrix of values where value[t][d] is the value of dimension d at timestep t
        /// </summary>
        /// <param name="jointVals">Whether the underlying skeleton should return joints or positions</param>
        /// <returns></returns>
        public double[][] toArray(bool jointVals = false)
        {

            double[][] arr = new double[states.Count][];

            for (int i = 0; i < states.Count; i++)
            {
                arr[i] = states[i].toArray(jointVals);
            }
            return arr;
        }
    }
}
