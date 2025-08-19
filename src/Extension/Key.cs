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
        /*//jump 跳跃键
        public static int[] JmpCounter = Enumerable.Repeat(0, 100).ToArray();
        //pckp 拾取键
        public static int[] pckpCounter = Enumerable.Repeat(0, 100).ToArray();
        //throw 投掷键
        public static int[] thrwCounter = Enumerable.Repeat(0, 100).ToArray();
        //map 地图键
        public static int[] mpCounter = Enumerable.Repeat(0, 100).ToArray();
        //special 特殊键
        public static int[] specCounter = Enumerable.Repeat(0, 100).ToArray();
        //y Y键  输入为正（上）、零（无输入）、负（下）
        public static int[] yHCounter = Enumerable.Repeat(0, 100).ToArray();
        public static int[] yNCounter = Enumerable.Repeat(0, 100).ToArray();
        public static int[] yLCounter = Enumerable.Repeat(0, 100).ToArray();
        //x X键  输入为正（右）、零（无输入）、负（左）
        public static int[] xHCounter = Enumerable.Repeat(0, 100).ToArray();
        public static int[] xNCounter = Enumerable.Repeat(0, 100).ToArray();
        public static int[] xLCounter = Enumerable.Repeat(0, 100).ToArray();*/


		/*public static void Hook()
        {
            On.Player.Update += Player_Update;
        }*/

		public static void Player_Update(ref bool Execute, ref On.Player.orig_Update orig, ref Player player, ref bool eu)
        {
            if (player.GetModule(out var module))
            {
				//jmp
				if (player.input[0].jmp)
				{
					module.JmpCounter++;
				}
				else
				{
					if (!player.input[1].jmp)
					{
						module.JmpCounter = 0;
					}
				}

				//pckp
				if (player.input[0].pckp)
				{
					module.pckpCounter++;
				}
				else
				{
					if (!player.input[1].pckp)
					{
						module.pckpCounter = 0;
					}
				}

				//thrw
				if (player.input[0].thrw)
				{
					module.thrwCounter++;
				}
				else
				{
					if (!player.input[1].thrw)
					{
						module.thrwCounter = 0;
					}
				}

				//mp
				if (player.input[0].mp)
				{
					module.mpCounter++;
				}
				else
				{
					if (!player.input[1].mp)
					{
						module.mpCounter = 0;
					}
				}

				//spec
				if (player.input[0].spec)
				{
					module.specCounter++;
				}
				else
				{
					if (!player.input[1].spec)
					{
						module.specCounter = 0;
					}
				}

				//y
				if (player.input[0].y > 0)
				{
					module.yHCounter++;
				}
				else
				{
					if (!(player.input[1].y > 0))
					{
						module.yHCounter = 0;
					}
				}

				if (player.input[0].y == 0)
				{
					module.yNCounter++;
				}
				else
				{
					if (!(player.input[1].y == 0))
					{
						module.yNCounter = 0;
					}
				}

				if (player.input[0].y < 0)
				{
					module.yLCounter++;
				}
				else
				{
					if (!(player.input[1].y < 0))
					{
						module.yLCounter = 0;
					}
				}

				//x
				if (player.input[0].x > 0)
				{
					module.xHCounter++;
				}
				else
				{
					if (!(player.input[1].x > 0))
					{
						module.xHCounter = 0;
					}
				}

				if (player.input[0].x == 0)
				{
					module.xNCounter++;
				}
				else
				{
					if (!(player.input[1].x == 0))
					{
						module.xNCounter = 0;
					}
				}

				if (player.input[0].x < 0)
				{
					module.xLCounter++;
				}
				else
				{
					if (!(player.input[1].x < 0))
					{
						module.xLCounter = 0;
					}
				}
			}

            //int N = player.playerState.playerNumber;

            /*//jmp
            if (player.input[0].jmp)
            {
                JmpCounter++;
            }
            else
            {
                if (!player.input[1].jmp)
                {
                    JmpCounter = 0;
                }
            }

            //pckp
            if (player.input[0].pckp)
            {
                pckpCounter++;
            }
            else
            {
                if (!player.input[1].pckp)
                {
                    pckpCounter = 0;
                }
            }

            //thrw
            if (player.input[0].thrw)
            {
                thrwCounter++;
            }
            else
            {
                if (!player.input[1].thrw)
                {
                    thrwCounter = 0;
                }
            }

            //mp
            if (player.input[0].mp)
            {
                mpCounter++;
            }
            else
            {
                if (!player.input[1].mp)
                {
                    mpCounter = 0;
                }
            }

            //spec
            if (player.input[0].spec)
            {
                specCounter++;
            }
            else
            {
                if (!player.input[1].spec)
                {
                    specCounter = 0;
                }
            }

            //y
            if (player.input[0].y > 0)
            {
                yHCounter++;
            }
            else
            {
                if (!(player.input[1].y > 0))
                {
                    yHCounter = 0;
                }
            }

            if (player.input[0].y == 0)
            {
                yNCounter++;
            }
            else
            {
                if (!(player.input[1].y == 0))
                {
                    yNCounter = 0;
                }
            }

            if (player.input[0].y < 0)
            {
                yLCounter++;
            }
            else
            {
                if (!(player.input[1].y < 0))
                {
                    yLCounter = 0;
                }
            }

            //x
            if (player.input[0].x > 0)
            {
                xHCounter++;
            }
            else
            {
                if (!(player.input[1].x > 0))
                {
                    xHCounter = 0;
                }
            }

            if (player.input[0].x == 0)
            {
                xNCounter++;
            }
            else
            {
                if (!(player.input[1].x == 0))
                {
                    xNCounter = 0;
                }
            }

            if (player.input[0].x < 0)
            {
                xLCounter++;
            }
            else
            {
                if (!(player.input[1].x < 0))
                {
                    xLCounter = 0;
                }
            }*/





        }
    }
}

