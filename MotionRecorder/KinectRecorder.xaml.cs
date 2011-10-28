using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;
using System.Windows.Threading;

namespace MotionRecorder
{
    /// <summary>
    /// Interaction logic for KinectRecorder.xaml
    /// </summary>
    public partial class KinectRecorder : Window
    {
        private enum Modes { PLAY, RECORD } ;

        // Our kinect thread
        IKinect km = new RealKinect();
        // Our visualizer thread
        Visualizer.GameManagerThread gmt = new Visualizer.GameManagerThread();
        // Our timer for creating timestamps
        Stopwatch stopwatch = new Stopwatch();
        // The contents of our current recording
        List<RecordingPoint> currentRecordingSequence = new List<RecordingPoint>();
        MotionRecording currentMotionRecording = null;
        // Are we currently recording
        private bool recording;
        private bool playing;

        MotionPlayer motion_player = new MotionPlayer();

        int playerPointsId = -1;
        int kinectPointsId = -1;
        private bool kinect_vis;

        #region window code
        public KinectRecorder()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialize the kinect
            km.NewData += new NewDataEventHandler(newKinectData);
            // Start kinect thread
            km.start();
            // Start visualizer thread
            gmt.start();
            // Recording is disabled until we set the recording path
            record.IsEnabled = false;

            motion_player.PlaybackProgress += 
                new PlaybackProgressEventHandler(motion_player_PlaybackProgress);

            SetGUIMode(Modes.PLAY);
            recording = false;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            // Stop our other threads
            km.stop();
            gmt.stop();
        }

        private void SetGUIMode(Modes mode)
        {
            switch (mode)
            {
                case Modes.PLAY:
                    // Enable the playback sliders
                    start_slider.IsEnabled = true;
                    end_slider.IsEnabled = true;
                    play_slider.IsEnabled = true;
                    play.IsEnabled = true;
                    break;
                case Modes.RECORD:
                    // Disable the playback sliders
                    start_slider.IsEnabled = false;
                    end_slider.IsEnabled = false;
                    play_slider.IsEnabled = false;
                    play.IsEnabled = false;
                    break;

            }
        }

        #endregion

        #region kinect code
        private void newKinectData(object sender, NewDataEventArgs e)
        {
            // Visualize the points if we are not playing currently
            if (gmt.ready())
            {
                if (kinectPointsId != -1)
                {
                    gmt.removePoints(kinectPointsId);
                }
                if (kinect_vis)
                {
                    kinectPointsId = gmt.showPoints(arrayToList(e.data));
                }
            }
            // And save them if we are recording
            if (recording)
                currentRecordingSequence.Add(new RecordingPoint(stopwatch.ElapsedMilliseconds, e.data));
        }

        private List<double[]> arrayToList(double[,] arry)
        {
            List<double[]> lst = new List<double[]>();
            for (int i = 0; i < arry.GetLength(0); i++)
            {
                double[] d = { arry[i, 0], arry[i, 1], arry[i, 2] };
                lst.Add(d);
            }
            return lst;
        }

        private void kinect_vis_enabled_Unchecked(object sender, RoutedEventArgs e)
        {
            kinect_vis = false;
        }

        private void kinect_vis_enabled_Checked(object sender, RoutedEventArgs e)
        {
            kinect_vis = true;
        }

        #endregion

        #region record button code
        private void record_Checked(object sender, RoutedEventArgs e)
        {
            // Check if we have a valid path first
            if (Directory.Exists(this.recordings_path.Text))
            {
                SetGUIMode(Modes.RECORD);
                // start recording the skeleton
                stopwatch.Restart();
                recording = true;
            }
            else
            {
                Console.WriteLine("Can't start recording, invalid save path");
                record.IsChecked = false;
            }
        }

