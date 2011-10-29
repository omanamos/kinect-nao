using System;

namespace KinectViewer
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (KinectAngleViewer game = new KinectAngleViewer())
            {
                game.Run();
            }
        }
    }
#endif
}

