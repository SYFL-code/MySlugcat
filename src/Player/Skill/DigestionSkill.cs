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
using System.Runtime.CompilerServices;


namespace MySlugcat
{
    //暴食能力
    public class DigestionSkill
    {

        public static float[] burning = Array.ConvertAll(Enumerable.Repeat(0, 20).ToArray(), x => (float)x);

        public static void Hook()
        {
#if MYDEBUG
            try
            {
#endif
            //On.Player.ctor += Player_ctor;
            On.Player.Update += Player_Update;
            On.Player.SwallowObject += Player_SwallowObject;

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

        private static void Player_Update(On.Player.orig_Update orig, Player player, bool eu)
        {
            orig(player, eu);

            /*if (self.slugcatStats.name == Plugin.YourSlugID)
                        {
                            if (self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.Rock)
                            {
                                if (self.FoodInStomach < self.MaxFoodInStomach - 1 || 
                                    (self.FoodInStomach == self.MaxFoodInStomach - 1 && self.playerState.quarterFoodPoints <= 2))
                                {
                                    self.objectInStomach = null;
                                    self.AddQuarterFood();
                                    self.AddQuarterFood();
                                }
                            }

                        }*/

            int N = player.playerState.playerNumber;
            Room room = player.room;
            float LightIntensity = Mathf.Pow(Mathf.Sin(burning[N] * 3.1415927f), 0.4f);

            if (!SC.DigestionSkill)
            {
                burning[N] = 0f;
            }

            if (burning[N] > 0f && player != null)
            {
                burning[N] += 0.016666668f;
                if (burning[N] > 1f)
                {
                    burning[N] = 0f;
                }
/*                this.lastFlickerDir = player.flickerDir;
                this.flickerDir = Custom.DegToVec(UnityEngine.Random.value * 360f) * 50f * LightIntensity;
                this.lastFlashAlpha = this.flashAplha;
                this.flashAplha = Mathf.Pow(UnityEngine.Random.value, 0.3f) * LightIntensity;
                this.lastFlashRad = this.flashRad;
                this.flashRad = Mathf.Pow(UnityEngine.Random.value, 0.3f) * LightIntensity * 200f * 16f;*/
                for (int i = 0; i < room.abstractRoom.creatures.Count; i++)
                {
                    if (room.abstractRoom.creatures[i].realizedCreature != null && player != null && (room.abstractRoom.creatures[i].rippleLayer == player.abstractPhysicalObject.rippleLayer || room.abstractRoom.creatures[i].rippleBothSides || player.abstractPhysicalObject.rippleBothSides) && (Custom.DistLess(player.firstChunk.pos, room.abstractRoom.creatures[i].realizedCreature.mainBodyChunk.pos, LightIntensity * 600f) || (Custom.DistLess(player.firstChunk.pos, room.abstractRoom.creatures[i].realizedCreature.mainBodyChunk.pos, LightIntensity * 1600f) && room.VisualContact(player.firstChunk.pos, room.abstractRoom.creatures[i].realizedCreature.mainBodyChunk.pos))))
                    {
                        if (room.abstractRoom.creatures[i].creatureTemplate.type == CreatureTemplate.Type.Spider && !room.abstractRoom.creatures[i].realizedCreature.dead)
                        {
                            room.abstractRoom.creatures[i].realizedCreature.firstChunk.vel += Custom.DegToVec(UnityEngine.Random.value * 360f) * UnityEngine.Random.value * 7f;
                            room.abstractRoom.creatures[i].realizedCreature.Die();
                        }
                        else if (room.abstractRoom.creatures[i].realizedCreature is BigSpider bigSpider)
                        {
                            bigSpider.poison = 1f;
                            bigSpider.State.health -= UnityEngine.Random.value * 0.2f;
                            room.abstractRoom.creatures[i].realizedCreature.Stun(UnityEngine.Random.Range(10, 20));
                            if (player != null)
                            {
                                room.abstractRoom.creatures[i].realizedCreature.SetKillTag(player.abstractCreature);
                            }
                        }
                        if (player != null)
                        {
                            room.abstractRoom.creatures[i].realizedCreature.Blind((int)Custom.LerpMap(Vector2.Distance(player.firstChunk.pos, room.abstractRoom.creatures[i].realizedCreature.VisionPoint), 60f, 600f, 400f, 20f));
                        }
                        
                    }
                }
            }

        }

        public static void Player_SwallowObject(On.Player.orig_SwallowObject orig, Player player, int grasp)
        {
            Log.Logger(6, "Digestion", "MySlugcat:Digestion​​:Player_SwallowObject_sst", $"Name ({player.slugcatStats.name == Plugin.YourSlugID}), ({SC.DigestionSkill})");
            if (player.slugcatStats.name == Plugin.YourSlugID && SC.DigestionSkill)
            {
                if (grasp < 0 || player.grasps[grasp] == null)
                {
                    return;
                }
                AbstractPhysicalObject? abstractPhysicalObject = player.grasps[grasp].grabbed.abstractPhysicalObject;
                Log.Logger(7, "Digestion", "MySlugcat:Digestion​​:Player_SwallowObject_st", $"Type ({abstractPhysicalObject.type.ToString()})");
                if (abstractPhysicalObject is AbstractSpear abstractSpear)
                {
                    abstractSpear.stuckInWallCycles = 0;
                }
                player.objectInStomach = abstractPhysicalObject;
                if (ModManager.MMF && player.room.game.session is StoryGameSession storyGameSession)
                {
                    storyGameSession.RemovePersistentTracker(player.objectInStomach);
                }
                player.ReleaseGrasp(grasp);
                player.objectInStomach.realizedObject.RemoveFromRoom();
                player.objectInStomach.Abstractize(player.abstractCreature.pos);
                player.objectInStomach.Room.RemoveEntity(player.objectInStomach);

                Log.Logger(7, "Digestion", "MySlugcat:Digestion​​:Player_SwallowObject_zh", $"Type ({abstractPhysicalObject.type.ToString()})");
                //
                if (abstractPhysicalObject.type == AbstractPhysicalObject.AbstractObjectType.Rock && true &&
                    (player.FoodInStomach < player.MaxFoodInStomach - 1 ||
                    (player.FoodInStomach == player.MaxFoodInStomach - 1 && player.playerState.quarterFoodPoints < 4)))
                {
                    abstractPhysicalObject = null;
                    player.AddQuarterFood();
                }
                else if (abstractPhysicalObject.type == AbstractPhysicalObject.AbstractObjectType.WaterNut && true &&
                        (player.FoodInStomach < player.MaxFoodInStomach - 1 ||
                        (player.FoodInStomach == player.MaxFoodInStomach - 1 && player.playerState.quarterFoodPoints < 4)))
                {
                    abstractPhysicalObject = null;
                    player.AddFood(1);
                }
                else if (abstractPhysicalObject.type == AbstractPhysicalObject.AbstractObjectType.ScavengerBomb && true &&
                        (player.FoodInStomach < player.MaxFoodInStomach - 1 ||
                        (player.FoodInStomach == player.MaxFoodInStomach - 1 && player.playerState.quarterFoodPoints < 4)))
                {
                    if (22 > UnityEngine.Random.Range(0, 100))
                    {
                        DeflagrationSkill.Explode(player, null, player);
                        player.Die();
                    }else
                    {
                        abstractPhysicalObject = null;
                        player.AddQuarterFood();
                        player.AddQuarterFood();
                        player.AddQuarterFood();
                    }
                }
                else if (abstractPhysicalObject.type == AbstractPhysicalObject.AbstractObjectType.SporePlant && true &&
                        (player.FoodInStomach < player.MaxFoodInStomach - 1 ||
                        (player.FoodInStomach == player.MaxFoodInStomach - 1 && player.playerState.quarterFoodPoints < 4)))
                {
                    abstractPhysicalObject = null;
                    player.AddFood(1);
                }
                else if (abstractPhysicalObject.type == AbstractPhysicalObject.AbstractObjectType.FlareBomb && true &&
                        (player.FoodInStomach < player.MaxFoodInStomach - 1 ||
                        (player.FoodInStomach == player.MaxFoodInStomach - 1 && player.playerState.quarterFoodPoints < 4)))
                {
                    if (12 > UnityEngine.Random.Range(0, 100))
                    {
                        if (abstractPhysicalObject.realizedObject is FlareBomb flareBomb)
                        {
                            int N = player.playerState.playerNumber;
                            if (burning[N] > 0f)
                            {
                                return;
                            }
                            burning[N] = 0.01f;
                            player.room.PlaySound(SoundID.Flare_Bomb_Burn, player.firstChunk);
                            //flareBomb.StartBurn();
                            player.Die();
                        }
                    }
                    else
                    {
                        abstractPhysicalObject = null;
                        player.AddQuarterFood();
                        player.AddQuarterFood();
                        player.AddQuarterFood();
                    }
                }
                else if (abstractPhysicalObject.type == AbstractPhysicalObject.AbstractObjectType.PuffBall && true &&
                        (player.FoodInStomach < player.MaxFoodInStomach - 1 ||
                        (player.FoodInStomach == player.MaxFoodInStomach - 1 && player.playerState.quarterFoodPoints < 4)))
                {
                    if (18 > UnityEngine.Random.Range(0, 100))
                    {
                        Color result;
                        GetPixelImpl_Injected(0, 0, 11, 4, out result);
                        Color color = Color.Lerp(new Color(0.9f, 1f, 0.8f), result, 0.5f);
                        Color sporeColor = Color.Lerp(color, new Color(0.02f, 0.1f, 0.08f), 0.85f);

                        InsectCoordinator? smallInsects = null;
                        for (int i = 0; i < player.room.updateList.Count; i++)
                        {
                            if (player.room.updateList[i] is InsectCoordinator)
                            {
                                smallInsects = (player.room.updateList[i] as InsectCoordinator);
                                break;
                            }
                        }
                        for (int j = 0; j < 70; j++)
                        {
                            player.room.AddObject(new SporeCloud(player.firstChunk.pos, Custom.RNV() * UnityEngine.Random.value * 10f, sporeColor, 1f, player.abstractCreature, j % 20, smallInsects, player.abstractPhysicalObject.rippleLayer));
                        }
                        player.room.AddObject(new SporePuffVisionObscurer(player.firstChunk.pos, player.abstractPhysicalObject.rippleLayer));
                        for (int k = 0; k < 7; k++)
                        {
                            player.room.AddObject(new PuffBallSkin(player.firstChunk.pos, Custom.RNV() * UnityEngine.Random.value * 16f, color, Color.Lerp(color, sporeColor, 0.5f)));
                        }
                        player.room.PlaySound(SoundID.Puffball_Eplode, player.firstChunk);

                        player.Die();
                    }
                    else
                    {
                        abstractPhysicalObject = null;
                        player.AddQuarterFood();
                        player.AddQuarterFood();
                    }
                }
                else if (abstractPhysicalObject.type == AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant && true &&
                        (player.FoodInStomach < player.MaxFoodInStomach - 1 ||
                        (player.FoodInStomach == player.MaxFoodInStomach - 1 && player.playerState.quarterFoodPoints < 4)))
                {
                    if (15 > UnityEngine.Random.Range(0, 100))
                    {
                        Room room = player.room;
                        Vector2 pos = player.firstChunk.pos;
                        Color explodeColor = new Color(1f, 0.4f, 0.3f);

                        for (int i = 0; i < UnityEngine.Random.Range(6, 13); i++)
                        {
                            for (int k = UnityEngine.Random.Range(1, 6); k >= 0; k--)
                            {
                                room.AddObject(new Spark(pos, Custom.RNV() * Mathf.Lerp(15f, 30f, UnityEngine.Random.value), explodeColor, null, 7, 17));
                            }
                            room.AddObject(new Explosion.FlashingSmoke(pos, Custom.RNV() * 5f * UnityEngine.Random.value, 1f, new Color(1f, 1f, 1f), explodeColor, 5));
                            Explosion.ExplosionLight obj = new Explosion.ExplosionLight(pos, Mathf.Lerp(50f, 150f, UnityEngine.Random.value), 0.5f, 4, explodeColor);
                            room.AddObject(obj);
                            room.PlaySound(SoundID.Firecracker_Bang, pos, player.abstractPhysicalObject);
                            for (int l = 0; l < room.abstractRoom.creatures.Count; l++)
                            {
                                if (room.abstractRoom.creatures[l].realizedCreature != null && (room.abstractRoom.creatures[l].rippleLayer == player.abstractPhysicalObject.rippleLayer || room.abstractRoom.creatures[l].rippleBothSides || player.abstractPhysicalObject.rippleBothSides) && room.abstractRoom.creatures[l].realizedCreature.room == room && !room.abstractRoom.creatures[l].realizedCreature.dead)
                                {
                                    room.abstractRoom.creatures[l].realizedCreature.Deafen((int)Custom.LerpMap(Vector2.Distance(pos, room.abstractRoom.creatures[l].realizedCreature.mainBodyChunk.pos), 40f, 80f, 110f, 0f));
                                    room.abstractRoom.creatures[l].realizedCreature.Stun((int)Custom.LerpMap(Vector2.Distance(pos, room.abstractRoom.creatures[l].realizedCreature.mainBodyChunk.pos), 40f, 80f, 10f, 0f));
                                }
                            }
                            if (i == 0)
                            {
                                var scareObj = new FirecrackerPlant.ScareObject(pos, player.abstractPhysicalObject.rippleLayer);
                                room.AddObject(scareObj);
                            }
                        }
                    }
                    else
                    {
                        abstractPhysicalObject = null;
                        player.AddQuarterFood();
                        player.AddQuarterFood();
                    }
                }
                else if (abstractPhysicalObject.type == AbstractPhysicalObject.AbstractObjectType.Lantern && true &&
                        (player.FoodInStomach < player.MaxFoodInStomach - 1 ||
                        (player.FoodInStomach == player.MaxFoodInStomach - 1 && player.playerState.quarterFoodPoints < 4)))
                {
                    abstractPhysicalObject = null;
                    player.AddQuarterFood();
                }
                else if (abstractPhysicalObject.type == AbstractPhysicalObject.AbstractObjectType.FlyLure && true &&
                        (player.FoodInStomach < player.MaxFoodInStomach - 1 ||
                        (player.FoodInStomach == player.MaxFoodInStomach - 1 && player.playerState.quarterFoodPoints < 4)))
                {
                    abstractPhysicalObject = null;
                    player.AddQuarterFood();
                }
                else if (abstractPhysicalObject.type == AbstractPhysicalObject.AbstractObjectType.BubbleGrass && true &&
                        (player.FoodInStomach < player.MaxFoodInStomach - 1 ||
                        (player.FoodInStomach == player.MaxFoodInStomach - 1 && player.playerState.quarterFoodPoints < 4)))
                {
                    abstractPhysicalObject = null;
                    player.AddQuarterFood();
                }
                else if (abstractPhysicalObject.type == AbstractPhysicalObject.AbstractObjectType.OverseerCarcass && true &&
                        (player.FoodInStomach < player.MaxFoodInStomach - 1 ||
                        (player.FoodInStomach == player.MaxFoodInStomach - 1 && player.playerState.quarterFoodPoints < 4)))
                {
                    abstractPhysicalObject = null;
                    player.AddFood(1);
                }
                //
                Log.Logger(7, "Digestion", "MySlugcat:Digestion​​:Player_SwallowObject_sh", $"Type ({abstractPhysicalObject?.type.ToString()}), Null({abstractPhysicalObject == null})");

                /*else if (abstractPhysicalObject.type == AbstractPhysicalObject.AbstractObjectType.FlareBomb && true &&
                        (player.FoodInStomach < player.MaxFoodInStomach - 1 ||
                        (player.FoodInStomach == player.MaxFoodInStomach - 1 && player.playerState.quarterFoodPoints < 4)))
                {
                    abstractPhysicalObject = null;
                    player.AddQuarterFood();
                    player.AddQuarterFood();
                }
                else if (abstractPhysicalObject.type == AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant && true &&
                        (player.FoodInStomach < player.MaxFoodInStomach - 1 ||
                        (player.FoodInStomach == player.MaxFoodInStomach - 1 && player.playerState.quarterFoodPoints < 4)))
                {
                    abstractPhysicalObject = null;
                    player.AddQuarterFood();
                    player.AddQuarterFood();
                }
                else if (abstractPhysicalObject.type == AbstractPhysicalObject.AbstractObjectType.PuffBall && true &&
                        (player.FoodInStomach < player.MaxFoodInStomach - 1 ||
                        (player.FoodInStomach == player.MaxFoodInStomach - 1 && player.playerState.quarterFoodPoints <= 2)))
                {
                    abstractPhysicalObject = null;
                    player.AddQuarterFood();
                    player.AddQuarterFood();
                }*/
                //

                player.objectInStomach = abstractPhysicalObject;
                if (player.objectInStomach != null)
                {
                    player.objectInStomach.Abstractize(player.abstractCreature.pos);
                }
                BodyChunk mainBodyChunk = player.mainBodyChunk;
                mainBodyChunk.vel.y = mainBodyChunk.vel.y + 2f;
                player.room.PlaySound(SoundID.Slugcat_Swallow_Item, player.mainBodyChunk);
                Log.Logger(7, "Digestion", "MySlugcat:Digestion​​:Player_SwallowObject_ssh", $"Type ({player.objectInStomach?.type.ToString()}), Null({player.objectInStomach == null})");
            }
            else
            {
                orig(player, grasp);
            }
        }


        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void GetPixelImpl_Injected(int image, int mip, int x, int y, out Color ret);

    }
}
