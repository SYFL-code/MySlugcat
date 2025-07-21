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
using Menu.Remix;
using MonoMod.RuntimeDetour;
using Watcher;


namespace MySlugcat
{
    //BUG:莫名卡顿，无响应
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

            On.Spear.HitSomething += Spear_HitSomething;
            On.Player.Die += Player_Die;

/*            //咬住挣脱
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
            On.Vulture.Carry += Vulture_Carry;*/

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


/*        private static void SaveState_LoadGame(On.SaveState.orig_LoadGame orig, SaveState saveState, string str, RainWorldGame game)
        {
            //24_2_16 保存bug
            //if (MyOption.Instance.OpCheckBoxSaveIceData_conf.Value == false)
            if (false)
            {
                orig.Invoke(saveState, str, game);
                return;
            }
            //]]
            orig.Invoke(saveState, str, game);
            string[] array = Regex.Split(str, "<svA>");
            foreach (var p in array)
            {
                string[] array2 = Regex.Split(p, "<svB>");
                if (array2.Length != 0 && array2[0].Length > 0)
                {
                    if (array2[0] == GlobalVar.MySlugcat_LH_KnitmeshSkill_Enable_savefield)
                    {
                        GlobalVar.MySlugcat_LH_KnitmeshSkill_Enable = bool.Parse(array2[1]);
                    }
                    else if (array2[0] == GlobalVar.MySlugcat2_iceshield_count_savefield)
                    {
                        GlobalVar.savedata_MySlugcat2_iceshield_count = array2[1];
                        GlobalVar.enableLoadData = true;

                    }
                }
            }
        }*/

/*        private static string SaveState_SaveToString(On.SaveState.orig_SaveToString orig, SaveState saveState)
        {
            //24_2_16 保存bug
            //if (MyOption.Instance.OpCheckBoxSaveIceData_conf.Value == false)
            if (false)
            {
                return orig.Invoke(saveState);
            }
            //]]
            string RemoveField(string dataText, string fieldName)
            {
                int index_start = dataText.IndexOf(fieldName);
                //直到清除字段
                while (index_start != -1)
                {
                    //清除字段数据
                    int index_end = dataText.IndexOf("<svA>", index_start) + 5;
                    dataText = dataText.Remove(index_start, index_end - index_start);
                    index_start = dataText.IndexOf(fieldName);
                }
                return dataText;
            }
            var text = orig.Invoke(saveState);
            //--------------------------------------冰盾能力解锁---------------------------------------------
            //清除原来字段
            text = RemoveField(text, GlobalVar.MySlugcat_LH_KnitmeshSkill_Enable_savefield);
            //写入能力启用数据
            text += string.Format(CultureInfo.InvariantCulture, GlobalVar.MySlugcat_LH_KnitmeshSkill_Enable_savefield + "<svB>{0}<svA>", GlobalVar.MySlugcat2_iceshield_lock);
            //---------------------------------------冰盾计数------------------------------------------------
            //检查玩家队伍里是否有MySlugcat
            List<Player> MySlugcatList = new List<Player>();

            //24_1_30 修复雨眠bug
            if (GlobalVar.game == null ||
                GlobalVar.game.Players == null)
            {
                return orig.Invoke(saveState);
            }
            //

            foreach (var absc in GlobalVar.game.Players)
            {
                //24_1_30 修复雨眠bug
                if (absc == null ||
                    absc.realizedCreature == null)
                    continue;
                //
                Player self = absc.realizedCreature as Player;
                //如果不是MySlugcat
                if (self.slugcatStats.name != Plugin.YourSlugID)
                    continue;
                MySlugcatList.Add(self);
            }
            //如果没有MySlugcat则不保存数据
            if (MySlugcatList.Count == 0)
                return orig.Invoke(saveState);

            //保存所有MySlugcat的冰盾数据
            string numArr = "";
            foreach (var self in MySlugcatList)
            {
                //取每个glaicer玩家变量
                GlobalVar.playerVar.TryGetValue(self, out PlayerVar pv);
                //保存所有的冰盾数据
                numArr += string.Format("{0},", pv.iceShieldList.Count);
            }
            //去除最后一个逗号
            numArr = numArr.Substring(0, numArr.Length - 1);
            //写入MySlugcat们的冰盾数据
            //清除原来字段
            text = RemoveField(text, GlobalVar.MySlugcat2_iceshield_count_savefield);
            //写入冰盾数据
            text += string.Format(CultureInfo.InvariantCulture, GlobalVar.MySlugcat2_iceshield_count_savefield + "<svB>{0}<svA>", numArr);
            return text;
        }*/


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
                GlobalVar.MySlugcat2_iceshield_lock = false;
            }*/
