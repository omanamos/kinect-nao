using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MotionRecorder
{

    public delegate void PlaybackProgressEventHandler(object sender, PlaybackProgressEventArgs args);

    class MotionPlayer
    {
        Thread t;

        public event PlaybackProgressEventHandler PlaybackProgress;
        private bool stopped = false;
        private bool playing = false;
        private MotionRecording rec;

        public MotionPlayer()
        {

        }

        // Load up a new motion recording to play, but puts the player in pause
        public void setMotionRecording(MotionRecording rec)
        {
            stop();
            this.rec = rec;
            playing = false;
            stopped = false;
            t = new Thread(new ThreadStart(this.run));
            t.Start();
        }

        public void togglePaused()
        {
            playing = !playing;
        }

        public void pause()
        {
            playing = false;
        }

        public void unpause()
        {
            playing = true;
        }

        public void stop()
        {
            stopped = true;
            if (t != null)
                t.Join();
        }

        protected virtual void OnPlaybackProgress(PlaybackProgressEventArgs e)
        {
            if (PlaybackProgress != null)
            {
                PlaybackProgress(this, e);
            }
        }

        private void run()
        {
            List<long> timestamps = rec.getPointTimestamps();
            if (timestamps.Count > 0)
            {
                long prev_ts = timestamps[0];
                long cur_ts;
                int idx = 0;
                while (!stopped)
                {
                    // If we are paused, chill until we are unpaused
                    while (!playing && !stopped)
                    {
                        Thread.Sleep(10);
                    }
                    if (stopped)
                    {
                        // Our sleeping loop was interrupted because we were told to exit
                        continue;
                    }
                    OnPlaybackProgress(new PlaybackProgressEventArgs(prev_ts));
                    idx++;
                    if (idx == timestamps.Count)
                    {
                        // Reached the end, pause playback and reset
                        playing = false;
                        idx = 0;
                        prev_ts = timestamps[0];
                        continue;
                    }
                    cur_ts = timestamps[idx];
                    Thread.Sleep((int)(cur_ts - prev_ts));
                    prev_ts = cur_ts;
                }
            }
        }

        internal void seek(double p)
        {
            throw new NotImplementedException();
        }
    }

    public class PlaybackProgressEventArgs : EventArgs
    {

        public PlaybackProgressEventArgs(long timestamp)
        {
            Timestamp = timestamp;
        }

        public long Timestamp { get; private set; }
    }
}
