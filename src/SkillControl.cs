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
using System.Threading;
using Expedition;


namespace MySlugcat
{
	// SkillControl 能力控制中心
	public class SC
	{
		private static bool StartRunning = true;

		public static bool  AllPlayerSkill = false;

		public static List<string> ownedPassages = new List<string>(); // 已拥有的通行证

		public static float pixelSize = 15f;          // 像素大小
		public static float Alpha = 0.9f;             // 像素不透明度

		public static int  MySlugcatStats = 0;        // 蛞蝓猫数据
		public static bool Exhausted = true;          // 精疲力竭

		public static bool Frame​​Skill = false;        // 嫁祸能力
		public static bool Deflagration​​Skill = false; // 爆燃能力
		public static bool KnitmeshSkill = false;     // 缠绕能力
		public static bool PerceptionSkill = false;   // 感知能力
		public static bool DigestionSkill = false;    // 暴食能力
		public static bool FixedSkill = false;        // 定身能力

		public static void Hook()
		{
			On.RainWorldGame.Update += RainWorldGame_Update;
			On.Player.ctor += Player_ctor;
            On.Player.Update += Player_Update;
		}

		public static void RainWorldGame_Update(On.RainWorldGame.orig_Update orig, RainWorldGame rainWorldGame)
		{
			orig(rainWorldGame);

			if (StartRunning)
			{
				if (Options.pixelSize != null && Options.pixelSize.Value != null)
				{
					pixelSize = Options.pixelSize.Value;
				}

                AllPlayerSkill = false;
                MySlugcatStats = 0;
                Exhausted = false;
                Frame​​Skill = false;
                Deflagration​​Skill = false;
                KnitmeshSkill = false;
                PerceptionSkill = false;
                DigestionSkill = false;
                FixedSkill = false;

                StartRunning = false;
			}
		}

		private static void Player_ctor(On.Player.orig_ctor orig, Player player, AbstractCreature abstractCreature, World world)
		{
			orig.Invoke(player, abstractCreature, world);

			if (Options.pixelSize != null && Options.pixelSize.Value != null)
			{
				pixelSize = Options.pixelSize.Value;
			}

			SetSkill(player);
        }


        private static int frameCounter = 0; // 帧计数器
		private const int N = 12000; // 每N帧执行一次（可调整）

		private static void Player_Update(On.Player.orig_Update orig, Player player, bool eu)
		{
			orig.Invoke(player, eu);


			// 每N帧执行一次自定义逻辑
			if (++frameCounter >= N)
			{
				frameCounter = 0;
                SetSkill(player);
            }


			//player.room.game.GetStorySession.saveState.deathPersistentSaveData.winState
			//player.SessionRecord.
		}

		public static void SetSkill(Player player)
		{
            MySlugcatStats = 0;
            Exhausted = true;

            Frame​​Skill = false;
            Deflagration​​Skill = false;
            KnitmeshSkill = false;
            PerceptionSkill = false;
            DigestionSkill = false;
            FixedSkill = false;

            if (player.slugcatStats.name == Plugin.YourSlugID || SC.AllPlayerSkill)
            {
                WinState winState = player.room.game.GetStorySession.saveState.deathPersistentSaveData.winState;
                ownedPassages = new List<string>();
                bool Survivor = false;

                if (winState != null && winState.endgameTrackers.Count > 0)
                {
                    for (int i = 0; i < winState.endgameTrackers.Count; i++)
                    {
                        if (winState.endgameTrackers[i].GoalFullfilled)
                        {
                            ownedPassages.Add(WinState.PassageDisplayName(winState.endgameTrackers[i].ID));
                            if (ownedPassages[i] == "The Survivor")
                            {
                                PerceptionSkill = true;
                                Survivor = true;
                            }
                        }
                    }
                }

                if (Survivor && ownedPassages != null && ownedPassages.Count > 0)
                {
                    for (int i = 0; i < ownedPassages.Count; i++)
                    {
                        if (ownedPassages[i] == "The Outlaw")//"暴徒"
                        {
							Deflagration​​Skill = true;
                        }

                    }
                }



                //player.room.game.GetStorySession.saveState.deathPersistentSaveData.winState
                //player.SessionRecord.
            }
        }


		//"The Survivor"        //"求生者"
		//"The Hunter"          //"猎手"
		//"The Saint"           //"圣徒"
		//"The Wanderer"        //"漫游者"
		//"The Chieftain"       //"酋长"
		//"The Monk"            //"僧侣"
		//"The Outlaw"          //"暴徒"
		//"The Dragon Slayer"   //"屠龙者"
		//"The Scholar"         //"学者"
		//"The Friend"          //"朋友"
		// ModManager.MSC
		//"The Nomad"           //"流浪者"
		//"The Martyr"          //"殉道者"
		//"The Pilgrim"         //"朝圣者"
		//"The Mother"          //"慈母"
		//


	}
}

