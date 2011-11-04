using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Accord.Statistics.Models.Markov;
using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Models.Markov.Topology;
using DataStore;

namespace Recognizer
{
    public class HMMRecognizer
    {
        List<HiddenMarkovModel<MultivariateNormalDistribution>> positionModels;
        List<HiddenMarkovModel<MultivariateNormalDistribution>> jointModels;
        
        SequenceClassifier<MultivariateNormalDistribution> jointActionClassifier;
        SequenceClassifier<MultivariateNormalDistribution> positionActionClassifier;

        List<string> positionClasses;
        List<string> jointClasses;

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
                string[] files = Directory.GetFiles(modelDirectoryName, "*.dat");
                this.positionModels = new List<HiddenMarkovModel<MultivariateNormalDistribution>>();
                this.jointModels = new List<HiddenMarkovModel<MultivariateNormalDistribution>>();
                this.positionClasses = new List<string>();
                this.jointClasses = new List<string>();
                foreach (string s in files)
                {
                    string actName = Path.GetFileNameWithoutExtension(s);
                    // strip off the hmm_ from the beginning
                    actName = actName.Substring(4);
                    bool isJointAction = Util.shouldUseJVals(actName);
                    
                    // Deserialize the model and add it to the correct classifier
                    SerializableHmm serMod = new SerializableHmm(actName, modelDirectoryName);
                    HiddenMarkovModel<MultivariateNormalDistribution> hmm = serMod.LoadFromDisk();
                    if (isJointAction)
                    {
                        jointModels.Add(hmm);
                        jointClasses.Add(actName);
                    }
                    else
                    {
                        positionModels.Add(hmm);
                        positionClasses.Add(actName);
                    }
                }

                jointActionClassifier = new SequenceClassifier<MultivariateNormalDistribution>(jointModels.ToArray());
                positionActionClassifier = new SequenceClassifier<MultivariateNormalDistribution>(positionModels.ToArray());
                // TODO(pbrook) if there is time we should add a threshold model to these SequenceClassifiers
            }
        }

        /// <summary>
        /// Uses a collection of HMMs to recognize which action among a corpus of actions was performed
        /// </summary>
        /// <param name="action">The motion sequence representing the action</param>
        /// <returns></returns>
        public Tuple<string, double> recognizeAction(ActionSequence<HumanSkeleton> action)
        {
            // Compute most likely position action
            double[] likelihoods = new double[positionActionClassifier.Classes];
            int cls = positionActionClassifier.Compute(action.toArray(), out likelihoods);
            double maxPositionLikelihood = likelihoods.Max();
            //maxPositionLikelihood = 0;
            // Compute most likely joint action
            likelihoods = new double[jointActionClassifier.Classes];
            int jointCls = jointActionClassifier.Compute(action.toArray(true), out likelihoods);
            double maxJointLikelihood = likelihoods.Max();

            double maxLikelihood = Math.Max(maxJointLikelihood, maxPositionLikelihood);

            if (maxLikelihood < 1e-300 || (jointCls == -1 && cls == -1))
            {
                return Tuple.Create("", maxLikelihood);
            }

            // And return the one which is more likely among the two
            if (maxJointLikelihood > maxPositionLikelihood)
            {
                return Tuple.Create(jointClasses[jointCls], maxJointLikelihood);
            }
            else
            {
                return Tuple.Create(positionClasses[cls], maxPositionLikelihood);
            }
        }
    }
}
