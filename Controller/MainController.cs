using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Speech.Recognition;

namespace Controller
{
    public class MainController
    {
        private enum State { start, har, perform, learn };
        private State state;
        private VoiceRecogition recog;

        private Dictionary<State, Mapping> map;

        private List<string> actions;

        private ActionController nao;
        
        public MainController()
        {
            init();

            map = new Dictionary<State,Mapping>();
            map[State.start] = new Mapping("nao", new List<string>() { 
                    "watch me", "listen", "learn this", "cancel", "exit" });
            map[State.har] = null;
            map[State.perform] = new Mapping("perform", new List<string>(actions) { "I'm done" });
            map[State.learn] = new Mapping("nao", new List<string>(actions) { "I'm done" });

            //this.nao = new ActionController();

            this.recog = new VoiceRecogition(map[State.start].prefix, map[State.start].list, this);
        }

        private void init()
        {
            this.actions = new List<string>();
            // load actions
            // load model library
            // load action library
        }

        public void exit()
        {
            this.save();
            this.recog.exit();
            this.nao.exit();
        }

        private void save()
        {
            // save model library
            // save action library
        }

        class Mapping {
            public readonly string prefix;
            public readonly List<string> list;

            private Mapping() {}

            public Mapping(string prefix, List<string> list)
            {
                this.prefix = prefix;
                this.list = list;
            }
        }
    }
}
