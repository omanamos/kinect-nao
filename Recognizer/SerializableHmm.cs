/*
 * SerializableHmm.cs
 * 
 * This class is provided for enabling the serialization of the Hidden Markov
 * Models (HMM) available in the Accord.NET framework. With an HMM specified
 * at construction time, this class will be able to serialize the HMM to disk
 * for later deserialization.
 * 
 * NOTE: This class is intended to be used with HMMs that utilize
 *       the type: "MultivariateNormalDistribution".
 * 
 * Author: Johnathan Davis, CSE 481c, 2011
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

// Use serialization.
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

// Use Hidden Markov Models.
using Accord.Statistics.Models.Markov;
using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Analysis;

namespace Recognizer
{
    [Serializable()]
    class SerializableHmm : ISerializable
    {
        private const string SERIALIZATION_PREFIX = @"hmm_"; // Default = "hmm_"
        private const string SERIALIZATION_SUFFIX = @".dat"; // Default = ".dat"

        private string name;
        private string directory;

        private double[,] transitions;

        // mean[0]: which gaussian distribution in the emissions matrix
        // mean[1]: mean vector for this gaussian distribution
        private double[,] mean;

        // covariance[0]: which gaussian distribution in the emissions matrix
        // covariance[1]: which row of the covariance matrix for this gaussian distribution
        // covariance[2]: which column of the covariance matrix for this gaussian distribution
        private double[,,] covariance;

        private double[] probabilities;
        private string actName;
        private HiddenMarkovModel<MultivariateNormalDistribution> hmm;
        private Accord.Statistics.Analysis.PrincipalComponentAnalysis pca;

        /// <summary>
        /// Saves to this object the information necessary for later
        /// reconstructing the given HMM.
        /// </summary>
        /// <param name="name">the name of this HMM</param>
        /// <param name="hmm">the HMM to store</param>
        public SerializableHmm(string name, HiddenMarkovModel<MultivariateNormalDistribution> hmm)
        {
            this.name = name;
            construct(hmm);
        }

        private void construct(HiddenMarkovModel<MultivariateNormalDistribution> hmm)
        {
            // Copy the transitions matrix.
            transitions = hmm.Transitions;

            //
            // Copy the mean vectors and covariance matrices for all the
            // gaussian distributions in the emissions matrix.
            //

            mean = new double[hmm.Emissions.GetLength(0),
                              hmm.Emissions[0].Mean.GetLength(0)];

            covariance = new double[hmm.Emissions.GetLength(0),
                                    hmm.Emissions[0].Covariance.GetLength(0),
                                    hmm.Emissions[0].Covariance.GetLength(1)];

            for (int distribution = 0; distribution < hmm.Emissions.Length; distribution++)
            {
                // Copy the mean vector for this distribution.
                for (int i = 0; i < hmm.Emissions[0].Mean.GetLength(0); i++)
                    mean[distribution, i] = hmm.Emissions[distribution].Mean[i];

                // Copy the covariance matrix for this distribution.
                for (int row = 0; row < hmm.Emissions[0].Covariance.GetLength(0); row++)
                {
                    for (int col = 0; col < hmm.Emissions[0].Covariance.GetLength(1); col++)
                    {
                        covariance[distribution, row, col] = hmm.Emissions[distribution].Covariance[row, col];
                    }
                }
            }

            // Copy the probabilities vector.
            probabilities = hmm.Probabilities;
        }

        /// <summary>
        /// Prepares this object for the deserialization of the HMM
        /// that was previously serialized with the given name.
        /// </summary>
        /// <param name="name">the name of this HMM</param>
        public SerializableHmm(string name, string directory)
        {
            this.name = name;
            this.directory = directory;
        }

        /// <summary>
        /// Saves this HMM representation to disk.
        /// </summary>
        public void SaveToDisk()
        {
            // Ensure path exists.
            if (!Directory.Exists(this.directory))
                Directory.CreateDirectory(this.directory);

            Stream stream = File.Open(this.directory + SERIALIZATION_PREFIX + name + SERIALIZATION_SUFFIX, FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();

            bf.Serialize(stream, this);
            stream.Close();
        }

        /// <summary>
        /// Loads and returns a HMM that was previously serialized to disk.
        /// </summary>
        /// <returns></returns>
        public HiddenMarkovModel<MultivariateNormalDistribution> LoadFromDisk()
        {
            Stream stream = File.Open(this.directory + SERIALIZATION_PREFIX + name + SERIALIZATION_SUFFIX, FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();

            // Deserialize the SerializableHmm into a new instance.
            SerializableHmm otherSHmm = (SerializableHmm)bf.Deserialize(stream);
            pca = otherSHmm.pca;
            stream.Close();

            MultivariateNormalDistribution[] emissions = new MultivariateNormalDistribution[otherSHmm.mean.GetLength(0)];

            // Populate the emissions matrix.
            for (int distribution = 0; distribution < otherSHmm.mean.GetLength(0); distribution++)
            {
                double[] mean = new double[otherSHmm.mean.GetLength(1)];
                double[,] covariance = new double[otherSHmm.covariance.GetLength(1), otherSHmm.covariance.GetLength(2)];

                // Copy the mean vector for this distribution.
                for (int i = 0; i < otherSHmm.mean.GetLength(1); i++)
                    mean[i] = otherSHmm.mean[distribution, i];

                // Copy the covariance matrix for this distribution.
                for (int row = 0; row < otherSHmm.covariance.GetLength(1); row++)
                {
                    for (int col = 0; col < otherSHmm.covariance.GetLength(2); col++)
                    {
                        covariance[row, col] = otherSHmm.covariance[distribution, row, col];
                    }
                }

                emissions[distribution] = new MultivariateNormalDistribution(mean, covariance);
            }

            return new HiddenMarkovModel<MultivariateNormalDistribution>(otherSHmm.transitions, emissions, otherSHmm.probabilities);
        }

        /// <summary>
        /// Deserializes the object.
        /// </summary>
        /// <param name="info">the data necessary to deserialize this object</param>
        /// <param name="ctxt">specifies the serialization stream</param>
        public SerializableHmm(SerializationInfo info, StreamingContext ctxt)
        {
            name = (string)info.GetValue("name", typeof(string));
            transitions = (double[,])info.GetValue("transitions", typeof(double[,]));
            mean = (double[,])info.GetValue("mean", typeof(double[,]));
            covariance = (double[,,])info.GetValue("covariance", typeof(double[,,]));
            probabilities = (double[])info.GetValue("probabilities", typeof(double[]));
            try
            {
                pca = (PrincipalComponentAnalysis)(info.GetValue("pca", typeof(PrincipalComponentAnalysis)));
            }
            catch (System.Runtime.Serialization.SerializationException ex)
            {
                pca = null;
            }
        }

        public SerializableHmm(string actName, HiddenMarkovModel<MultivariateNormalDistribution> hmm, Accord.Statistics.Analysis.PrincipalComponentAnalysis pca)
        {
            this.pca = pca;
            this.name = actName;
            construct(hmm);
        }

        /// <summary>
        /// Specifies the attributes that should be serialized.
        /// </summary>
        /// <param name="info">the data necessary to deserialize this object</param>
        /// <param name="ctxt">specifies the serialization stream</param>
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("name", name);
            info.AddValue("transitions", transitions);
            info.AddValue("mean", mean);
            info.AddValue("covariance", covariance);
            info.AddValue("probabilities", probabilities);
            info.AddValue("pca", pca);
        }

        internal Accord.Statistics.Analysis.PrincipalComponentAnalysis getPCA()
        {
            return pca;
        }
    }
}
