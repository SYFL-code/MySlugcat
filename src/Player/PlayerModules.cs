using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using UnityEngine;
using MySlugcat;


namespace MySlugcat;
internal static class PlayerModuleManager
{
	//public static ConditionalWeakTable<SlugcatStats, Player> slugcatStatsPlayers = new ConditionalWeakTable<SlugcatStats, Player>();
	public static ConditionalWeakTable<Player, PlayerModule> playerModules = new ConditionalWeakTable<Player, PlayerModule>();
	public static List<WeakReference<Player>> players = new List<WeakReference<Player>>();
	public static List<WeakReference<Creature>> UndeadCreatures = new List<WeakReference<Creature>>();

	internal class PlayerModule
	{
		WeakReference<Player> playerRef;

		public List<string> Passages = new List<string>(); // 已拥有的通行证

		public int  MySlugcatStats = 0;        // 蛞蝓猫数据
		public bool Exhausted = true;          // 精疲力竭

		public bool Frame​​Skill = false;        // 嫁祸能力
		public bool Deflagration​​Skill = false; // 爆燃能力
		public bool KnitmeshSkill = false;     // 缠绕能力
		public bool PerceptionSkill = false;   // 感知能力
		public bool DigestionSkill = false;    // 暴食能力
		public bool FixedSkill = false;        // 定身能力

		public int HungryCoolDown = 12000;//冷却计时器


        public PlayerModule(Player player)
		{
			playerRef = new WeakReference<Player>(player);
			SetSkill(player);
			Console.WriteLine($"{DigestionSkill}");
		}

