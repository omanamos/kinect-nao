/**
 * @author Aldebaran Robotics
 * Aldebaran Robotics (c) 2009 All Rights Reserved.\n
 *
 * Version : $Id$
 */
using System;
using System.Collections;

using Aldebaran.Proxies;

namespace Aldebaran.NaoCamCSharpExample
{
    class HeadPanning
    {
        public MotionProxy _motion = null;

        private float _minYaw = 0.0f;
        private float _maxYaw = 0.0f;
        private float _minPitch = 0.0f;
        private float _maxPitch = 0.0f;

        public void Connect(string ip)
        {
            try
            {
                _motion = new MotionProxy(ip,9559);
                // --------------- prepare limits --------------------------
                // three floats as an ArrayList for each joint in the chain
                // min,max,maxNoLoadSpeedPerCycle
                ArrayList limits = (ArrayList)_motion.getLimits("Head");
                ArrayList yawLimits = (ArrayList)limits[0];
                ArrayList pitchLimits = (ArrayList)limits[1];
                _minYaw = (float)yawLimits[0];
                _maxYaw = (float)yawLimits[1];
                _minPitch = (float)pitchLimits[0];
                _maxPitch = (float)pitchLimits[1];
                Console.WriteLine("MinYaw: " + _minYaw + " MaxYaw:" + _maxYaw + " MinPitch: " + _minPitch + " MaxPitch:" + _maxPitch);
                // give the joints some stiffness
                _motion.stiffnessInterpolation("Head",1.0f,1.0f);

            } 
            catch(Exception e)
            {
                Console.Out.WriteLine("HeadPanning.Connect exception: " + e);
            }
        }

        public void UpdateYaw(int val)
        {
            if (_motion != null)
            {
                try
                {
                    _motion.setAngles("HeadYaw", ScaleToRange(-val, _minYaw, _maxYaw),0.1f);
                }
                catch (Exception e)
                {
                    Console.Out.WriteLine("HeadPanning.UpdateYaw exception: " + e);
                }
            }
        }

        public void UpdatePitch(int val)
        {
            if (_motion != null)
            {
                try
                {
                    _motion.setAngles("HeadPitch", ScaleToRange(-val, _minPitch, _maxPitch),0.1f);
                }
                catch (Exception e)
                {
                    Console.Out.WriteLine("HeadPanning.UpdateYaw exception: " + e);
                }
            }
        }

        private static float ScaleToRange(int val, float min, float max)
        {
            float returnVal = (((val + 100.0f)/200.0f)*(max - min)) + min;
            return returnVal;
        }
    }
}
