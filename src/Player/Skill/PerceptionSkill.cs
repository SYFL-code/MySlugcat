﻿using On;
using IL;
using System;
using System.Threading.Tasks;
using Mono.Cecil;
using MoreSlugcats;
using RWCustom;
using HUD;
using Smoke;
using static PhysicalObject;
using UnityEngine;
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Collections;
using SlugBase.Features;
using System.Diagnostics;
using ImprovedInput;
using System.Linq;
using RewiredConsts;
using Menu.Remix;
using MonoMod.RuntimeDetour;
using Watcher;
using static MonoMod.InlineRT.MonoModRule;
using Kittehface.Framework20;
using System.Threading;


namespace MySlugcat;

// PerceptionSkill 感知能力
public class PointerFSprite
{
    public readonly Creature owner;
    private readonly FContainer pointerContainer;
    //private readonly FSprite pointerSprite;
    private readonly FSprite circleSprite;

    private const float PointerLength = 25f; // 缩短指针长度
    private const float PointerWidth = 10f;  // 增加指针宽度
    private const float CircleRadius = 60f;

    // 摄像机坐标
    private float camX = 300f;
    private float camY = 300f;

    public int i = -200;
    public int lasti = -200;
    public int itoo = -200;
    public int ic = -200;
    public bool start = false;

    public bool slatedForDestroy = false;

    private readonly TriangleMesh pointerMesh; // 使用网格创建自定义形状

    // 平滑旋转
    private const float OrbitRadius = 50f; // 根据视觉效果调整
    private const float RotationSmoothness = 0.9f; // 旋转平滑度(0-1)，值越小越平滑
    private float currentAngle;
    private float targetAngle;
    //private const float CircleRadius = 30f;
    private const float RotationSpeed = 10f; // 度/秒

    // 动态颜色变化
    private Color startColor = Color.green;
    private Color endColor = Color.red;
    private float maxDistance = 750f; // 最大距离阈值

    // 脉冲动画
    private float pulseSpeed = 3f; // 脉冲速度
    private float pulseIntensity = 0.2f; // 脉冲强度
    private float baseLength; // 存储原始长度
    private float baseWidth; // 存储原始宽度


    // 新增淡入淡出控制变量
    private float fadeState = 0f; // 0-1表示淡入淡出进度
    private const float FadeSpeed = 2f; // 淡入淡出速度
    private bool isActive = false;

    private List<LightSource> glowEffects = new List<LightSource>();

    public PointerFSprite(Creature owner)
    {
        this.owner = owner;

        try
        {
            // 1. 创建显示容器
            pointerContainer = new FContainer();
            Futile.stage.AddChild(pointerContainer);
            pointerContainer.alpha = 0f; // 初始透明

            // 2. 使用可靠的"pixel"精灵创建指针
            pointerMesh = new TriangleMesh("pixel", new TriangleMesh.Triangle[]
            {
                new TriangleMesh.Triangle(0, 1, 2) // 单个三角形
            }, true, true);

            // 定义三角形顶点 (等腰三角形)
            pointerMesh.vertices[0] = new Vector2(0, PointerLength);  // 顶点
            pointerMesh.vertices[1] = new Vector2(-PointerWidth / 2, 0);    // 左下角
            pointerMesh.vertices[2] = new Vector2(PointerWidth / 2, 0);     // 右下角

            pointerMesh.color = Color.green;
            // 确保指针初始朝向正确
            pointerMesh.rotation = 0f; // 初始朝向右侧
            pointerMesh.anchorX = 0.5f;
            pointerMesh.anchorY = 0.5f; // 中心锚点便于旋转

            /*pointerSprite = new FSprite("pixel")
            {
                scaleX = PointerWidth,
                scaleY = PointerLength,
                color = Color.green,
                anchorX = 0.5f,
                anchorY = 0f // 中心锚点
            };*/

            // 3. 创建圆环背景
            circleSprite = new FSprite("Circle20")
            {
                scale = CircleRadius / 20f,
                color = new Color(1f, 1f, 1f, 0.01f),
                anchorX = 0.5f,
                anchorY = 0.5f
            };

            // 4. 添加到容器
            pointerContainer.AddChild(circleSprite);
            pointerContainer.AddChild(pointerMesh);
            //pointerContainer.AddChild(pointerSprite);

            // 5. 初始化其他变量
            baseLength = PointerLength;
            baseWidth = PointerWidth;
            currentAngle = 0f;
            targetAngle = 0f;

            // 6. 安全创建光效
            if (owner.room != null)
            {
                for (int i = 0; i < 3; i++)
                {
                    var light = new LightSource(owner.mainBodyChunk.pos, false, Color.red, owner);
                    glowEffects.Add(light);
                    owner.room.AddObject(light);
                }
            }

            // 确保初始状态完全透明且不可见
            pointerContainer.alpha = 0f;
            pointerContainer.isVisible = false;
            pointerMesh.alpha = 0f;
            circleSprite.alpha = 0f;
        }
        catch (Exception e)
        {
            Debug.LogError($"CreaturePointer初始化失败: {e.Message}");
            // 失败时确保清理资源
            if (pointerContainer != null)
            {
                pointerContainer.RemoveFromContainer();
            }
            throw; // 重新抛出异常让上层处理
        }
    }

