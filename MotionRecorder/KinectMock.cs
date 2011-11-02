using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MotionRecorder
{
    // delegate for new data callback
    public delegate void NewDataEventHandler(object sender, NewDataEventArgs e);

    public class NewDataEventArgs : EventArgs
    {
        public double[,] data;
        public NewDataEventArgs(double[,] data)
        {
            this.data = data;
        }
    }

    public class KinectMock : IKinect
    {
        Thread t;
        private double[,] generateData()
        {
            int njoints = 7;
            double[,] data = new double[njoints, 3];
            for (int i=0; i < njoints; i++) {
                data[i,0] = i;
                data[i,1] = -i;
                data[i,2] = 1.0;
            }
            return data;
        }

        public event NewDataEventHandler NewData;

        protected virtual void OnNewData(NewDataEventArgs e)
        {
            if (NewData != null)
            {
                NewData(this, e);
            }
        }

        public void start()
        {
            t = new Thread(new ThreadStart(this.run));
            t.Start();
        }

        public void stop()
        {
            t.Abort();
        }

        public void run()
        {
            while (true)
            {
                double[,] data = generateData();
                OnNewData(new NewDataEventArgs(data));
                Thread.Sleep(100);
            }
        }

        ~KinectMock()
        {
            stop();
        }

    }
}
