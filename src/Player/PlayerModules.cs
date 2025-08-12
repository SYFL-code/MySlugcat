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
		On.LizardGraphics.DrawSprites += LizardGraphics_DrawSprites;
		On.ScavengerGraphics.DrawSprites += ScavengerGraphics_DrawSprites;
		//On.PlayerGraphics.InitiateSprites += PlayerGraphicsOnInitiateSprites;
	}

	private static void LizardGraphics_DrawSprites(On.LizardGraphics.orig_DrawSprites orig, LizardGraphics lizardGraphics, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
	{
		orig(lizardGraphics, sLeaser, rCam, timeStacker, camPos);

		if (!rCam.room.game.DEBUGMODE)
		{
			if (PlayerModuleManager.UndeadCreatures.Count > 0)
			{
				foreach (WeakReference<Creature> weakCreatureRef in PlayerModuleManager.UndeadCreatures)
				{
					Creature creature1;
					if (weakCreatureRef.TryGetTarget(out creature1))
					{
						Creature creature = lizardGraphics.lizard;
						if (creature1 != null && creature1 == creature)
						{

							if (creature is Lizard || creature is Scavenger)
							{
								try
								{
									for (int i = 0; i < sLeaser.sprites.Length; i++)
									{
										sLeaser.sprites[i].shader = rCam.game.rainWorld.Shaders["GhostDistortion"];
										sLeaser.sprites[i].alpha = 0.8f;
										//sLeaser.sprites[i].shader = rCam.game.rainWorld.Shaders["GhostSkin"];
										//sLeaser.sprites[i].alpha = 0.95f;
									}
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

	private static void ScavengerGraphics_DrawSprites(On.ScavengerGraphics.orig_DrawSprites orig, ScavengerGraphics scavengerGraphics, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
	{
		orig(scavengerGraphics, sLeaser, rCam, timeStacker, camPos);

		if (!rCam.room.game.DEBUGMODE)
		{
			if (PlayerModuleManager.UndeadCreatures.Count > 0)
			{
				foreach (WeakReference<Creature> weakCreatureRef in PlayerModuleManager.UndeadCreatures)
				{
					Creature creature1;
					if (weakCreatureRef.TryGetTarget(out creature1))
					{
						Creature creature = scavengerGraphics.scavenger;
						if (creature1 != null && creature1 == creature)
						{

							if (creature is Lizard || creature is Scavenger)
							{
								try
								{
									FContainer container = new FContainer();
									//container.shader
									for (int i = 0; i < sLeaser.sprites.Length; i++)
									{
										//sLeaser.sprites[i].shader = rCam.game.rainWorld.Shaders["GhostSkin"];
										sLeaser.sprites[i].shader = rCam.game.rainWorld.Shaders["GhostSkin"];
										sLeaser.sprites[i].alpha = 0.8f;
									}
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
	/// 查找当前房间中距离自身最近的非亡灵生物
	/// </summary>
	public static Creature? RandomlySelectedUnUndeadCreature(Vector2 selfPos, Room room, bool IncludePlayer, Creature? creature, bool IncludeDeadCreature, int select)
	{
		Creature? nearest = null;        // 最近非亡灵生物对象
		float minSqrDistance = float.MaxValue;  // 最小平方距离（初始设为最大浮点数）
		//List<Creature> UnUnCreatures = new List<Creature>();

		if (!(room.abstractRoom.creatures.Count > 0))
		{
			return null;
		}
		if (selfPos == null || room == null || room.abstractRoom == null || room.abstractRoom.creatures == null || room.abstractRoom.creatures.Count == null || !(room.abstractRoom.creatures.Count > 0))
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
			if (Extension.DisabledCreature(c) && select == 1)// 禁用生物
			{
				continue; // 跳过无效项，继续检查下一个
			}
			if (Extension.HarmlessCreature(c) && select == 2)// 无害生物
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
			// 计算位置差（目标位置 - 自身位置）
			Vector2 offset = c.mainBodyChunk.pos - selfPos;
			// 计算平方距离（比Vector2.Distance更高效）
			float sqrDistance = offset.sqrMagnitude;

			// 检查是否为更近的生物
			if (sqrDistance < minSqrDistance)
			{
				// 更新最近生物和最小距离记录
				minSqrDistance = sqrDistance;
				nearest = c;
			}
		}

		if (nearest == null)
		{
			Console.WriteLine("RandomlySelectedUnUndeadCreature: No UnUndeadCreatures found");
			return null;
		}
		return nearest; // 返回最近生物（可能为null）
	}

	private static void Creature_Update(On.Creature.orig_Update orig, Creature creature, bool eu)
	{
		bool isUndeadCreature = false;
		if (PlayerModuleManager.UndeadCreatures.Count > 0)
		{
			foreach (WeakReference<Creature> weakCreatureRef in PlayerModuleManager.UndeadCreatures)
			{
				Creature creature1;
				if (weakCreatureRef.TryGetTarget(out creature1))
				{
					if (creature1 != null && creature1 == creature)
					{

						if (creature is Lizard || creature is Scavenger)
						{ 
							isUndeadCreature = true;
						}

						break;
					}
				}
			}
		}


		/*if (isUndeadCreature)
		{
			if (creature is Lizard lizard && lizard.AI is LizardAI lizardAI && lizardAI.behavior == LizardAI.Behavior.FollowFriend)
			{
				
			}
		}*/
		orig.Invoke(creature, eu);


		if (isUndeadCreature)
		{
			try
			{
				if (creature.room.world.rainCycle.TimeUntilRain < 60)
				{
					Health.KillCreature(creature.room.game, creature);
					return;
				}

				//Health.ReviveCreature(creature);
				Tame.TameCreature(creature.room.game, creature);
				//PlayerModuleManager.UndeadCreatures.Add(new WeakReference<Creature>(creature));

				Creature? target = RandomlySelectedUnUndeadCreature(creature.firstChunk.pos, creature.room, false, creature, false, 0);
				AbstractCreature? targetAb = target?.abstractCreature;

				if (creature is Lizard lizard)
				{
					// 玩家攻击欲望
					lizard.spawnDataEvil = -1f;

					// 蜥蜴专属好感设置
					if (lizard.AI is LizardAI lizardAI)
					{
						// 强制设置关系为“忽略”
						foreach (WeakReference<Player> weakPlayerRef in PlayerModuleManager.players)
						{
							Player player;
							if (weakPlayerRef.TryGetTarget(out player))
							{
								// 获取玩家在当前蜥蜴中的动态关系
								var rep = lizard.AI.tracker.RepresentationForCreature(player.abstractCreature, false);
								if (rep != null && rep.dynamicRelationship != null)
								{
									// 直接覆盖为“无视”
									rep.dynamicRelationship.currentRelationship = new CreatureTemplate.Relationship(
										CreatureTemplate.Relationship.Type.Ignores,
										0f
									);
								}
								CreatureState state = lizard.abstractCreature.state;
								SocialMemory.Relationship? relationship = null;
								if (state != null)
								{
									SocialMemory socialMemory = state.socialMemory;
									relationship = ((socialMemory != null) ? socialMemory.GetOrInitiateRelationship(player.abstractCreature.ID) : null);
								}
								if (relationship != null)
								{
									relationship.InfluenceTempLike(2f);
									relationship.InfluenceLike(2f);
									relationship.InfluenceKnow(0.9f);
								}
							}
						}

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
						else
						{
							lizardAI.behavior = LizardAI.Behavior.FollowFriend;
							lizardAI.focusCreature = null;
							Tracker.CreatureRepresentation playerRep = lizardAI.tracker.RepresentationForCreature(targetAb, false);
							lizard.abstractCreature.abstractAI.SetDestination(playerRep.BestGuessForPosition());
						}

					}
				}
				if (creature is Scavenger scavenger)
				{
					// 拾荒者专属好感设置
					if (scavenger.AI is ScavengerAI scavAI)
					{
						// 强制设置关系为“忽略”
						foreach (WeakReference<Player> weakPlayerRef in PlayerModuleManager.players)
						{
							Player player;
							if (weakPlayerRef.TryGetTarget(out player))
							{
								// 获取玩家在当前拾荒者中的动态关系
								if (scavenger.abstractCreature.abstractAI.RealAI is ScavengerAI scavAI_)
								{
									var rep = scavAI_.tracker.RepresentationForCreature(player.abstractCreature, false);
									if (rep != null && rep.dynamicRelationship != null)
									{
										rep.dynamicRelationship.currentRelationship = new CreatureTemplate.Relationship(
											CreatureTemplate.Relationship.Type.Ignores,
											0f
										);
									}
								}
								CreatureState state = scavenger.abstractCreature.state;
								SocialMemory.Relationship? relationship = null;
								if (state != null)
								{
									SocialMemory socialMemory = state.socialMemory;
									relationship = ((socialMemory != null) ? socialMemory.GetOrInitiateRelationship(player.abstractCreature.ID) : null);
								}
								if (relationship != null)
								{
									relationship.InfluenceTempLike(2f);
									relationship.InfluenceLike(2f);
									relationship.InfluenceKnow(0.9f);
								}
							}
						}

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
						else
						{
							scavAI.behavior = ScavengerAI.Behavior.FindPackLeader;
							scavAI.focusCreature = null;
							Tracker.CreatureRepresentation playerRep = scavAI.tracker.RepresentationForCreature(targetAb, false);
							scavenger.abstractCreature.abstractAI.SetDestination(playerRep.BestGuessForPosition());
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
									room.PlaySound(SoundID.Slugcat_Pick_Up_Spear, creature.mainBodyChunk);
									PlayerModuleManager.UndeadCreatures.Add(new WeakReference<Creature>(creature));

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
