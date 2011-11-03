using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Speech.Recognition;

using DataStore;
using Recognizer;

namespace Controller
{
    public class MainController
    {
        public static readonly String ACTION_LIB_PATH = "Z:/dev/kinect-nao/DataStore/lib.data";
        public static readonly String MODEL_LIB_PATH = "Z:/dev/kinect-nao/Recognizer/HmmData/";

        private enum State { start, har, perform, learn, listenForNewName, confirmation };
        private State state;
        private State prevState;
        private VoiceRecogition recog;

        private Dictionary<State, Mapping> map;

        private List<string> actions;
        private ActionLibrary lib;

        private ActionController nao;
        private HMMRecognizer har;
        private MainWindow window;
        
        public MainController(MainWindow window)
        {
            this.window = window;
            init();

            map = new Dictionary<State, Mapping>();
            map[State.start] = new Mapping("nao", new List<string>() { 
                    "watch me", "perform this", "learn this", "exit" });
            map[State.har] = new Mapping("nao", new List<string>() { "go back" });
            map[State.perform] = new Mapping("perform", new List<string>(actions) { "go back" });
            map[State.learn] = new Mapping("nao", new List<string>(actions) { "go back", "restart",
                "save"});
            map[State.listenForNewName] = new Mapping("it is named");
            map[State.confirmation] = new Mapping("confirm", new List<string>() { "yes", "no" });

            this.nao = new ActionController();

            switchStates(State.start);
        }

        private void switchStates(State state)
        {
            window.Dispatcher.BeginInvoke(new Action(
                delegate()
                {
                    window.currentState.Text = state.ToString();
                    window.availableCommands.Items.Clear();
                    foreach (string s in map[state].list)
                        window.availableCommands.Items.Add(s);
                    window.prefix.Text = map[state].prefix;
                }));

            this.prevState = this.state;
            this.state = state;
            if (this.recog != null)
            {
                this.recog.exit();
            }
            this.recog = new VoiceRecogition(map[state], this);
        }

        public void processCommand(string command)
        {
            Mapping mapping = map[this.state];
            if (!mapping.contains(command))
            {
                this.nao.speak(command + " isn't a valid command you idiot");
                Console.WriteLine("\"" + command + "\" is not a valid command");
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
                            this.nao.speak("Are you sure you want to cancel learning this action?");
                            this.switchStates(State.confirmation);
                        }
                        else if (suffix.Equals("save"))
                        {
                            Console.WriteLine("What should this new sequence be called?");
                            this.nao.speak("What should this new sequence be called?");
                            this.switchStates(State.listenForNewName);
                        }
                        else if (suffix.Equals("restart"))
                        {
                            Console.WriteLine("Restarting...");
                            this.nao.speak("Starting over");
                            this.lib.clearCache();
                        }
                        else
                        {
                            Console.WriteLine("Nao perform: " + suffix);
                            ActionSequence<NaoSkeleton> seq = this.lib.getSequence(suffix);
                            this.lib.appendToCache(seq);
                            this.nao.runAction(seq);
                        }
                        break;
                    case State.listenForNewName:
                        this.switchStates(State.confirmation);
                        this.nao.speak("is " + suffix + " the correct name?");
                        this.lib.setCachedName(suffix);
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
                            this.nao.runAction(this.lib.getSequence(suffix));
                        }
                        break;
                    case State.har:
                        if (suffix.Equals("go back"))
                        {
                            Console.WriteLine("Going back");
                            this.switchStates(State.start);
                        }
                        break;
                    case State.confirmation:
                        if (suffix.Equals("yes"))
                        {
                            switch (this.prevState)
                            {
                                case State.start:
                                    this.nao.speak("Ok, I'm exiting");
                                    this.exit();
                                    break;
                                case State.learn:
                                    this.nao.speak("Ok, I won't learn this action");
                                    this.lib.clearCache();
                                    this.switchStates(State.start);
                                    break;
                                case State.listenForNewName:
                                    if (this.lib.saveCache())
                                    {
                                        this.nao.speak("Ok, I saved the new action called "
                                            + this.lib.getCachedName());
                                    }
                                    else
                                    {
                                        this.nao.speak("You didn't record anything, I'm not going"
                                            + " to save an empty action silly");
                                    }
                                    this.switchStates(State.start);
                                    break;
                            }
                        }
                        else
                        {
                            switch (this.prevState)
                            {
                                case State.start:
                                    this.nao.speak("Ok, I won't exit");
                                    this.switchStates(State.start);
                                    break;
                                case State.learn:
                                    this.nao.speak("Ok, keep teaching me");
                                    this.switchStates(State.learn);
                                    break;
                                case State.listenForNewName:
                                    this.nao.speak("Ok, what is it really called?");
                                    this.switchStates(State.listenForNewName);
                                    break;
                            }
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
                Console.WriteLine("What do you want me to do?");
                this.switchStates(State.perform);
                this.nao.speak("What do you want me to do?");
            }
            else if (suffix.Equals("learn this"))
            {
                Console.WriteLine("Teach me");
                this.switchStates(State.learn);
                this.nao.speak("Ok, I'm ready");
            }
            else if (suffix.Equals("exit"))
            {
                Console.WriteLine("Confirm exit please!");
                this.nao.speak("Are you sure you want to exit?");
                this.switchStates(State.confirmation);
            }
            else
            {
                this.nao.speak(suffix + " isn't a valid command");
                Console.WriteLine("\"" + suffix + "\" is not a valid command");
            }
        }

        private void init()
        {
            this.har = new HMMRecognizer(MODEL_LIB_PATH);
            this.lib = ActionLibrary.load(ACTION_LIB_PATH);
            this.actions = new List<string>(this.lib.getActionNames());
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
            this.lib.save(ACTION_LIB_PATH);
        }

        public class Mapping {
            public readonly string prefix;
            public readonly bool useDictation;
            public readonly List<string> list;
            private HashSet<string> lookup;

            private Mapping() {}

            public Mapping(string prefix)
            {
                this.prefix = prefix;
                this.useDictation = true;
            }
            
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
