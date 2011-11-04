using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using DataStore;

namespace Controller
{
    class ActionController
    {
        private static readonly int FPS = 30;
        private NaoController nao;

        public ActionController()
        {
            this.nao = new NaoController("127.0.0.1");
            //this.nao = new NaoController("128.208.4.225");
        }

        public void runAction(ActionSequence<NaoSkeleton> action)
        {
            Executor executor = new Executor(action, nao, FPS);
            Thread t = new Thread(new ThreadStart(executor.run));
            t.Start();
            t.Join();
        }

        public void speak(String context)
        {
            this.nao.speak(context);
        }

        public void exit()
        {
            nao.exit();
        }
    }

    public class Executor
    {
        private ActionSequence<NaoSkeleton> action;
        private NaoController nao;
        private int interval;

        public Executor(ActionSequence<NaoSkeleton> action, NaoController nao, int fps)
        {
            this.action = action;
            this.nao = nao;
            this.interval = 1000 / fps;
        }

        public void run()
        {
            // TODO(namos): Possibly do some smoothing here
            action.reset();
            while (action.hasNext())
            {
                nao.update(action.next());
                Thread.Sleep(interval);
            }
        }
    }
}
