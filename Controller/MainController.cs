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
                    "watch me", "perform this", "learn this", "exit" });
            map[State.har] = new Mapping("nao", new List<string>() { "go back" });
            map[State.perform] = new Mapping("perform", new List<string>(actions) { "go back" });
            map[State.learn] = new Mapping("nao", new List<string>(actions) { "go back", "restart" });

            //this.nao = new ActionController();

            switchStates(State.start);
        }

        private void switchStates(State state)
        {
            this.state = state;
            if (this.recog != null)
            {
                this.recog.exit();
            }
            this.recog = new VoiceRecogition(map[state].prefix, map[state].list, this);
        }

        public void processCommand(string command)
        {
            Mapping mapping = map[this.state];
            if (!mapping.contains(command))
            {
                Console.WriteLine("\"" + command + "\" is not a valid command");
                // TODO(namos): make the NAO say an error
            }
            else
            {
                string suffix = command.Replace(mapping.prefix, "").Trim();;
                Console.WriteLine("Current State: " + this.state);
                switch (this.state)
                {
                    case State.start:
                        processStartCommand(suffix);
                        break;
                    case State.learn:
                        if (suffix.Equals("go back"))
                        {
                            // TODO(namos): add action sequence to library
                            Console.WriteLine("Added Sequence");
                            this.switchStates(State.start);
                        }
                        else if (suffix.Equals("restart"))
                        {
                            Console.WriteLine("Restarting...");
                            // TODO(namos): clear cached action sequences from library
                        }
                        else
                        {
                            Console.WriteLine("Nao perform: " + suffix);
                            // TODO(namos): have nao perform action
                        }
                        break;
                    case State.perform:
                        if (suffix.Equals("go back"))
                        {
                            Console.WriteLine("Done performing actions.");
                            this.switchStates(State.start);
                        }
                        else
                        {
                            Console.WriteLine("Nao perform: " + suffix);
                            // TODO(namos): have nao perform action
                        }
                        break;
                    case State.har:
                        if (suffix.Equals("go back"))
                        {
                            Console.WriteLine("Going back");
                            this.switchStates(State.start);
                        }
                        break;
                }
            }
            Console.WriteLine("Current State: " + this.state);
        }

        private void processStartCommand(string suffix)
        {
            if (suffix.Equals("watch me"))
            {
                // TODO(namos): record data from kinect -> classify data -> have Nao speak
                Console.WriteLine("I'm Watching you");
                this.switchStates(State.har);
            }
            else if (suffix.Equals("listen"))
            {
                Console.WriteLine("I'm listening");
                this.switchStates(State.perform);
                // TODO(namos): have Nao say "What do you want me to do?"
            }
            else if (suffix.Equals("learn this"))
            {
                Console.WriteLine("Teach me");
                // TODO(namos): Nao say begin
                this.switchStates(State.learn);
            }
            else if (suffix.Equals("exit"))
            {
                Console.WriteLine("exit");
                this.exit();
            }
            else
            {
                Console.WriteLine("Error: " + suffix);
                // TODO(namos): have Nao say error message
            }
        }

        private void init()
        {
            this.lib = ActionLibrary.load(ACTION_LIB_PATH);
            this.actions = new List<string>() { 
                "walk forward", "walk backwards", "walk left", "walk right",
                "wave right", "wave left", "raise the roof", "do the macarena"
            };
            this.state = State.start;
            // load actions
            // load model library
            // load action library
        }

        public void exit()
        {
            this.save();
            this.recog.exit();
            //this.nao.exit();
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
