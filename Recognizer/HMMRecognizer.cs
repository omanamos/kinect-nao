using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Accord.Statistics.Models.Markov;
using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Models.Markov.Topology;

namespace Recognizer
{
    class HMMRecognizer
    {
        List<HiddenMarkovModel<MultivariateNormalDistribution>> models;
        SequenceClassifier<MultivariateNormalDistribution> classifier;

        /// <summary>
        /// Creates a new HMMRecognizer by loading all of the stored models in the given directory
        /// </summary>
        /// <param name="modelDirectoryName"></param>
        public HMMRecognizer(string modelDirectoryName)
        {
            loadAllModels(modelDirectoryName);
        }

        private void loadAllModels(string modelDirectoryName)
        {
            if (Directory.Exists(modelDirectoryName))
            {
                // Get all files of type .rec in the directory
                string[] files = Directory.GetFiles(modelDirectoryName, "*.hmm");
                this.models = new List<HiddenMarkovModel<MultivariateNormalDistribution>>();
                foreach (string s in files)
                {
                    MultivariateNormalDistribution probs = new MultivariateNormalDistribution(4);
                    var hmm = new HiddenMarkovModel<MultivariateNormalDistribution>(new Forward(8), probs);
                    hmm = new HiddenMarkovModel<MultivariateNormalDistribution>(hmm.Transitions, hmm.Emissions, hmm.Probabilities);
                    models.Add(ModelSerializer.deserialize(s));
                }
            }
        }
    }
}
