using System; // 引入基础类型(EventArgs 等)
using OpenCvSharp; // 引入 OpenCV 的 Mat 类(图像容器)

namespace VisionInspector.Camera // 本文件所属命名空间(对应 Camera 文件夹)
{
    // 一帧图像的"包裹"：相机取到图后，用这个事件参数把 Mat 传出去
    public class FrameReadyEventArgs : EventArgs
    {
        public Mat Image { get; set; } // 这一帧的图像(OpenCV 矩阵)
    }

    // 相机统一接口(岗位说明书)：上层只认这个接口，不关心是海康还是模拟
    public interface ICamera
    {
        bool IsConnected { get; } // 属性：相机是否已连接
        event EventHandler<FrameReadyEventArgs> FrameReady; // 事件：取到一帧时自动广播
        void Connect(); // 方法：连接/打开相机
        void Disconnect(); // 方法：断开/关闭相机
        void StartGrabbing(); // 方法：开始连续采集
        void StopGrabbing(); // 方法：停止采集
        Mat CaptureOne(); // 方法：软件触发，立刻拍一张返回
    }
}
