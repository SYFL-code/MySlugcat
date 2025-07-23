using On;
using IL;
using System;
using System.Threading.Tasks;
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
using System.Threading;
using System.IO;
using Expedition;
using JollyCoop;


namespace MySlugcat
{
    // 蛞蝓猫数据

    public class MySlugcatStats
    {

        public static void Hook()
        {
#if MYDEBUG
            try
            {
#endif
            On.SlugcatStats.ctor += SlugcatStats_ctor;
            On.Player.MovementUpdate += Player_MovementUpdate;

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

        private static void SlugcatStats_ctor(On.SlugcatStats.orig_ctor orig, SlugcatStats slugcatStats, SlugcatStats.Name slugcat, bool malnourished)
        {
            if (slugcat == Plugin.YourSlugID && SC.MySlugcatStats == 0)
            {
                orig(slugcatStats, slugcat, malnourished);

                slugcatStats.runspeedFac = 0.74f;
                slugcatStats.bodyWeightFac = 0.68f;
                slugcatStats.generalVisibilityBonus = 3f;
                slugcatStats.visualStealthInSneakMode = -0.5f;
                slugcatStats.loudnessFac = 2f;
                slugcatStats.lungsFac = 1.4f;
                slugcatStats.throwingSkill = 0;
            }
            else
            {
                orig(slugcatStats, slugcat, malnourished);
            }
        }

        private static void Player_MovementUpdate(On.Player.orig_MovementUpdate orig, Player player, bool eu)
        {
            if (player.slugcatStats.name == Plugin.YourSlugID && SC.MySlugcatStats == 1)
            {
                int num2 = 0;
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        if (player.IsTileSolid(j, Custom.eightDirections[i].x, Custom.eightDirections[i].y) && player.IsTileSolid(j, Custom.eightDirections[i + 4].x, Custom.eightDirections[i + 4].y))
                        {
                            num2++;
                        }
                    }
                }

                if ((num2 > 1 && player.bodyChunks[0].onSlope == 0 && player.bodyChunks[1].onSlope == 0 && (!player.IsTileSolid(0, 0, 0) || !player.IsTileSolid(1, 0, 0))) || (player.IsTileSolid(0, -1, 0) && player.IsTileSolid(0, 1, 0)) || (player.IsTileSolid(1, -1, 0) && player.IsTileSolid(1, 1, 0)))
                {
                }
                else
                {
                    bool flag4 = player.bodyChunks[0].ContactPoint.y == -1 || player.bodyChunks[1].ContactPoint.y == -1;
                    if (flag4)
                    {

                    }
                    else if (player.jumpBoost > 0f && (player.input[0].jmp || player.simulateHoldJumpButton > 0))
                    {
                        player.jumpBoost += 0.9f;
                        //BodyChunk bodyChunk = player.bodyChunks[0];
                        //bodyChunk.vel.y = bodyChunk.vel.y + (player.jumpBoost + 1f) * 0.3f;
                        //BodyChunk bodyChunk2 = player.bodyChunks[1];
                        //bodyChunk2.vel.y = bodyChunk2.vel.y + (player.jumpBoost + 1f) * 0.3f;
                    }
                }

                orig(player, eu);
            }
            else
            {
                orig(player, eu);
            }

        }


    }
}
