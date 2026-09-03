using OpenCvSharp;                // 引入 Mat / Cv2 / 各种枚举
using System;                     // 引入基础类型(Exception 等)
using System.Collections.Generic; // 引入 List<T>
using VisionInspector.Models;

namespace VisionInspector.Vision // 视觉层命名空间(不是 Models!)
{
    // 视觉引擎：吃一张原图 + 参数,吐出判定结果与各阶段图像
    public class VisionEngine
    {
        // Process：核心方法。src=原图，recipe=参数，返回 VisionResult
        public VisionResult Process(Mat src, VisionRecipe recipe)
        {
            // 三态创建灰度图：用三元决定 gray 怎么"出生"(避开 CopyTo 兼容性坑)
            using var gray = src.Channels() == 3  // 如果原图是 3 通道
                ? new Mat(src.Size(), MatType.CV_8UC1) // 开一个空的单通道容器(灰度图专用)
                : src.Clone();                        // 已经是单通道,直接克隆一份即可
            if (src.Channels() == 3)              // 3 通道才需要做颜色转换
                Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY); // BGR→Gray,填进刚开的 gray

            using var blur = new Mat();           // 高斯滤波后的图像容器
            Cv2.GaussianBlur(gray, blur, new Size(5, 5), 0); // 5×5 高斯核磨皮去噪

            using var binary = new Mat();         // 二值图容器
            Cv2.Threshold(blur, binary, recipe.Threshold, 255, ThresholdTypes.BinaryInv); // 暗→白(255)

            using var morph = new Mat();          // 形态学结果容器
            using var kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(3, 3)); // 3×3 矩形核
            Cv2.MorphologyEx(binary, morph, MorphTypes.Open, kernel); // 开运算：先腐蚀后膨胀去小噪点

            Cv2.FindContours(morph, out var contours, out _, // 找白色连通区域的外轮廓
                RetrievalModes.External, ContourApproximationModes.ApproxSimple); // 只取最外圈、简化点数

            var defects = new List<Rect>();       // 装合格缺陷的外接矩形
            foreach (var c in contours)           // 遍历每个轮廓
            {
                double area = Cv2.ContourArea(c); // 算这个轮廓占多少像素
                if (area >= recipe.MinDefectArea) // 面积够大才算真缺陷
                    defects.Add(Cv2.BoundingRect(c)); // 用最小外接矩形框住它
            }

            using var annotated = src.Clone();    // 复制原图用于画框(不污染原始数据)
            foreach (var r in defects)            // 遍历所有缺陷框
                Cv2.Rectangle(annotated, r, new Scalar(0, 0, 255), 2); // 画红色(OpenCV 是 BGR, 0,0,255=红)粗 2 的框

            return new VisionResult               // 打包返回
            {
                Pass = defects.Count == 0,       // 没缺陷=合格
                DefectCount = defects.Count,      // 缺陷数
                DefectRects = defects,            // 缺陷框列表
                Gray = gray.Clone(),              // 复制一份灰度图交出去(原 gray 会被 using 释放)
                Binary = morph.Clone(),           // 复制一份二值图交出去
                Annotated = annotated             // annotated 本身就是 Clone 出来的,直接交出去
            };
            // ⚠️ 注意：Gray/Binary/Annotated 由调用方负责释放(Step 8 会讲)
        }
    }
}
