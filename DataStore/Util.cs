using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataStore
{
    class Util
    {
        public static bool isWithinRange(double val, double lowerBound, double upperBound)
        {
            return val > lowerBound && val < upperBound;
        }

        public static double[] toDoubleArray(float[] input)
        {
            if (input == null)
            {
                return null;
            }
            double[] output = new double[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                output[i] = input[i];
            }
            return output;
        }
    }
}
