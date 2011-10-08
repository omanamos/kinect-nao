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
        Runtime nui = null;//new Runtime();
        private int depthHeight;
        private int depthWidth;
        private SkeletonData _skel;
        private icp_net.ManagedICP icp;
        private double[,] current_head_points;

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

        void nui_DepthFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            // Depth is in mm
            short[][] depth = e.ImageFrame.ToDepthArray2D();
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
                double[,] head_pts = new double[head_points.Count, 3];
                for (int i=0; i < head_points.Count; i++)
                {
                    MCvPoint3D32f p = head_points[i];
                    head_pts[i, 0] = p.x;
                    head_pts[i, 1] = p.y;
                    head_pts[i, 2] = p.z;
                }
                if (head_points.Count > 0)
                {
                    if (this.icp == null)
                    {
                        this.current_head_points = head_pts;
                        this.icp = new icp_net.ManagedICP(head_pts, head_pts.GetLength(0), 3);
                    }
                    else
                    {
                        double[,] R = new double[3, 3];
                        R[0, 0] = 1.0;
                        R[1, 1] = 1.0;
                        R[2, 2] = 1.0;
                        double[] t = new double[3];
                        t[0] = head_pos.x;
                        t[1] = head_pos.y;
                        t[2] = head_pos.z;
                        icp.fit(head_pts, head_pts.GetLength(0), R, t, -1);

                        Matrix<double> Rot = new Matrix<double>(R);
                        Matrix<double> new_pts_matrix = new Matrix<double>(head_pts);
                        Matrix<double> trans = new Matrix<double>(t);
                        // Move the new head to the origin
                        for (int rowIdx = 0; rowIdx < new_pts_matrix.Rows; rowIdx++)
                        {
                            Matrix<double> ro = new_pts_matrix.GetRow(rowIdx);
                            ro -= trans;
                        }

                        // And rotate it in to place with the other head
                        new_pts_matrix *= Rot;
                        double[,] new_points_array = new double[new_pts_matrix.Rows, 3];
                        for (int i=0; i < new_pts_matrix.Rows; i++)
                        {
                            for (int j=0; j < 3; j++)
                            {
                                new_points_array[i, j] = new_pts_matrix[i, j];
                            }
                        }
                        double[,] refined_head_points = refineHead(current_head_points, new_points_array);
                        // And update our icp model
                        this.icp = new icp_net.ManagedICP(refined_head_points, refined_head_points.GetLength(0), 3);
                        this.current_head_points = refined_head_points;
                    }
                }
                //= _skel.Joints[JointID.Head].Position;
            }
        }

        double[,] refineHead(double[,] head_points, double[,] new_head_points)
        {
            MCvPoint3D32f[] pts = new MCvPoint3D32f[head_points.GetLength(0)];
            Dictionary<int, SortedList<double, int>> point_closests_map = new Dictionary<int,SortedList<double,int>>();
            for (int i=0; i < head_points.GetLength(0); i++)
            {
                pts[i] = new MCvPoint3D32f((float)head_points[i, 0], (float)head_points[i, 1], (float)head_points[i, 2]);
                point_closests_map[i] = new SortedList<double, int>();
            }
            
            // For each point p0 in the new head find the closest point p_c in the original head
            Emgu.CV.Flann.Index3D pts_index = new Emgu.CV.Flann.Index3D(pts);
            MCvPoint3D32f[] new_head_pts = new MCvPoint3D32f[new_head_points.GetLength(0)];
            for (int i = 0; i < new_head_points.GetLength(0); i++)
            {
                MCvPoint3D32f pt = new MCvPoint3D32f((float)new_head_points[i, 0], (float)new_head_points[i, 1], (float)new_head_points[i, 2]);
                new_head_pts[i] = pt;
                double dist = 0.0;
                int idx = pts_index.ApproximateNearestNeighbour(pt, out dist);
                point_closests_map[i].Add(dist, i);
                // And add p0 to the sorted list of points which have marked p_c as their closest point.
            }

            // Then, add to the head every point other than the first one from each list.
            List<MCvPoint3D32f> new_points = new List<MCvPoint3D32f>(pts);
            foreach (int key in point_closests_map.Keys)
            {
                // A list of the points which think this point is the closest
                SortedList<double, int> points_closest = point_closests_map[key];
                if (points_closest.Count > 0)
                {
                    // Remove the closest point.
                    points_closest.RemoveAt(0);
                    // And add the rest of the points
                    foreach (int i in points_closest.Values)
                    {
                        new_points.Add(new_head_pts[i]);
                    }
                }
            }
            
            // Finally, return the newly extended array of points
            double[,] return_pts = new double[new_points.Count, 3];
            for (int i = 0; i < new_points.Count; i++)
            {
                return_pts[i, 0] = new_points[i].x;
                return_pts[i, 1] = new_points[i].y;
                return_pts[i, 2] = new_points[i].z;
            }
            return return_pts;
        }
    }
}