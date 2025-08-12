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
		On.PlayerGraphics.InitiateSprites += PlayerGraphicsOnInitiateSprites;
	}

	private static void PlayerGraphicsOnInitiateSprites(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
	{
		orig(self, sLeaser, rCam);

		if (!rCam.room.game.DEBUGMODE)
		{
			for (int ghostSprite = 0; ghostSprite < 9; ghostSprite++)
			{
				sLeaser.sprites[ghostSprite].shader = rCam.game.rainWorld.Shaders["BallToy"];
				sLeaser.sprites[ghostSprite].alpha = 0.95f;
			}

		}
	}


	/// <summary>
	/// 是否为亡灵生物
	/// </summary>
	public static bool IsUndeadCreature(Creature creature)
	{
		bool isUndeadCreature = false;
		foreach (WeakReference<Creature> weakPlayerRef in PlayerModuleManager.UndeadCreatures)
		{
			Creature creature2;
			if (weakPlayerRef.TryGetTarget(out creature2))
			{
				if (creature2 != null && creature == creature2)
				{
					isUndeadCreature = true;
				}
			}
		}
		return isUndeadCreature;
	}

	/// <summary>
	/// 随机查找当前房间的非亡灵生物
	/// </summary>
	public static Creature? RandomlySelectedUnUndeadCreature(Room room, bool IncludePlayer, Creature creature, bool IncludeDeadCreature)
	{
		List<Creature> UnUnCreatures = new List<Creature>();

		if (!(room.abstractRoom.creatures.Count > 0))
		{
			return null;
		}

		// 遍历当前房间所有生物
		foreach (AbstractCreature abstractCreature in room.abstractRoom.creatures)
		{

			Creature c = abstractCreature.realizedCreature;
			// 排除检查：玩家、无效引用、自身、或没有身体部位的对象
			if (!IncludePlayer)
			{
				var player1 = c as Player;
				if (player1 != null)
				{
					continue; // 跳过无效项，继续检查下一个
				}
			}
			if (c == null ||             // 确保生物存在
				c == creature ||             // 排除自身
				c.mainBodyChunk == null) // 确保有有效的mainBodyChunk
			{
				continue; // 跳过无效项，继续检查下一个
			}
			if (Extension.DisabledCreature(c))// 禁用生物
			{
				continue; // 跳过无效项，继续检查下一个
			}
			if (c.dead == true && !IncludeDeadCreature)// 死亡的生物
			{
				continue; // 跳过无效项，继续检查下一个
			}
			if (IsUndeadCreature(c))
			{
				continue; // 跳过无效项，继续检查下一个
			}
			UnUnCreatures.Add(c);
		}

		if (UnUnCreatures.Count == 0)
		{
			Console.WriteLine("RandomlySelectedUnUndeadCreature: No UnUndeadCreatures found");
			return null;
		}
		return UnUnCreatures[UnityEngine.Random.Range(0, UnUnCreatures.Count)];
	}

	private static void Creature_Update(On.Creature.orig_Update orig, Creature creature, bool eu)
	{
		orig.Invoke(creature, eu);

		foreach (WeakReference<Creature> weakPlayerRef in PlayerModuleManager.UndeadCreatures)
		{
			Creature creature1;
			if (weakPlayerRef.TryGetTarget(out creature1))
			{
				if (creature1 != null && creature1 == creature)
				{

					if (creature is Lizard || creature is Scavenger)
					{
						try
						{
							Health.ReviveCreature(creature);
							Tame.TameCreature(creature.room.game, creature);
							PlayerModuleManager.UndeadCreatures.Add(new WeakReference<Creature>(creature));

							Creature? target = RandomlySelectedUnUndeadCreature(creature.room, false, creature, false);
							AbstractCreature? targetAb = target?.abstractCreature;

							if (creature is Lizard lizard)
							{
								lizard.spawnDataEvil = -1f;
								// 蜥蜴专属好感设置
								if (lizard.AI is LizardAI lizardAI)
								{
                                    if (targetAb != null && target != null)
									{
                                        lizard.AI.tracker.SeeCreature(targetAb); // 强制让蜥蜴“看到”目标
                                        Tracker.CreatureRepresentation targetRep = lizardAI.tracker.RepresentationForCreature(targetAb, false);
                                        if (targetRep != null)
                                        {
                                            // 强制设置攻击目标
                                            lizardAI.focusCreature = targetRep;

                                            // 3. 直接修改动态关系
                                            targetRep.dynamicRelationship.currentRelationship = new CreatureTemplate.Relationship(
                                                CreatureTemplate.Relationship.Type.Eats, // 设为捕食
                                                1f                                       // 强度最大
                                            );

                                            // 触发攻击行为（可选）
                                            lizardAI.behavior = LizardAI.Behavior.Hunt; // 切换为狩猎模式
                                            lizardAI.AggressiveBehavior(targetRep, 1f); // 直接调用攻击逻辑
                                        }
                                    }

                                }
							}
							if (creature is Scavenger scavenger)
							{
								// 拾荒者专属好感设置
								if (scavenger.AI is ScavengerAI scavAI)
								{
									if (targetAb != null && target != null)
									{
										// 获取关系状态
										Tracker.CreatureRepresentation targetRep = scavAI.tracker.RepresentationForObject(target, false);
										if (targetRep != null && targetRep.dynamicRelationship != null)
										{
											// 设置攻击关系
											targetRep.dynamicRelationship.currentRelationship.type = CreatureTemplate.Relationship.Type.Attacks;
											targetRep.dynamicRelationship.currentRelationship.intensity = 1f;

											// 设置暴力类型为致命攻击
											var trackState = targetRep.dynamicRelationship.state as ScavengerAI.ScavengerTrackState;
											if (trackState != null)
											{
												trackState.taggedViolenceType = ScavengerAI.ViolenceType.Lethal;
											}

											// 强制进入攻击行为
											scavAI.behavior = ScavengerAI.Behavior.Attack;
											scavAI.focusCreature = targetRep;

                                            // 触发攻击行为（可选）
                                            if (scavAI.CheckHandsForSpear())
											{
												scavenger.TryThrow(target.bodyChunks[0], ScavengerAI.ViolenceType.Lethal);
											}
										}

									}

								}
							}

							creature.Template.shortcutColor = new Color(84f / 255f, 84f / 255f, 84f / 255f, 1f);
						}
						catch (Exception e)
						{
							Debug.LogException(e);
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
			if (!player.dead)
			{
				Room room = player.room;
				for (int i = room.abstractRoom.creatures.Count - 1;  i >= 0; i--)
				{
					Creature creature = room.abstractRoom.creatures[i].realizedCreature;
					if (creature != null)
					{
						if (creature.dead)
						{
							if (creature is Lizard || creature is Scavenger)
							{
								try
								{
									Health.ReviveCreature(creature);
									Tame.TameCreature(player.room.game, creature);
									PlayerModuleManager.UndeadCreatures.Add(new WeakReference<Creature>(creature));
									room.PlaySound(SoundID.Slugcat_Pick_Up_Spear, creature.mainBodyChunk);

									if (creature is Lizard lizard)
									{
										lizard.spawnDataEvil = -1f;
										// 蜥蜴专属好感设置
										if (lizard.AI is LizardAI lizardAI)
										{
											//lizardAI.aggression = 0f;  // 清零攻击性
											lizardAI.friendTracker.friend = player; // 设为永久好友
											//lizardAI.riskiness = 0.2f;  // 降低冒险倾向
										}
									}
									if (creature is Scavenger scavenger)
									{
										// 拾荒者专属好感设置
										if (scavenger.AI is ScavengerAI scavAI)
										{

										}
									}

									creature.Template.shortcutColor = new Color(84f / 255f, 84f / 255f, 84f / 255f, 1f);
								}
								catch (Exception e)
								{
									Debug.LogException(e);
								}
							}


						}
					}
				}
			}
		}


	}
}
