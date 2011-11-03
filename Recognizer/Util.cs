using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataStore;

namespace Recognizer
{
    class Util
    {
        public static bool shouldUseJVals(string actName)
        {
            string[] jointActs = {"wave right", "wave left", "raise the roof", "macarena"};
            string[] posActs = {"walk forward", "walk backward", "walk left", "walk right"};
            
            bool useJoint = false;
            foreach (string s in jointActs)
            {
                if (actName.IndexOf(s) > -1)
                    useJoint = true;
            }
            return useJoint;
        }

        /// <summary>
        /// Checks if the motion of each joint in the sequence is below a threshold
        /// </summary>
        /// <param name="seq"></param>
        /// <returns></returns>
        public static bool isStableSequence(double[][] seq, double threshold)
        {
            bool belowThreshold = true;
            Console.Write("STDDEV:");
            for (int dim=0; dim < seq[0].Length; dim++)
            {
                IEnumerable<double> s = seq.Select(pt => pt[dim]);
                double stddev = CalculateStdDev(s);
                //Console.WriteLine("Min: " + s.Min());
                //Console.WriteLine("Max: " + s.Max());
                Console.Write(" " + stddev + " ");
                belowThreshold = belowThreshold && (stddev < threshold);
            }
            Console.WriteLine();
            return belowThreshold;

        }

        private static double CalculateStdDev(IEnumerable<double> values)
        {
            double ret = 0;
            if (values.Count() > 0)
            {
                //Compute the Average      
                double avg = values.Average();
                //Perform the Sum of (value-avg)_2_2      
                double sum = values.Sum(d => Math.Pow(d - avg, 2));
                //Put it all together      
                ret = Math.Sqrt((sum) / (values.Count() - 1));
            }
            return ret;
        }
    }
}
