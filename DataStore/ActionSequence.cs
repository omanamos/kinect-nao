using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataStore
{
    [Serializable]
    public class ActionSequence<T> where T : ISkeleton , ISerializable
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

        /// <summary>
        /// Returns a TxD matrix of values where value[t][d] is the value of dimension d at timestep t
        /// </summary>
        /// <param name="jointVals">Whether the underlying skeleton should return joints or positions</param>
        /// <returns></returns>
        public double[][] toArray(bool jointVals = false)
        {

            double[][] arr = new double[states.Count][];

            arr[0] = states[0].toArray(jointVals);

            for (int i = 0; i < states.Count; i++)
            {
                arr[i] = states[i].toArray(jointVals);
                for (int j = 0; j < arr[0].Length; j++)
                {
                    arr[i][j] -= arr[0][j];
                }
            }
            return arr;
        }
    }
}