    /*public void Update_(Vector2 camPos)
    {
        Log.Logger(7, "PerceptionSkill", "MySlugcat:CreaturePointer:Update__st",
            $"({owner == null})");

        if (owner == null || owner.slatedForDeletetion || slatedForDestroy)
        {
            this.Destroy();
            return;
        }

        Log.Logger(7, "PerceptionSkill", "MySlugcat:CreaturePointer:Update__st",
            $"({owner.room == null}), ({owner.slatedForDeletetion}), ({!owner.inShortcut})");//

        camX = camPos.x;
        camY = camPos.y;
    }*/

    public void Update(int N, bool Destroy)
    {
        Log.Logger(7, "PerceptionSkill", "MySlugcat:CreaturePointer:Update_st",
            $"({owner == null}), ({N})");//

        if (owner == null || owner.slatedForDeletetion || Destroy || slatedForDestroy)
        {
            this.Destroy();
            return;
        }

        Log.Logger(7, "PerceptionSkill", "MySlugcat:CreaturePointer:Update_st",
            $"({owner.room == null}), ({owner.slatedForDeletetion}), ({!owner.inShortcut}), ({N}), ({lasti}), ({i})");//

        if (Control.RWG != null)
        {
            if (Control.RWG.GamePaused)
            {
                return;
            }
        }
        if (Control.camPos != null)
        {
            camX = Control.camPos.x;
            camY = Control.camPos.y;
        }

        /*        lasti += 1;
                if (lasti < 0)
                {
                    i = lasti;
                }
                if (owner.room == null && owner.inShortcut)
                {
                    i = lasti;
                }
                if (start == false)
                {
                    i = lasti;
                }

                if (Math.Abs(lasti - i) <= 20)
                {

                    if (itoo == i)
                    {
                        ic += 1;
                    }
                    else
                    {
                        ic = 0;
                    }
                    if (ic > 30)
                    {
                        if (owner.room == null && owner.inShortcut)
                        {
                            i = lasti;
                        }
                        else
                        {
                            this.Destroy();
                            return;
                        }
                    }

                    itoo = i;

                }
                else
                {
                    if (owner.room == null && owner.inShortcut)
                    {
                        i = lasti;
                    }
                    else
                    {
                        this.Destroy();
                        return;
                    }
                }*/

        if (!Control.isRunning[N])
        {
            this.Destroy();
            return;
        }

        bool shouldBeActive = true;
        Creature? creature = null;
        float timeStacker = Time.deltaTime;//1秒60帧，那增量时间就是 1/60 秒  (Time.deltaTime)

        if (owner.room == null && owner.inShortcut)
        {
            shouldBeActive = false;
        }
        if (owner is Player player && player.Sleeping)
        {
            shouldBeActive = false;
        }
        if (owner.room != null)
        {
            creature = MyPlayer.FindNearestCreature(owner.firstChunk.pos, owner.room, false, owner, false, 2);
            if (creature == null)
            {
                shouldBeActive = false;
            }
            if (creature != null)
            {
                Vector2? vector = creature.firstChunk.pos;
                if (vector == null)
                {
                    shouldBeActive = false;
                }
            }
        }

        if (shouldBeActive != isActive)
        {
            isActive = shouldBeActive;
            fadeState = Mathf.Clamp01(fadeState); // 确保在0-1范围内
        }

        // 改进的淡入淡出控制
        float targetAlpha = isActive ? 1f : 0f;
        float fadeDelta = (isActive ? FadeSpeed : -FadeSpeed) * Time.deltaTime;

        // 更平滑的渐变过渡
        fadeState = Mathf.Clamp01(fadeState + fadeDelta * 0.5f); // 降低变化速度

        // 使用更明显的缓动函数
        float currentAlpha = EnhancedEaseInOut(fadeState);

        // 应用透明度到所有元素
        pointerContainer.alpha = currentAlpha;
        pointerMesh.alpha = currentAlpha;
        circleSprite.alpha = currentAlpha * 0.8f;

        // 更新光效透明度
        foreach (var light in glowEffects)
        {
            light.setAlpha = currentAlpha * 0.7f;
        }

        // 确保当完全透明时停止更新
        if (fadeState <= 0f)
        {
            pointerContainer.isVisible = false; // 直接隐藏整个容器
            return;
        }
        else
        {
            pointerContainer.isVisible = true;
        }


        Log.Logger(7, "PerceptionSkill", "MySlugcat:CreaturePointer:Update_st",
            $"渐变状态: fadeState={fadeState}, alpha={currentAlpha}, isActive={isActive}");

        /*        // 淡入淡出控制
                fadeState += (isActive ? FadeSpeed : -FadeSpeed) * timeStacker;
                fadeState = Mathf.Clamp01(fadeState);
                pointerContainer.alpha = EaseInOut(fadeState); // 使用缓动函数使过渡更平滑
                pointerMesh.alpha = EaseInOut(fadeState);
                //circleSprite.alpha = EaseInOut(fadeState) * (0.9f + EaseInOut(fadeState) * 0.2f);

                // 淡入淡出圆环动画
                circleSprite.scale = (CircleRadius / 20f) * (0.9f + EaseInOut(fadeState) * 0.2f);*/

        // 如果没有激活或完全透明，跳过更新
        if (fadeState <= 0f || !shouldBeActive || owner.inShortcut) return;
        if (creature == null) return;


        // 1. 获取世界坐标
        Vector2 targetWorldPos = creature.mainBodyChunk.pos;
        Vector2 ownerWorldPos = owner.firstChunk.pos;

        // 2. 计算方向向量(从玩家指向目标)
        Vector2 direction = (targetWorldPos - ownerWorldPos).normalized;
        targetAngle = Custom.VecToDeg(direction);
        //targetAngle = Custom.VecToDeg(targetWorldPos - ownerWorldPos);

        // 平滑旋转
        float currentAngle = Mathf.LerpAngle(
            pointerMesh.rotation,
            targetAngle,
            timeStacker * RotationSpeed * 0.01f);


        // 3. 计算屏幕空间位置
        Vector2 ownerScreenPos = new Vector2(ownerWorldPos.x - camX, ownerWorldPos.y - camY);
        //Vector2 pointerScreenPos = ownerScreenPos + worldDirection * CircleRadius / 3 * 2;

/*        // 5. 更新指针位置(公转)
        Vector2 pointerScreenPos = ownerScreenPos + orbitOffset;
        pointerMesh.SetPosition(pointerScreenPos);*/

        // 更新指针位置和旋转
        pointerContainer.SetPosition(ownerScreenPos);
        pointerMesh.rotation = currentAngle;

        // 调整指针位置(尖端指向目标)
        Vector2 pointerOffset = Custom.DegToVec(currentAngle) * 30f;
        pointerMesh.SetPosition(pointerOffset);

        /*// 保持指针固定朝向(指向目标)
        pointerMesh.rotation = targetAngle;*/

        //pointerMesh.rotation = targetAngle;

        // 更新圆环位置
        //circleSprite.SetPosition(ownerScreenPos);



/*        // 计算指向目标的角度
        float targetAngle = Custom.VecToDeg(targetPos - ownerPos);

        // 平滑旋转
        float currentAngle = Mathf.LerpAngle(
              pointerMesh.rotation,
              targetAngle,
              timeStacker * RotationSpeed * 0.01f);

        // 计算从玩家到目标的方向向量
        Vector2 Direction = Custom.DegToVec(currentAngle);

        // 更新指针旋转时使用pointerMesh代替pointerSprite
        pointerMesh.rotation = currentAngle;
        //pointerMesh.SetPosition(pos);//
        circleSprite.SetPosition(camPos);*/


        // 危险程度指示(根据生物类型)
        /*if (owner.room != null)
        {
            float threatLevel = CalculateThreatLevel(target);
            pointerSprite.alpha = 0.5f + threatLevel * 0.5f;

            // 危险时增加脉冲强度
            SetPulseEffect(3f, 0.1f + threatLevel * 0.3f);
        }*/

        // 目标接近时震动效果
        float distance = Vector2.Distance(ownerWorldPos, targetWorldPos);
        distance = distance * (distance > 600 ? 0.8f : 1) * (distance > 450 ? 0.9f : 1) * (distance > 300 ? 0.94f : 1f) * (distance > 200 ? 0.98f : 1);
        float shakeIntensity = Mathf.Clamp01(1f - distance / 300f) * fadeState;
        Vector2 exactPos = pointerOffset + Custom.RNV() * shakeIntensity * 3f;
        pointerMesh.SetPosition(exactPos);

        // 动态颜色变化
        float Lerp = Mathf.Clamp01(distance / maxDistance);
        pointerMesh.color = Color.Lerp(endColor, startColor, Lerp / 1.5f);

        /*// 更新光效位置
        for (int i = 0; i < glowEffects.Count; i++)
        {
            float offsetAngle = radianAngle + i * Mathf.PI * 0.66f;
            Vector2 lightOffset = new Vector2(
                Mathf.Cos(offsetAngle) * OrbitRadius * 1.3f,
                Mathf.Sin(offsetAngle) * OrbitRadius * 1.3f
            );
            glowEffects[i].pos = ownerWorldPos + lightOffset;
            glowEffects[i].setRad = 30f + Mathf.Sin(Time.time * 2f + i) * 10f;
            glowEffects[i].setAlpha = 0.7f;
            glowEffects[i].color = Color.Lerp(endColor, startColor, Lerp / 1.5f);
        }*/
        // 更新发光效果位置(跟随公转)
        /*for (int i = 0; i < glowEffects.Count; i++)
        {
            Vector2 lightPos = ownerWorldPos + orbitOffset * 1.2f; // 稍微远离中心
            glowEffects[i].pos = lightPos;
            // ... 其他光效设置 ...
            glowEffects[i].setRad = 30f + Mathf.Sin(Time.time * 2f + i) * 10f;
            glowEffects[i].setAlpha = 0.7f;
            glowEffects[i].color = Color.Lerp(endColor, startColor, Lerp / 1.5f);
        }*/

        // 脉冲动画
        float pulse = 0.5f + Mathf.Sin(Time.time * pulseSpeed) * 0.5f;
        float pulseIntensity_ = Mathf.Clamp01(maxDistance / distance / 5) / 3;
        if (distance >= 1100)
        {
            pulseIntensity_ = 0f;
        }
        pointerMesh.scaleX = baseLength * (1f + pulse * pulseIntensity_) * 0.075f;
        pointerMesh.scaleY = baseWidth * (1f + pulse * pulseIntensity_) * 0.075f;
    }

