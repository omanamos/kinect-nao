using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Aldebaran.NaoCamCSharpExample;
using Coding4Fun.Kinect.Wpf;
using Microsoft.Research.Kinect.Nui;
using NaoKinectTest.HumanModel;

namespace NaoKinectTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Runtime nui = new Runtime();
        private NaoController controller = new NaoController("128.208.4.10");
        private SkeletonData _skel;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            nui.Initialize(RuntimeOptions.UseColor | RuntimeOptions.UseDepth | RuntimeOptions.UseSkeletalTracking);
            /*
            //Must set to true and set after call to Initialize
            nui.SkeletonEngine.TransformSmooth = true;

            //Use to transform and reduce jitter
            var parameters = new TransformSmoothParameters
            {
                Smoothing = 1.0f,
                Correction = 0.1f,
                Prediction = 0.1f,
                JitterRadius = 0.05f,
                MaxDeviationRadius = 0.05f
            };

            nui.SkeletonEngine.SmoothParameters = parameters;
            */
            nui.VideoFrameReady += new EventHandler<ImageFrameReadyEventArgs>(nui_VideoFrameReady);
            nui.DepthFrameReady += new EventHandler<ImageFrameReadyEventArgs>(nui_DepthFrameReady);
            nui.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(nui_SkeletonFrameReady);
            nui.VideoStream.Open(ImageStreamType.Video, 2, ImageResolution.Resolution640x480, ImageType.Color);
            nui.DepthStream.Open(ImageStreamType.Depth, 2, ImageResolution.Resolution320x240, ImageType.Depth);
            nui.NuiCamera.ElevationAngle = 8;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            nui.Uninitialize();
        }

        void nui_VideoFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            image1.Source = e.ImageFrame.ToBitmapSource();
        }

        void nui_DepthFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            // Depth is in mm
            image2.Source = e.ImageFrame.ToBitmapSource();
        }

        void nui_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            SkeletonFrame allSkeletons = e.SkeletonFrame;

            //get the first tracked skeleton
            SkeletonData skeleton = (from s in allSkeletons.Skeletons
                                     where s.TrackingState == SkeletonTrackingState.Tracked
                                     select s).FirstOrDefault();
            if (skeleton != null)
            {
                this._skel = skeleton;
                Joint head = skeleton.Joints[JointID.Head];
                HumanSkeleton hs = new HumanSkeleton(skeleton.Joints);
                NaoSkeleton ns = new NaoSkeleton(hs);
                UpdateTextBoxes(ns, hs);
                controller.update(ns);

                SetLineAngle(RightArmTopDown, Util.radToDeg(hs.RightShoulderYaw));
                SetLineAngle(LeftArmTopDown, Util.radToDeg(hs.LeftShoulderYaw));
                SetLineAngle(RightArmFront, Util.radToDeg(-hs.RightShoulderPitch) + 90);
                SetLineAngle(LeftArmFront, Util.radToDeg(hs.LeftShoulderPitch) - 90);
                SetLineAngle(RightArm, Util.radToDeg(-hs.RightShoulderRoll) + 90);
                SetLineAngle(LeftArm, Util.radToDeg(hs.LeftShoulderRoll) - 90);
            }
        }

        private void UpdateTextBoxes(NaoSkeleton ns, HumanSkeleton hs)
        {
            NaoElbowRoll.Text = "Left: " + ns.LeftElbow.getRoll() + "(NaoRoll) - " + hs.LeftElbowYaw + "(HumanYaw) || Right: " + ns.RightElbow.getRoll() + "(NaoRoll) - " + hs.RightElbowYaw + "(HumanYaw)";
            NaoElbowYaw.Text = "Left: " + ns.LeftElbow.getYaw() + "(NaoYaw) - " + hs.LeftShoulderRoll + "(HumanRoll) || Right: " + ns.RightElbow.getYaw() + "(NaoYaw) - " + hs.RightShoulderRoll + "(HumanRoll)";

            NaoShoulderRoll.Text = "Left: " + ns.LeftShoulder.Roll + "(NaoRoll) - " + hs.LeftShoulderPitch + "(HumanPitch) || Right: " + ns.RightShoulder.Roll + "(NaoRoll) - " + hs.RightShoulderPitch + "(HumanPitch)";
            NaoShoulderPitch.Text = "Left: " + ns.LeftShoulder.Pitch + "(NaoPitch) - " + hs.LeftShoulderYaw + "(HumanYaw) || Right: " + ns.RightShoulder.Pitch + "(NaoPitch) - " + hs.RightShoulderYaw + "(HumanYaw)";
        }

        private void SetLineAngle(Line line, double p)
        {
            RotateTransform rot = new RotateTransform(p, line.X1, line.Y1);
            line.RenderTransform = rot;
        }
    }
}
