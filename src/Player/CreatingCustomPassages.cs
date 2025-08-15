using On;
using IL;
using System;
using BepInEx;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using MoreSlugcats;
using System.Linq;
using Noise;
using System.Globalization;
using System.Threading;
using RWCustom;                       // WinState / EndgameTracker
using UnityEngine;                   // Mathf 等
using CustomPassageSupport;          // Passage / IPassage
//using CustomPassageSupport.Trackers; // IntegerTracker
using static CustomPassageSupport.CustomPassageSupport;
using static On.WinState;


namespace MySlugcat
{
	public class MyPassage : Passage, IPassage
	{
		//public static readonly string ID = "theaccommodator.help_lizards";
		//public static readonly string Display = "The Dragonlord";

		public MyPassage() : base(idName: "thedragonlord.ThanksToVanguard", displayName: "The Dragonlord")
		{
			/// <summary> 表示在玩家达到"求生者"通行证之前，你的通行证是否可以被跟踪。（默认为 true） </summary>
			RequiresSurvivor = true;
			/// <summary> 即使开启"Survivor Not Required"混音选项，仍强制需要"求生者"（默认 false） </summary>
			IgnoresSurvivorToggle = false;
			/// <summary> 远征分数（默认 20） </summary>
			ScoreValue = 45;
			/// <summary> 需要战斗（默认 false） </summary>
			CombatRequired = false;
			/// <summary> 饥饿进度重置（默认 true） </summary>
			StarveProgressReset = true;
		}

		public WinState.EndgameTracker initTracker()
		{
			return new WinState.FloatTracker(
				this.PassageID,   // ID
				dflt: 0f,          // 默认值
				min: 0f,           // 最小值
				showFrom: 0f,      // 开始显示的进度
				max: 1f           // 最大值
			);

			/*return new WinState.IntegerTracker(
				this.PassageID,   // ID
				dflt: 0,          // 默认值
				min: 0,           // 最小值
				showFrom: 1,      // 开始显示的进度
				max: 10           // 最大值
			);*/
		}

		// 成功休眠时调用（更新进度）
		/// <summary> 玩家雨眠 </summary>
		public void onPlayerSuccess(WinState winState, RainWorldGame game, WinState.EndgameTracker tracker)
		{
			CreatureCommunities.CommunityID communityID = new CreatureCommunities.CommunityID(ExtEnum<CreatureCommunities.CommunityID>.values.GetEntry(3), false);
			float like = game.session.creatureCommunities.LikeOfPlayer(communityID, game.world.RegionNumber, 0);

			if (like >= 0.1)
			{
				if (tracker is WinState.FloatTracker ftracker)
				{
					ftracker.SetProgress(Math.Min(1f, like));
				}
			}

			if (tracker is WinState.FloatTracker ftracker1)//
			{
				ftracker1.SetProgress(ftracker1.progress + 0.5f);
			}
			//(tracker as WinState.FloatTracker)?.SetProgress(5);
		}

		/// <summary> 玩家死亡 </summary>
		public void onPlayerFail(WinState.EndgameTracker _) { }

		/// <summary> 玩家退出 </summary>
		public void onPlayerExit(WinState.EndgameTracker _) { }

		/// <summary> 饥饿周期处理 玩家挨饿 </summary>
		public void onPlayerStarve(WinState.EndgameTracker _) { }

		/// <summary> 玩家重生 </summary>
		//public void onPlayerRespawn(WinState.EndgameTracker _) { }

		//public void onPlayerWin(WinState winState, RainWorldGame game, WinState.EndgameTracker tracker) { }

		//public void onPlayerRevive(WinState.EndgameTracker _) { }

		//public void onPlayerRevived(WinState.EndgameTracker _) { }


	}
}