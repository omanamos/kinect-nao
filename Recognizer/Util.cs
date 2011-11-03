using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