/*            if (self.room.world.game.session is ArenaGameSession)//在竞技场模式里也开启冰盾能力
            {
                GlobalVar.MySlugcat2_iceshield_lock = false;
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

                StackTrace st = new StackTrace(new StackFrame(true));
                StackFrame sf = st.GetFrame(0);
                var sr = sf.GetFileName().Split('\\');
                MyDebug.outStr = sr[sr.Length - 1] + "\n";
                MyDebug.outStr += sf.GetMethod() + "\n";
                MyDebug.outStr += "MySlugcat:wewe";
                Console.WriteLine("MySlugcat:wewe");

                Creature? creature = RandomlySelectedCreature(self.room, false, self, false);
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
            //pv.flyAbility.MySlugcat2_Fly(self);
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



        // 有害的生物
        public static bool HarmfulCreature(Creature creature)
        {
            if (creature == null || creature is SandGrub || creature is TentaclePlant || creature is Lizard ||
                creature is BigEel || creature is DaddyLongLegs || creature is Vulture ||
                creature is EggBug || creature is Centipede || creature is Spider || creature is MirosBird ||
                creature is Scavenger || creature is BigNeedleWorm || creature is DropBug || creature is BigMoth ||
                (creature is Inspector inspector && inspector.Consious == false) || creature is PoleMimic || 
                creature is BigJellyFish || creature is StowawayBug || creature is Loach || creature is Frog ||
                (creature is BoxWorm boxWorm && boxWorm.Consious == false) || creature is DrillCrab)
            {
                return true;// creature is Leech || 
            }
            return false;
        }

        // 禁用的生物
        public static bool DisabledCreature(Creature creature)
        {
            if (creature == null || creature is Fly || creature is SandGrub || creature is TentaclePlant ||
                creature is Leech || creature is BigEel || creature is DaddyLongLegs || creature is Overseer ||
                creature is GarbageWorm || creature is Deer || creature is Inspector || creature is PoleMimic ||
                creature is BigJellyFish || creature is StowawayBug || creature is Loach || creature is Frog ||
                creature is SkyWhale || creature is BoxWorm || creature is FireSprite || creature is DrillCrab)
            {
                return true;
            }
            return false;
        }

        // 查找获取一定范围内所有生物
        public static List<Creature>? CreaturesInRange(Room room, Vector2 centerPos, float radius, bool IncludePlayer, Creature creature, bool IncludeSpecificCreature, bool IncludeDeadCreature)
        {
#if MYDEBUG
            try
            {
#endif

            List<(Creature creature, float sqrDistance)> results = new List<(Creature, float)>();
            float radiusSquared = radius * radius;

            if (!(room.abstractRoom.creatures.Count > 0))
            {
                return null;
            }

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
                if (DisabledCreature(c) && !IncludeSpecificCreature)// 禁用生物
                {
                    continue; // 跳过无效项，继续检查下一个
                }
                if (c.dead == true && !IncludeDeadCreature)// 死亡的生物
                {
                    continue; // 跳过无效项，继续检查下一个
                }
                // 计算平方距离（性能优化）
                Vector2 offset = c.mainBodyChunk.pos - centerPos;
                float sqrDist = offset.sqrMagnitude;

                // 距离检测
                if (sqrDist <= radiusSquared)
                {
                    results.Add((c, sqrDist));
                }
            }
            // 按平方距离排序（无需计算真实距离）
            results.Sort((a, b) => a.sqrDistance.CompareTo(b.sqrDistance));

            // 转换为最终结果
            return results.ConvertAll(x => x.creature);


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

        // 查找当前房间中距离自身最近的生物
        public static Creature? FindNearestCreature(Vector2 selfPos, Room room, bool IncludePlayer, Creature creature, bool IncludeDeadCreature, int select)
        {
#if MYDEBUG
            try
            {
#endif
            // 初始化变量
            Creature? nearest = null;        // 最近生物对象
            float minSqrDistance = float.MaxValue;  // 最小平方距离（初始设为最大浮点数）
            //List<Creature> creatures = new List<Creature>();

            if (!(room.abstractRoom.creatures.Count > 0))
            {
                return null;
            }

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
                if (DisabledCreature(c) && select ==1)// 禁用生物
                {
                    continue; // 跳过无效项，继续检查下一个
                }
                if (!HarmfulCreature(c) && select == 2)// 无害生物
                {
                    continue; // 跳过无效项，继续检查下一个
                }
                if (c.dead == true && !IncludeDeadCreature)// 死亡的生物
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

        // 随机查找当前房间的生物
        public static Creature? RandomlySelectedCreature(Room room, bool IncludePlayer, Creature creature, bool IncludeDeadCreature)
        {
#if MYDEBUG
            try
            {
#endif
            // 初始化变量
            //Creature? nearest = null;        // 最近生物对象
            //float minSqrDistance = float.MaxValue;  // 最小平方距离（初始设为最大浮点数）
            List<Creature> creatures = new List<Creature>();

            if (!(room.abstractRoom.creatures.Count > 0))
            {
                return null;
            }

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
                if (c.dead == true && !IncludeDeadCreature)// 死亡的生物
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
            if (creatures.Count == 0)
            {
                Console.WriteLine("MySlugcat:RandomlySelectedCreature: No valid creatures found");
                return null;
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

        private static bool Spear_HitSomething(On.Spear.orig_HitSomething orig, Spear spear, SharedPhysics.CollisionResult result, bool eu)
        {
            Log.Logger(-7, "", "", "");
            Log.Logger(7, "Spear", "MySlugcat:MyPlayer:Spear_HitSomething_sst", $"result.obj_Type ({result.obj?.GetType()}), result.obj_NulL ({result.obj == null})");
            //Console.WriteLine($"\n MySlugcat MyPlayer:sst Spear_HitSomething: sst {result.obj?.GetType()}, {result.obj == null}");
            PhysicalObject? obje = Frame​​Skill.Spear_HitSomething(spear, result, eu);
            Log.Logger(7, "Spear", "MySlugcat:MyPlayer:Spear_HitSomething_st", $"result.obj_Type ({result.obj?.GetType()}), result.obj_NulL ({result.obj == null})");
            //Console.WriteLine($"MySlugcat MyPlayer:st Spear_HitSomething: st {obje?.GetType()}, {obje == null}");
            if (obje != null)
            {
                result.obj = obje;
            }

            Weapon.Mode mode = spear.mode;
            bool obj = orig.Invoke(spear, result, eu);

            Log.Logger(7, "Spear", "MySlugcat:MyPlayer:Spear_HitSomething_zh", $"result.obj_Type ({result.obj?.GetType()}), result.obj_NulL ({result.obj == null})");
            //Console.WriteLine($"MySlugcat MyPlayer:zh Spear_HitSomething: zh , GetType {result.obj?.GetType()}, result.obj {result.obj == null}");
            Deflagration​​Skill.Spear_HitSomething(spear, result, eu, obj, mode);
            Log.Logger(7, "Spear", "MySlugcat:MyPlayer:Spear_HitSomething_sh", $"result.obj_Type ({result.obj?.GetType()}), result.obj_NulL ({result.obj == null})");
            //Console.WriteLine($"MySlugcat MyPlayer:sh Spear_HitSomething: sh {result.obj?.GetType()}, {result.obj == null}");

            return obj;
        }

        private static void Player_Die(On.Player.orig_Die orig, Player self)
        {
            if (self.slugcatStats.name == Plugin.YourSlugID)
            {
                Creature obj = FrameSkill.Player_Die(self);

                if (obj == self)
                {
                    if (obj is Player player)
                    {
                        //布尔值wasDead判断动物(玩家)是否死亡
                        bool wasDead = self.dead;
                        orig(self);

                        DeflagrationSkill.Player_Die(obj, wasDead);
                        self.Destroy();
                    }
                }
            }
            else
            {
                orig(self);
            }

        }



    }
}
