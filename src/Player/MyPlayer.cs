using On;
using IL;
using System;
using Mono.Cecil;
using MoreSlugcats;
using RWCustom;
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
//using BeastMaster;
//using DevConsole;
//using DevConsole.Commands;
using Menu.Remix;
using MonoMod.RuntimeDetour;
//using SBCameraScroll;
//using SplitScreenCoop;
using Watcher;


namespace MySlugcat
{
    public class MyPlayer
    {
        //按下跳跃键的时长
        static int[] JmpCounter = Enumerable.Repeat(0, 20).ToArray();

        public static void Hook()
        {
#if MYDEBUG
            try
            {
#endif
            On.Player.ctor += Player_ctor;
            On.Player.Update += Player_Update;

            //咬住挣脱
            On.Creature.Violence += Creature_Violence;
            //挣脱蜥蜴
            On.Lizard.Bite += Lizard_Bite;
            //挣脱蘑菇
            On.DaddyLongLegs.Eat += DaddyLongLegs_Eat;
            //挣脱蜈蚣
            On.Centipede.UpdateGrasp += Centipede_UpdateGrasp;
            //挣脱利维坦
            On.BigEel.JawsSnap += BigEel_JawsSnap;
            //挣脱红树
            On.TentaclePlant.Carry += TentaclePlant_Carry;
            //挣脱拟态草
            On.PoleMimic.Carry += PoleMimic_Carry;
            //挣脱火虫
            On.EggBug.CarryObject += EggBug_CarryObject;
            //冰盾转换
            On.Spear.HitSomething += Spear_HitSomething;
            On.ScavengerBomb.HitSomething += ScavengerBomb_HitSomething;
            //挣脱魔王秃鹫
            On.Vulture.Carry += Vulture_Carry;

            //增强矛的伤害
            //On.Spear.HitSomething += Spear_HitSomething;
            //冰矛合成
            //IcsSpearCraft.Hook();
            //激怒小青
            //On.ArtificialIntelligence.Update += ArtificialIntelligence_Update;
            //吃拾荒眩晕
            //On.Player.EatMeatUpdate += Player_EatMeatUpdate;
            //靠近生物减速
            //On.Creature.Update += Creature_Update;
            //保存数据
            //On.SaveState.SaveToString += SaveState_SaveToString;
            //读取数据
            //On.SaveState.LoadGame += SaveState_LoadGame;
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
#if MYDEBUG
            try
            {
#endif
            orig.Invoke(self, abstractCreature, world);
            if (self.slugcatStats.name != Plugin.YourSlugID)
                return;
            /*------------------------------------------------------------------剧情模式游戏变量设置------------------------------------------------------------------------------*/
/*            if (self.room.world.game.manager.menuSetup.startGameCondition == ProcessManager.MenuSetup.StoryGameInitCondition.New)
            {
                GlobalVar.NewGameGlobalVarSet();
            }*/
/*            if (self.room.world.game.rainWorld.ExpeditionMode)//在探险模式里开启冰盾能力
            {
                GlobalVar.glacier2_iceshield_lock = false;
            }*/
/*            if (self.room.world.game.session is ArenaGameSession)//在竞技场模式里也开启冰盾能力
            {
                GlobalVar.glacier2_iceshield_lock = false;
            }*/
            //赋值给全局变量供其他函数使用
            GlobalVar.game = self.room.world.game;
            /*------------------------------------------------------------------玩家变量初始化------------------------------------------------------------------------------*/
            var pv = new PlayerVar();
            GlobalVar.playerVar.Add(self, pv);
            /*------------------------------------------------------------------调试图像------------------------------------------------------------------------------*/
            pv.myDebug = new MyDebug(self);

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

        private static void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
        {
#if MYDEBUG
            try
            {
#endif
            orig.Invoke(self, eu);
            if (self.slugcatStats.name != Plugin.YourSlugID)
                return;
            /******************************24_2_16 保存bug**********************************/
/*            if (MyOption.Instance.OpCheckBoxSaveIceData_conf.Value)
            {
                LoadMyData();
            }*/
            //]]
            //取玩家变量
            GlobalVar.playerVar.TryGetValue(self, out PlayerVar pv);

            int N = self.playerState.playerNumber;
            if (self.input[0].jmp)
            {
                JmpCounter[N]++;
            }
            else
            {
                JmpCounter[N] = 0;
            }

            //
            if (JmpCounter[N] >= 60)
            {
                //self.room.AddObject(new Explosion(self.room, self, self.mainBodyChunk.pos + new Vector2(0, -10), 5, 330f, 2f, 0.4f, 100f, 0.3f, self, 0.5f, 20f, 0.8f));
                //在 当前房间 从自己身体 在当前身体的位置 生成一个 持续时长7 半径250，力度6.2，伤害2，眩晕280，致聋0.25，判定击杀由自己造成，伤害乘数0.7，最小眩晕160，背景噪声1的爆炸
                //self.room.AddObject(new ShockWave(self.mainBodyChunk.pos + new Vector2(0, -10), 500f, 0.080f, 10, false));
                //或许没那么累 在自身位置生成冲击波效果 大小330 强度0.045 时长5 false表示绘制顺序（绘制到HUD图层，True就是HUD2
                //self.slugcatStats.runspeedFac = 1.75f;

                Creature creature = RandomlySelectedCreature(self.room, false, self);
                if (creature != null)
                {
                    for (int i = 0; i < 13; i++)
                    {
                        SporePlant.Bee bee = new SporePlant.Bee(null, true, creature.firstChunk.pos, new Vector2(0f, 0f), SporePlant.Bee.Mode.Hunt);
                        bee.forceAngry = true;
                        creature.room.AddObject(bee);
                    }
                    creature.room.PlaySound(SoundID.Spore_Bees_Emerge, creature.firstChunk);
                }

                JmpCounter[N] = 0;
            }

#if DEBUG
            //启用投掷键输出调试信息
            PutDebugMsgOnThrow(self);
#endif
            //吞炸弹爆炸
            //SelfExplode(self);
            //飞行能力
            //pv.flyAbility.Glacier2_Fly(self);
            /******************************24_2_16 保存bug**********************************/
/*            if (MyOption.Instance.OpCheckBoxSaveIceData_conf.Value)
            {
                //靠近生物减速
                SlowDownCreature(self);
            }*/
            //]]  
            //冰盾合成
            //MyIceShield.IceShieldCraft(self);
            //披风
            //if (pv.cloak != null)
            //    pv.cloak.Update(self, eu);
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

        private static void PutDebugMsgOnThrow(Player self)
        {
#if MYDEBUG
            try
            {
#endif
            if (self.input[0].thrw)
            {
                UnityEngine.Debug.Log(GlobalVar.dbgstr);
            }
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


        /*        public struct PositionContainer
                {
                    public Vector2 Position { get; }

                    public PositionContainer(Vector2 pos)
                    {
                        Position = pos;
                    }

                    public PositionContainer WithNewPositiom(Vector2 newPos)
                    {
                        return new PositionContainer(newPos);
                    }
                }*/

        /*        public class Vector2Ref
                {
                    public float X { get; set; }
                    public float Y { get; set; }

                    // 必须添加接受两个float参数的构造函数
                    public Vector2Ref(float x, float y)
                    {
                        X = x;
                        Y = y;
                    }

                    // 默认构造函数（可选，如果其他代码需要无参构造）
                    public Vector2Ref()
                    {
                        // 默认值
                    }

                    // 深拷贝方法
                    public Vector2Ref DeepCopy() => new Vector2Ref(X, Y);
                }*/

//#nullable enable
        public static bool DisabledCreature(Creature creature)
        {
            if (creature == null || creature is SandGrub)
            {
                return true;
            }
            return false;
        }

        // 查找当前房间中距离自身最近的生物
        public static Creature FindNearestCreature(Vector2 selfPos, Room room, bool IncludePlayer, Creature creature)
        {
#if MYDEBUG
            try
            {
#endif
            // 初始化变量
            Creature nearest = null;        // 最近生物对象
            float minSqrDistance = float.MaxValue;  // 最小平方距离（初始设为最大浮点数）
            //List<Creature> creatures = new List<Creature>();

            // 遍历当前房间所有生物
            foreach (AbstractCreature abstractCreature in room.abstractRoom.creatures)
            {

                Creature c = abstractCreature.realizedCreature;
                // 排除检查：玩家、无效引用、自身、或没有身体部位的对象
                if (!IncludePlayer)
                {
                    var player1 = c as Player;
                    if (player1 != null)
                    {
                        continue; // 跳过无效项，继续检查下一个
                    }
                }
                if (c == null ||             // 确保生物存在
                    c == creature ||             // 排除自身
                    c.mainBodyChunk == null) // 确保有有效的mainBodyChunk
                {
                    continue; // 跳过无效项，继续检查下一个
                }
                if (DisabledCreature(c))// 禁用生物
                {
                    continue; // 跳过无效项，继续检查下一个
                }
                // 计算位置差（目标位置 - 自身位置）
                Vector2 offset = c.mainBodyChunk.pos - selfPos;
                // 计算平方距离（比Vector2.Distance更高效）
                float sqrDistance = offset.sqrMagnitude;

                //creatures.Add(c);

                // 检查是否为更近的生物
                if (sqrDistance < minSqrDistance)
                {
                    // 更新最近生物和最小距离记录
                    minSqrDistance = sqrDistance;
                    nearest = c;
                }
            }
            //return creatures[UnityEngine.Random.Range(0, creatures.Count)];
            return nearest; // 返回最近生物（可能为null）
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

        public static Creature RandomlySelectedCreature(Room room, bool IncludePlayer, Creature creature)
        {
#if MYDEBUG
            try
            {
#endif
            // 初始化变量
            //Creature? nearest = null;        // 最近生物对象
            //float minSqrDistance = float.MaxValue;  // 最小平方距离（初始设为最大浮点数）
            List<Creature> creatures = new List<Creature>();

            // 遍历当前房间所有生物
            foreach (AbstractCreature abstractCreature in room.abstractRoom.creatures)
            {

                Creature c = abstractCreature.realizedCreature;
                // 排除检查：玩家、无效引用、自身、或没有身体部位的对象
                if (!IncludePlayer)
                {
                    var player1 = c as Player;
                    if (player1 != null)
                    {
                        continue; // 跳过无效项，继续检查下一个
                    }
                }
                if (c == null ||             // 确保生物存在
                    c == creature ||             // 排除自身
                    c.mainBodyChunk == null) // 确保有有效的mainBodyChunk
                {
                    continue; // 跳过无效项，继续检查下一个
                }
                if (DisabledCreature(c))// 禁用生物
                {
                    continue; // 跳过无效项，继续检查下一个
                }
                // 计算位置差（目标位置 - 自身位置）
                //Vector2 offset = c.mainBodyChunk.pos - selfPos;
                // 计算平方距离（比Vector2.Distance更高效）
                //float sqrDistance = offset.sqrMagnitude;

                creatures.Add(c);

                // 检查是否为更近的生物
/*                if (sqrDistance < minSqrDistance)
                {
                    // 更新最近生物和最小距离记录
                    minSqrDistance = sqrDistance;
                    nearest = c;
                }*/
            }
            return creatures[UnityEngine.Random.Range(0, creatures.Count)];
            //return nearest; // 返回最近生物（可能为null）
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

        public static Creature Frame(Player self, bool IncludePlayer, Creature NotIncludeCreature, int probability = -1)
        {
#if MYDEBUG
            try
            {
#endif
            Creature creature = RandomlySelectedCreature(self.room, false, self);
            int percentage = 40;
            if (probability == -1)
            {
                percentage = 100;
            }
            else
            {
                percentage = probability;
            }
            if (percentage > UnityEngine.Random.Range(0, 100) && creature != null)
            {
                /*                Vector2 CV0 = creature.bodyChunks[0].pos;
                                Vector2 CV1 = creature.bodyChunks[1].pos;
                                Vector2 PV0 = self.bodyChunks[0].pos;
                                Vector2 PV1 = self.bodyChunks[1].pos;
                                Vector2[] Cvector = new Vector2[] { CV0, CV1 };
                                Vector2[] Pvector = new Vector2[] { PV0, PV1 };*/

                /*                var CV0 = new PositionContainer(creature.bodyChunks[0].pos);
                                var CV1 = new PositionContainer(creature.bodyChunks[1].pos);
                                var PV0 = new PositionContainer(self.bodyChunks[0].pos);
                                var PV1 = new PositionContainer(self.bodyChunks[1].pos);*/

                /*                Vector2Ref ACV0 = new Vector2Ref(creature.bodyChunks[0].pos.x, creature.bodyChunks[0].pos.y);
                                Vector2Ref ACV1 = new Vector2Ref(creature.bodyChunks[1].pos.x, creature.bodyChunks[1].pos.y);
                                Vector2Ref APV0 = new Vector2Ref(self.bodyChunks[0].pos.x, self.bodyChunks[0].pos.y);
                                Vector2Ref APV1 = new Vector2Ref(self.bodyChunks[1].pos.x, self.bodyChunks[1].pos.y);
                                Vector2Ref CV0 = ACV0.DeepCopy(); // 创建独立副本
                                Vector2Ref CV1 = ACV1.DeepCopy();
                                Vector2Ref PV0 = APV0.DeepCopy();
                                Vector2Ref PV1 = APV1.DeepCopy();*/

                if (self.tongue != null)
                {
                    self.tongue.resetRopeLength();
                    self.tongue.mode = Player.Tongue.Mode.Retracted;
                    self.tongue.rope.Reset();
                }
                self.room.AddObject(new ExplosionSpikes(self.room, self.mainBodyChunk.pos, 14, 30f, 9f, 7f, 170f, creature.ShortCutColor()));
                self.room.AddObject(new ShockWave(self.mainBodyChunk.pos, 500f, 0.080f, 10, false));

                Vector2 CV0 = creature.bodyChunks[0].pos;
                Vector2 CV1 = creature.bodyChunks[1].pos;
                Vector2 PV0 = self.bodyChunks[0].pos;
                Vector2 PV1 = self.bodyChunks[1].pos;
                Vector2[] Cvector = new Vector2[] { CV0, CV1 };
                //Vector2[] Cvector = new Vector2[100];
                //for (int num12 = 0; num12 < creature.bodyChunks.Count(); num12++)
                //{
                //    //Vector2[] Cvector = new Vector2[] { CV0, CV1 };
                //    Cvector
                //}
                Vector2[] Pvector = new Vector2[] { PV0, PV1 };

#if MYDEBUG
            try
            {
#endif
/*                for (int num12 = 0; num12 < self.bodyChunks.Count(); num12++)
                {
                    //self.bodyChunks[num12].vel = Custom.DegToVec(UnityEngine.Random.value * 360f) * 12f;
                    self.bodyChunks[num12].pos = (Cvector[num12] + self.room.game.cameras[0].pos) * (RoomCamera.doubleZoomMode ? 0.5f : 1f);
                    self.bodyChunks[num12].lastPos = (Cvector[num12] + self.room.game.cameras[0].pos) * (RoomCamera.doubleZoomMode ? 0.5f : 1f);
                    self.bodyChunks[num12].vel = new Vector2(0, 0);
                    self.mainBodyChunk.vel = new Vector2(0, 0);
                    self.firstChunk.vel = new Vector2(0, 0);
                }*/
//***
                //new CommandBuilder("md_pause_all").Help("md_pause_all [types] [action]").RunGame(delegate(RainWorldGame game, string[] args)

                Room selfroom = self.room;
                Room creatureroom = creature.room;
                WorldCoordinate selfpos = self.abstractPhysicalObject.pos;
                WorldCoordinate creaturepos = creature.abstractPhysicalObject.pos;
                Clipboard.CutObject(creature);
                Clipboard.CutObject(self);
                Clipboard.PasteObject(selfroom, creaturepos);
                Clipboard.PasteObject(creatureroom, selfpos);
                //return;

                //self.bodyChunks[0].pos = (new Vector2(CV0.X, CV0.Y) + self.room.game.cameras[0].pos) * (RoomCamera.doubleZoomMode ? 0.5f : 1f);
                //self.bodyChunks[0].lastPos = (new Vector2(CV0.X, CV0.Y) + self.room.game.cameras[0].pos) * (RoomCamera.doubleZoomMode ? 0.5f : 1f);
                //self.bodyChunks[1].pos = (new Vector2(CV1.X, CV1.Y) + self.room.game.cameras[1].pos) * (RoomCamera.doubleZoomMode ? 0.5f : 1f);
                //self.bodyChunks[1].lastPos = (new Vector2(CV1.X, CV1.Y) + self.room.game.cameras[1].pos) * (RoomCamera.doubleZoomMode ? 0.5f : 1f);
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

#if MYDEBUG
            try
            {
#endif

                //creature.mainBodyChunk.pos = self.mainBodyChunk.pos;
                //creature.mainBodyChunk.lastPos = self.mainBodyChunk.lastPos;
                /*                for (int num12 = 0; num12 < 2; num12++)
                                {
                                    creature.bodyChunks[num12].pos = (Pvector[num12] + creature.room.game.cameras[0].pos) * (RoomCamera.doubleZoomMode ? 0.5f : 1f);
                                    creature.bodyChunks[num12].lastPos = (Pvector[num12] + creature.room.game.cameras[0].pos) * (RoomCamera.doubleZoomMode ? 0.5f : 1f);
                                    creature.bodyChunks[num12].vel = new Vector2(0, 0);
                                    creature.mainBodyChunk.vel = new Vector2(0, 0);
                                    creature.firstChunk.vel = new Vector2(0, 0);
                                }*/
                //***


                //for (int num12 = 0; num12 < creature.bodyChunks.Count(); num12++)
                //{
                //    //creature.bodyChunks[num12].vel = Custom.DegToVec(UnityEngine.Random.value * 360f) * 12f;
                //    creature.bodyChunks[num12].pos = (self.mainBodyChunk.pos + creature.room.game.cameras[0].pos) * (RoomCamera.doubleZoomMode ? 0.5f : 1f);
                //    creature.bodyChunks[num12].lastPos = (self.mainBodyChunk.lastPos + creature.room.game.cameras[0].pos) * (RoomCamera.doubleZoomMode ? 0.5f : 1f);

                /*                    if (self.bodyChunks[num12].pos == null || self.bodyChunks[num12].lastPos == null)
                                    {
                                        creature.bodyChunks[num12].pos = (self.mainBodyChunk.pos + creature.room.game.cameras[0].pos) * (RoomCamera.doubleZoomMode ? 0.5f : 1f);
                                        creature.bodyChunks[num12].lastPos = (self.mainBodyChunk.lastPos + creature.room.game.cameras[0].pos) * (RoomCamera.doubleZoomMode ? 0.5f : 1f);
                                    }
                                    else
                                    {
                                        try
                                        {
                                            creature.bodyChunks[num12].pos = (self.bodyChunks[num12].pos + creature.room.game.cameras[0].pos) * (RoomCamera.doubleZoomMode ? 0.5f : 1f);
                                            creature.bodyChunks[num12].lastPos = (self.bodyChunks[num12].lastPos + creature.room.game.cameras[0].pos) * (RoomCamera.doubleZoomMode ? 0.5f : 1f);
                                        }
                                        catch (Exception ex)
                                        {
                                            creature.bodyChunks[num12].pos = (self.mainBodyChunk.pos + creature.room.game.cameras[0].pos) * (RoomCamera.doubleZoomMode ? 0.5f : 1f);
                                            creature.bodyChunks[num12].lastPos = (self.mainBodyChunk.lastPos + creature.room.game.cameras[0].pos) * (RoomCamera.doubleZoomMode ? 0.5f : 1f);
                                        }
                                    }*/


                //}
                //creature.bodyChunks[0].pos = (new Vector2(PV0.X, PV0.Y) + creature.room.game.cameras[0].pos) * (RoomCamera.doubleZoomMode ? 0.5f : 1f);
                //creature.bodyChunks[0].lastPos = (new Vector2(PV0.X, PV0.Y) + creature.room.game.cameras[0].pos) * (RoomCamera.doubleZoomMode ? 0.5f : 1f);
                //creature.bodyChunks[1].pos = (new Vector2(PV1.X, PV1.Y) + creature.room.game.cameras[1].pos) * (RoomCamera.doubleZoomMode ? 0.5f : 1f);
                //creature.bodyChunks[1].lastPos = (new Vector2(PV1.X, PV1.Y) + creature.room.game.cameras[1].pos) * (RoomCamera.doubleZoomMode ? 0.5f : 1f);

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

                /*                for (int num12 = 0; num12 < 2; num12++)
                                {
                                    self.bodyChunks[num12].pos = (Cvector[num12] + self.room.game.cameras[0].pos) * (RoomCamera.doubleZoomMode ? 0.5f : 1f);
                                    self.bodyChunks[num12].lastPos = (Cvector[num12] + self.room.game.cameras[0].pos) * (RoomCamera.doubleZoomMode ? 0.5f : 1f);
                                }
                                for (int num12 = 0; num12 < 2; num12++)
                                {
                                    creature.bodyChunks[num12].pos = (Pvector[num12] + creature.room.game.cameras[0].pos) * (RoomCamera.doubleZoomMode ? 0.5f : 1f);
                                    creature.bodyChunks[num12].lastPos = (Pvector[num12] + creature.room.game.cameras[0].pos) * (RoomCamera.doubleZoomMode ? 0.5f : 1f);
                                }*/

                self.room.AddObject(new ExplosionSpikes(self.room, self.mainBodyChunk.pos + new Vector2(0, -10), 14, 30f, 9f, 7f, 170f, self.ShortCutColor()));
                self.room.AddObject(new ShockWave(self.mainBodyChunk.pos + new Vector2(0, -10), 500f, 0.080f, 10, false));
                return creature;
            }
            return null;


            if (100 > UnityEngine.Random.Range(0, 100) && 1 < self.room.abstractRoom.creatures.Count)
            {
            RandomlySelected:
                int num = UnityEngine.Random.Range(0, self.room.abstractRoom.creatures.Count);
                //Creature creature = self.room.abstractRoom.creatures[num].realizedCreature;
                creature = self.room.abstractRoom.creatures[num].realizedCreature;
                if (IncludePlayer)
                {
                    //T

/*                    var player1 = creature as Player;
                    if (player1 != null)
                    {
                        if (player1.slugcatStats.name != Plugin.YourSlugID)
                        {
                            //T
                        }
                        else
                        {
                            //F
                            goto RandomlySelected;
                        }
                    }
                    else
                    {
                        //T
                    }*/
                }
                else
                {
                    var player2 = creature as Player;
                    if (player2 != null)
                    {
                        //F
                        goto RandomlySelected; // 跳过无效项，继续检查下一个
                    }
                }
                if (creature == NotIncludeCreature)
                {
                    //F
                    goto RandomlySelected;
                }

/*              for (num = Random.Range(0, self.room.abstractRoom.creatures.Count); i < length; num = Random.Range(0, self.room.abstractRoom.creatures.Count))
                {
                    int num = Random.Range(0, self.room.abstractRoom.creatures.Count);
                    creature = self.room.abstractRoom.creatures[num].realizedCreature;
                    var player1 = creature as Player;
                    if (player1 == null)
                    {

                    }
                }*/
                //Vector2 vector2 = creature.firstChunk.pos;
                //Vector2 Cvector[] = creature.bodyChunks[1].pos;
                //Vector2 Pvector[] =
                Vector2 CV0 = creature.bodyChunks[0].pos;
                Vector2 CV1 = creature.bodyChunks[1].pos;
                Vector2 PV0 = self.bodyChunks[0].pos;
                Vector2 PV1 = self.bodyChunks[1].pos;
                Vector2[] Cvector = new Vector2[] { CV0, CV1 };
                Vector2[] Pvector = new Vector2[] { PV0, PV1 };
                //creature.mainBodyChunk.pos

                if (self.tongue != null)
                {
                    self.tongue.resetRopeLength();
                    self.tongue.mode = Player.Tongue.Mode.Retracted;
                    self.tongue.rope.Reset();
                }
                self.room.AddObject(new ExplosionSpikes(self.room, self.mainBodyChunk.pos, 14, 30f, 9f, 7f, 170f, creature.ShortCutColor()));
                self.room.AddObject(new ShockWave(self.mainBodyChunk.pos, 500f, 0.080f, 10, false));

                for (int num12 = 0; num12 < 2; num12++)
                {
                    //self.bodyChunks[num12].vel = Custom.DegToVec(UnityEngine.Random.value * 360f) * 12f;
                    self.bodyChunks[num12].pos = (Cvector[num12] + self.room.game.cameras[0].pos) * (RoomCamera.doubleZoomMode ? 0.5f : 1f);
                    self.bodyChunks[num12].lastPos = (Cvector[num12] + self.room.game.cameras[0].pos) * (RoomCamera.doubleZoomMode ? 0.5f : 1f);
                }
                for (int num12 = 0; num12 < 2; num12++)
                {
                    //creature.bodyChunks[num12].vel = Custom.DegToVec(UnityEngine.Random.value * 360f) * 12f;
                    creature.bodyChunks[num12].pos = (Pvector[num12] + creature.room.game.cameras[0].pos) * (RoomCamera.doubleZoomMode ? 0.5f : 1f);
                    creature.bodyChunks[num12].lastPos = (Pvector[num12] + creature.room.game.cameras[0].pos) * (RoomCamera.doubleZoomMode ? 0.5f : 1f);
                }

                self.room.AddObject(new ExplosionSpikes(self.room, self.mainBodyChunk.pos + new Vector2(0, -10), 14, 30f, 9f, 7f, 170f, self.ShortCutColor()));
                self.room.AddObject(new ShockWave(self.mainBodyChunk.pos + new Vector2(0, -10), 500f, 0.080f, 10, false));
                return creature;
            }
            return null;

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
//#nullable disable

        private static bool Spear_HitSomething(On.Spear.orig_HitSomething orig, Spear spear, SharedPhysics.CollisionResult result, bool eu)
        {
            if (result.obj == null)
            {
                return false;
            }
            //如果被命中的不是玩家
            if (result.obj is not Player self)
                return orig.Invoke(spear, result, eu);
            //如果玩家不是Glacier则运行原程序
            if (self.slugcatStats.name != Plugin.YourSlugID)
                return orig.Invoke(spear, result, eu);
            //取玩家变量
            GlobalVar.playerVar.TryGetValue(self, out PlayerVar pv);

            Creature obj = Frame(self, false, self); 
            //spear.thrownBy = null;
            //Creature obj = FindNearestCreature(spear., Frameobj.room, false, null);
            if (obj != null)
            {
                //(!this.dead && this.State is HealthState && (this.State as HealthState).health < 0f && UnityEngine.Random.value < -(this.State as HealthState).health && UnityEngine.Random.value < 0.025f)
                //obj.health -= 1;
                //if((obj.State as HealthState).health -= 1)
                var hs = obj.State as HealthState;
                if (hs != null)
                {
                    hs.health -= spear.spearDamageBonus;
                }

                result.obj = obj;
            }

            return orig.Invoke(spear, result, eu);
        }

        private static bool ScavengerBomb_HitSomething(On.ScavengerBomb.orig_HitSomething orig, ScavengerBomb bomb, SharedPhysics.CollisionResult result, bool eu)
        {
            //如果被命中的不是玩家
            if (result.obj is not Player self)
                return orig.Invoke(bomb, result, eu);
            //如果玩家不是Glacier则运行原程序
            if (self.slugcatStats.name != Plugin.YourSlugID)
                return orig.Invoke(bomb, result, eu);
            //取玩家变量
            GlobalVar.playerVar.TryGetValue(self, out PlayerVar pv);

            Creature obj = Frame(self, false, self);
            //bomb.thrownBy = null;
            //Creature obj = FindNearestCreature(bomb., Frameobj.room, false, null);
            if (obj != null)
            {
                result.obj = obj;
            }

            return orig.Invoke(bomb, result, eu);
        }

        private static void Creature_Violence(On.Creature.orig_Violence orig, Creature creature, BodyChunk source, Vector2? directionAndMomentum, BodyChunk hitChunk, PhysicalObject.Appendage.Pos hitAppendage, Creature.DamageType type, float damage, float stunBonus)
        {
#if MYDEBUG
            try
            {
#endif
            if (hitChunk == null)
            {
                orig.Invoke(creature, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                return;
            }
            //如果被伤害的物体不是玩家则运行原程序
            if (hitChunk.owner is not Player self)
            {
                orig.Invoke(creature, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                return;
            }
            //如果玩家不是Glacier则运行原程序
            if (self.slugcatStats.name != Plugin.YourSlugID)
            {
                orig.Invoke(creature, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                return;
            }
            //取玩家变量
            GlobalVar.playerVar.TryGetValue(self, out PlayerVar pv);
/*            //如果没有冰盾
            if (pv.iceShieldList.Count == 0)
            {
                orig.Invoke(creature, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                return;
            }*/

            if (creature is Lizard)
            {
                orig.Invoke(creature, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                return;
            }
            //如果伤害类型不是 x 也运行原程序
            if (type != Creature.DamageType.Bite &&
               type != Creature.DamageType.Electric &&
               type != Creature.DamageType.Stab)
            {
                orig.Invoke(creature, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                return;
            }
            //偷渡虫情况特殊处理
            if (source != null && source.owner is StowawayBug)
            {
                //钩子伤害不处理
                if (damage < 1f)
                    orig.Invoke(creature, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);
                else
                {
                    Creature newobj2 = MyPlayer.Frame(self, false, self);
                    if (newobj2 != null)
                    {
                        hitChunk.owner = newobj2;
                        //eggBug.Stun(10);
                        self.stun = 0;
                    }

                    //Frame(self, false, self);
                    orig.Invoke(creature, source, directionAndMomentum, hitChunk, hitAppendage, type, 0, stunBonus);
                    //self.stun = 0;
                }
                return;
            }
            Creature newobj = MyPlayer.Frame(self, false, self);
            if (newobj != null)
            {
                hitChunk.owner = newobj;
                //eggBug.Stun(10);
                self.stun = 0;
            }

            //Frame(self, false, self);


            //creature.Stun(10);
            //Frame(self, false, self);
            //防止玩家被咬死
            orig.Invoke(creature, source, directionAndMomentum, hitChunk, hitAppendage, type, 0, stunBonus);
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

        private static void Lizard_Bite(On.Lizard.orig_Bite orig, Lizard lizard, BodyChunk chunk)
        {
            //orig.Invoke(lizard, chunk);
            if (chunk == null)
            {
                orig.Invoke(lizard, chunk);
                return;
            }
            if (chunk.owner is not Player self)
            {
                orig.Invoke(lizard, chunk);
                return;
            }
            //如果玩家不是Glacier则运行原程序
            if (self.slugcatStats.name != Plugin.YourSlugID)
            {
                orig.Invoke(lizard, chunk);
                return;
            }
            //取玩家变量
            GlobalVar.playerVar.TryGetValue(self, out PlayerVar pv);

            Creature newobj = MyPlayer.Frame(self, false, self);
            if (newobj != null)
            {
                chunk.owner = newobj;
                //eggBug.Stun(10);
                self.stun = 0;
            }

            //lizard.Stun(10);
            //Frame(self, false, self);
            orig.Invoke(lizard, chunk);
        }

        private static void DaddyLongLegs_Eat(On.DaddyLongLegs.orig_Eat orig, DaddyLongLegs daddyLongLegs, bool eu)
        {
#if MYDEBUG
            try
            {
#endif
            //List<DaddyLongLegs.EatObject> removeList = new List<DaddyLongLegs.EatObject>();
            foreach (var obj in daddyLongLegs.eatObjects)
            {
                if (obj == null || obj.chunk == null)
                    continue;
                if (obj.chunk.owner is Player)
                {
                    Player self = obj.chunk.owner as Player;
                    if (self.slugcatStats.name != Plugin.YourSlugID)
                        continue;
                    //取玩家变量
                    GlobalVar.playerVar.TryGetValue(self, out PlayerVar pv);

                    Creature newobj = MyPlayer.Frame(self, false, self);
                    if (newobj != null)
                    {
                        obj.chunk.owner = newobj;
                        //eggBug.Stun(10);
                        self.stun = 0;
                    }

                    //daddyLongLegs.Stun(10);
                    //Frame(self, false, self);
                    //removeList.Add(obj);
                }
            }
/*            if (removeList.Count > 0)
            {
                daddyLongLegs.eatObjects.Clear();
                foreach (var p in daddyLongLegs.tentacles)
                    p.grabChunk = null;
            }*/
            orig.Invoke(daddyLongLegs, eu);
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

        private static void Centipede_UpdateGrasp(On.Centipede.orig_UpdateGrasp orig, Centipede centipede, int g)
        {
#if MYDEBUG
            try
            {
#endif
            //orig.Invoke(centipede, g);
            if (centipede.grasps == null ||
                centipede.grasps[g] == null)
            {
                orig.Invoke(centipede, g);
                return;
            }
            if (centipede.grasps[g].grabbed is not Player self)
            {
                orig.Invoke(centipede, g);
                return;
            }
            //如果玩家不是Glacier则运行原程序
            if (self.slugcatStats.name != Plugin.YourSlugID)
            {
                orig.Invoke(centipede, g);
                return;
            }
            //取玩家变量
            GlobalVar.playerVar.TryGetValue(self, out PlayerVar pv);

            Creature newobj = MyPlayer.Frame(self, false, self);
            if (newobj != null)
            {
                centipede.grasps[g].grabbed = newobj;
                //eggBug.Stun(10);
                self.stun = 0;
            }

            orig.Invoke(centipede, g);

            //centipede.Stun(10);
            //Frame(self, false, self);

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

        private static void BigEel_JawsSnap(On.BigEel.orig_JawsSnap orig, BigEel bigEel)
        {
#if MYDEBUG
            try
            {
#endif
            for (int j = 0; j < bigEel.room.physicalObjects.Length; j++)
            {
                for (int num = bigEel.room.physicalObjects[j].Count - 1; num >= 0; num--)
                {
                    if (!(bigEel.room.physicalObjects[j][num] is BigEel))
                    {
                        for (int k = 0; k < bigEel.room.physicalObjects[j][num].bodyChunks.Length; k++)
                        {
                            if (!bigEel.InBiteArea(bigEel.room.physicalObjects[j][num].bodyChunks[k].pos, bigEel.room.physicalObjects[j][num].bodyChunks[k].rad / 2f))
                            {
                                continue;
                            }

                            if (bigEel.room.physicalObjects[j][num] is Creature)
                            {
                                if (bigEel.room.physicalObjects[j][num] is Player self)
                                {
                                    //如果玩家不是Glacier则运行原程序
                                    if (self.slugcatStats.name == Plugin.YourSlugID)
                                    {
                                        //取玩家变量
                                        GlobalVar.playerVar.TryGetValue(self, out PlayerVar pv);

                                        Creature newobj = MyPlayer.Frame(self, false, self);
                                        if (newobj != null)
                                        {
                                            bigEel.room.physicalObjects[j][num] = newobj;
                                            //eggBug.Stun(10);
                                            self.stun = 0;
                                        }
                                        
                                        //bigEel.Stun(10);
                                        //Frame(self, false, self);
                                    }
                                }
                            }

                        }
                    }
                }
            }

            orig.Invoke(bigEel);

            /*bigEel.snapFrame = true;
            bigEel.room.PlaySound(SoundID.Leviathan_Bite, bigEel.mainBodyChunk);
            bigEel.room.ScreenMovement(bigEel.mainBodyChunk.pos, new Vector2(0f, 0f), 1.3f);
            for (int i = 1; i < bigEel.bodyChunks.Length; i++)
            {
                bigEel.bodyChunks[i].vel += (Custom.RNV() + Custom.DirVec(bigEel.bodyChunks[i - 1].pos, bigEel.bodyChunks[i].pos)) * Mathf.Sin(Mathf.InverseLerp(1f, 11f, i) * (float)Math.PI) * 8f;
            }

            Vector2 pos = bigEel.mainBodyChunk.pos;
            Vector2 vector = Custom.DirVec(bigEel.bodyChunks[1].pos, bigEel.mainBodyChunk.pos);
            bigEel.beakGap = 0f;
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            for (int j = 0; j < bigEel.room.physicalObjects.Length; j++)
            {
                for (int num = bigEel.room.physicalObjects[j].Count - 1; num >= 0; num--)
                {
                    if (!(bigEel.room.physicalObjects[j][num] is BigEel))
                    {
                        for (int k = 0; k < bigEel.room.physicalObjects[j][num].bodyChunks.Length; k++)
                        {
                            if (!bigEel.InBiteArea(bigEel.room.physicalObjects[j][num].bodyChunks[k].pos, bigEel.room.physicalObjects[j][num].bodyChunks[k].rad / 2f))
                            {
                                continue;
                            }

                            Vector2 b = Custom.ClosestPointOnLine(pos, pos + vector, bigEel.room.physicalObjects[j][num].bodyChunks[k].pos);
                            if (!ModManager.MSC || (!(bigEel.room.physicalObjects[j][num] is BigJellyFish) && !(bigEel.room.physicalObjects[j][num] is EnergyCell)))
                            {
                                bigEel.clampedObjects.Add(new BigEel.ClampedObject(bigEel.room.physicalObjects[j][num].bodyChunks[k], Vector2.Distance(pos, b)));
                                UnityEngine.Debug.Log("Caught: " + bigEel.room.physicalObjects[j][num].ToString());
                            }

                            if (ModManager.MSC && bigEel.room.physicalObjects[j][num] is EnergyCell)
                            {
                                (bigEel.room.physicalObjects[j][num] as EnergyCell).Explode();
                            }

                            if (bigEel.room.physicalObjects[j][num].bodyChunks[k].rad > bigEel.beakGap)
                            {
                                bigEel.beakGap = bigEel.room.physicalObjects[j][num].bodyChunks[k].rad;
                            }

                            if (bigEel.room.physicalObjects[j][num] is Creature)
                            {
                                if (bigEel.room.physicalObjects[j][num] is Player self)
                                {
                                    //如果玩家不是Glacier则运行原程序
                                    if (self.slugcatStats.name == Plugin.YourSlugID)
                                    {
                                        //取玩家变量
                                        GlobalVar.playerVar.TryGetValue(self, out PlayerVar pv);

                                        bigEel.Stun(10);
                                        Frame(self, false, self);
                                    }
                                    flag3 = true;
                                }
                                else
                                {
                                    flag = true;
                                }
                                 (bigEel.room.physicalObjects[j][num] as Creature).Die();
                            }
                            else
                            {
                                flag2 = true;
                            }

                            if (bigEel.graphicsModule != null)
                            {
                                if (bigEel.room.physicalObjects[j][num] is IDrawable)
                                {
                                    bigEel.graphicsModule.AddObjectToInternalContainer(bigEel.room.physicalObjects[j][num] as IDrawable, 0);
                                }
                                else if (bigEel.room.physicalObjects[j][num].graphicsModule != null)
                                {
                                    bigEel.graphicsModule.AddObjectToInternalContainer(bigEel.room.physicalObjects[j][num].graphicsModule, 0);
                                }
                            }
                        }
                    }
                }
            }

            if (flag)
            {
                bigEel.room.PlaySound(SoundID.Leviathan_Crush_NPC, bigEel.mainBodyChunk);
            }

            if (flag2)
            {
                bigEel.room.PlaySound(SoundID.Leviathan_Crush_Non_Organic_Object, bigEel.mainBodyChunk);
            }

            if (flag3)
            {
                bigEel.room.PlaySound(SoundID.Leviathan_Crush_Player, bigEel.mainBodyChunk);
            }

            for (float num2 = 20f; num2 < 100f; num2 += 1f)
            {
                if (bigEel.room.GetTile(pos + vector * num2).Solid)
                {
                    bigEel.room.PlaySound(SoundID.Leviathan_Clamper_Hit_Terrain, bigEel.mainBodyChunk.pos);
                    break;
                }
            }

            for (int l = 0; l < bigEel.clampedObjects.Count; l++)
            {
                bigEel.clampedObjects[l].chunk.owner.ChangeCollisionLayer(0);
                bigEel.Crush(bigEel.clampedObjects[l].chunk.owner);
            }*/
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

        private static void TentaclePlant_Carry(On.TentaclePlant.orig_Carry orig, TentaclePlant tentaclePlant, bool eu)
        {
#if MYDEBUG
            try
            {
#endif
            //orig.Invoke(tentaclePlant, eu);
            if (tentaclePlant.grasps == null ||
                tentaclePlant.grasps[0] == null ||
                tentaclePlant.grasps[0].grabbedChunk == null)
            {
                orig.Invoke(tentaclePlant, eu);
                return;
            }
            if (tentaclePlant.grasps[0].grabbedChunk.owner is not Player self)
            {
                orig.Invoke(tentaclePlant, eu);
                return;
            }
            //如果玩家不是Glacier则运行原程序
            if (self.slugcatStats.name != Plugin.YourSlugID)
            {
                orig.Invoke(tentaclePlant, eu);
                return;
            }
            //取玩家变量
            GlobalVar.playerVar.TryGetValue(self, out PlayerVar pv);

            Creature newobj = MyPlayer.Frame(self, false, self);
            if (newobj != null)
            {
                tentaclePlant.grasps[0].grabbedChunk.owner = newobj;
                //eggBug.Stun(10);
                self.stun = 0;
            }
            orig.Invoke(tentaclePlant, eu);
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

        private static void PoleMimic_Carry(On.PoleMimic.orig_Carry orig, PoleMimic poleMimic, bool eu)
        {
#if MYDEBUG
            try
            {
#endif
            //orig.Invoke(poleMimic, eu);
            if (poleMimic.grasps == null ||
                poleMimic.grasps[0] == null ||
                poleMimic.grasps[0].grabbedChunk == null)
            {
                orig.Invoke(poleMimic, eu);
                return;
            }
            if (poleMimic.grasps[0].grabbedChunk.owner is not Player self)
            {
                {
                    orig.Invoke(poleMimic, eu);
                    return;
                }
            }
            //如果玩家不是Glacier则运行原程序
            if (self.slugcatStats.name != Plugin.YourSlugID)
            {
                orig.Invoke(poleMimic, eu);
                return;
            }
            //取玩家变量
            GlobalVar.playerVar.TryGetValue(self, out PlayerVar pv);

            Creature newobj = MyPlayer.Frame(self, false, self);
            if (newobj != null)
            {
                poleMimic.grasps[0].grabbedChunk.owner = newobj;
                //eggBug.Stun(10);
                self.stun = 0;
                for (int i = 0; i < poleMimic.stickChunks.Length; i++)
                {
                    poleMimic.stickChunks[i] = null;
                }
            }

            orig.Invoke(poleMimic, eu);
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

        private static void EggBug_CarryObject(On.EggBug.orig_CarryObject orig, EggBug eggBug, bool eu)
        {
            //orig.Invoke(eggBug, eu);
            PhysicalObject obj = eggBug.grasps[0].grabbed;
            if (obj == null)
            {
                orig.Invoke(eggBug, eu);
                return;
            }

            if (obj is not Player self)
            {
                orig.Invoke(eggBug, eu);
                return;
            }

            //如果玩家不是Glacier则运行原程序
            if (self.slugcatStats.name != Plugin.YourSlugID)
            {
                orig.Invoke(eggBug, eu);
                return;
            }
            //取玩家变量
            GlobalVar.playerVar.TryGetValue(self, out PlayerVar pv);

            Creature newobj = MyPlayer.Frame(self, false, self);
            if (newobj != null)
            {
                eggBug.grasps[0].grabbed = newobj;
                //eggBug.Stun(10);
                self.stun = 0;
            }
            orig.Invoke(eggBug, eu);
        }

        private static void Vulture_Carry(On.Vulture.orig_Carry orig, Vulture vulture)
        {
            if (vulture.IsKing == false)
            {
                orig.Invoke(vulture);
                return;
            }
            if (vulture.grasps == null ||
                vulture.grasps[0] == null ||
                 vulture.grasps[0].grabbedChunk == null)
            {
                orig.Invoke(vulture);
                return;
            }
            //如果被伤害的物体不是玩家则运行原程序
            if (vulture.grasps[0].grabbedChunk.owner is not Player self)
            {
                orig.Invoke(vulture);
                return;
            }
            //如果玩家不是Glacier则运行原程序
            if (self.slugcatStats.name != Plugin.YourSlugID)
            {
                orig.Invoke(vulture);
                return;
            }
            //取玩家变量
            GlobalVar.playerVar.TryGetValue(self, out PlayerVar pv);

            Creature obj = MyPlayer.Frame(self, false, self);
            if (obj != null)
            {
                vulture.grasps[0].grabbedChunk.owner = obj;
                //vulture.Stun(10);
                self.stun = 0;
            }
            orig.Invoke(vulture);

            if (obj != null)
            {
                self.stun = 0;
            }
        }


    }
}