		public void SetSkill(Player player)
		{
			MySlugcatStats = 0;
			Exhausted = false;

			Frame​​Skill = false;
			Deflagration​​Skill = false;
			KnitmeshSkill = false;
			PerceptionSkill = false;
			DigestionSkill = false;
			FixedSkill = false;

			if (player.slugcatStats.name == Plugin.YourSlugID || Control.AllPlayerSkill)
			{
				MySlugcatStats = -1;
				Exhausted = true;

				Frame​​Skill = false;
				Deflagration​​Skill = false;
				KnitmeshSkill = false;
				PerceptionSkill = false;
				DigestionSkill = true;//False
				FixedSkill = false;

				WinState winState = player.room.game.GetStorySession.saveState.deathPersistentSaveData.winState;
				Passages = new List<string>();
				bool Survivor = false;

				if (winState != null && winState.endgameTrackers.Count > 0)
				{
					for (int i = 0; i < winState.endgameTrackers.Count; i++)
					{
						if (winState.endgameTrackers[i].GoalFullfilled)
						{
							Passages.Add(WinState.PassageDisplayName(winState.endgameTrackers[i].ID));
							if (Passages[i] == "The Survivor")
							{
								PerceptionSkill = true;
								Survivor = true;
							}
						}
					}
				}

				if (Survivor && Passages != null && Passages.Count > 0)
				{
					for (int i = 0; i < Passages.Count; i++)
					{
						if (Passages[i] == "The Outlaw")//"暴徒"
						{
							Deflagration​​Skill = true;
						}

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
		On.Player.Update += Player_Update;
		On.Creature.Update += Creature_Update;
	}

	private static void Creature_Update(On.Creature.orig_Update orig, Creature creature, bool eu)
	{
		orig.Invoke(creature, eu);

		if (creature.Template.shortcutColor == new Color(84f / 255f, 84f / 255f, 84f / 255f, 1f) && creature.room != null)
		{
			foreach (AbstractCreature ac in creature.room.world.game.Players)
			{
				if (ac.realizedCreature is Player player)
				{
					creature.abstractCreature.world.game.session.creatureCommunities.SetLikeOfPlayer
						(creature.abstractCreature.creatureTemplate.communityID, creature.abstractCreature.world.RegionNumber, player.playerState.playerNumber, 1.0f);
				}
			}
		}

        if (creature is Lizard lizard)
        {
            lizard.spawnDataEvil = 0.8f;
        }

        foreach (WeakReference<Creature> weakPlayerRef in PlayerModuleManager.UndeadCreatures)
		{
			Creature creature1;
			if (weakPlayerRef.TryGetTarget(out creature1))
			{
				if (creature1 != null && creature1 == creature)
				{
					creature.Template.shortcutColor = new Color(84f / 255f, 84f / 255f, 84f / 255f, 1f);

					foreach (AbstractCreature ac in creature.room.world.game.Players)
					{
						if (ac.realizedCreature is Player player)
						{
							creature.abstractCreature.world.game.session.creatureCommunities.SetLikeOfPlayer
								(creature.abstractCreature.creatureTemplate.communityID, creature.abstractCreature.world.RegionNumber, player.playerState.playerNumber, 1.0f);

                            creature.abstractCreature.ChangeOverlapColor(1f);
                            CreatureGhost
                        }
					}

					break;
				}
			}
		}

	}



    private static void Player_ctor(On.Player.orig_ctor orig, Player player, AbstractCreature abstractCreature, World world)
	{
		orig.Invoke(player, abstractCreature, world);

		PlayerModuleManager.players.Add(new WeakReference<Player>(player));
		PlayerModuleManager.playerModules.Add(player, new PlayerModuleManager.PlayerModule(player));
	}

	private static void Player_Update(On.Player.orig_Update orig, Player player, bool eu)
	{
		orig.Invoke(player, eu);

		//creature.Template.shortcutColor = new Color(0.2f, 0.2f, 0.2f, 1f);

		int N = player.playerState.playerNumber;
		if (Key.JmpCounter[N] >= 60 && !player.input[0].jmp && player.input[1].jmp)
		{
			Console.WriteLine($"Player 1");

			if (!player.dead)
			{
				Console.WriteLine($"Player 2");
				Room room = player.room;
				for (int i = room.abstractRoom.creatures.Count - 1;  i >= 0; i--)
				{
					Console.WriteLine($"Player 3");
					Creature creature = room.abstractRoom.creatures[i].realizedCreature;
					if (creature != null)
					{
						Console.WriteLine($"Player 4");
						if (creature.dead)
						{
							Console.WriteLine($"Player 5");
							Health.ReviveCreature(creature);

							creature.abstractCreature.world.game.session.creatureCommunities.SetLikeOfPlayer
								(creature.abstractCreature.creatureTemplate.communityID, creature.abstractCreature.world.RegionNumber, player.playerState.playerNumber, 1.0f);

							creature.Template.shortcutColor = new Color(84f/255f, 84f/255f, 84f/255f, 1f);
							PlayerModuleManager.UndeadCreatures.Add(new WeakReference<Creature>(creature));
							Console.WriteLine($"Player 6");
						}
					}
				}
				Console.WriteLine($"Player 7");
			}
			Console.WriteLine($"Player 8");
		}

		/*ArtificialIntelligence self = creature.abstractCreature.abstractAI.RealAI;

		Player player;
		foreach (AbstractCreature ac in self.creature.world.game.Players)
		{
			if (ac.realizedCreature is Player && player.slugcatStats.name == Plugin.YourSlugID)
			{
				player = (Player)ac.realizedCreature;
				if (self is LizardAI ai &&
					ai.lizard.Template.type == CreatureTemplate.Type.CyanLizard &&
					player.room == self.creature.Room.realizedRoom)
				{
					Lizard cyanLizard = ai.lizard;
					cyanLizard.abstractCreature.world.game.session.creatureCommunities.
						SetLikeOfPlayer(cyanLizard.abstractCreature.creatureTemplate.communityID,
						cyanLizard.abstractCreature.world.RegionNumber,
						(player.State as PlayerState).playerNumber,
						-1.0f);
					//self.tracker.SeeCreature(player.abstractCreature);
				}
			}
		}

		creature.abstractCreature.world.game.session.creatureCommunities.SetLikeOfPlayer
			(creature.abstractCreature.creatureTemplate.communityID, creature.abstractCreature.world.RegionNumber, (player.State as PlayerState).playerNumber, 1.0f);*/

		/*if (PlayerModuleManager.playerModules.TryGetValue(self, out var module))
		{
			module.Hungry_Update(self);
		}*/
	}
}
