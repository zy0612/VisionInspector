namespace VisionInspector.Vision // 视觉层命名空间(跟 VisionEngine 同层,无需 using)
{
    // 检测"配方"：把可调参数单独抽出来,界面以后能改、能存 JSON 复用
    public class VisionRecipe
    {
        public double Threshold { get; set; } = 128;     // 二值化阈值：低于它的暗像素判为缺陷
        public double MinDefectArea { get; set; } = 200; // 最小缺陷面积(像素²)：小于它的当噪点忽略
    }
}
