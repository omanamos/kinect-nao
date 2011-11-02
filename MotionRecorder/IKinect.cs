using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MotionRecorder
{
    public interface IKinect
    {
        event NewDataEventHandler NewData;

        void start();
        void stop();
    }
}
