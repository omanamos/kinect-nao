using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Accord.Statistics.Models.Markov;
using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Models.Markov.Topology;
using DataStore;
using NaoKinectTest.HumanModel;

namespace Recognizer
{
    class HMMRecognizer
    {
        List<HiddenMarkovModel<MultivariateNormalDistribution>> models;
        SequenceClassifier<MultivariateNormalDistribution> classifier;

        List<string> classes;

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
                this.classes = new List<string>();
                foreach (string s in files)
                {
                    MultivariateNormalDistribution probs = new MultivariateNormalDistribution(4);
                    var hmm = new HiddenMarkovModel<MultivariateNormalDistribution>(new Forward(8), probs);
                    hmm = new HiddenMarkovModel<MultivariateNormalDistribution>(hmm.Transitions, hmm.Emissions, hmm.Probabilities);
                    models.Add(ModelSerializer.deserialize(s));
                    classes.Add(Path.GetFileNameWithoutExtension(s));
                }

                classifier = new SequenceClassifier<MultivariateNormalDistribution>(models.ToArray());
            }
        }

        /// <summary>
        /// Uses a collection of HMMs to recognize which action among a corpus of actions was performed
        /// </summary>
        /// <param name="action">The motion sequence representing the action</param>
        /// <returns></returns>
        public string recognizeAction(ActionSequence<HumanSkeleton> action)
        {
            double[] likelihoods = new double[classifier.Classes];
            int cls = classifier.Compute(action.toArray(), out likelihoods);

            // TODO: maybe threshold on likelihood values?

            return classes[cls];
        }
    }
}
