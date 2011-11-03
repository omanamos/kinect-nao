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
            bool doTrain = true;
            if (doTrain)
            {
                if (datafiles.Count > 0)
                {
                    // train a HMM
                    HiddenMarkovModel<MultivariateNormalDistribution> hmm = trainHMM(sequences);
                    // save it to filename.hmm
                    SerializableHmm s = new SerializableHmm(actName, hmm);
                    s.SaveToDisk();
                }
            }
            else
            {
                SerializableHmm ser = new SerializableHmm("wave right");
                HiddenMarkovModel<MultivariateNormalDistribution> hmm = ser.LoadFromDisk();

                foreach (double[][] seq in sequences)
                {
                    double l = hmm.Evaluate(seq, false);
                    Console.WriteLine("Likelihood: " + l);
                }
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
                    Iterations = 500,
                    // Specify a regularization constant
                    FittingOptions = new NormalOptions() { Regularization = 1e-4 }
                };

            // Train the hmm
            teacher.Run(sequences);
            return hmm;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            List<string> train_files = new List<string>();
            
            /*
            for (int i = 1; i <= 5; i++)
            {
                train_files.Add(@"Z:/WindowsFolders/Desktop/raisetheroof/raise the roof" + i + ".rec");
            }
            */
            
            for (int i = 1; i <= 10; i++)
            {
                train_files.Add(@"Z:/WindowsFolders/Desktop/waveleft/wave left" + i + ".rec");
            }
            
            /*
            train_files.Add(@"Z:/WindowsFolders/Desktop/walk forward11.rec");
            train_files.Add(@"Z:/WindowsFolders/Desktop/walk forward12.rec");
            train_files.Add(@"Z:/WindowsFolders/Desktop/walk forward13.rec");
            */

            train(train_files);
            /*
            t.Elapsed += new ElapsedEventHandler(t_Elapsed);
            t.Start();*/
        }

        void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            recognize();
        }
        System.Timers.Timer t = new System.Timers.Timer(1000 / 30);
        HMMRecognizer rec = new HMMRecognizer(@"Z:/WindowsFolders/MyDocs/Capstone/NaoKinectTest/Recognizer/HmmData");
        // start thread creating actionSequences
        KinectSequencer sequencer = new KinectSequencer();
        bool gotAction = false;
        string act = "";
        private void recognize()
        {
            if (!gotAction)
            {
                // if there is a sequence available
                if (sequencer.SequenceAvailable)
                {
                    // grab the latest sequence
                    ActionSequence<HumanSkeleton> seq = sequencer.getLatestSequence();
                    // perform recognition
                    Tuple<string, double> guess = rec.recognizeAction(seq);
                    act = guess.Item1;
                    Console.WriteLine("Action: " + act + " L: " + guess.Item2);
                    Dispatcher.BeginInvoke(new Action(
                        delegate(){ 
                            action.Text = act;
                            score.Text = guess.Item2.ToString();
                            if (act == "")
                            {
                                score.Foreground = new SolidColorBrush(Colors.Red);
                            }
                            else
                            {
                                score.Foreground = new SolidColorBrush(Colors.Green);
                            }
                        }));
                }
            }
        }

    }
}
