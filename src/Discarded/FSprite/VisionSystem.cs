/*using UnityEngine;
using static Futile; // 假设使用 Futile 引擎

public class TransparentCircleInRect : FContainer
{
    public TransparentCircleInRect()
    {
        // 1. 创建一个 FContainer 作为父容器
        FContainer container = new FContainer();
        AddChild(container);

        // 2. 添加一个长方形（背景）
        FSprite rect = new FSprite("square"); // 假设 "square" 是白色矩形
        rect.width = 400; // 设置宽度
        rect.height = 200; // 设置高度
        rect.color = Color.blue; // 设置颜色
        container.AddChild(rect);

        // 3. 添加一个圆形（作为“透明”部分）
        FSprite circle = new FSprite("circle"); // 假设 "circle" 是白色圆形
        circle.width = 100; // 设置直径
        circle.height = 100;
        circle.x = 0; // 居中
        circle.y = 0;
        circle.color = new Color(0, 0, 0, 0); // 完全透明（如果引擎支持）

        // 如果引擎不支持直接透明，可以使用混合模式（如 Futile 的 `BlendMode.Subtractive`）
        // circle.blendMode = BlendMode.Subtractive; // 可能需要调整

        container.AddChild(circle);
    }
}*/