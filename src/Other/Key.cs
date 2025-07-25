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
    //按键输入
    public class Key
    {
        // player.input[0].jmp 玩家当前帧是否按下跳跃键
        // player.input[1].jmp 玩家上一帧是否按下跳跃键

        //按下按键的时长(40次 = 1秒)
        //jump 跳跃键
        public static int[] JmpCounter = Enumerable.Repeat(0, 20).ToArray();
        //pckp 拾取键
        public static int[] pckpCounter = Enumerable.Repeat(0, 20).ToArray();
        //throw 投掷键
        public static int[] thrwCounter = Enumerable.Repeat(0, 20).ToArray();
        //map 地图键
        public static int[] mpCounter = Enumerable.Repeat(0, 20).ToArray();
        //special 特殊键
        public static int[] specCounter = Enumerable.Repeat(0, 20).ToArray();
        //y Y键  输入为正（上）、零（无输入）、负（下）
        public static int[] yHCounter = Enumerable.Repeat(0, 20).ToArray();
        public static int[] yNCounter = Enumerable.Repeat(0, 20).ToArray();
        public static int[] yLCounter = Enumerable.Repeat(0, 20).ToArray();
        //x X键  输入为正（右）、零（无输入）、负（左）
        public static int[] xHCounter = Enumerable.Repeat(0, 20).ToArray();
        public static int[] xNCounter = Enumerable.Repeat(0, 20).ToArray();
        public static int[] xLCounter = Enumerable.Repeat(0, 20).ToArray();


        public static void Hook()
        {
            On.Player.Update += Player_Update;
        }

        private static void Player_Update(On.Player.orig_Update orig, Player player, bool eu)
        {
            orig(player, eu);

            int N = player.playerState.playerNumber;

            //jmp
            if (player.input[0].jmp)
            {
                JmpCounter[N]++;
            }
            else
            {
                if (!player.input[1].jmp)
                {
                    JmpCounter[N] = 0;
                }
            }

            //pckp
            if (player.input[0].pckp)
            {
                pckpCounter[N]++;
            }
            else
            {
                if (!player.input[1].pckp)
                {
                    pckpCounter[N] = 0;
                }
            }

            //thrw
            if (player.input[0].thrw)
            {
                thrwCounter[N]++;
            }
            else
            {
                if (!player.input[1].thrw)
                {
                    thrwCounter[N] = 0;
                }
            }

            //mp
            if (player.input[0].mp)
            {
                mpCounter[N]++;
            }
            else
            {
                if (!player.input[1].mp)
                {
                    mpCounter[N] = 0;
                }
            }

            //spec
            if (player.input[0].spec)
            {
                specCounter[N]++;
            }
            else
            {
                if (!player.input[1].spec)
                {
                    specCounter[N] = 0;
                }
            }

            //y
            if (player.input[0].y > 0)
            {
                yHCounter[N]++;
            }
            else
            {
                if (!(player.input[1].y > 0))
                {
                    yHCounter[N] = 0;
                }
            }

            if (player.input[0].y == 0)
            {
                yNCounter[N]++;
            }
            else
            {
                if (!(player.input[1].y == 0))
                {
                    yNCounter[N] = 0;
                }
            }

            if (player.input[0].y < 0)
            {
                yLCounter[N]++;
            }
            else
            {
                if (!(player.input[1].y < 0))
                {
                    yLCounter[N] = 0;
                }
            }

            //x
            if (player.input[0].x > 0)
            {
                xHCounter[N]++;
            }
            else
            {
                if (!(player.input[1].x > 0))
                {
                    xHCounter[N] = 0;
                }
            }

            if (player.input[0].x == 0)
            {
                xNCounter[N]++;
            }
            else
            {
                if (!(player.input[1].x == 0))
                {
                    xNCounter[N] = 0;
                }
            }

            if (player.input[0].x < 0)
            {
                xLCounter[N]++;
            }
            else
            {
                if (!(player.input[1].x < 0))
                {
                    xLCounter[N] = 0;
                }
            }





        }


    }
}