        private void record_Unchecked(object sender, RoutedEventArgs e)
        {
            if (recording)
            {
                SetGUIMode(Modes.PLAY);
                // stop recording
                recording = false;
                stopwatch.Stop();
                // Convert from a list of points to a MotionRecording
                string name = this.recording_title.Text;
                currentMotionRecording = new MotionRecording(currentRecordingSequence, name);
                currentRecordingSequence.Clear();
                recordings.Items.Add(currentMotionRecording);
            }
            // else the button was pressed but recording couldn't start for some reason
        }
        #endregion

        #region saving code
        private void save_btn_Click(object sender, RoutedEventArgs e)
        {
            // and save the recording.
            saveCurrentRecording();
            // add the recording to the list
            populateRecordings();
        }

        private void saveCurrentRecording()
        {
            string directory = recordings_path.Text;
            string filename = recording_rep.Text + ".rec";
            string totalName = System.IO.Path.Combine(directory, recording_title.Text + filename);
            currentMotionRecording.saveToFile(totalName);
        }
        #endregion

        #region recordings list
        private void directoryButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (Directory.Exists(this.recordings_path.Text))
            {
                dialog.SelectedPath = this.recordings_path.Text;
            }
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            string filePath = dialog.SelectedPath;
            this.recordings_path.Text = filePath;
            this.record.IsEnabled = true;
            // populate the list of previous recordings
            populateRecordings();
        }

        private void populateRecordings()
        {
            if (Directory.Exists(this.recordings_path.Text))
            {
                // Get all files of type .rec in the directory
                string[] files = Directory.GetFiles(this.recordings_path.Text, "*.rec");
                this.recordings.Items.Clear();
                foreach (string s in files) {
                    this.recordings.Items.Add(MotionRecording.loadFromFile(s));
                }
            }
        }

        private void recordings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            motion_player.stop();
            if (e.AddedItems.Count == 0)
            {
                currentMotionRecording = null;
            }
            else
            {
                currentMotionRecording = (MotionRecording)e.AddedItems[0];
            }

            updatePlaybackStatus();
        }
        #endregion

        #region playback
        private void play_Click(object sender, RoutedEventArgs e)
        {
            if (playing)
            {
                playing = false;
                // pause
                motion_player.togglePaused();
            }
            else
            {
                playing = true;
                motion_player.togglePaused();
            }
        }

        private void updatePlaybackStatus()
        {
            List<double> ticks = new List<double>();
            if (currentMotionRecording == null || currentMotionRecording.getPointTimestamps().Count == 0)
            {
                play_slider.Ticks = new DoubleCollection(ticks);
                play_slider.Value = 0;
                play_slider.Minimum = 0;
                play_slider.Maximum = 0;
                Console.WriteLine("Note: " + currentMotionRecording +  " has no points");
            }
            else
            {
                motion_player.setMotionRecording(currentMotionRecording);
                List<long> timestamps = currentMotionRecording.getPointTimestamps();
                ticks = new List<double>(timestamps.Count);
                foreach (long l in timestamps)
                {
                    ticks.Add((double)l);
                }
            
                play_slider.Minimum = ticks[0];
                play_slider.Value = play_slider.Minimum;
                play_slider.Maximum = ticks[ticks.Count - 1];
                play_slider.IsSnapToTickEnabled = true;
            }
        }

        void motion_player_PlaybackProgress(object sender, PlaybackProgressEventArgs e)
        {
            Action<Slider, double> act = new Action<Slider, double>(updateSlider);
            Dispatcher.BeginInvoke(act, play_slider, e.Timestamp);
            //play_slider.Value = e.Timestamp;
            showFrameAtTimestamp(e.Timestamp);
        }

        void updateSlider(Slider s, double v)
        {
            s.Value = v;
        }

        void showFrameAtTimestamp(long timestamp)
        {
            RecordingPoint pt = currentMotionRecording.getPointFromTimestamp(timestamp);
            if (playerPointsId != -1)
            {
                // Remove the old points
                gmt.removePoints(playerPointsId);
            }
            // And add the new ones, updating our id
            playerPointsId = gmt.showPoints(arrayToList(pt.Data));
        }

        #endregion
   
    }
}
