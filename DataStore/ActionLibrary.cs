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

        public ActionLibrary()
        {
            this.actions = new Dictionary<string, ActionSequence<NaoSkeleton>>();
        }

        public Dictionary<string, ActionSequence<NaoSkeleton>>.KeyCollection getCommands()
        {
            return this.actions.Keys;
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
            ActionLibrary al = new ActionLibrary();
            
            Stream stream = File.Open(path, FileMode.Open);
            BinaryFormatter bformatter = new BinaryFormatter();

            al = (ActionLibrary)bformatter.Deserialize(stream);
            stream.Close();

            return al;
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
