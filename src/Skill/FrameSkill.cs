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
using static MonoMod.InlineRT.MonoModRule;
using System.Reflection;


namespace MySlugcat
{
    //嫁祸技能
    public class Frame​​Skill
    {
        public static void Hook()
        {
#if MYDEBUG
            try
            {
#endif
            //On.Player.ctor += Player_ctor;
            //On.Player.Update += Player_Update;

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
            //On.Spear.HitSomething += Spear_HitSomething;
            On.ScavengerBomb.HitSomething += ScavengerBomb_HitSomething;
            //挣脱魔王秃鹫
            On.Vulture.Carry += Vulture_Carry;

            //On.Player.Die += Player_Die;

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

/*        public static void Object(PhysicalObject obj, Room room, WorldCoordinate pos)
        {
            bool flag;
            if (obj == null)
            {
                flag = (null != null);
            }
            else
            {
                Room room2 = obj.room;
                flag = (((room2 != null) ? room2.game : null) != null);
            }
            if (!flag || obj.abstractPhysicalObject == null)
            {
                return;
            }
            AbstractPhysicalObject abstractPhysicalObject = obj.abstractPhysicalObject;
            if (obj is Player && (obj as Player).playerState != null)
            {
                try
                {
                    if ((obj as Player).objectInStomach is AbstractCreature)
                    {
                        AbstractCreature abstractCreature3 = (obj as Player).objectInStomach as AbstractCreature;
                        AbstractCreature abstractCreature2 = (obj as Player).abstractCreature;
                        bool flag2;
                        if (abstractCreature2 == null)
                        {
                            flag2 = (null != null);
                        }
                        else
                        {
                            World world = abstractCreature2.world;
                            flag2 = (((world != null) ? world.GetAbstractRoom(abstractCreature3.pos.room) : null) != null);
                        }
                        if (!flag2)
                        {
                            abstractCreature3.pos = (obj as Player).coord;
                        }
                        World world2 = abstractCreature3.world;
                        bool flag3;
                        if (world2 == null)
                        {
                            flag3 = false;
                        }
                        else
                        {
                            RainWorldGame game = world2.game;
                            flag3 = ((game != null) ? new bool?(game.IsStorySession) : null).GetValueOrDefault();
                        }
                        if (flag3)
                        {
                            (obj as Player).playerState.swallowedItem = SaveState.AbstractCreatureToStringStoryWorld(abstractCreature3);
                        }
                        else
                        {
                            World world3 = abstractCreature3.world;
                            bool flag4;
                            if (world3 == null)
                            {
                                flag4 = false;
                            }
                            else
                            {
                                RainWorldGame game2 = world3.game;
                                flag4 = ((game2 != null) ? new bool?(game2.IsArenaSession) : null).GetValueOrDefault();
                            }
                            if (flag4)
                            {
                                (obj as Player).playerState.swallowedItem = SaveState.AbstractCreatureToStringSingleRoomWorld(abstractCreature3);
                            }
                            else
                            {
                                Plugin.Logger.LogWarning("Clipboard.CutObject, could not store swallowed creature");
                            }
                        }
                    }
                    else
                    {
                        PlayerState playerState = (obj as Player).playerState;
                        AbstractPhysicalObject objectInStomach = (obj as Player).objectInStomach;
                        playerState.swallowedItem = ((objectInStomach != null) ? objectInStomach.ToString() : null);
                    }
                }
                catch (Exception ex)
                {
                    Plugin.Logger.LogWarning("Clipboard.CutObject exception: " + ((ex != null) ? ex.ToString() : null));
                }
            }
            if (obj != null)
            {
                obj.RemoveFromRoom();
            }
            if (obj != null)
            {
                AbstractPhysicalObject abstractPhysicalObject2 = obj.abstractPhysicalObject;
                if (abstractPhysicalObject2 != null)
                {
                    AbstractRoom room2 = abstractPhysicalObject2.Room;
                    if (room2 != null)
                    {
                        room2.RemoveEntity(obj.abstractPhysicalObject);
                    }
                }
            }
            if (!(obj is Player) && obj != null)
            {
                obj.Destroy();
            }

            if (((room != null) ? room.world : null) == null || ((room != null) ? room.abstractRoom : null) == null)
            {
                return;
            }

            if (abstractPhysicalObject == null)
            {
                return;
            }
            abstractPhysicalObject.pos = pos;
            abstractPhysicalObject.world = room.world;
            AbstractCreature abstractCreature = abstractPhysicalObject as AbstractCreature;
            if (abstractCreature != null)
            {
                AbstractCreatureAI abstractAI = abstractCreature.abstractAI;
                if (abstractAI != null)
                {
                    abstractAI.NewWorld(room.world);
                }
            }
            if (abstractPhysicalObject is AbstractCreature)
            {
                abstractPhysicalObject.Abstractize(pos);
            }
            if (abstractPhysicalObject.realizedObject != null)
            {
                abstractPhysicalObject.realizedObject.slatedForDeletetion = false;
                abstractPhysicalObject.realizedObject.room = room;
            }
            room.abstractRoom.AddEntity(abstractPhysicalObject);
            abstractPhysicalObject.RealizeInRoom();
            if (abstractPhysicalObject.realizedObject is Player)
            {
                try
                {
                    PlayerState playerState = (abstractPhysicalObject.realizedObject as Player).playerState;
                    if (string.IsNullOrEmpty((playerState != null) ? playerState.swallowedItem : null))
                    {
                        (abstractPhysicalObject.realizedObject as Player).objectInStomach = null;
                    }
                    else
                    {
                        AbstractPhysicalObject abstractPhysicalObject2 = null;
                        string text = (abstractPhysicalObject.realizedObject as Player).playerState.swallowedItem;
                        if (text.Contains("<oA>"))
                        {
                            abstractPhysicalObject2 = SaveState.AbstractPhysicalObjectFromString(abstractPhysicalObject.world, text);
                        }
                        else if (text.Contains("<cA>"))
                        {
                            string[] array = text.Split(new string[]
                            {
                                "<cA>"
                            }, StringSplitOptions.None);
                            text = text.Replace(array[2], string.Format(CultureInfo.InvariantCulture, "{0}.{1}", pos.ResolveRoomName() ?? pos.room.ToString(), pos.abstractNode));
                            abstractPhysicalObject2 = SaveState.AbstractCreatureFromString(abstractPhysicalObject.world, text, false, default(WorldCoordinate));
                        }
                        if (abstractPhysicalObject2 != null)
                        {
                            abstractPhysicalObject2.pos = abstractPhysicalObject.pos;
                        }
                        (abstractPhysicalObject.realizedObject as Player).objectInStomach = abstractPhysicalObject2;
                        if (abstractPhysicalObject2 == null && !string.IsNullOrEmpty(text))
                        {
                            Plugin.Logger.LogWarning("Clipboard.PasteObject, swallowedItem string available but objectInStomach became null");
                        }
                        (abstractPhysicalObject.realizedObject as Player).playerState.swallowedItem = "";
                    }
                }
                catch (Exception ex)
                {
                    Plugin.Logger.LogWarning("Clipboard.PasteObject exception: " + ((ex != null) ? ex.ToString() : null));
                }
            }

        }*/

        //#nullable enable

        public static Creature? Frame(Player self, bool IncludePlayer, Creature NotIncludeCreature, int probability = -1)
        {
#if MYDEBUG
            try
            {
#endif
            Creature? creature = MyPlayer.RandomlySelectedCreature(self.room, true, self, false);
            int percentage = 40;
            if (probability == -1)
            {
                percentage = 100;
            }
            else
            {
                percentage = probability;
            }
            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(2);
            MethodBase methodBase = stackFrame.GetMethod();
            Log.Logger(8, "Frame", "MySlugcat:Frame​​Skill​​:Frame", $"Frameer ({creature}), Null  ({creature == null}), ({methodBase.DeclaringType?.Name}), ({methodBase.Name})");
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

                //Room selfroom = self.room;
                //Room creatureroom = creature.room;
                //WorldCoordinate selfpos = self.abstractPhysicalObject.pos;
                //WorldCoordinate creaturepos = creature.abstractPhysicalObject.pos;

                Vector2 selfpos = self.mainBodyChunk.pos;
                Vector2 creaturepos = creature.mainBodyChunk.pos;

                Log.Logger(7, "Frame", "MySlugcat:Frame​​Skill​​:Frame_Teleport_st", $"P({self})， PV({self.mainBodyChunk.pos}), C({creature}), CV({creature.mainBodyChunk.pos})");
                Teleport.SetObjectPosition(creature, selfpos);
                Log.Logger(7, "Frame", "MySlugcat:Frame​​Skill​​:Frame_Teleport_zh", $"P({self})， PV({self.mainBodyChunk.pos}), C({creature}), CV({creature.mainBodyChunk.pos})");
                Teleport.SetObjectPosition(self, creaturepos);
                Log.Logger(7, "Frame", "MySlugcat:Frame​​Skill​​:Frame_Teleport_sh", $"P({self})， PV({self.mainBodyChunk.pos}), C({creature}), CV({creature.mainBodyChunk.pos})");


                //Object(self, creatureroom, creaturepos);
                //Object(creature, selfroom, selfpos);

/*                Console.WriteLine("MySlugcat:st");
                Clipboard.CutObject(creature);
                Console.WriteLine($"MySlugcat:CutObject_C {Clipboard.cutObjects}");
                Clipboard.CutObject(self);
                Console.WriteLine($"MySlugcat:CutObject_P {Clipboard.cutObjects}");
                Clipboard.PasteObject(selfroom, creaturepos);
                Console.WriteLine($"MySlugcat:PasteObject_P {Clipboard.cutObjects}");
                Clipboard.PasteObject(creatureroom, selfpos);
                Console.WriteLine($"MySlugcat:PasteObject_C {Clipboard.cutObjects}");
                Console.WriteLine("sh");*/
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

        public static Creature Player_Die(Player self)
        {
            Creature creature = self;
            if (self.slugcatStats.name == Plugin.YourSlugID && !self.dead)
            {
                Log.Logger(7, "FrameDie", "MySlugcat:Frame​​Skill​​:Player_Die_st", $"");

                Creature? obj = Frame​​Skill.Frame(self, false, self, 12);

                Log.Logger(7, "FrameDie", "MySlugcat:Frame​​Skill​​:Player_Die_sh", $"Creature type: ({obj?.GetType()}), BodyChunks: ({obj?.bodyChunks?.Length}), Null ({obj == null})");
                if (obj != null)
                {
                    self.dead = false;
                    self.stun = 0;
                    //obj.Die();
                    var hs = obj.State as HealthState;
                    if (hs != null)
                    {
                        hs.health -= 1.5f;
                    }
                    creature = obj;
                }
            }
            return creature;
        }

/*        private static bool Spear_HitSomething(On.Spear.orig_HitSomething orig, Spear spear, SharedPhysics.CollisionResult result, bool eu)
        {
            Console.WriteLine($"MySlugcat:Spear_HitSomething,{result.obj == null},{result.obj is not Player},{result.obj is Player self1 && self1.slugcatStats.name == Plugin.YourSlugID}");
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

            Console.WriteLine("MySlugcat:Spear_HitSomething: st");

            Creature obj = Frame(self, false, self);

            Console.WriteLine($"MySlugcat:Spear_HitSomething: sh  Creature type: {obj?.GetType()}, BodyChunks: {obj?.bodyChunks?.Length}, {obj == null}");

            //spear.thrownBy = null;
            //Creature obj = FindNearestCreature(spear., Frameobj.room, false, null);
            if (obj != null)
            {
                //(!this.dead && this.State is HealthState && (this.State as HealthState).health < 0f && UnityEngine.Random.value < -(this.State as HealthState).health && UnityEngine.Random.value < 0.025f)
                //obj.health -= 1;
                //if((obj.State as HealthState).health -= 1)
                var hs = obj.State as HealthState;
                Console.WriteLine($"MySlugcat:Spear_HitSomething: sh hs {hs == null}");
                if (hs != null)
                {
                    hs.health -= spear.spearDamageBonus;
                }
                result.obj = obj;
                Console.WriteLine($"MySlugcat:Spear_HitSomething: sh result.obj {result.obj}");
            }

            return orig.Invoke(spear, result, eu);
        }*/


        public static PhysicalObject? Spear_HitSomething(Spear spear, SharedPhysics.CollisionResult result, bool eu)
        {
            Log.Logger(8, "Frame", "MySlugcat:Frame​​Skill​​:Spear_HitSomething", $"({result.obj != null}), ({result.obj is Player}), ({result.obj is Player self1 && self1.slugcatStats.name == Plugin.YourSlugID})");
            if (result.obj != null && result.obj is Player self && self.slugcatStats.name == Plugin.YourSlugID)
            {
                //Console.WriteLine("MySlugcat:Spear_HitSomething: st");
                Log.Logger(8, "Frame", "MySlugcat:Frame​​Skill​​:Spear_HitSomething_st", $"");
                //Console.WriteLine($"MySlugcat:Frame:Spear_HitSomething: st |");

                Creature? obj = Frame(self, false, self);

                //Console.WriteLine($"MySlugcat:Spear_HitSomething: sh \n Creature type: {obj?.GetType()}, BodyChunks: {obj?.bodyChunks?.Length}");
                Log.Logger(8, "Frame", "MySlugcat:Frame​​Skill​​:Spear_HitSomething_sh", $"Creature type ({obj?.GetType()}), BodyChunks ({obj?.bodyChunks?.Length}), Null ({obj == null})");
                //Console.WriteLine($"MySlugcat:Frame:Spear_HitSomething: sh |{obj?.GetType()}, {obj?.bodyChunks?.Length}, {obj != null}");

/*                if (obj != null)
                {
                    //(!this.dead && this.State is HealthState && (this.State as HealthState).health < 0f && UnityEngine.Random.value < -(this.State as HealthState).health && UnityEngine.Random.value < 0.025f)
                    //obj.health -= 1;
                    //if((obj.State as HealthState).health -= 1)
                    var hs = obj.State as HealthState;
                    if (hs != null)
                    {
                        hs.health -= spear.spearDamageBonus;
                    }

                    //result.obj = obj;
                }*/

                if (obj != null && obj.State is HealthState hs)
                {
                    hs.health -= spear.spearDamageBonus;
                    if (hs != null && hs.health != null)
                    {
                        Log.Logger(8, "Frame", "MySlugcat:Frame​​Skill​​:Spear_HitSomething", $"hs_Null ({hs == null}), hs ({hs?.health}) _ ({spear.spearDamageBonus})");
                        //Console.WriteLine($"MySlugcat:Frame:Spear_HitSomething: hs {hs != null}, {hs?.health} _ {spear.spearDamageBonus}");
                    }
                }

                if (obj == null && result.obj is Creature objself && objself.State is HealthState hs2)
                {
                    hs2.health -= spear.spearDamageBonus;
                    if (hs2 != null && hs2.health != null)
                    {
                        Log.Logger(8, "Frame", "MySlugcat:Frame​​Skill​​:Spear_HitSomething", $"hs2_Null ({hs2 == null}), hs ({hs2?.health}) _ ({spear.spearDamageBonus})");
                        //Console.WriteLine($"MySlugcat:Frame:Spear_HitSomething: hs {hs != null}, {hs?.health} _ {spear.spearDamageBonus}");
                    }
                }

                Log.Logger(8, "Frame", "MySlugcat:Frame​​Skill​​:Spear_HitSomething", $"Creature type ({obj?.GetType()})");
                //Console.WriteLine($"MySlugcat:Frame:Spear_HitSomething: obj {obj?.GetType()}");
                return obj;
            }
            Log.Logger(8, "Frame", "MySlugcat:Frame​​Skill​​:Spear_HitSomething", $"Creature type ({result.obj?.GetType()})");
            //Console.WriteLine($"MySlugcat:Frame:Spear_HitSomething: obj {result.obj?.GetType()}");
            return result.obj;

/*            if (result.obj == null)
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

            Console.WriteLine("MySlugcat:Spear_HitSomething: st");

            Creature obj = Frame(self, false, self);

            Console.WriteLine($"MySlugcat:Spear_HitSomething: sh \n Creature type: {obj?.GetType()}, BodyChunks: {obj?.bodyChunks?.Length}");

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

            return orig.Invoke(spear, result, eu);*/
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

            Creature? obj = Frame(self, false, self);
            //bomb.thrownBy = null;
            //Creature obj = FindNearestCreature(bomb., Frameobj.room, false, null);
            if (obj != null)
            {
                result.obj = obj;
                self.stun = 0;
            }

            bool resultbool = orig.Invoke(bomb, result, eu);

            if (obj != null)
            {
                self.stun = 0;
            }

            return resultbool;
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
                    Creature? newobj2 = Frame(self, false, self);
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
            Creature? newobj = Frame(self, false, self);
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

            Creature? newobj = Frame(self, false, self);
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
                    Player? self = obj.chunk.owner as Player;
                    if (self == null)
                        continue;
                    if (self.slugcatStats.name != Plugin.YourSlugID)
                        continue;
                    //取玩家变量
                    GlobalVar.playerVar.TryGetValue(self, out PlayerVar pv);

                    Creature? newobj = Frame(self, false, self);
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

            Creature? newobj = Frame(self, false, self);
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

                                        Creature? newobj = Frame(self, false, self);
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

            Creature? newobj = Frame(self, false, self);
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

            Creature? newobj = Frame(self, false, self);
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

            Creature? newobj = Frame(self, false, self);
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

            Creature? obj = Frame(self, false, self);
            if (obj != null)
            {
                vulture.grasps[0].grabbedChunk.owner = null;
                vulture.grasps[0].grabbedChunk.owner = obj;
                vulture.grasps[0].grabbed = obj;
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
