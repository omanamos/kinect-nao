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
using Recognizer;
using System.Threading;
using System.Timers;
using Accord.Statistics.Analysis;

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
                bool useJointVals = Util.shouldUseJVals(actName);
                double[][] trainSeq = actSeq.toArray(useJointVals);
                // add that to the sequences array
                sequences[i] = trainSeq;
            }
                
            bool doTrain = false;
            if (doTrain)
            {
                if (datafiles.Count > 0)
                {
                    PrincipalComponentAnalysis pca = computePCA(sequences);
                    // train a HMM
                    double[][][] projSeqs = getProjectedSequences(sequences, pca);
                    HiddenMarkovModel<MultivariateNormalDistribution> hmm = trainHMM(projSeqs);
                    // save it to filename.hmm
                    SerializableHmm s = new SerializableHmm(actName, hmm, pca);
                    s.SaveToDisk();
                }
            }
            else
            {
                SerializableHmm ser = new SerializableHmm("wave right");
                HiddenMarkovModel<MultivariateNormalDistribution> hmm = ser.LoadFromDisk();
                PrincipalComponentAnalysis pca = ser.getPCA();

                foreach (double[][] seq in sequences)
                {
                    double[,] data = jaggedToMulti(seq);
                    string fn = System.IO.Path.GetRandomFileName();
                    using (StreamWriter sr = new StreamWriter("Z:/WindowsFolders/Desktop/" + fn))
                    {
                        for (int i = 0; i < data.GetLength(0); i++)
                        {
                            for (int j = 0; j < data.GetLength(1); j++)
                            {
                                sr.Write(data[i, j] + " ");
                            }
                            sr.WriteLine();
                        }
                    }
                    //double[][] projSeq = getProjectedSequence(seq, pca);
                    double l = hmm.Evaluate(seq, false);
                    Console.WriteLine("Likelihood: " + l);
                }
            }
            
        }

        private PrincipalComponentAnalysis computePCA(double[][][] sequences)
        {
            PrincipalComponentAnalysis pca;
            // Create combined array for computing PCA

            int numTotalRows = sequences.Select(e => e.GetLength(0)).Sum();

            double[][] pcaCombined = new double[numTotalRows][];
            int total = 0;
            for (int i = 0; i < sequences.GetLength(0); i++)
            {
                for (int j = 0; j < sequences[i].GetLength(0); j++)
                {
                    pcaCombined[total + j] = sequences[i][j];
                }
                total += sequences[i].GetLength(0);
            }

            // PCA
            double[,] pcaCombinedMulti = jaggedToMulti(pcaCombined);
            pca = new PrincipalComponentAnalysis(pcaCombinedMulti);
            pca.Compute();

            return pca;
        }

        private double[][] getProjectedSequence(double[][] sequence, PrincipalComponentAnalysis pca)
        {
            if (pca == null)
                return sequence;

            int numComponents = pca.GetNumberOfComponents(1.0f);

            double[,] data = jaggedToMulti(sequence);
            string fn = System.IO.Path.GetRandomFileName();
            using (StreamWriter sr = new StreamWriter("Z:/WindowsFolders/Desktop/" + fn))
            {
                for (int i = 0; i < data.GetLength(0); i++)
                {
                    for (int j = 0; j < data.GetLength(1); j++)
                    {
                        sr.Write(data[i, j] + " ");
                    }
                    sr.WriteLine();
                }
            }
            double[,] projectedData = pca.Transform(data, numComponents);
            double[][] projTrainSeq = multiToJagged(projectedData);

            return projTrainSeq;
        }

        private double[][][] getProjectedSequences(double[][][] sequences, PrincipalComponentAnalysis pca)
        {
            int nseqs = sequences.GetLength(0);
            double[][][] projSeqs = new double[nseqs][][];
            for (int i = 0; i < nseqs; i++)
            {
                projSeqs[i] = getProjectedSequence(sequences[i], pca);
            }
            return projSeqs;
        }


        private double[,] jaggedToMulti(double[][] inarry)
        {
            int rows = inarry.GetLength(0);
            int cols = inarry[0].GetLength(0);
            double[,] outArry = new double[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    outArry[i, j] = inarry[i][j];
                }
            }
            return outArry;
        }

        private double[][] multiToJagged(double[,] inarry)
        {
            int rows = inarry.GetLength(0);
            int cols = inarry.GetLength(1);
            double[][] outArry = new double[rows][];
            for (int i = 0; i < rows; i++)
            {
                outArry[i] = new double[cols];
                for (int j = 0; j < cols; j++)
                {
                    outArry[i][j] = inarry[i, j];
                }
            }
            return outArry;
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
                    Iterations = 500,
                    // Specify a regularization constant
                    FittingOptions = new NormalOptions() { Regularization = 1e-4 }
                };

            // Train the hmm
            double ll = teacher.Run(sequences);
            Console.WriteLine("Average likelihood during training: " + Math.Exp(ll));
            return hmm;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            List<string> train_files = new List<string>();
            /*
            for (int i = 1; i <= 5; i++)
            {
                train_files.Add(@"Z:/WindowsFolders/Desktop/raisetheroof/raise the roof" + i + ".rec");
            }*/
            
            for (int i = 1; i <= 6; i++)
            {
                train_files.Add(@"Z:/WindowsFolders/Desktop/raisetheroof/raise the roof" + i + ".rec");
            }
            /*
            for (int i = 11; i <= 14; i++)
            {
                train_files.Add(@"Z:/WindowsFolders/Desktop/waveright2/wave right" + i + ".rec");
            }*/
            
            /*
            train_files.Add(@"Z:/WindowsFolders/Desktop/walk forward11.rec");
            train_files.Add(@"Z:/WindowsFolders/Desktop/walk forward12.rec");
            train_files.Add(@"Z:/WindowsFolders/Desktop/walk forward13.rec");
            */

            train(train_files);
            
            HumanActionRecognizer har = new HumanActionRecognizer();
            har.Recognition += new HumanActionRecognizer.RecognitionEventHandler(har_Recognition);
            har.RecordingStart += new HumanActionRecognizer.RecordingEventHandler(har_RecordingStart);
            har.RecordingReady += new HumanActionRecognizer.RecordingEventHandler(har_RecordingReady);
            har.RecordingStop += new HumanActionRecognizer.RecordingEventHandler(har_RecordingStop);
            har.start();
            
        }

        void har_RecordingReady(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(
                delegate()
                {
                    recording_indicator.Background = new SolidColorBrush(Colors.Green);
                }));
        }

        void har_Recognition(object sender, RecognitionEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(
                    delegate()
                    {
                        action.Text = e.Class;
                        score.Text = e.Likelihood.ToString();
                        if (e.Class == "")
                        {
                            score.Foreground = new SolidColorBrush(Colors.Red);
                        }
                        else
                        {
                            score.Foreground = new SolidColorBrush(Colors.Green);
                        }
                    }));
        }

        void har_RecordingStart(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(
                delegate()
                {
                    recording_indicator.Background = new SolidColorBrush(Colors.Red);
                }));
        }

        void har_RecordingStop(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(
            delegate()
            {
                recording_indicator.Background = new SolidColorBrush(Colors.Black);
            }));
        }
    }
}
