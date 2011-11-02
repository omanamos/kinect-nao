using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Models.Markov;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Recognizer
{
    static class ModelSerializer
    {
        public static void serialize(HiddenMarkovModel<MultivariateNormalDistribution> model, string filename)
        {
            // want model.Probabilities
            // model.Transitions
            // model.Emissions
            /// For each emission em
            /// want em.Mean
            /// em.Covariance
            using (StreamWriter w = new StreamWriter(filename))
            {

            }
 
        }

        public static HiddenMarkovModel<MultivariateNormalDistribution> deserialize(string filename)
        {
            using (StreamReader r = new StreamReader(filename))
            {
                string line = r.ReadLine();
                // read each line...
            }
            
            return h;
        }
    }
}
