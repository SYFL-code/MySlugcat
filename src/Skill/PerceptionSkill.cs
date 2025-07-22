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
using Kittehface.Framework20;


namespace MySlugcat;

// PerceptionSkill 感知能力
public class CreaturePointer
{
    private readonly Creature owner;
    private readonly FContainer pointerContainer;
    //private readonly FSprite pointerSprite;
    private readonly FSprite circleSprite;

    private const float PointerLength = 20f; // 缩短指针长度
    private const float PointerWidth = 8f;  // 增加指针宽度
    //private const float CircleRadius = 45f;
    //private const float RotationSpeed = 90f; // 度/秒

    private float camX;
    private float camY;

    // 调整指针尺寸参数
    private const float PointerHeight = 30f;  // 纸飞机高度
    private const float BaseWidth = 20f;     // 底部宽度
    private const float NotchDepth = 5f;     // 底部凹陷深度
    private const float CircleRadius = 45f;
    private const float RotationSpeed = 90f;

    private readonly TriangleMesh pointerMesh; // 使用网格创建自定义形状

    // 在CreaturePointer类中添加
    private Color startColor = Color.green;
    private Color endColor = Color.red;
    private float maxDistance = 750f; // 最大距离阈值

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
            pointerMesh = new TriangleMesh("pixel", new TriangleMesh.Triangle[]
            {
                new TriangleMesh.Triangle(0, 1, 2) // 单个三角形
            }, true, true);

            // 定义三角形顶点 (等腰三角形)
            pointerMesh.vertices[0] = new Vector2(0, PointerHeight);  // 顶点
            pointerMesh.vertices[1] = new Vector2(-BaseWidth / 2, 0);    // 左下角
            pointerMesh.vertices[2] = new Vector2(BaseWidth / 2, 0);     // 右下角

            pointerMesh.color = Color.red;
            pointerMesh.anchorX = 0.5f;
            pointerMesh.anchorY = 0f; // 底部锚点

            /*pointerSprite = new FSprite("pixel")
            {
                scaleX = PointerWidth,
                scaleY = PointerLength,
                color = Color.red,
                anchorX = 0.5f,
                anchorY = 0f // 中心锚点
            };*/

            // 3. 创建圆环背景
            circleSprite = new FSprite("Circle20")
            {
                scale = CircleRadius / 20f,
                color = new Color(1f, 1f, 1f, 0.1f),
                anchorX = 0.5f,
                anchorY = 0.5f
            };

            // 4. 添加到容器
            pointerContainer.AddChild(circleSprite);
            pointerContainer.AddChild(pointerMesh);
            //pointerContainer.AddChild(pointerSprite);

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


    public void Update_(Vector2 camPos)
    {
        camX = camPos.x;
        camY = camPos.y;

        if (owner == null || (owner.room == null && !owner.inShortcut) || (owner.slatedForDeletetion && !owner.inShortcut))
        {
            this.Destroy();
            return;
        }
    }

    public void Update(Creature? creature, float timeStacker)
    {
        if (owner == null || owner.room == null || owner.slatedForDeletetion)
        {
            this.Destroy();
            return;
        }

        bool shouldBeActive = (creature != null && !owner.inShortcut);

        if (creature != null)
        {
            Vector2? vector = creature.firstChunk.pos;
            if (vector == null)
            {
                shouldBeActive = false;
            }
        }
        if (owner.inShortcut)
        {
            shouldBeActive = false;
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
        if (fadeState <= 0f || !shouldBeActive || owner.inShortcut) return;
        if (creature == null) return;


        // 1. 获取世界坐标
        Vector2 targetWorldPos = creature.firstChunk.pos;
        Vector2 ownerWorldPos = owner.mainBodyChunk.pos;

        // 2. 计算世界空间中的方向向量
        Vector2 worldDirection = (targetWorldPos - ownerWorldPos).normalized;
        float targetAngle = Custom.VecToDeg(worldDirection);

        // 3. 计算屏幕空间位置
        Vector2 camPos = new Vector2(ownerWorldPos.x - camX, ownerWorldPos.y - camY);
        Vector2 ownerScreenPos = ownerWorldPos - camPos;
        Vector2 pointerScreenPos = ownerScreenPos + worldDirection * CircleRadius;

        // 4. 更新指针位置和旋转
        pointerMesh.SetPosition(pointerScreenPos);
        pointerMesh.rotation = targetAngle;

        // 更新圆环位置
        circleSprite.SetPosition(ownerScreenPos);



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

        // 淡入淡出圆环动画
        circleSprite.scale = (CircleRadius / 20f) * (0.9f + EaseInOut(fadeState) * 0.2f);

        // 目标接近时震动效果
        float distance2 = Vector2.Distance(ownerWorldPos, targetWorldPos);
        float shakeIntensity = Mathf.Clamp01(1f - distance2 / 300f) * fadeState;
        pointerMesh.SetPosition(pointerScreenPos + Custom.RNV() * shakeIntensity * 3f);

        // 更新发光效果
        Vector2 pointerTip = owner.mainBodyChunk.pos +
        Custom.DegToVec(pointerMesh.rotation) * (CircleRadius + PointerLength);
        for (int i = 0; i < glowEffects.Count; i++)
        {
            Vector2 offset = Custom.RNV() * 5f * (i + 1);
            glowEffects[i].pos = pointerTip + offset;
            glowEffects[i].setRad = 30f + Mathf.Sin(Time.time * 2f + i) * 10f;
            glowEffects[i].setAlpha = 0.7f;
        }

        // 动态颜色变化
        float distance = Vector2.Distance(owner.mainBodyChunk.pos, targetWorldPos);
        float Lerp = Mathf.Clamp01(distance / maxDistance);
        pointerMesh.color = Color.Lerp(endColor, startColor, Lerp / 1.5f);

        // 脉冲动画
        float pulse = 0.5f + Mathf.Sin(Time.time * pulseSpeed) * 0.5f;
        float pulseIntensity_ = Mathf.Clamp01(maxDistance / distance / 5) / 3;
        if (distance >= 800)
        {
            pulseIntensity_ = 0f;
        }
        pointerMesh.scaleX = baseLength * (1f + pulse * pulseIntensity_) * 0.03f;
        pointerMesh.scaleY = 8f * (1f + pulse * pulseIntensity_) * 0.03f;
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
        On.RoomCamera.SpriteLeaser.Update += SLeaser_Update;

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
            if (N == 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    if (pointer[i] != null)
                    {
                        pointer[i].Destroy();
                    }
                }
            }
            pointer[N] = new CreaturePointer(self);
        }
    }

    public static void SLeaser_Update(On.RoomCamera.SpriteLeaser.orig_Update orig, RoomCamera.SpriteLeaser self, float timeStacker, RoomCamera rCam, Vector2 camPos)
    {
        orig.Invoke(self, timeStacker, rCam, camPos);

        for (int i = 0; i < 20; i++)
        {
            if (pointer[i] != null)
            {
                pointer[i].Update_(camPos);
            }
        }

    }

    private static void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
    {
        orig(self, eu);

        int N = self.playerState.playerNumber;
        if (pointer != null && pointer[N] != null && self.slugcatStats.name == Plugin.YourSlugID && SC.PerceptionSkill)
        {
            Creature? creature = MyPlayer.FindNearestCreature(self.firstChunk.pos, self.room, false, self, false, 2);
            pointer[N].Update(creature, Time.deltaTime);

        }
    }

}


