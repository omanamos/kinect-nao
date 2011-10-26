using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Visualizer
{
    public class GameManagerThread
    {
        private VisualizerGame _game;
        private Thread _thread;
        
        public GameManagerThread()
        {
            _thread = new Thread(new ThreadStart(run));
        }

        public void start()
        {
            _thread.Start();
        }

        public void stop()
        {
            _game.Exit();
            _thread.Join();
        }

        protected void run()
        {
            _game = new VisualizerGame();
            _game.Run();
        }

        public bool ready()
        {
            return _game != null;
        }

        public int showPoints(List<double[]> points)
        {
            int id = _game.addPoints(points);
            return id;
        }

        public void removePoints(int pointsId)
        {
            _game.removePoints(pointsId);
        }
    }
}