    private float CalculateThreatLevel(Creature target)
    {
        // 根据生物类型返回威胁等级(0-1)
        if (target is Lizard lizard)
            return 1f;
        if (target is Lizard Scavenger)
            return 0.7f;
        return 0.3f;
    }

    // 更明显的缓动函数
    private float EnhancedEaseInOut(float t)
    {
        // 使用三次方缓动，效果更明显
        return t < 0.5f ?
            4f * t * t * t :
            1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
    }

    // 缓动函数 - 使动画更自然
    private float EaseInOut(float t)
    {
        return t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
    }

    // 可以公开这些属性供外部调整
    public void SetColorRange(Color nearColor, Color farColor, float maxDist)
    {
        endColor = nearColor;
        startColor = farColor;
        maxDistance = maxDist;
    }
    public void SetPulseEffect(float speed, float intensity)
    {
        pulseSpeed = speed;
        pulseIntensity = intensity;
    }

    public void Destroy()
    {
        // 先隐藏再移除
        slatedForDestroy = true;
        if (pointerContainer != null)
        {
            pointerContainer.isVisible = false;
            pointerContainer.RemoveFromContainer();
        }

        // 清理光效
        foreach (var light in glowEffects)
        {
            if (light != null && light.room != null)
            {
                light.Destroy();
            }
        }
        glowEffects.Clear();
    }



}
