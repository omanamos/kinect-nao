using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Speech.Recognition;

using DataStore;

namespace Controller
{
    public class MainController
    {
        public static readonly String ACTION_LIB_PATH = "temp";

        private enum State { start, har, perform, learn };
        private State state;
        private VoiceRecogition recog;

        private Dictionary<State, Mapping> map;

        private List<string> actions;
        private ActionLibrary lib;

        private ActionController nao;
        
        public MainController()
        {
            init();

            map = new Dictionary<State, Mapping>();
            map[State.start] = new Mapping("nao", new List<string>() { 
                    "watch me", "listen", "learn this", "cancel", "exit" });
            map[State.har] = null;
            map[State.perform] = new Mapping("perform", new List<string>(actions) { "I'm done" });
            map[State.learn] = new Mapping("nao", new List<string>(actions) { "I'm done" });

            //this.nao = new ActionController();

            this.recog = new VoiceRecogition(map[State.start].prefix, map[State.start].list, this);
        }

        public void processCommand(string command)
        {
            Mapping mapping = map[this.state];
            if (mapping.contains(command))
            {
                // TODO(namos): make the NAO say an error
            }
            else
            {
                switch (this.state)
                {
                    case State.start:
                        string suffix = command.Replace(mapping.prefix, "").Trim();
                        processStartCommand(suffix);
                        break;
                    case State.learn:

                        break;
                    case State.perform:
                        break;
                    default: case State.har:
                        break;
                }
            }
        }

        private void processStartCommand(string suffix)
        {
            if (suffix.Equals("watch me"))
            {
                this.state = State.har;
            }
            else if (suffix.Equals("listen"))
            {
                this.state = State.perform;
            }
            else if (suffix.Equals("learn this"))
            {
                this.state = State.learn;
            }
            else if (suffix.Equals("cancel"))
            {
                this.state = State.start;
            }
            else if (suffix.Equals("exit"))
            {
                this.exit();
            }
            else
            {
                // TODO(namos): have Nao say error message
            }
        }

        private void init()
        {
            this.lib = ActionLibrary.load(ACTION_LIB_PATH);
            this.actions = new List<string>();
            this.state = State.start;
            // load actions
            // load model library
            // load action library
        }

        public void exit()
        {
            this.save();
            this.recog.exit();
            this.nao.exit();
            Environment.Exit(0);
        }

        private void save()
        {
            // save model library
            // save action library
        }

        private class Mapping {
            public readonly string prefix;
            public readonly List<string> list;
            private HashSet<string> lookup;

            private Mapping() {}

            public Mapping(string prefix, List<string> list)
            {
                this.prefix = prefix;
                this.list = list;
                this.lookup = new HashSet<string>(list);
            }

            public bool contains(string command)
            {
                if (command.StartsWith(prefix)) {
                    return this.lookup.Contains(command.Replace(prefix, "").Trim());
                } else {
                    return false;
                }
            }
        }
    }
}
