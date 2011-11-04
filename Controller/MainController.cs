using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Speech.Recognition;

using DataStore;
using Recognizer;
using System.Windows.Media;

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

        private ActionLibrary lib;

        private ActionController nao;
        private MainWindow window;
        private HumanActionRecognizer har;
        
        public MainController(MainWindow window)
        {
            this.window = window;
            init();

            map = new Dictionary<State, Mapping>();
            map[State.start] = new Mapping("nao", new List<string>() { 
                    "watch me", "perform this", "learn this", "exit" });
            map[State.har] = new Mapping("nao", new List<string>() { "go back" });
            map[State.perform] = new Mapping("perform", new List<string>(this.lib.getActionNames()) { "go back" });
            map[State.learn] = new Mapping("nao", new List<string>(this.lib.getActionNames()) { "go back", "restart",
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
                this.recog.stop();
            }
            if (this.state == State.perform || this.state == State.learn)
            {
                map[state].update(new List<string> (this.lib.getActionNames()));
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
                            this.recog.stop();
                            this.nao.runAction(seq);
                            this.recog.start();
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
                            this.recog.stop();
                            this.nao.runAction(this.lib.getSequence(suffix));
                            this.recog.start();
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
                                    String name = this.lib.getCachedName();
                                    if (this.lib.saveCache())
                                    {
                                        Console.WriteLine("Saving action: " + name + "!");
                                        this.nao.speak("Ok, I saved the new action called "
                                            + name);
                                    }
                                    else
                                    {
                                        this.lib.clearCache();
                                        Console.WriteLine("Cannot save action!");
                                        this.nao.speak("You didn't record anything, I'm not going"
                                            + " to save an empty action. you silly fool");
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
                this.recog.stop();
                this.har.start();
            }
            else if (suffix.Equals("perform this"))
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
            this.lib = ActionLibrary.load(ACTION_LIB_PATH);
            har = new HumanActionRecognizer(MODEL_LIB_PATH);
            har.Recognition += new HumanActionRecognizer.RecognitionEventHandler(har_Recognition);
            har.RecordingStart += new HumanActionRecognizer.RecordingEventHandler(har_RecordingStart);
            har.RecordingReady += new HumanActionRecognizer.RecordingEventHandler(har_RecordingReady);
            har.RecordingStop += new HumanActionRecognizer.RecordingEventHandler(har_RecordingStop);
        }

        public void exit()
        {
            this.save();
            this.recog.stop();
            this.nao.exit();
            this.har.exit();
            Environment.Exit(0);
        }

        private void save()
        {
            this.lib.save(ACTION_LIB_PATH);
        }

        #region har callbacks
        void har_RecordingReady(object sender, EventArgs e)
        {
            this.window.Dispatcher.BeginInvoke(new Action(
                delegate()
                {
                    this.window.recording_indicator.Background = new SolidColorBrush(Colors.Green);
                }));
        }

        void har_Recognition(object sender, Recognizer.RecognitionEventArgs e)
        {
            this.window.Dispatcher.BeginInvoke(new Action(
                    delegate()
                    {
                        this.window.action.Text = e.Class;
                        this.window.score.Text = e.Likelihood.ToString();
                        if (e.Class == "")
                        {
                            this.window.score.Foreground = new SolidColorBrush(Colors.Red);
                        }
                        else
                        {
                            this.window.score.Foreground = new SolidColorBrush(Colors.Green);
                        }
                    }));
            
            if (e.Class != "")
            {
                this.nao.speak("You are performing " + e.Class);
                this.har.stop();
                this.switchStates(State.start);
            }
            else
            {
                this.nao.speak("I don't know what you are doing.");
            }
        }

        void har_RecordingStart(object sender, EventArgs e)
        {
            this.window.Dispatcher.BeginInvoke(new Action(
                delegate()
                {
                    this.window.recording_indicator.Background = new SolidColorBrush(Colors.Red);
                }));
        }

        void har_RecordingStop(object sender, EventArgs e)
        {
            this.window.Dispatcher.BeginInvoke(new Action(
            delegate()
            {
                this.window.recording_indicator.Background = new SolidColorBrush(Colors.Black);
            }));
        }
        #endregion

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
                this.list = new List<string>();
                this.lookup = new HashSet<string>();
            }
            
            public Mapping(string prefix, List<string> list)
            {
                this.prefix = prefix;
                this.list = list;
                this.useDictation = false;
                this.lookup = new HashSet<string>(list);
            }

            public void update(List<string> set)
            {
                foreach (string s in set)
                {
                    if (!this.lookup.Contains(s))
                    {
                        this.lookup.Add(s);
                        this.list.Add(s);
                    }
                }
            }

            public bool contains(string command)
            {
                if (this.useDictation)
                {
                    return true;
                }
                else if (command.StartsWith(prefix))
                {
                    return this.lookup.Contains(command.Replace(prefix, "").Trim());
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
