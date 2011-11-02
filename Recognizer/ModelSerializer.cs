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
                // model.Probabilities
                double[] prob = model.Probabilities;

                Console.WriteLine("prob:");
                Console.WriteLine(prob[0]);

                string probDelimitedStr = createDelimitedStringOneDim(prob, '|');

                // model Transitions
                double[,] trans = model.Transitions;
                string transDelimitedStr = createDelimitedStringTwoDim(trans, '|', '*');

                w.Write(probDelimitedStr);
                Console.WriteLine(probDelimitedStr);
                w.Write("#");
                w.Write(transDelimitedStr);
                w.Write("#");

                // model.Emissions
                MultivariateNormalDistribution[] multiVNormD = model.Emissions;


                // prob # trans # emissions[]
                //                emissions[0]                  $ emissions[1] $ emissions[2]
                //                means[]     &    covariance[] $
                //                1.1|2.2|3.3 & 2.2|4.4*7.7|8.8 $
                for (int i = 0; i < multiVNormD.Length; i++)
                {
                    double[] means = multiVNormD[i].Mean;
                    string meansDelimitedStr = createDelimitedStringOneDim(means, '|');

                    double[,] covariance = multiVNormD[i].Covariance;
                    string covarianceDelimitedStr = createDelimitedStringTwoDim(covariance, '|', '*');

                    w.Write(meansDelimitedStr);
                    w.Write("&");
                    w.Write(covarianceDelimitedStr);

                    // fence post, no $ on last one
                    if (i < multiVNormD.Length - 1)
                    {
                        w.Write("$");
                    }
                }
            }
        }

        public static HiddenMarkovModel<MultivariateNormalDistribution> deserialize(string filename)
        {
            using (StreamReader r = new StreamReader(filename))
            {
                String data = r.ReadToEnd();
                String[] dataArr = data.Split('#');

                // probDelimitedStr dataArr[0]
                double[] prob = createSingleDimDoubleArray(dataArr[0], '|');

                // transDelimitedStr dataArr[1]
                double[,] trans = createDoubleDimDoubleArray(dataArr[1], '|', '*');

                // emissionsDelimitedStr dataArr[2]
                String[] emissions = dataArr[2].Split('$');

                MultivariateNormalDistribution[] e2 = new MultivariateNormalDistribution[emissions.Length];
                for (int i = 0; i < emissions.Length; i++)
                {
                    String[] meansNCovariance = emissions[i].Split('&');
                    String meansStr = meansNCovariance[0];
                    String covarianceStr = meansNCovariance[1];

                    double[] means = createSingleDimDoubleArray(meansStr, '|');
                    double[,] covariance = createDoubleDimDoubleArray(covarianceStr, '|', '*');
                    MultivariateNormalDistribution dist = new MultivariateNormalDistribution(means, covariance);
                    e2[i] = dist;
                }
                HiddenMarkovModel<MultivariateNormalDistribution> hmm = new HiddenMarkovModel<MultivariateNormalDistribution>(trans, e2, prob);
                return hmm;   
            }
        }

        private static double[] createSingleDimDoubleArray(String data, char delim)
        {
            String[] arr = data.Split(delim);
            int totalNum = arr.Length;
            double[] ans = new double[totalNum];

            for (int i = 0; i < totalNum; i++)
            {
                ans[i] = Convert.ToDouble(arr[i]);
            }

            return ans;
        }

        private static double[,] createDoubleDimDoubleArray(String data, char delim1, char delim2)
        {
            String[] rows = data.Split(delim2);

            int rowTotal = rows.Length;
            int colTotal = rows[0].Split(delim1).Length;

            double[,] ans = new double[rowTotal, colTotal];

            for (int row = 0; row < rowTotal; row++)
            {
                String[] rowElements = rows[row].Split(delim1);
                for (int col = 0; col < colTotal; col++)
                {
                    ans[row, col] = Convert.ToDouble(rowElements[col]);
                }
            }

            return ans;
        }

        // [4.5, 45.6, 34.9] returns 4.5|45.6|34.9
        private static String createDelimitedStringOneDim(double[] arr, char delim)
        {
            String ans = "";

            
            Console.WriteLine(arr[1]);

            for (int i = 0; i < arr.Length - 1; i++) {
                Console.WriteLine("before ans: " + ans);
                ans += arr[i] + delim;
                Console.WriteLine("after ans: " + ans);
            }
            // fence post, adding last element without delimiter
            // 4.5|45.6|34.9
            ans += arr[arr.Length - 1];

            return ans;
        }


        //delim1 is used to separate between elements in the same dimension
        //delim2 is used to separate between the dimensions
        // [1 2 3]
        // [4 5 6]
        // [7 8 9]
        // delim1 = |
        // delim2 = *
        // returns 1|2|3*4|5|6*7|8|9
        private static String createDelimitedStringTwoDim(double[,] arr, char delim1, char delim2)
        {
            String ans = "";

            int totalRows = arr.GetLength(0);
            int totalColumns = arr.GetLength(1);

            // loops over the rows
            for (int row = 0; row < totalRows; row++)
            {
                for (int col = 0; col < totalColumns; col++)
                {
                    ans += arr[row, col] + delim1;
                }
                // fence post, remove extra delim1;
                ans = ans.Substring(0, ans.Length - 1);

                ans += delim2;
            }

            // fence post, remove extra delim2;
            ans = ans.Substring(0, ans.Length - 1);

            return ans;
        }
    }
}
