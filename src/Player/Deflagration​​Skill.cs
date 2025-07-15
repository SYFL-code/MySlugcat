using On;
using IL;
using System;
using Mono.Cecil;
using MoreSlugcats;
using Noise;
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
using static MonoMod.InlineRT.MonoModRule;


namespace MySlugcat
{
    //爆燃技能
    public class Deflagration​​Skill
    {

        public static void Hook()
        {
#if MYDEBUG
            try
            {
#endif
            //On.Player.ctor += Player_ctor;
            //On.Player.Update += Player_Update;

            //On.Spear.HitSomething += Spear_HitSomething;
            On.Spear.SetRandomSpin += Spear_SetRandomSpin;
            On.Rock.HitSomething += Rock_HitSomething;
            On.Weapon.SetRandomSpin += Weapon_SetRandomSpin;
            //On.PuffBall.HitSomething += PuffBall_HitSomething;
            On.PuffBall.Explode += PuffBall_Explode;
            //On.FlareBomb.HitSomething += FlareBomb_HitSomething;
            On.FlareBomb.StartBurn += FlareBomb_StartBurn;
            On.MoreSlugcats.LillyPuck.HitSomething += LillyPuck_HitSomething;
            On.MoreSlugcats.LillyPuck.SetRandomSpin += LillyPuck_SetRandomSpin;
            //On.ScavengerBomb.HitSomething += ScavengerBomb_HitSomething;
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

        public static void Explode(PhysicalObject self, BodyChunk? hitChunk, Creature ThrownBy)
        {
            if (self.slatedForDeletetion)
            {
                return;
            }
            Room room = self.room;
            Color explodeColor = new Color(1f, 0.4f, 0.3f);
            Vector2 vector = Vector2.Lerp(self.firstChunk.pos, self.firstChunk.lastPos, 0.35f);
            room.AddObject(new SootMark(room, vector, 80f, true));
            //if (!this.explosionIsForShow)
            //{
            //}
            room.AddObject(new Explosion(room, self, vector, 7, 250f, 6.2f, 2f, 280f, 0.25f, ThrownBy, 0.7f, 160f, 1f));

            room.AddObject(new Explosion.ExplosionLight(vector, 280f, 1f, 7, explodeColor));
            room.AddObject(new Explosion.ExplosionLight(vector, 230f, 1f, 3, new Color(1f, 1f, 1f)));
            room.AddObject(new ExplosionSpikes(room, vector, 14, 30f, 9f, 7f, 170f, explodeColor));
            room.AddObject(new ShockWave(vector, 330f, 0.045f, 5, false));
            for (int i = 0; i < 25; i++)
            {
                Vector2 a = Custom.RNV();
                if (room.GetTile(vector + a * 20f).Solid)
                {
                    if (!room.GetTile(vector - a * 20f).Solid)
                    {
                        a *= -1f;
                    }
                    else
                    {
                        a = Custom.RNV();
                    }
                }
                for (int j = 0; j < 3; j++)
                {
                    room.AddObject(new Spark(vector + a * Mathf.Lerp(30f, 60f, UnityEngine.Random.value), a * Mathf.Lerp(7f, 38f, UnityEngine.Random.value) + Custom.RNV() * 20f * UnityEngine.Random.value, Color.Lerp(explodeColor, new Color(1f, 1f, 1f), UnityEngine.Random.value), null, 11, 28));
                }
                room.AddObject(new Explosion.FlashingSmoke(vector + a * 40f * UnityEngine.Random.value, a * Mathf.Lerp(4f, 20f, Mathf.Pow(UnityEngine.Random.value, 2f)), 1f + 0.05f * UnityEngine.Random.value, new Color(1f, 1f, 1f), explodeColor, UnityEngine.Random.Range(3, 11)));
            }

            BombSmoke? smoke = null;
            if (40 > UnityEngine.Random.Range(0, 100))
            {
                smoke = new BombSmoke(room, self.firstChunk.pos, self.firstChunk, explodeColor);
                room.AddObject(smoke);
                for (int k = 0; k < 8; k++)
                {
                    smoke.EmitWithMyLifeTime(vector + Custom.RNV(), Custom.RNV() * UnityEngine.Random.value * 17f);
                }
            }
            for (int l = 0; l < 6; l++)
            {
                room.AddObject(new ScavengerBomb.BombFragment(vector, Custom.DegToVec(((float)l + UnityEngine.Random.value) / 6f * 360f) * Mathf.Lerp(18f, 38f, UnityEngine.Random.value)));
            }
            room.ScreenMovement(new Vector2?(vector), default(Vector2), 1.3f);
            for (int m = 0; m < self.abstractPhysicalObject.stuckObjects.Count; m++)
            {
                self.abstractPhysicalObject.stuckObjects[m].Deactivate();
            }
            room.PlaySound(SoundID.Bomb_Explode, vector, self.abstractPhysicalObject);
            room.InGameNoise(new InGameNoise(vector, 9000f, self, 1f));
            bool flag = hitChunk != null;
            for (int n = 0; n < 5; n++)
            {
                if (room.GetTile(vector + Custom.fourDirectionsAndZero[n].ToVector2() * 20f).Solid)
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                if (smoke == null)
                {
                    smoke = new BombSmoke(room, vector, null, explodeColor);
                    room.AddObject(smoke);
                }
                if (hitChunk != null)
                {
                    smoke.chunk = hitChunk;
                }
                else
                {
                    smoke.chunk = null;
                    smoke.fadeIn = 1f;
                }
                smoke.pos = vector;
                smoke.stationary = true;
                smoke.DisconnectSmoke();
            }
            else if (smoke != null)
            {
                smoke.Destroy();
            }
            //self.Destroy();

/*            if (smoke != null)
            {
                smoke.Destroy();
            }*/
        }

        /*        private static bool Spear_HitSomething(On.Spear.orig_HitSomething orig, Spear spear, SharedPhysics.CollisionResult result, bool eu)
                {
                    Console.WriteLine($"MySlugcat:Spear_1,{spear.thrownBy == null},{spear.thrownBy is not Player},{spear.thrownBy is Player self1 && self1.slugcatStats.name == Plugin.YourSlugID}");
                    if (spear.thrownBy == null)
                    {
                        return orig.Invoke(spear, result, eu);
                    }
                    //如果被命中的不是玩家
                    if (spear.thrownBy is not Player self)
                        return orig.Invoke(spear, result, eu);
                    //如果玩家不是MySlugcat则运行原程序
                    if (self.slugcatStats.name != Plugin.YourSlugID)
                        return orig.Invoke(spear, result, eu);

                    Weapon.Mode mode = spear.mode;
                    bool obj = orig.Invoke(spear, result, eu);
                    Console.WriteLine($"MySlugcat:Spear_1, mode {mode} , obj {obj}, result.chunk {result.chunk}");
                    //if (26 > UnityEngine.Random.Range(0, 100))
                    if (17 > UnityEngine.Random.Range(0, 100) && (obj || mode != spear.mode))
                    {
                        Explode(spear, result.chunk, self);
                        //spear.abstractPhysicalObject.stuckObjects[0].Deactivate();
                        //ScavengerBomb.Explode(result.chunk);
                        //public void Explode(BodyChunk hitChunk)
                    }

                    Console.WriteLine($"MySlugcat:Spear_1, obj {obj}");
                    return obj;
                }*/

        public static void Spear_HitSomething(Spear spear, SharedPhysics.CollisionResult result, bool eu, bool obj, Weapon.Mode mode)
        {
            Log.Logger(4, "Spear", "MySlugcat:Deflagration​​:Spear_HitSomething", $"({spear.thrownBy != null}), ({spear.thrownBy is Player}), ({spear.thrownBy is Player && ((Player)spear.thrownBy).slugcatStats.name == Plugin.YourSlugID})");
            //Console.WriteLine($"MySlugcat:Deflagration​​:Spear_HitSomething: {spear.thrownBy != null}, {spear.thrownBy is Player self1 && self1.slugcatStats.name == Plugin.YourSlugID}, {spear.thrownBy is Player}");
            if (spear.thrownBy != null && spear.thrownBy is Player self && self.slugcatStats.name == Plugin.YourSlugID)
            {
                Log.Logger(4, "Spear", "MySlugcat:Deflagration​​:Spear_HitSomething", $"obj ({obj}), mode ({mode}), spear.mode ({spear.mode})");
                //Console.WriteLine($"MySlugcat:Deflagration​​:Spear_HitSomething: obj {obj}, mode {mode}, spear.mode {spear.mode}");
                if (17 > UnityEngine.Random.Range(0, 100) && (obj || mode != spear.mode))
                {
                    Log.Logger(4, "Spear", "MySlugcat:Deflagration​​:Spear_HitSomething", $"thrownBy ({self})");
                    Explode(spear, result.chunk, self);
                }
            }

/*            if (spear.thrownBy == null)
            {
                return orig.Invoke(spear, result, eu);
            }
            //如果被命中的不是玩家
            if (spear.thrownBy is not Player self)
                return orig.Invoke(spear, result, eu);
            //如果玩家不是MySlugcat则运行原程序
            if (self.slugcatStats.name != Plugin.YourSlugID)
                return orig.Invoke(spear, result, eu);

            Weapon.Mode mode = spear.mode;
            bool obj = orig.Invoke(spear, result, eu);
            //if (26 > UnityEngine.Random.Range(0, 100))
            if (17 > UnityEngine.Random.Range(0, 100) && (obj || mode != spear.mode))
            {
                Explode(spear, result.chunk, self);
                //spear.abstractPhysicalObject.stuckObjects[0].Deactivate();
                //ScavengerBomb.Explode(result.chunk);
                //public void Explode(BodyChunk hitChunk)
            }

            return obj;*/
        }

        private static void Spear_SetRandomSpin(On.Spear.orig_SetRandomSpin orig, Spear spear)
        {
            orig(spear);

            Log.Logger(7, "Spear", "MySlugcat:Deflagration​​:Spear_SetRandomSpin", $"({spear.thrownBy != null}), ({spear.thrownBy is Player}), ({spear.thrownBy is Player && ((Player)spear.thrownBy).slugcatStats.name == Plugin.YourSlugID})");
            if (spear.thrownBy != null && spear.thrownBy is Player self && self.slugcatStats.name == Plugin.YourSlugID)
            {
                if (17 > UnityEngine.Random.Range(0, 300))
                {
                    Log.Logger(7, "Spear", "MySlugcat:Deflagration​​:Spear_SetRandomSpin", $"thrownBy ({spear.thrownBy})");
                    Explode(spear, null, spear.thrownBy);
                }

            }
        }

        private static void Weapon_SetRandomSpin(On.Weapon.orig_SetRandomSpin orig, Weapon weapon)
        {
            orig(weapon);

            //Log.Logger(8, "Weapon", "MySlugcat:Deflagration​​:Rock_SetRandomSpin", $"({weapon is Rock}), ({weapon.thrownBy != null}), ({weapon.thrownBy is Player}), ({weapon.thrownBy is Player && ((Player)weapon.thrownBy).slugcatStats.name == Plugin.YourSlugID})");
            if (weapon is Rock rock)
            {
                Log.Logger(7, "Rock", "MySlugcat:Deflagration​​:Rock_SetRandomSpin", $"({weapon is Rock}), ({rock.thrownBy != null}), ({rock.thrownBy is Player}), ({rock.thrownBy is Player && ((Player)rock.thrownBy).slugcatStats.name == Plugin.YourSlugID})");
                if (rock.thrownBy != null && rock.thrownBy is Player self && self.slugcatStats.name == Plugin.YourSlugID)
                {
                    if (10 > UnityEngine.Random.Range(0, 100))
                    {
                        Log.Logger(4, "Rock", "MySlugcat:Deflagration​​:Rock_SetRandomSpin", $"thrownBy ({rock.thrownBy})");
                        Explode(rock, null, rock.thrownBy);
                    }

                }
            }

        }

        private static bool Rock_HitSomething(On.Rock.orig_HitSomething orig, Rock rock, SharedPhysics.CollisionResult result, bool eu)
        {
            if (rock.thrownBy == null)
            {
                return orig.Invoke(rock, result, eu);
            }
            //如果被命中的不是玩家
            if (rock.thrownBy is not Player self)
                return orig.Invoke(rock, result, eu);
            //如果玩家不是MySlugcat则运行原程序
            if (self.slugcatStats.name != Plugin.YourSlugID)
                return orig.Invoke(rock, result, eu);

            Weapon.Mode mode = rock.mode;
            bool obj = orig.Invoke(rock, result, eu);
            //if (26 > UnityEngine.Random.Range(0, 100))
            if (10 > UnityEngine.Random.Range(0, 100) && (obj || mode != rock.mode))
            {
                Explode(rock, result.chunk, self);
                rock.Destroy();
                return true;
                //spear.abstractPhysicalObject.stuckObjects[0].Deactivate();
                //ScavengerBomb.Explode(result.chunk);
                //public void Explode(BodyChunk hitChunk)
            }
            return obj;
        }

        private static bool LillyPuck_HitSomething(On.MoreSlugcats.LillyPuck.orig_HitSomething orig, LillyPuck lillyPuck, SharedPhysics.CollisionResult result, bool eu)
        {
            if (lillyPuck.thrownBy == null)
            {
                return orig.Invoke(lillyPuck, result, eu);
            }
            //如果被命中的不是玩家
            if (lillyPuck.thrownBy is not Player self)
                return orig.Invoke(lillyPuck, result, eu);
            //如果玩家不是MySlugcat则运行原程序
            if (self.slugcatStats.name != Plugin.YourSlugID)
                return orig.Invoke(lillyPuck, result, eu);

            Weapon.Mode mode = lillyPuck.mode;
            bool obj = orig.Invoke(lillyPuck, result, eu);
            if (30 > UnityEngine.Random.Range(0, 100) && (obj || mode != lillyPuck.mode))
            {
                Explode(lillyPuck, result.chunk, self);
                lillyPuck.Destroy();
                //return true;
                //lillyPuck.abstractPhysicalObject.stuckObjects[0].Deactivate();
                //ScavengerBomb.Explode(result.chunk);
                //public void Explode(BodyChunk hitChunk)
            }

            return obj;
        }

        private static void LillyPuck_SetRandomSpin(On.MoreSlugcats.LillyPuck.orig_SetRandomSpin orig, LillyPuck lillyPuck)
        {
            orig(lillyPuck);

            Log.Logger(7, "LillyPuck", "MySlugcat:Deflagration​​:LillyPuck_SetRandomSpin", $"({lillyPuck.thrownBy != null}), ({lillyPuck.thrownBy is Player}), ({lillyPuck.thrownBy is Player && ((Player)lillyPuck.thrownBy).slugcatStats.name == Plugin.YourSlugID})");
            if (lillyPuck.thrownBy != null && lillyPuck.thrownBy is Player self && self.slugcatStats.name == Plugin.YourSlugID)
            {
                if (30 > UnityEngine.Random.Range(0, 300))
                {
                    Log.Logger(7, "LillyPuck", "MySlugcat:Deflagration​​:LillyPuck_SetRandomSpin", $"thrownBy ({lillyPuck.thrownBy})");
                    Explode(lillyPuck, null, lillyPuck.thrownBy);
                    lillyPuck.Destroy();
                }

            }
        }

        private static bool PuffBall_HitSomething(On.PuffBall.orig_HitSomething orig, PuffBall puffBall, SharedPhysics.CollisionResult result, bool eu)
        {
        if (puffBall.thrownBy == null)
        {
            return orig.Invoke(puffBall, result, eu);
        }
        //如果被命中的不是玩家
        if (puffBall.thrownBy is not Player self)
            return orig.Invoke(puffBall, result, eu);
        //如果玩家不是MySlugcat则运行原程序
        if (self.slugcatStats.name != Plugin.YourSlugID)
            return orig.Invoke(puffBall, result, eu);

        bool obj = orig.Invoke(puffBall, result, eu);
        if (18 > UnityEngine.Random.Range(0, 100) && obj)
        {
            Explode(puffBall, result.chunk, self);
            //puffBall.abstractPhysicalObject.stuckObjects[0].Deactivate();
            //ScavengerBomb.Explode(result.chunk);
           //public void Explode(BodyChunk hitChunk)
        }
        return obj;
        }

        private static void PuffBall_Explode(On.PuffBall.orig_Explode orig, PuffBall puffBall)
        {
            orig(puffBall);

            if (15 > UnityEngine.Random.Range(0, 100))
            {
                Explode(puffBall, null, puffBall.thrownBy);
                //return true;
                //lillyPuck.abstractPhysicalObject.stuckObjects[0].Deactivate();
                //ScavengerBomb.Explode(result.chunk);
                //public void Explode(BodyChunk hitChunk)
            }
        }


        /*        private static bool FlareBomb_HitSomething(On.FlareBomb.orig_HitSomething orig, FlareBomb flareBomb, SharedPhysics.CollisionResult result, bool eu)
                {
                    if (flareBomb.thrownBy == null)
                    {
                        return orig.Invoke(flareBomb, result, eu);
                    }
                    //如果被命中的不是玩家
                    if (flareBomb.thrownBy is not Player self)
                        return orig.Invoke(flareBomb, result, eu);
                    //如果玩家不是MySlugcat则运行原程序
                    if (self.slugcatStats.name != Plugin.YourSlugID)
                        return orig.Invoke(flareBomb, result, eu);

                    bool obj = orig.Invoke(flareBomb, result, eu);
                    if (18 > UnityEngine.Random.Range(0, 100) && obj)
                    {
                        Explode(flareBomb, result.chunk, self);
                        //puffBall.abstractPhysicalObject.stuckObjects[0].Deactivate();
                        //ScavengerBomb.Explode(result.chunk);
                        //public void Explode(BodyChunk hitChunk)
                    }

                    return obj;
                }*/

        private static void FlareBomb_StartBurn(On.FlareBomb.orig_StartBurn orig, FlareBomb flareBomb)
        {
            orig(flareBomb);

            if (18 > UnityEngine.Random.Range(0, 100) && (flareBomb.color != new Color(0.3f, 0f, 0.9f) || flareBomb.color == new Color(0.2f, 0f, 1f)))
            {
                Explode(flareBomb, null, flareBomb.thrownBy);
                //return true;
                //lillyPuck.abstractPhysicalObject.stuckObjects[0].Deactivate();
                //ScavengerBomb.Explode(result.chunk);
                //public void Explode(BodyChunk hitChunk)
            }
            flareBomb.color = new Color(0.3f, 0f, 0.9f);
        }

        public static void Player_Die(Creature self, bool orig)
        {
            if (self is Player player&& player.slugcatStats.name == Plugin.YourSlugID && !orig && player.dead)
            {
                if (100 > UnityEngine.Random.Range(0, 100))
                {
                    Explode(player, null, player);
                    //puffBall.abstractPhysicalObject.stuckObjects[0].Deactivate();
                    //ScavengerBomb.Explode(result.chunk);
                    //public void Explode(BodyChunk hitChunk)
                }
            }

        }

    }
}
