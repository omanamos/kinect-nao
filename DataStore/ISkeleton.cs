using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataStore
{
    public interface ISkeleton
    {
        double[] toArray(bool useJointVals);
    }
}
