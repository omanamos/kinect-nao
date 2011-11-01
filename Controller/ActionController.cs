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
        }

        public void runAction(ActionSequence action)
        {
            Executor executor = new Executor(action, nao, FPS);
            Thread t = new Thread(new ThreadStart(executor.run));
            t.Start();
            t.Join();
        }
    }

    public class Executor
    {
        private ActionSequence action;
        private NaoController nao;
        private int interval;

        public Executor(ActionSequence action, NaoController nao, int fps)
        {
            this.action = action;
            this.nao = nao;
            this.interval = 1000 / fps;
        }

        public void run()
        {
            // TODO(namos): Possibly do some smoothing here
            while (action.hasNext())
            {
                nao.update(action.next());
                Thread.Sleep(interval);
            }
        }
    }
}
