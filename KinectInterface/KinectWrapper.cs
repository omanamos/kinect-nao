using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectInterface
{
    public delegate void NewKinectDataEventHandler(object sender, NewDataEventArgs e);

    class KinectWrapper
    {
        public event NewDataEventHandler NewData;

        private IKinect _kinect;
        public KinectWrapper(IKinect kinect)
        {
            _kinect = kinect;
            _kinect.NewData += new NewDataEventHandler(newKinectData);
        }

        private void newKinectData(object sender, NewDataEventArgs e)
        {
            if (NewData != null)
                NewData(sender, e);
        }
    }
}
