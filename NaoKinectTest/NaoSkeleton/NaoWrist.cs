﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NaoKinectTest.HumanModel;

namespace NaoKinectTest
{
    class NaoWrist
    {
        public static readonly double YAW_MIN = -1.8238;
        public static readonly double YAW_MAX = 1.8238;
        public static readonly double YAW_RANGE = YAW_MAX - YAW_MIN;

        private double yaw;

        private NaoWrist(){}

        public NaoWrist(double humanYaw)
        {
            this.yaw = scaleYaw(humanYaw);
        }

        public double getYaw()
        {
            return this.getYaw();
        }

        private double scaleYaw(double humanYaw)
        {
            return (humanYaw - HumanShoulder.YAW_MIN / HumanShoulder.YAW_RANGE) * YAW_RANGE;
        }
    }
}
