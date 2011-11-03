using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using DataStore;

namespace NaoPlayer
{
    class Program
    {
        static void Main(string[] args)
        {
            string filename = "Z:/WindowsFolders/Desktop/waveleft/wave left1.rec";

            List<HumanSkeleton> seq = new List<HumanSkeleton>();
            using (StreamReader s = new StreamReader(filename))
            {
                while (!s.EndOfStream)
                {
                    seq.Add(new HumanSkeleton(s.ReadLine()));
                }
            }

            List<NaoSkeleton> naoSeq = new List<NaoSkeleton>();
            foreach (HumanSkeleton h in seq)
            {
                AngleConverter ac = new AngleConverter(h);
                NaoSkeleton ns = ac.getNaoSkeleton();
                naoSeq.Add(ns);
            }
            ActionSequence<NaoSkeleton> naoActs = new ActionSequence<NaoSkeleton>(naoSeq);
            bool isJointAction = true; // false if is a walking action
            ExecuteOnNao(naoActs, isJointAction);
        }

        static void ExecuteOnNao(ActionSequence<NaoSkeleton> actSeq, bool isJointAction)
        {
            // do stuff with naoskeletons and real NAO
        }

    }
}
