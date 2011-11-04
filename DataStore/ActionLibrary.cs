using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace DataStore
{
    [Serializable]
    public class ActionLibrary : ISerializable
    {
        private Dictionary<String, ActionSequence<NaoSkeleton>> actions;
        private ActionSequence<NaoSkeleton> cachedStates;
        private String cachedName;

        public ActionLibrary()
        {
            this.actions = new Dictionary<string, ActionSequence<NaoSkeleton>>();
            this.cachedStates = new ActionSequence<NaoSkeleton>();
            this.cachedName = null;
        }

        public Dictionary<string, ActionSequence<NaoSkeleton>>.KeyCollection getActionNames()
        {
            return this.actions.Keys;
        }

        public bool saveCache()
        {
            if (this.cachedName != null && !this.cachedStates.isEmpty())
            {
                this.actions[this.cachedName] = this.cachedStates;
                this.clearCache();
                return true;
            }
            return false;
        }

        public String getCachedName()
        {
            return this.cachedName;
        }

        public void setCachedName(String name)
        {
            this.cachedName = name;
        }

        public void appendToCache(ActionSequence<NaoSkeleton> action)
        {
            this.cachedStates.append(action);
        }

        public void clearCache()
        {
            this.cachedName = null;
            this.cachedStates = new ActionSequence<NaoSkeleton>();
        }

        public void mapAction(string name, ActionSequence<NaoSkeleton> seq)
        {
            this.actions[name] = seq;
        }

        public ActionSequence<NaoSkeleton> getSequence(string name)
        {
            if (!this.actions.ContainsKey(name))
            {
                throw new ArgumentException(name + " is an unknown action.");
            }
            return this.actions[name];
        }

        public ActionLibrary(SerializationInfo info, StreamingContext ctxt)
        {
            this.cachedStates = new ActionSequence<NaoSkeleton>();
            this.cachedName = null;
            // retrieving keys and values separately as dictionary is not serializable
            String[] keys = (String[])info.GetValue("keys", typeof(String[]));
            ActionSequence<NaoSkeleton>[] values = (ActionSequence<NaoSkeleton>[])info.GetValue("values",
                typeof(ActionSequence<NaoSkeleton>[]));

            // reconstructs the dictionary
            Dictionary<String, ActionSequence<NaoSkeleton>> actDict = new Dictionary<string, ActionSequence<NaoSkeleton>>();
            for (int i = 0; i < keys.Length; i++)
            {
                actDict.Add(keys[i], values[i]);
            }
            this.actions = actDict;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            int dictSize = actions.Count;

            String[] keys = new String[dictSize];
            ActionSequence<NaoSkeleton>[] values = new ActionSequence<NaoSkeleton>[dictSize];

            // serializing dictionary separately because a dictionary is not serializable
            int i = 0;
            foreach (KeyValuePair<String, ActionSequence<NaoSkeleton>> pair in actions)
            {
                keys[i] = pair.Key;
                values[i] = pair.Value;
                i++;
            }

            info.AddValue("keys", keys);
            info.AddValue("values", values);
        }

        public static ActionLibrary load(String path)
        {
            /*ActionLibrary al = new ActionLibrary();
            
            Stream stream = File.Open(path, FileMode.Open);
            BinaryFormatter bformatter = new BinaryFormatter();

            al = (ActionLibrary)bformatter.Deserialize(stream);
            stream.Close();

            return al;*/


            Dictionary<string, string> actions = new Dictionary<string, string>();
            actions["raise the roof"] = "raisetheroof";
            actions["walk forward"] = "walkforward";
            actions["walk left"] = "walkleft";
            actions["walk back"] = "walkbackward";
            actions["walk right"] = "walkright";
            actions["wave right"] = "waveright";
            actions["wave left"] = "waveleft";
            string filename = "Z:/dev/kinect-nao/recordings/";
            ActionLibrary lib = new ActionLibrary();

            foreach (string actionName in actions.Keys)
            {
                List<HumanSkeleton> seq = new List<HumanSkeleton>();
                using (StreamReader s = new StreamReader(filename + actions[actionName] + "/"
                    + actionName + "1.rec"))
                {
                    while (!s.EndOfStream)
                    {
                        seq.Add(new HumanSkeleton(s.ReadLine()));
                    }
                }

                ActionSequence<NaoSkeleton> naoSeq = new ActionSequence<NaoSkeleton>();
                foreach (HumanSkeleton h in seq)
                {
                    AngleConverter ac = new AngleConverter(h);
                    naoSeq.append(ac.getNaoSkeleton());
                }

                if (actionName.StartsWith("walk"))
                {
                    NaoSkeleton start = naoSeq.get(0);
                    NaoSkeleton end = naoSeq.get(naoSeq.size() - 1);
                    NaoSkeleton diff = new NaoSkeleton(new NaoPosition(end.Position.X - start.Position.X, end.Position.Y - start.Position.Y, end.Position.Z - start.Position.Z));
                    naoSeq = new ActionSequence<NaoSkeleton>();
                    naoSeq.append(diff);
                }
                lib.appendToCache(naoSeq);
                lib.setCachedName(actionName);
                lib.saveCache();
            }

            return lib;
        }

        public void save(String path)
        {
            Stream stream = File.Open(path, FileMode.Create);
            BinaryFormatter bformatter = new BinaryFormatter();

            bformatter.Serialize(stream, this);
            stream.Close();
        }
    }
}
