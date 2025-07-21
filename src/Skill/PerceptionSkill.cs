using On;
using IL;
using System;
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


namespace MySlugcat;

// PerceptionSkill 感知能力
public class CreaturePointer
{
    private readonly Creature owner;
    private readonly FContainer pointerContainer;
    private readonly FSprite pointerSprite;
    private readonly FSprite circleSprite;

    private const float PointerLength = 20f; // 缩短指针长度
    private const float PointerWidth = 8f;  // 增加指针宽度
    private const float CircleRadius = 30f;
    private const float RotationSpeed = 90f; // 度/秒

    // 在CreaturePointer类中添加
    private Color startColor = Color.green;
    private Color endColor = Color.red;
    private float maxDistance = 500f; // 最大距离阈值

    // 在CreaturePointer类中添加
    private float pulseSpeed = 3f; // 脉冲速度
    private float pulseIntensity = 0.2f; // 脉冲强度
    private float baseLength; // 存储原始长度

    // 新增淡入淡出控制变量
    private float fadeState = 0f; // 0-1表示淡入淡出进度
    private const float FadeSpeed = 2f; // 淡入淡出速度
    private bool isActive = false;

    private List<LightSource> glowEffects = new List<LightSource>();

    public CreaturePointer(Creature owner)
    {
        this.owner = owner;

        try
        {
            // 1. 创建显示容器
            pointerContainer = new FContainer();
            Futile.stage.AddChild(pointerContainer);
            pointerContainer.alpha = 0f; // 初始透明

            // 2. 使用可靠的"pixel"精灵创建指针
            pointerSprite = new FSprite("pixel")
            {
                scaleX = PointerWidth,
                scaleY = PointerLength,
                color = Color.red,
                anchorX = 0.5f,
                anchorY = 0.5f // 中心锚点
            };

            // 3. 创建圆环背景
            circleSprite = new FSprite("Circle20")
            {
                scale = CircleRadius / 20f,
                color = new Color(1f, 1f, 1f, 0.2f)
            };

            // 4. 添加到容器
            pointerContainer.AddChild(circleSprite);
            pointerContainer.AddChild(pointerSprite);

            // 5. 初始化其他变量
            baseLength = PointerLength;

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


    public void Update(Creature? creature, float timeStacker)
    {
        if (owner.room == null || owner.slatedForDeletetion || owner == null)
        {
            return;
        }
        
        bool shouldBeActive = (creature != null);

        if (creature != null)
        {
            Vector2? vector = creature.firstChunk.pos;
            if (vector == null)
            {
                shouldBeActive = false;
            }
        }
        
        if (shouldBeActive != isActive)
        {
            isActive = shouldBeActive;
            fadeState = Mathf.Clamp01(fadeState); // 确保在0-1范围内
        }

        // 淡入淡出控制
        fadeState += (isActive ? FadeSpeed : -FadeSpeed) * timeStacker;
        fadeState = Mathf.Clamp01(fadeState);
        pointerContainer.alpha = EaseInOut(fadeState); // 使用缓动函数使过渡更平滑

        // 如果没有激活或完全透明，跳过更新
        if (fadeState <= 0f || !shouldBeActive) return;
        if (creature == null) return;

        Vector2 nPos = creature.firstChunk.pos;

        // 计算指向目标的角度
        Vector2 ownerPos = owner.mainBodyChunk.pos;
        float targetAngle = Custom.VecToDeg(nPos - ownerPos);

        // 平滑旋转
        float currentAngle = Mathf.LerpAngle(
            pointerSprite.rotation,
            targetAngle,
            timeStacker * RotationSpeed * 0.01f);

        // 更新指针位置和旋转
        pointerContainer.SetPosition(ownerPos);
        pointerSprite.rotation = currentAngle;

        // 调整指针位置(尖端指向目标)
        Vector2 pointerOffset = Custom.DegToVec(currentAngle) * CircleRadius;
        pointerSprite.SetPosition(pointerOffset);

        // 根据淡入淡出状态调整大小(可选效果)
        float scaleMod = 0.8f + EaseInOut(fadeState) * 0.4f;
        pointerSprite.scaleX = PointerWidth * 0.5f * scaleMod;
        pointerSprite.scaleY = PointerLength * 0.5f * scaleMod;

        // 危险程度指示(根据生物类型)
        /*if (owner.room != null)
        {
            float threatLevel = CalculateThreatLevel(target);
            pointerSprite.alpha = 0.5f + threatLevel * 0.5f;

            // 危险时增加脉冲强度
            SetPulseEffect(3f, 0.1f + threatLevel * 0.3f);
        }*/

        // 淡入淡出圆环动画
        circleSprite.scale = (CircleRadius / 20f) * (0.9f + EaseInOut(fadeState) * 0.2f);

        // 淡入淡出颜色变化
        /*pointerSprite.color = Color.Lerp(
            new Color(1f, 1f, 1f, 0f),
            Color.red,
            EaseInOut(fadeState));*/

        // 目标接近时震动效果
        float distance2 = Vector2.Distance(owner.mainBodyChunk.pos, nPos);
        float shakeIntensity = Mathf.Clamp01(1f - distance2 / 300f) * fadeState;
        pointerSprite.SetPosition(pointerOffset + Custom.RNV() * shakeIntensity * 3f);

        // 更新发光效果
        Vector2 pointerTip = owner.mainBodyChunk.pos +
            Custom.DegToVec(pointerSprite.rotation) * (CircleRadius + PointerLength);
        for (int i = 0; i < glowEffects.Count; i++)
        {
            Vector2 offset = Custom.RNV() * 5f * (i + 1);
            glowEffects[i].pos = pointerTip + offset;
            glowEffects[i].setRad = 30f + Mathf.Sin(Time.time * 2f + i) * 10f;
            glowEffects[i].setAlpha = 0.7f;
        }

        // 动态颜色变化
        float distance = Vector2.Distance(owner.mainBodyChunk.pos, nPos);
        float colorLerp = Mathf.Clamp01(distance / maxDistance);
        pointerSprite.color = Color.Lerp(endColor, startColor, colorLerp);

        // 脉冲动画
        float pulse = 0.5f + Mathf.Sin(Time.time * pulseSpeed) * 0.5f;
        pointerSprite.scaleX = baseLength * (1f + pulse * pulseIntensity);
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
        pointerContainer.RemoveFromContainer();
    }




    private static CreaturePointer[] pointer = new CreaturePointer[20];

    public static void Hook()
    {
#if MYDEBUG
            try
            {
#endif
        On.Player.ctor += Player_ctor;
        On.Player.Update += Player_Update;

#if MYDEBUG
            }
            catch (Exception e)
            {
                StackTrace st = new StackTrace(new StackFrame(true));
                StackFrame sf = st.GetFrame(0);
                var sr = sf.GetFileName().Split('\\');
                MyDebug.outStr = sr[sr.Length - 1] + "\n";
                MyDebug.outStr += sf.GetMethod() + "\n";
                MyDebug.outStr += e;
                UnityEngine.Debug.Log(e);
            }
#endif
    }

    private static void Player_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
    {
        orig.Invoke(self, abstractCreature, world);

        if (self.slugcatStats.name == Plugin.YourSlugID && SC.PerceptionSkill)
        {
            int N = self.playerState.playerNumber;
            pointer[N] = new CreaturePointer(self);
        }
    }

    private static void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
    {
        orig(self, eu);

        int N = self.playerState.playerNumber;
        if (pointer[N] != null && self.slugcatStats.name == Plugin.YourSlugID && SC.PerceptionSkill)
        {
            Creature? creature = MyPlayer.FindNearestCreature(self.firstChunk.pos, self.room, false, self, false, 2);
            pointer[N].Update(creature, Time.deltaTime);
        }
    }

}


