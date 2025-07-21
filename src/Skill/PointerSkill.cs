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

        // 创建显示容器
        pointerContainer = new FContainer();
        Futile.stage.AddChild(pointerContainer);

        // 初始完全透明
        pointerContainer.alpha = 0f;

        // 创建指针精灵 - 使用三角形而不是矩形
        pointerSprite = new FSprite("Triangle")
        {
            scaleX = PointerWidth * 0.5f,
            scaleY = PointerLength * 0.5f,
            color = Color.red,
            anchorX = 0.5f,
            anchorY = 0f // 锚点在底部中心
        };

        // 创建背景圆环 - 更透明
        circleSprite = new FSprite("Circle20")
        {
            scale = CircleRadius / 20f,
            color = new Color(1f, 1f, 1f, 0.2f) // 增加透明度
        };

        pointerContainer.AddChild(circleSprite);
        pointerContainer.AddChild(pointerSprite);

        baseLength = PointerLength;

        for (int i = 0; i < 3; i++)
        {
            var light = new LightSource(owner.mainBodyChunk.pos, false, Color.red, owner);
            glowEffects.Add(light);
            owner.room.AddObject(light);
        }
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

    public void Update(Vector2 nearestPos, float timeStacker, AbstractCreature abstractCreature)
    {
        // 销毁检查
        if (owner.room == null || owner.slatedForDeletetion)
        {
            Destroy();
            return;
        }

        // 更新激活状态
        bool shouldBeActive = (nearestPos != null);
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
        if (fadeState <= 0f || !(nearestPos != null)) return;

        // 计算指向目标的角度
        Vector2 ownerPos = owner.mainBodyChunk.pos;
        float targetAngle = Custom.VecToDeg(nearestPos - ownerPos);

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

        /*        // 计算指针位置(围绕生物旋转)
                Vector2 ownerPos = owner.mainBodyChunk.pos;
                float angle = Mathf.LerpAngle(
                    pointerSprite.rotation,
                    Custom.VecToDeg(nearestPos - ownerPos),
                    timeStacker * RotationSpeed * 0.01f);*/

        /*        // 更新指针位置和旋转
                pointerContainer.SetPosition(ownerPos);
                pointerSprite.rotation = angle;*/

        // 危险程度指示(根据生物类型)
        if (owner.room != null)
        {
            AbstractCreature target = abstractCreature;
            if (target != null)
            {
                float threatLevel = CalculateThreatLevel(target);
                pointerSprite.alpha = 0.5f + threatLevel * 0.5f;

                // 危险时增加脉冲强度
                SetPulseEffect(3f, 0.1f + threatLevel * 0.3f);
            }
        }

        // 在Update方法中添加
        circleSprite.scale = (CircleRadius / 20f) * (0.9f + EaseInOut(fadeState) * 0.2f);

        // 在Update方法中添加
        pointerSprite.color = Color.Lerp(
            new Color(1f, 1f, 1f, 0f),
            Color.red,
            EaseInOut(fadeState));

        if ((nearestPos != null))
        {
            float distance2 = Vector2.Distance(owner.mainBodyChunk.pos, nearestPos);
            float shakeIntensity = Mathf.Clamp01(1f - distance2 / 300f) * fadeState;

            pointerSprite.SetPosition(pointerOffset + Custom.RNV() * shakeIntensity * 3f);
        }

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
        float distance = Vector2.Distance(owner.mainBodyChunk.pos, nearestPos);
        float colorLerp = Mathf.Clamp01(distance / maxDistance);
        pointerSprite.color = Color.Lerp(endColor, startColor, colorLerp);

        // 脉冲动画
        float pulse = 0.5f + Mathf.Sin(Time.time * pulseSpeed) * 0.5f;
        pointerSprite.scaleX = baseLength * (1f + pulse * pulseIntensity);

/*        // 使指针指向目标
        Vector2 pointerDir = Custom.DegToVec(angle);
        pointerSprite.SetPosition(pointerDir * CircleRadius);*/
    }

    private float CalculateThreatLevel(AbstractCreature creature)
    {
        // 根据生物类型返回威胁等级(0-1)
        if (creature.creatureTemplate.type == CreatureTemplate.Type.RedLizard)
            return 1f;
        if (creature.creatureTemplate.type == CreatureTemplate.Type.BlueLizard)
            return 0.7f;
        return 0.3f;
    }

    // 缓动函数 - 使动画更自然
    private float EaseInOut(float t)
    {
        return t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
    }

    public void Destroy()
    {
        pointerContainer.RemoveFromContainer();
    }
}






