using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataStore;

namespace DataStore
{
    public class SkeletonVisualizer
    {
        // Our visualizer thread
        Visualizer.GameManagerThread gmt = new Visualizer.GameManagerThread();
        private int kinectPointsId;

        public SkeletonVisualizer()
        {
            gmt.start();
        }

        public void showSkeleton(HumanSkeleton skel)
        {
            // Visualize the points if we are not playing currently
            if (gmt.ready())
            {
                if (kinectPointsId != -1)
                {
                    gmt.removePoints(kinectPointsId);
                }
                kinectPointsId = gmt.showPoints(skel.JointPositions.ToList());
            }
        }

        public void stop()
        {
            gmt.stop();
        }
    }
}
