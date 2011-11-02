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
        private Dictionary<string, ActionSequence<NaoSkeleton>> actions;

        public ActionLibrary()
        {
            this.actions = new Dictionary<string, ActionSequence<NaoSkeleton>>();
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
            this.actions = (Dictionary<String, ActionSequence<NaoSkeleton>>)info.GetValue("actions",
                typeof(Dictionary<String, ActionSequence<NaoSkeleton>>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("actions", actions);
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
