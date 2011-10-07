using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Aldebaran.NaoCamCSharpExample;
using Coding4Fun.Kinect.Wpf;
using Microsoft.Research.Kinect.Nui;

using Emgu.CV;
using Emgu.CV.Structure;

namespace NaoKinectTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Runtime nui = new Runtime();
        private HeadPanning _headPanning = new HeadPanning();
        private SkeletonData _skel;
        private int depthHeight;
        private int depthWidth;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            nui.Initialize(RuntimeOptions.UseColor | RuntimeOptions.UseDepth | RuntimeOptions.UseSkeletalTracking);
            nui.VideoFrameReady += new EventHandler<ImageFrameReadyEventArgs>(nui_VideoFrameReady);
            nui.DepthFrameReady += new EventHandler<ImageFrameReadyEventArgs>(nui_DepthFrameReady);
            //nui.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(nui_SkeletonFrameReady);
            nui.VideoStream.Open(ImageStreamType.Video, 2, ImageResolution.Resolution640x480, ImageType.Color);
            nui.DepthStream.Open(ImageStreamType.Depth, 2, ImageResolution.Resolution320x240, ImageType.Depth);
            nui.NuiCamera.ElevationAngle = Camera.ElevationMaximum;
            depthHeight = nui.DepthStream.Height;
            depthWidth = nui.DepthStream.Width;
            //_headPanning.Connect("127.0.0.1");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            nui.Uninitialize();
        }

        void nui_VideoFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            image1.Source = e.ImageFrame.ToBitmapSource();
        }

        void projectiveToRealWorld(double depth, int x_proj, int y_proj, out double x_rw, out double y_rw)
        {
            const double FovH = 1.0144686707507438; // (rad)
            const double FovV = 0.78980943449644714; // (rad)
            double XtoZ = Math.Tan(FovH / 2) * 2;
            double YtoZ = Math.Tan(FovV / 2) * 2;
            x_rw = (x_proj / depthWidth - .5) * depth * XtoZ;
            y_rw = (0.5 - y_proj / depthHeight) * depth * YtoZ;
        }

        void nui_DepthFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            // Depth is in mm
            short[][] depth = e.ImageFrame.ToDepthArray2D();
            image2.Source = e.ImageFrame.ToBitmapSource();
            /*
            if (true)
            {
                Matrix<double> data = new Matrix<double>(depthHeight*depthWidth, 3);
                int row = 0;
                for (int x=0; x < depthWidth; x++) {
                    for (int y = 0; y < depthHeight; y++)
                    {
                        double depthVal = depth[x][y];
                        depthVal /= 1000.0; // To convert into meters
                        double x_rw, y_rw;
                        projectiveToRealWorld(depthVal, x, y, out x_rw, out y_rw);
                        data[row, 0] = x_rw;
                        data[row, 1] = y_rw;
                        data[row, 2] = depthVal;
                    }
                }
                Matrix<double> evals = new Matrix<double>(1, 3);
                Matrix<double> evecs = new Matrix<double>(3, 3);
                Emgu.CV.CvInvoke.cvCalcPCA(data.Ptr, IntPtr.Zero, evals.Ptr, evecs.Ptr, Emgu.CV.CvEnum.PCA_TYPE.CV_PCA_DATA_AS_ROW);
                
                //= _skel.Joints[JointID.Head].Position;
            }*/
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
                
                SetEllipsePosition(headEllipse, skeleton.Joints[JointID.Head]);
                SetEllipsePosition(leftEllipse, skeleton.Joints[JointID.HandLeft]);
                SetEllipsePosition(rightEllipse, skeleton.Joints[JointID.HandRight]);
                head = head.ScaleTo(200, 200);
                position.Text = "(" + head.Position.X + ", " + head.Position.Y + ", " + head.Position.Z + ")";
                Console.WriteLine("(" + (head.Position.X - 100) + ", " + (head.Position.Y - 100) + ", " + (head.Position.Z - 100) + ")");
                _headPanning.UpdatePitch(100 - (int)head.Position.Y);
                _headPanning.UpdateYaw(100 - (int)head.Position.X);
            }
        }

        private void SetEllipsePosition(FrameworkElement ellipse, Joint joint)
        {
            var scaledJoint = joint.ScaleTo(640, 480, .5f, .5f);

            Canvas.SetLeft(ellipse, scaledJoint.Position.X);
            Canvas.SetTop(ellipse, scaledJoint.Position.Y);
        }
    }
}
