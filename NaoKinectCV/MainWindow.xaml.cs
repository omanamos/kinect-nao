using System;
using System.Collections.Generic;
using System.Collections;
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
using System.IO;

using Coding4Fun.Kinect.Wpf;
using Microsoft.Research.Kinect.Nui;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace NaoKinectCV
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Runtime nui = new Runtime();
        private int depthHeight;
        private int depthWidth;
        private SkeletonData _skel;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            nui.Initialize(RuntimeOptions.UseColor | RuntimeOptions.UseDepth | RuntimeOptions.UseSkeletalTracking);
            //nui.VideoFrameReady += new EventHandler<ImageFrameReadyEventArgs>(nui_VideoFrameReady);
            nui.DepthFrameReady += new EventHandler<ImageFrameReadyEventArgs>(nui_DepthFrameReady);
            nui.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(nui_SkeletonFrameReady);
            nui.VideoStream.Open(ImageStreamType.Video, 2, ImageResolution.Resolution640x480, ImageType.Color);
            nui.DepthStream.Open(ImageStreamType.Depth, 2, ImageResolution.Resolution320x240, ImageType.Depth);
            nui.NuiCamera.ElevationAngle = Camera.ElevationMaximum;
            depthHeight = nui.DepthStream.Height;
            depthWidth = nui.DepthStream.Width;
            //_headPanning.Connect("127.0.0.1");
        }

        void projectiveToRealWorld(float depth, int x_proj, int y_proj, out float x_rw, out float y_rw)
        {
            const float FovH = 1.0144686707507438f; // (rad)
            const float FovV = 0.78980943449644714f; // (rad)
            float XtoZ = (float)Math.Tan(FovH / 2) * 2;
            float YtoZ = (float)Math.Tan(FovV / 2) * 2;
            x_rw = (x_proj / (float)depthWidth - .5f) * depth * XtoZ;
            y_rw = (0.5f - y_proj / (float)depthHeight) * depth * YtoZ;
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

                head = head.ScaleTo(200, 200);
                Console.WriteLine("(" + (head.Position.X - 100) + ", " + (head.Position.Y - 100) + ", " + (head.Position.Z - 100) + ")");
                //_headPanning.UpdatePitch(100 - (int)head.Position.Y);
                //_headPanning.UpdateYaw(100 - (int)head.Position.X);
            }
        }

        void dumpToFile(Matrix<double> pts, string fname)
        {
            using (StreamWriter writer = new StreamWriter(fname))
            {
                for (int i = 0; i < pts.Rows; i++)
                {

                    StringBuilder sb = new StringBuilder();
                    sb.Append(i);
                    sb.Append(" ");
                    sb.Append(pts[i, 0]);
                    sb.Append(" ");
                    sb.Append(pts[i, 1]);
                    sb.Append(" ");
                    sb.Append(pts[i, 2]);
                    sb.Append(" ");
                    sb.Append(1); // Group ID
                    writer.WriteLine(sb.ToString());
                }
            }
        }

        void nui_DepthFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            // Depth is in mm
            short[][] depth = e.ImageFrame.ToDepthArray2D();
            //image2.Source = e.ImageFrame.ToBitmapSource();
            if (this._skel != null)
            {
                Matrix<double> data = new Matrix<double>(depthHeight * depthWidth, 3);
                List<MCvPoint3D32f> head_points = new List<MCvPoint3D32f>();
                MCvPoint3D32f head_pos = new MCvPoint3D32f();
                head_pos.x = this._skel.Joints[JointID.Head].Position.X;
                head_pos.y = this._skel.Joints[JointID.Head].Position.Y;
                head_pos.z = this._skel.Joints[JointID.Head].Position.Z;
                
                int row = 0;
                for (int x = 0; x < depthWidth; x++)
                {
                    for (int y = 0; y < depthHeight; y++)
                    {
                        float depthVal = depth[x][y];
                        depthVal /= 1000.0f; // To convert into meters
                        float x_rw, y_rw;
                        projectiveToRealWorld(depthVal, x, y, out x_rw, out y_rw);

                        MCvPoint3D32f pt = new MCvPoint3D32f(x_rw, y_rw, depthVal);
                        MCvPoint3D32f delta = pt - head_pos;

                        if (delta.Norm < 0.3) // If the point is closer than 10cm
                        {
                            head_points.Add(pt);
                        }

                        data[row, 0] = x_rw;
                        data[row, 1] = y_rw;
                        data[row, 2] = depthVal;
                        row++;
                    }
                }

                // Get a matrix of all of the points in the head
                Matrix<double> head_pts = new Matrix<double>(head_points.Count, 3);
                for (int i=0; i < head_points.Count; i++)
                {
                    MCvPoint3D32f p = head_points[i];
                    head_pts[i, 0] = p.x;
                    head_pts[i, 1] = p.y;
                    head_pts[i, 2] = p.z;
                }
                dumpToFile(head_pts, "head");
                dumpToFile(data, "scene");
                if (head_points.Count > 0)
                {
                    Matrix<double> evals = new Matrix<double>(1, 3);
                    Matrix<double> avg = new Matrix<double>(1, 3);
                    Matrix<double> evecs = new Matrix<double>(3, 3);
                    Emgu.CV.CvInvoke.cvCalcPCA(head_pts.Ptr, avg.Ptr, evals.Ptr, evecs.Ptr, Emgu.CV.CvEnum.PCA_TYPE.CV_PCA_DATA_AS_ROW);
                    Console.WriteLine("<" + evals[0, 0] + ", " + evals[0, 1] + ", " + evals[0, 2] + ">");
                }
                //= _skel.Joints[JointID.Head].Position;
            }
        }
    }
}
