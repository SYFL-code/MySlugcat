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


namespace MySlugcat
{
    //精疲力竭
    public class Exhausted
    {

        public static void Hook()
        {
            On.Player.Update += Player_Update;
            On.Player.ThrownSpear += Player_ThrownSpear;
        }

        private static void Player_Update(On.Player.orig_Update orig, Player player, bool eu)
        {
            orig(player, eu);

            if (player.slugcatStats.name == Plugin.YourSlugID && (SC.MySlugcatStats == 0 && SC.Exhausted))
            {
                player.gourmandAttackNegateTime--;

                if (player.lungsExhausted && (!player.gourmandExhausted))
                {
                    player.aerobicLevel = 1f;
                }

                if ((double)player.aerobicLevel >= 0.95)
                {
                    player.gourmandExhausted = true;
                }
                if (player.aerobicLevel < 0.4f)
                {
                    player.gourmandExhausted = false;
                }
                if (player.gourmandExhausted)
                {
                    player.slowMovementStun = Math.Max(player.slowMovementStun, (int)Custom.LerpMap(player.aerobicLevel, 0.7f, 0.4f, 6f, 0f));
                    player.lungsExhausted = true;
                }
            }
        }

        private static void Player_ThrownSpear(On.Player.orig_ThrownSpear orig, Player player, Spear spear)
        {
            if (player.slugcatStats.name == Plugin.YourSlugID && SC.MySlugcatStats == 0)
            {
                spear.throwModeFrames = 18;
                spear.spearDamageBonus = 0.6f + 0.3f * Mathf.Pow(UnityEngine.Random.value, 4f);
                BodyChunk firstChunk = spear.firstChunk;
                firstChunk.vel.x = firstChunk.vel.x * 0.77f;
                if (!player.gourmandExhausted)
                {
                    /*if (player.canJump != 0)
                    {
                        player.animation = Player.AnimationIndex.Roll;
                    }
                    else
                    {
                        player.animation = Player.AnimationIndex.Flip;
                    }*/
                    if ((player.room != null && player.room.gravity == 0f) || Mathf.Abs(spear.firstChunk.vel.x) < 1f)
                    {
                        //player.firstChunk.vel += spear.firstChunk.vel.normalized * 9f;
                    }
                    else
                    {
                        //player.rollDirection = (int)Mathf.Sign(spear.firstChunk.vel.x);
                        player.rollCounter = 0;
                        //BodyChunk firstChunk3 = player.firstChunk;
                        //firstChunk3.vel.x = firstChunk3.vel.x + Mathf.Sign(spear.firstChunk.vel.x) * 9f;
                    }
                    player.gourmandAttackNegateTime = 80;
                }

                if (player.gourmandExhausted)
                {
                    spear.spearDamageBonus = 0.25f;
                }

                /*                //风之祝福
                                if (spear.thrownBy == player)
                                {
                                    int N = player.playerState.playerNumber;
                                    spear.spearDamageBonus = 1.5f;
                                    BodyChunk firstChunk = spear.firstChunk;
                                    firstChunk.vel.x = firstChunk.vel.x * 1.2f;
                                    if (ModManager.MSC && player.gourmandExhausted)
                                    {
                                        spear.spearDamageBonus = 0.3f;
                                    }
                                    if (WindBlessingCooling[N] == 1)
                                    {
                                        spear.spearDamageBonus = 2.5f;
                                        spear.firstChunk.vel *= 1.2f;
                                    }

                                }*/
            }
            else
            {
                orig(player, spear);
            }
        }

    }
}