/*using On;
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


namespace MySlugcat
{
    //感知能力
    public class PointerSkill
    {

        // 指针线数组，用于绘制指向最近拾荒者的3D效果指针
        public FSprite[]? scavPointerLines;

        // 指针应该指向的理想方向（归一化向量）
        public Vector2 shouldPointAt;

        // 指针当前实际指向的方向（用于平滑过渡）
        public Vector2 pointAt;

        // 上一帧的指针方向（用于插值计算）
        public Vector2 lastPointAt;

        // 指针的淡入淡出透明度（0-1）
        public float pointerFade;

        // 是否应该显示指针的标志
        public bool showPointer;

        // 指针的旋转值（用于3D效果）
        public float pointerRotation;

        // 上一帧的旋转值（用于插值计算）
        public float lastPointerRotation;

        // 指针3D效果的宽度
        public float pointer3DWidth = 3f;

        // 指针基础宽度
        public float pointerWidth = 6f;

        // 指针旋转的增量（控制旋转速度）
        public float pointerRotationAdd;

        // 指针的目标颜色（根据拾荒者等级变化）
        public Color goalPointerColor;

        // 褪色速度
        public const float fadeSpeed = 0.2f;

        public void Hook()
        {
#if MYDEBUG
            try
            {
#endif
            //On.Player.ctor += Player_ctor;
            //On.Player.Update += Player_Update;
            On.Player.ctor += Player_ctor;
            On.Player.Update += Player_Update;
            On.HUD.HudPart.Draw += Draw;
            On.HUD.HudPart.ClearSprites += ClearSprites;

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

        public void Player_ctor(On.Player.orig_ctor orig, Player player, AbstractCreature abstractCreature, World world)
        {
            //player.room.game.cameras[0].hud
            //world.game.cameras[0].hud
            //player.room.roo

            orig.Invoke(player, abstractCreature, world);

            Log.Logger(7, "PointerSkill", "MySlugcat:PointerSkill:Player_ctor_", $"st_");

            // 在构造函数中初始化指针线
            scavPointerLines = new FSprite[9]; // 创建9个精灵用于组成3D指针
            for (int i = 0; i < 9; i++)
            {
                // 每个精灵使用"pixel"图像和"Hologram"着色器
                scavPointerLines[i] = new FSprite("pixel")
                {
                    shader = Custom.rainWorld.Shaders["Hologram"],
                    color = Color.white
                };
                player.room.game.cameras[0].hud.fContainers[1].AddChild(scavPointerLines[i]); // 添加到HUD容器
            }
            // 初始化指针效果参数
            pointerRotationAdd = 1f; // 初始旋转增量
            goalPointerColor = Color.white; // 初始颜色设为白色

        }

        public void Player_Update(On.Player.orig_Update orig, Player player, bool eu)
        {
            orig.Invoke(player, eu);

            Log.Logger(9, "IntelHUD", "MySlugcat:IntelHUD​​:Update", $"st");
            // 在Update()方法中更新指针逻辑
            showPointer = false; // 默认不显示指针

            if (player.slugcatStats.name == Plugin.YourSlugID)
            {
                Creature? creature = MyPlayer.FindNearestCreature(player.mainBodyChunk.pos, player.room, false, player, false, 2);
                if (creature != null)
                {
                    shouldPointAt = Custom.DirVec(player.mainBodyChunk.pos, creature.mainBodyChunk.pos);
                    goalPointerColor = creature.ShortCutColor();
                }
                showPointer = creature != null && !player.inShortcut;
            }

            // 平滑更新指针方向（使用球面线性插值）
            lastPointAt = pointAt;
            pointAt = Vector3.Slerp(pointAt, shouldPointAt, 0.3f);

            // 更新指针淡入淡出效果
            if (showPointer)
            {
                pointerFade = Mathf.Min(1f, pointerFade + fadeSpeed * 0.1f); // 淡入
            }
            else
            {
                pointerFade = Mathf.Max(0f, pointerFade - fadeSpeed * 0.1f); // 淡出
            }

            // 更新指针旋转效果（模拟3D效果）
            lastPointerRotation = pointerRotation;
            pointerRotation += 1f / Mathf.Lerp(120f, 40f, UnityEngine.Random.value) * pointerRotationAdd;

            // 限制旋转范围并反转方向
            if (pointerRotation > 1f)
            {
                pointerRotationAdd = -1f;
            }
            else if (pointerRotation < -1f)
            {
                pointerRotationAdd = 1f;
            }

        }

        public void Draw(On.HUD.HudPart.orig_Draw orig, HUD.HudPart hudPart, float timeStacker)
        {
            orig(hudPart, timeStacker);

            Log.Logger(9, "IntelHUD", "MySlugcat:IntelHUD​​:Draw", $"player ({hudPart.hud.owner is Player})");
            if (hudPart.hud.owner is Player player && player.slugcatStats.name == Plugin.YourSlugID)
            {
                Room room = player.abstractCreature.world.game.cameras[0].room;
                Log.Logger(9, "IntelHUD", "MySlugcat:IntelHUD​​:Draw", $"Room_Null ({room == null})");
                if (room != null && scavPointerLines != null)
                {
                    // 计算插值后的旋转因子
                    float rotationFac = Mathf.Lerp(lastPointerRotation, pointerRotation, timeStacker);
                    float rotationSign = Mathf.Sign(rotationFac);

                    // 计算插值后的指针方向
                    Vector2 pointerDir = Vector3.Slerp(lastPointAt, pointAt, timeStacker);
                    Vector2 playerPos = Vector2.Lerp(player.mainBodyChunk.lastPos, player.mainBodyChunk.pos, timeStacker);
                    Vector2 perp = Custom.PerpendicularVector(pointerDir); // 垂直向量

                    // 计算指针各关键点位置
                    Vector2 backPoint = playerPos + pointerDir * 20f; // 指针起点（靠近玩家）
                    Vector2 frontPoint = playerPos + pointerDir * 34f; // 指针终点（指向目标方向）
                    Vector2 rightPoint = backPoint + perp * pointerWidth * (1f * rotationSign - rotationFac);
                    Vector2 leftPoint = backPoint - perp * pointerWidth * (1f * rotationSign - rotationFac);
                    Vector2 rightFac = perp * pointer3DWidth * rotationFac; // 3D效果偏移量

                    float gruh = 14f;
                    float grug = Mathf.Sqrt(gruh * gruh + pointerWidth * pointerWidth);

                    scavPointerLines[3].SetPosition(backPoint + rightFac - room.game.cameras[0].pos);
                    scavPointerLines[3].scaleX = pointerWidth * 2f * (1f * rotationSign - rotationFac);
                    scavPointerLines[3].rotation = Custom.VecToDeg(pointerDir);

                    scavPointerLines[4].SetPosition(Vector2.Lerp(rightPoint + rightFac, frontPoint + rightFac * 0.5f, 0.5f) - room.game.cameras[0].pos);
                    scavPointerLines[4].scaleY = grug;
                    scavPointerLines[4].rotation = Custom.VecToDeg(Custom.DirVec(rightPoint + rightFac, frontPoint + rightFac * 0.5f));

                    scavPointerLines[5].SetPosition(Vector2.Lerp(leftPoint + rightFac, frontPoint + rightFac * 0.5f, 0.5f) - room.game.cameras[0].pos);
                    scavPointerLines[5].scaleY = grug;
                    scavPointerLines[5].rotation = Custom.VecToDeg(Custom.DirVec(leftPoint + rightFac, frontPoint + rightFac * 0.5f));

                    scavPointerLines[6].SetPosition(backPoint - rightFac - room.game.cameras[0].pos);
                    scavPointerLines[6].scaleX = pointerWidth * 2f * (1f * rotationSign - rotationFac);
                    scavPointerLines[6].rotation = Custom.VecToDeg(pointerDir);

                    scavPointerLines[7].SetPosition(Vector2.Lerp(rightPoint - rightFac, frontPoint - rightFac * 0.5f, 0.5f) - room.game.cameras[0].pos);
                    scavPointerLines[7].scaleY = grug;
                    scavPointerLines[7].rotation = Custom.VecToDeg(Custom.DirVec(rightPoint - rightFac, frontPoint - rightFac * 0.5f));

                    scavPointerLines[8].SetPosition(Vector2.Lerp(leftPoint - rightFac, frontPoint - rightFac * 0.5f, 0.5f) - room.game.cameras[0].pos);
                    scavPointerLines[8].scaleY = grug;
                    scavPointerLines[8].rotation = Custom.VecToDeg(Custom.DirVec(leftPoint - rightFac, frontPoint - rightFac * 0.5f));

                    for (int i = 0; i < 2; i++)
                    {
                        scavPointerLines[i + 1].SetPosition(backPoint + perp * pointer3DWidth * (i == 0 ? 1 : -1) * (1f * rotationSign - rotationFac) - room.game.cameras[0].pos);
                        scavPointerLines[i + 1].scaleX = pointer3DWidth * 2f * rotationFac;
                        scavPointerLines[i + 1].rotation = Custom.VecToDeg(pointerDir);
                    }
                    scavPointerLines[0].SetPosition(frontPoint - room.game.cameras[0].pos);
                    scavPointerLines[0].scaleX = pointer3DWidth * rotationFac;
                    scavPointerLines[0].rotation = Custom.VecToDeg(pointerDir);

                    *//*                    foreach (var stat in scavStats)
                                        {
                                            stat.Draw(timeStacker, room.game.cameras[0].pos);
                                        }*/

/*// 设置指针颜色渐变
for (int i = 0; i < 9; i++)
{
    scavPointerLines[i].color = Color.Lerp(scavPointerLines[i].color, goalPointerColor, 0.2f);
}*//*
}
}

// 设置指针颜色渐变
for (int i = 0; i < 9; i++)
{
if (scavPointerLines != null)
{
scavPointerLines[i].alpha = pointerFade;
scavPointerLines[i].isVisible = scavPointerLines[i].alpha > 0f;
scavPointerLines[i].color = Color.Lerp(scavPointerLines[i].color, goalPointerColor, 0.2f);
}

}

}

// 清除指针精灵
public void ClearSprites(On.HUD.HudPart.orig_ClearSprites orig, HUD.HudPart hudPart)
{
orig(hudPart);

Log.Logger(9, "IntelHUD", "MySlugcat:IntelHUD​​:ClearSprites", $"st");
for (int i = 0; i < 9; i++)
{
if (scavPointerLines != null)
{
scavPointerLines[i].RemoveFromContainer();
}

}
}



}
}*/
