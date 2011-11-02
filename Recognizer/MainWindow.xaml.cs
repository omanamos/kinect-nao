using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Accord.Statistics.Models.Markov;
using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Models.Markov.Topology;
using Accord.Statistics.Models.Markov.Learning;
using Accord.Statistics.Distributions.Univariate;
using Accord.Statistics.Distributions.Fitting;
using MotionRecorder;
using System.IO;
using DataStore;

namespace Recognizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void foo()
        {
            double[][] sequences = new double[][] 
            {
                new double[] { 0.1, 5.2, 0.3, 6.7, 0.1, 6.0 },
                new double[] { 0.2, 6.2, 0.3, 6.3, 0.1, 5.0 },
                new double[] { 0.1, 7.0, 0.1, 7.0, 0.2, 5.6 },
            };

            NormalDistribution density = new NormalDistribution();

            var model = new HiddenMarkovModel<NormalDistribution>(new Ergodic(2), density);

            var teacher = new BaumWelchLearning<NormalDistribution>(model)
            {
                Tolerance = 0.0001,
                Iterations = 0,
            };

            double likelihood = teacher.Run(sequences);

            double l1 = model.Evaluate(new[] { 0.1, 5.2, 0.3, 6.7, 0.1, 6.0 }); // 0.87
            double l2 = model.Evaluate(new[] { 0.2, 6.2, 0.3, 6.3, 0.1, 5.0 }); // 1.00

            double l3 = model.Evaluate(new[] { 1.1, 2.2, 1.3, 3.2, 4.2, 1.0 }); // 0.00
        }

        private void foo2()
        {
            double[][][] sequences = new double[][][] 
            {
                new double[][] { new double[] {0.1, 10}, new double[] {5.2, 10}, new double[] {0.3, 10}, new double[] {6.7, 10}, new double[] {0.1, 10}, new double[] {6.0, 10} },
                new double[][] { new double[] {0.2, 0}, new double[] {6.2, 0}, new double[] {0.3, 0}, new double[] {6.3, 0}, new double[] {0.1, 0}, new double[] {5.0, 0} },
                new double[][] { new double[] {0.1, 0}, new double[] {7.0, 0}, new double[] {0.1, 0}, new double[] {7.0, 0}, new double[] {0.2, 0}, new double[] {5.6, 0} },
            };

            MultivariateNormalDistribution density = new MultivariateNormalDistribution(2);

            var model = new HiddenMarkovModel<MultivariateNormalDistribution>(new Ergodic(2), density);

            var teacher = new BaumWelchLearning<MultivariateNormalDistribution>(model)
            {
                Tolerance = 0.0001,
                Iterations = 0,
                // Specify a regularization constant
                FittingOptions = new NormalOptions() { Regularization = 1e-5 }
            };

            double likelihood = teacher.Run(sequences);

            double l1 = model.Evaluate(new double[][] { new double[] { 0.1, 0 }, new double[] { 7.0, 0 }, new double[] { 0.1, 0 }, new double[] { 7.0, 0 }, new double[] { 0.2, 0 }, new double[] { 5.6, 0 } }, true); // 0.87
            double l2 = model.Evaluate(new double[][] { new double[] { 0.2, 0 }, new double[] { 6.2, 0 }, new double[] { 0.3, 0 }, new double[] { 6.3, 0 }, new double[] { 0.1, 0 }, new double[] { 5.0, 0 } }, true); // 1.00

            double l3 = model.Evaluate(new double[][] { new double[] { 1.2, 0 }, new double[] { 1.2, 0 }, new double[] { 1.3, 0 }, new double[] { 1.3, 0 }, new double[] { 1.1, 0 }, new double[] { 1.0, 0 } }, true); // 0.00
        }

        private void train(List<string> datafiles)
        {
            double[][][] sequences = new double[datafiles.Count][][];
            // For each file
            string actName = "";
            string dirName = "";
            for (int i = 0; i < datafiles.Count; i++)
            {
                string fname = datafiles[i];
                actName = System.IO.Path.GetFileNameWithoutExtension(fname);
                dirName = System.IO.Path.GetDirectoryName(fname);
                // Read the file
                List<HumanSkeleton> seq = new List<HumanSkeleton>();
                using (StreamReader s = new StreamReader(fname))
                {
                    while (!s.EndOfStream)
                    {
                        seq.Add(new HumanSkeleton(s.ReadLine()));
                    }
                }
                // convert it into an actionSequence of humanskeletons
                ActionSequence<HumanSkeleton> actSeq = new ActionSequence<HumanSkeleton>(seq);
                // Convert that actionSequence in to a double[][]
                bool useJointVals = shouldUseJVals(actName);
                double[][] trainSeq = actSeq.toArray(useJointVals);
                // add that to the sequences array
                sequences[i] = trainSeq;
            }
            if (datafiles.Count > 0)
            {
                // train a HMM
                HiddenMarkovModel<MultivariateNormalDistribution> hmm = trainHMM(sequences);
                // save it to filename.hmm
                ModelSerializer.serialize(hmm, 
                    System.IO.Path.Combine(dirName, actName + ".hmm"));
            }
        }

        private HiddenMarkovModel<MultivariateNormalDistribution>
            trainHMM(double[][][] sequences)
        {
            int dimensionality = sequences[0][0].GetLength(0);
            var emissionProbs =
                new MultivariateNormalDistribution(dimensionality);
            // Creates a continuous hidden Markov Model with two states organized in a forward
            //  topology and an underlying univariate Normal distribution as probability density.
            var hmm = new HiddenMarkovModel<MultivariateNormalDistribution>(
                    new Forward(8), emissionProbs);
            // Configure the learning algorithms to train the sequence classifier until the
            // difference in the average log-likelihood changes only by as little as 0.0001
            var teacher =
                new BaumWelchLearning<MultivariateNormalDistribution>(hmm)
                {
                    Tolerance = 0.0001,
                    Iterations = 0,
                    // Specify a regularization constant
                    FittingOptions = new NormalOptions() { Regularization = 1e-5 }
                };

            // Train the hmm
            teacher.Run(sequences);
            return hmm;
        }

        private bool shouldUseJVals(string actName)
        {
            switch (actName)
            {
                case "waveright":
                case "waveleft":
                case "raisetheroof":
                case "macarena":
                    return true;
                case "walkforward":
                case "walkbackward":
                case "walkleft":
                case "walkright":
                default:
                    return false;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> train_files = new List<string>();
            train_files.Add("\\\\cseexec.cs.washington.edu\\cs\\nt\\homes\\istudents\\pbrook\\WindowsFolders\\Desktop\\wave2.rec");

            train(train_files);

            /*
            

            // 10 sequences, each of 100pts, in 4 dimensions
            int nseqs = 100;
            double[][][] sequences = new double[nseqs][][];//, 100, 4];

            for (int i = 0; i < nseqs; i++)
            {
                // Fill the sequence with random numbers
                sequences[i] = emissionProbs.Generate(10);

                //sequences[i] = new double[10][];

                // First dimension should linearly increase
                for (int j = 0; j < 10; j++)
                {
                    //sequences[i][j] = new double[4];
                    sequences[i][j][0] = j;
                    sequences[i][j][1] *= 10;
                    sequences[i][j][2] *= 10;
                    sequences[i][j][3] *= 10;
                }
            }

            // Fit the model
            double likelihood = teacher.Run(sequences);

            ModelSerializer.serialize(hmm, "Z:\\WindowsFolders\\Desktop\\hmm.ser");
            HiddenMarkovModel<MultivariateNormalDistribution> hmm2 = ModelSerializer.deserialize("Z:\\WindowsFolders\\Desktop\\hmm.ser");

            Console.WriteLine("Average LL for training sequences: " + likelihood);
            double[][] query1 = emissionProbs.Generate(10);
            for (int i = 0; i < query1.GetLength(0); i++)
            {
                query1[i][0] = i;
                query1[i][1] *= 10;
                query1[i][2] *= 10;
                query1[i][3] *= 10;
            }
            foreach (var em in hmm.Emissions)
            {
                foreach (var m in em.Mean)
                {
                    Console.Write(m + " ");
                }
                foreach (var v in em.Variance)
                {
                    Console.Write(v + " ");
                }
                Console.WriteLine();
            }
            double l1 = hmm.Evaluate(query1, false);
            Console.WriteLine("Likelihood: " + l1);
            double l2c = hmm2.Evaluate(query1, false);
            Console.WriteLine("Likelihood (should be same): " + l2c);


            double[][] query2 = emissionProbs.Generate(10);
            for (int i = 0; i < query2.GetLength(0); i++)
            {
                query2[i][0] = i;
                query2[i][1] *= 10;
                query2[i][1] += 0.00001;
                query2[i][2] *= 10;
                query2[i][3] *= 10;
            }


            double l2 = hmm.Evaluate(query2, false);
            Console.WriteLine("Likelihood: " + l2);
            Console.ReadKey();
             * */
        }
    }
}
