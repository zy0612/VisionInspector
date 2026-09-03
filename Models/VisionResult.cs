using System.Collections.Generic; // 引入 List<T> 泛型集合(存多个缺陷框用)
using OpenCvSharp;                // 引入 Mat(图像容器) 和 Rect(矩形) 类型

namespace VisionInspector.Models // 模型层：放"数据包裹"类
{
    // 一次视觉检测的结果：判定结论 + 缺陷位置 + 三张图(给界面显示)
    public class VisionResult
    {
        public bool Pass { get; set; }              // 是否合格：true=无缺陷
        public int DefectCount { get; set; }        // 缺陷数量(整数)
        public List<OpenCvSharp.Rect> DefectRects { get; set; } // 每个缺陷的外接矩形(位置+大小)
        public Mat Gray { get; set; }               // 灰度图(中间结果，界面显示用)
        public Mat Binary { get; set; }             // 二值图(中间结果，界面显示用)
        public Mat Annotated { get; set; }          // 标注图(原图上画了红色缺陷框)
    }
}
