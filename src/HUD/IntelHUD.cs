using UnityEngine;
using RWCustom;
using HUD;
using MoreSlugcats;
using System;
using System.Collections.Generic;
using System.Linq;


namespace MySlugcat
{
    // PerceptionSkill 感知能力
    public class IntelHUD : HudPart
    {
        // 指针线数组，用于绘制指向最近拾荒者的3D效果指针
        public FSprite[] scavPointerLines;

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


        public static void Hook()
        {
            Log.Logger(7, "IntelHUD", "MySlugcat:IntelHUD​​:Hook", $"sst");
            On.HUD.HUD.InitSleepHud += HUD_InitSleepHud;
            On.HUD.HUD.InitSinglePlayerHud += HUD_InitSinglePlayerHud;
        }

        private static void HUD_InitSleepHud(On.HUD.HUD.orig_InitSleepHud orig, HUD.HUD self, Menu.SleepAndDeathScreen sleepAndDeathScreen, HUD.Map.MapData mapData, SlugcatStats charStats)
        {
            Log.Logger(7, "IntelHUD", "MySlugcat:IntelHUD​​:HUD_InitSleepHud", $"st");
            orig.Invoke(self, sleepAndDeathScreen, mapData, charStats);
            self.AddPart(new IntelHUD(self));
        }

        private static void HUD_InitSinglePlayerHud(On.HUD.HUD.orig_InitSinglePlayerHud orig, HUD.HUD self, RoomCamera cam)
        {
            Log.Logger(7, "IntelHUD", "MySlugcat:IntelHUD​​:HUD_InitSinglePlayerHud", $"st");
            orig.Invoke(self, cam);
            self.AddPart(new IntelHUD(self));
        }

        public IntelHUD(HUD.HUD hud) : base(hud)
        {
            Log.Logger(7, "IntelHUD", "MySlugcat:IntelHUD​​:IntelHUD_", $"st_");
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
                hud.fContainers[1].AddChild(scavPointerLines[i]); // 添加到HUD容器
            }
            // 初始化指针效果参数
            pointerRotationAdd = 1f; // 初始旋转增量
            goalPointerColor = Color.white; // 初始颜色设为白色
        }

        public int ScoreOfPointScav(Player player, Creature scav)
        {
            Log.Logger(7, "IntelHUD", "MySlugcat:IntelHUD​​:ScoreOfPointScav", $"st");
            int score = Custom.ManhattanDistance(player.abstractCreature.pos, scav.abstractCreature.pos);

            if (scav.room == null) return score;

            if (!scav.room.VisualContact(player.abstractCreature.pos, scav.abstractCreature.pos)) score *= 3;
            score += (int)(20f * (1f - Mathf.Abs(Vector2.Dot(Custom.DirVec(scav.room.MiddleOfTile(player.abstractCreature.pos), scav.room.MiddleOfTile(scav.abstractCreature.pos)), Vector2.right))));

            return score;
        }

        public override void Update()
        {
            Log.Logger(9, "IntelHUD", "MySlugcat:IntelHUD​​:Update", $"st");
            // 在Update()方法中更新指针逻辑
            showPointer = false; // 默认不显示指针

            /*if (hud.owner is Player player && player.slugcatStats.name == Plugin.YourSlugID)
            {
                Creature? creature = MyPlayer.FindNearestCreature(player.mainBodyChunk.pos, player.room, false, player, false, 2);
                if (creature != null)
                {
                    shouldPointAt = Custom.DirVec(player.mainBodyChunk.pos, creature.mainBodyChunk.pos);
                    goalPointerColor = creature.ShortCutColor();
                }
                showPointer = creature != null && !player.inShortcut;
            }*/

            if (hud.owner is Player player && SC.PerceptionSkill)
            {
                Log.Logger(9, "IntelHUD", "MySlugcat:IntelHUD​​:Update", $"player ({hud.owner is Player})");
                // 获取玩家当前房间
                Room room = player.abstractCreature.world.game.cameras[0].room;
                Log.Logger(9, "IntelHUD", "MySlugcat:IntelHUD​​:Update", $"Room_Null ({room == null})");
                if (room != null)
                {
                    //Scavenger? pointCreature = null;
                    Creature? creature = null;

                    // 如果显示指针功能开启
                    //if (Plugin.optiones.ShowPointer.Value)
                    if (true && room.abstractRoom.creatures.Count > 0)
                    {
                        // 寻找房间内最近的拾荒者作为指向目标
                        foreach (var crit in room.abstractRoom.creatures)
                        {
                            //if (是有效的拾荒者)
                            //if (!crit.state.dead &&
                            //    (crit.creatureTemplate.type == CreatureTemplate.Type.Scavenger || crit.creatureTemplate.type == DLCSharedEnums.CreatureTemplateType.ScavengerElite) &&
                            //    crit.realizedCreature != null && !crit.realizedCreature.inShortcut)
                            if (!crit.state.dead && crit.realizedCreature != null && !crit.realizedCreature.inShortcut && crit.realizedCreature is Creature creature1 && crit.realizedCreature is not Player)
                            {
                                // 使用ScoreOfPointScav方法计算最佳目标
                                if (creature == null || (crit.realizedCreature != null && ScoreOfPointScav(player, creature1) < ScoreOfPointScav(player, creature)))
                                {
                                    creature = creature1;
                                }
                            }
                        }
                    }
                    // 如果有指向目标且玩家不在捷径中，则显示指针
                    if (creature != null)
                    {
                        goalPointerColor = creature.ShortCutColor();
                        // 计算从玩家指向目标的单位向量
                        shouldPointAt = Custom.DirVec(player.mainBodyChunk.pos, creature.mainBodyChunk.pos);
                    }
                    showPointer = creature != null && !player.inShortcut;

                }
            }

            // 平滑更新指针方向（使用球面线性插值）
            lastPointAt = pointAt;
            pointAt = Vector3.Slerp(pointAt, shouldPointAt, 0.3f);

            // 更新指针淡入淡出效果
            if (showPointer && SC.PerceptionSkill)
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

        public override void Draw(float timeStacker)
        {
            Log.Logger(9, "IntelHUD", "MySlugcat:IntelHUD​​:Draw", $"player ({hud.owner is Player})");
            if (hud.owner is Player player && player.slugcatStats.name == Plugin.YourSlugID)
            {
                Room room = player.abstractCreature.world.game.cameras[0].room;
                Log.Logger(9, "IntelHUD", "MySlugcat:IntelHUD​​:Draw", $"Room_Null ({room == null})");
                if (room != null)
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

                    /*foreach (var stat in scavStats)
                    {
                        stat.Draw(timeStacker, room.game.cameras[0].pos);
                    }*/

                    /*// 设置指针颜色渐变
                    for (int i = 0; i < 9; i++)
                    {
                        scavPointerLines[i].color = Color.Lerp(scavPointerLines[i].color, goalPointerColor, 0.2f);
                    }*/
                }
            }

            // 设置指针颜色渐变
            for (int i = 0; i < 9; i++)
            {
                scavPointerLines[i].alpha = pointerFade;
                scavPointerLines[i].isVisible = scavPointerLines[i].alpha > 0f;
                scavPointerLines[i].color = Color.Lerp(scavPointerLines[i].color, goalPointerColor, 0.2f);
            }

        }

        // 清除指针精灵
        public override void ClearSprites()
        {
            Log.Logger(9, "IntelHUD", "MySlugcat:IntelHUD​​:ClearSprites", $"st");
            for (int i = 0; i < 9; i++)
            {
                scavPointerLines[i].RemoveFromContainer();
            }
        }


    }
}
