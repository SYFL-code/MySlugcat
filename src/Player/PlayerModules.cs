using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using UnityEngine;
using MoreSlugcats;
using RWCustom;
using System.Linq;
using Noise;
using System.Globalization;
using Watcher;


namespace MySlugcat;
internal static class PlayerModuleManager
{
	public static List<WeakReference<Player>> players = new List<WeakReference<Player>>();
	public static ConditionalWeakTable<Player, PlayerModule> playerModules = new ConditionalWeakTable<Player, PlayerModule>();


	internal class PlayerModule
	{
		WeakReference<Player> playerRef;

		public List<string> Passages = new List<string>(); // 已拥有的通行证

		public int  MySlugcatStats = 0;        // 蛞蝓猫数据
		public bool Exhausted = false;         // 精疲力竭
		public bool VisionSystem = false;      // 视觉系统

		public bool FrameSkill = false;        // 嫁祸能力
		public bool DeflagrationSkill = false; // 爆燃能力
		public bool KnitmeshSkill = false;     // 缠绕能力
		public bool PerceptionSkill = false;   // 感知能力
		public bool DigestionSkill = false;    // 暴食能力
		public bool SpawnNecrophytes = false;  // 死灵能力 死灵法师
		public bool FixedSkill = false;        // 定身能力

		//public int HungryCoolDown = 12000;//冷却计时器


        public PlayerModule(Player player)
		{
			playerRef = new WeakReference<Player>(player);
			Console.WriteLine($"{Exhausted}_1");
			SetSkill(player);
			Console.WriteLine($"{Exhausted}_2");
			Console.WriteLine($"{player.slugcatStats.name == Plugin.YourSlugID}_2{Control.AllPlayerSkill}_{player.slugcatStats.name}");
		}

		public void SetSkill(Player player)
		{
			MySlugcatStats = 0;
			Exhausted = false;
			VisionSystem = false;

			FrameSkill = false;
			DeflagrationSkill = false;
			KnitmeshSkill = false;
			PerceptionSkill = false;
			DigestionSkill = false;
			SpawnNecrophytes = false;
			FixedSkill = false;

			if (player.slugcatStats.name == Plugin.YourSlugID || Control.AllPlayerSkill)
			{
				MySlugcatStats = -1;
				Exhausted = true;
				VisionSystem = true;

				FrameSkill = false;
				DeflagrationSkill = false;
				KnitmeshSkill = false;
				PerceptionSkill = false;
				DigestionSkill = true;//
				SpawnNecrophytes = true;//
				FixedSkill = false;

				//WinState winState = player.room.game.GetStorySession.saveState.deathPersistentSaveData.winState;
				var session = player?.room?.game?.GetStorySession;
				var dpsd = session?.saveState?.deathPersistentSaveData;
				var winState = dpsd?.winState;
				if (winState == null) return;

				Passages.Clear();
				bool Survivor = false;

				if (winState != null && winState.endgameTrackers.Count > 0)
				{
					for (int i = 0; i < winState.endgameTrackers.Count; i++)
					{
						if (winState.endgameTrackers[i].GoalFullfilled)
						{
							string name = WinState.PassageDisplayName(winState.endgameTrackers[i].ID);
							Passages.Add(name);
							if (name == "The Survivor")//"求生者"
							{
								PerceptionSkill = true;
								Survivor = true;
							}
						}
					}
				}

				if (Survivor && Passages != null && Passages.Count > 0)
				{
					if (Passages.Contains("The Outlaw"))//"暴徒"
					{
						DeflagrationSkill = true;
					}
					if (Passages.Contains("The Chieftain") && Passages.Contains("The Friend"))//"酋长"&"朋友"
					{
						SpawnNecrophytes = true;
					}
				}



				//player.room.game.GetStorySession.saveState.deathPersistentSaveData.winState
				//player.SessionRecord.
			}
		}


		/*public void Hungry_Update(Player player)
		{
			if ((player.slugcatStats.name == Plugin.YourSlugID || SC.AllPlayerSkill) && (SC.MySlugcatStats == 0 && SC.Exhausted))
			{
				if (player.FoodInStomach > 0 || player.playerState.quarterFoodPoints > 0)
				{
					if (HungryCoolDown > 0)
					{
						HungryCoolDown--;

					}
					else
					{
						HungryCoolDown = 12000;
						MyPlayer.SubtractQuarterFood(1, player);

					}
				}
			}
		}*/

	}
}

internal static class PlayerHooks
{

	public static void HookOn()
	{
		On.Player.ctor += Player_ctor;
		On.Player.Destroy += Player_Destroy;
	}

	private static void Player_ctor(On.Player.orig_ctor orig, Player player, AbstractCreature abstractCreature, World world)
	{
		orig.Invoke(player, abstractCreature, world);

		PlayerModuleManager.players.Add(new WeakReference<Player>(player));
		PlayerModuleManager.playerModules.Add(player, new PlayerModuleManager.PlayerModule(player));
	}

	private static void Player_Destroy(On.Player.orig_Destroy orig, Player self)
	{
		orig(self);
		if (self.dead || self.slatedForDeletetion)
		{
			PlayerModuleManager.players.RemoveAll(r => !r.TryGetTarget(out var p) || p == self);
			PlayerModuleManager.playerModules.Remove(self);
		}

	}


}
