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
    }
}
