using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;  

namespace VisionInspector.Camera
{
    internal class SimulatedCamera : ICamera
    {
        private bool _isConnected;
        private bool _grabbing;
        private Thread _grabThread;
        private readonly Random _rand = new Random();

        public bool IsConnected => _isConnected;
        public event EventHandler<FrameReadyEventArgs> FrameReady;

        public void Connect()
        {
            _isConnected = true;
        }

        public void Disconnect()
        {
            StopGrabbing();
            _isConnected = false;
        }

        public void StartGrabbing()
        {
            if (!_isConnected) return;
            _grabbing = true;
            _grabThread = new Thread(GrabLoop) { IsBackground = true };
            _grabThread.Start();
        }

        public void StopGrabbing()
        {
            _grabbing = false;
        }

        private void GrabLoop()
        {
            while (_grabbing)
            {
                var frame = GenerateTestImage();
                FrameReady?.Invoke(this, new FrameReadyEventArgs { Image = frame });
                Thread.Sleep(500); // Simulate frame rate
            }
        }

        private Mat GenerateTestImage()
        {
            using (var bg = new Mat(480, 640, MatType.CV_8UC3, new Scalar(220, 220, 220))) 
            { 
            int cx = _rand.Next(100, 540);
            int cy = _rand.Next(80, 400);
            int r = _rand.Next(15, 45);
            if(_rand.Next(0,3)!=0)
                Cv2.Circle(bg, new Point(cx, cy), r, new Scalar(60, 60, 60), -1); // Draw a red circle
            return bg.Clone();
        }
        }

        public Mat CaptureOne()
        {
            return GenerateTestImage();
        }
    }
}
