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
using System.Drawing;


namespace MySlugcat
{
	// 死灵法师
	// CreatureModules
	public static class NecrophytesCreature
	{
		private const float GrayizationSpeed = 1f / 400f;
		private const int BeforeRainToFlee = 60;


		/*public static void Hook()
		{
			On.Player.Update += Player_Update;
			On.Creature.Update += Creature_Update;
			On.LizardGraphics.DrawSprites += LizardGraphics_DrawSprites;
			On.ScavengerGraphics.DrawSprites += ScavengerGraphics_DrawSprites;
			On.Creature.Die += Creature_Die;
		}*/

		/*private static void LizardGraphics_ApplyPalette(On.LizardGraphics.orig_ApplyPalette orig, LizardGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
		{
			orig(self, sLeaser, rCam, palette);
			if (IsNecrophyte(self.lizard) && CreatureModuleManager.CreatureModules.TryGetValue(self.lizard, out var m))
			{
				float g = Mathf.Clamp01(m.Grayization);
				for (int i = 0; i < sLeaser.sprites.Length; i++)
					sLeaser.sprites[i].color = Extension.ToGrayscale(sLeaser.sprites[i].color, g);
			}
		}

		private static void ScavengerGraphics_ApplyPalette(On.ScavengerGraphics.orig_ApplyPalette orig, ScavengerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
		{
			orig(self, sLeaser, rCam, palette);
			if (IsNecrophyte(self.scavenger) && CreatureModuleManager.CreatureModules.TryGetValue(self.scavenger, out var m))
			{
				float g = Mathf.Clamp01(m.Grayization);
				for (int i = 0; i < sLeaser.sprites.Length; i++)
					sLeaser.sprites[i].color = Extension.ToGrayscale(sLeaser.sprites[i].color, g);
			}
		}*/

		public static void LizardGraphics_DrawSprites(ref bool Execute, ref On.LizardGraphics.orig_DrawSprites orig, ref LizardGraphics lizardGraphics, ref RoomCamera.SpriteLeaser sLeaser, ref RoomCamera rCam, ref float timeStacker, ref Vector2 camPos)
		{
			if (!rCam.room.game.DEBUGMODE)
			{
				Creature lizard = lizardGraphics.lizard;
				if (IsNecrophyte(lizard) && lizard.abstractCreature.GetModule(out var module_c) && module_c.IsNecrophyte)
				{
					if (lizard is Lizard || lizard is Scavenger)
					{
						float Grayization = Mathf.Clamp01(module_c.Grayization);
						try
						{
							for (int i = 0; i < sLeaser.sprites.Length; i++)
							{
								sLeaser.sprites[i]._color = Extension.ToGrayscale(sLeaser.sprites[i]._color, Grayization);
								//sLeaser.sprites[i].shader = rCam.game.rainWorld.Shaders["Hologram"];
								//sLeaser.sprites[i].alpha = Grayization;
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

		public static void ScavengerGraphics_DrawSprites(ref bool Execute, ref On.ScavengerGraphics.orig_DrawSprites orig, ref ScavengerGraphics scavGraphics, ref RoomCamera.SpriteLeaser sLeaser, ref RoomCamera rCam, ref float timeStacker, ref Vector2 camPos)
		{
			if (!rCam.room.game.DEBUGMODE)
			{
				Creature scavenger = scavGraphics.scavenger;
				if (IsNecrophyte(scavenger) && scavenger.abstractCreature.GetModule(out var module_c) && module_c.IsNecrophyte)
				{
					if (scavenger is Lizard || scavenger is Scavenger)
					{
						float Grayization = Mathf.Clamp01(module_c.Grayization);

						try
						{
							for (int i = 0; i < sLeaser.sprites.Length; i++)
							{
								sLeaser.sprites[i]._color = Extension.ToGrayscale(sLeaser.sprites[i]._color, Grayization);
							}

							// 2. 处理特殊子模块
							// 2.1 处理手部 (ScavengerHand)
							for (int i = 0; i < 2; i++)
							{
								if (scavGraphics.hands[i] != null)
								{
									int firstSprite = scavGraphics.hands[i].firstSprite;
									// 手部通常有3个sprite（根据源码中的InitiateSprites）
									for (int j = 0; j < 3; j++)
									{
										if (firstSprite + j < sLeaser.sprites.Length)
										{
											sLeaser.sprites[firstSprite + j].color = Extension.ToGrayscale(
												sLeaser.sprites[firstSprite + j].color, Grayization);
										}
									}
								}
							}

							// 2.2 处理腿部 (ScavengerLeg)
							for (int i = 0; i < 2; i++)
							{
								if (scavGraphics.legs[i] != null)
								{
									int firstSprite = scavGraphics.legs[i].firstSprite;
									// 腿部通常有2个sprite（根据源码中的InitiateSprites）
									for (int j = 0; j < 2; j++)
									{
										if (firstSprite + j < sLeaser.sprites.Length)
										{
											sLeaser.sprites[firstSprite + j].color = Extension.ToGrayscale(
												sLeaser.sprites[firstSprite + j].color, Grayization);
										}
									}
								}
							}

							// 2.3 处理耳朵装饰 (Eartlers)
							if (scavGraphics.eartlers != null)
							{
								int totalEartlerSprites = scavGraphics.eartlers.TotalSprites;
								for (int i = 0; i < totalEartlerSprites; i++)
								{
									int spriteIndex = scavGraphics.eartlers.firstSprite + i;
									if (spriteIndex >= 0 && spriteIndex < sLeaser.sprites.Length)
									{
										sLeaser.sprites[spriteIndex].color = Extension.ToGrayscale(
											sLeaser.sprites[spriteIndex].color, Grayization);
									}
								}
							}

							// 2.4 处理面具 (maskGfx)
							if (scavGraphics.maskGfx != null && !scavGraphics.scavenger.readyToReleaseMask)
							{
								for (int i = 0; i < scavGraphics.maskGfx.TotalSprites; i++)
								{
									int spriteIndex = scavGraphics.MaskSprite + i;
									//if (spriteIndex < sLeaser.sprites.Length)
									if (spriteIndex >= 0 && spriteIndex < sLeaser.sprites.Length)
									{
										sLeaser.sprites[spriteIndex].color = Extension.ToGrayscale(
											sLeaser.sprites[spriteIndex].color, Grayization);
									}
								}
							}

							// 2.5 处理壳装饰 (shells)
							if (ModManager.DLCShared && scavGraphics.shells != null)
							{
								for (int i = 0; i < scavGraphics.shells.Length; i++)
								{
									if (scavGraphics.shells[i] != null)
									{
										for (int j = 0; j < scavGraphics.shells[i].TotalSprites; j++)
										{
											int spriteIndex = scavGraphics.ShellSprite + j;
											if (spriteIndex < sLeaser.sprites.Length)
											{
												sLeaser.sprites[spriteIndex].color = Extension.ToGrayscale(
													sLeaser.sprites[spriteIndex].color, Grayization);
											}
										}
									}
								}
							}

							scavGraphics.bodyColor = Extension.ToGrayscale(scavGraphics.bodyColor, Grayization);
							scavGraphics.headColor = Extension.ToGrayscale(scavGraphics.headColor, Grayization);
							scavGraphics.decorationColor = Extension.ToGrayscale(scavGraphics.decorationColor, Grayization);
							scavGraphics.bellyColor = Extension.ToGrayscale(scavGraphics.bellyColor, Grayization);

							// 立即用新的 HSLColor 刷新一次
							scavGraphics.ApplyPalette(sLeaser, rCam, rCam.currentPalette);
						}
						catch (Exception e)
						{
							Debug.LogException(e);
						}
					}
				}
			}
		}

		public static void Creature_Die(ref bool Execute, ref On.Creature.orig_Die orig, ref Creature creature)
		{
			if (creature.abstractCreature.GetModule(out var module_c))
			{
				if (IsNecrophyte(creature) && module_c.IsNecrophyte)
				{
					if (module_c.NecrophyteDying)
					{
						return;
					}
					//Health.KillCreature(creature.room.game, creature);
					Execute = false;
					orig(creature);
					module_c.NecrophyteDying = true;
					creature.room?.AddObject(new DespawnAnimation(creature));
					return;
				}
			}
		}

		public static void Creature_Update(ref bool Execute, ref On.Creature.orig_Update orig, ref Creature creature, ref bool eu)
		{
			if (creature.room == null || creature.firstChunk == null)
			{
				return;
			}

			if (creature.abstractCreature.GetModule(out var module_c))
			{
				if (module_c.IsNecrophyte)
				{
					try
					{
						if (module_c.NecrophyteDying)
						{
							return;
						}
						if (creature.room.world.rainCycle.TimeUntilRain < BeforeRainToFlee)
						{
							Health.KillCreature(creature.room.game, creature);
							module_c.NecrophyteDying = true;
							creature.room.AddObject(new DespawnAnimation(creature));
							return;
						}

						//Health.ReviveCreature(creature);
						Tame.TameCreature(creature.room.game, creature);
						//PlayerModuleManager.UndeadCreatures.Add(new WeakReference<Creature>(creature));

						Creature? target = FindNearestUnNecrophyte(creature.firstChunk.pos, creature.room, false, creature, false, 0);
						AbstractCreature? targetAb = target?.abstractCreature;

						if (module_c.Grayization < 1f)
						{
							module_c.Grayization = Mathf.Clamp01(module_c.Grayization + GrayizationSpeed);
						}

						if (creature is Lizard lizard)
						{
							// 玩家攻击欲望
							lizard.spawnDataEvil = -1f;

							// 蜥蜴专属好感设置
							if (lizard.AI is LizardAI lizardAI)
							{
								var Players = PlayerModuleManager.GetActivePlayers();
								// 强制设置关系为“忽略”
								foreach (var player in Players)
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
								var Players = PlayerModuleManager.GetActivePlayers();
								// 强制设置关系为“忽略”
								foreach (var player in Players)
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

						//creature.Template.shortcutColor = new Color(84f / 255f, 84f / 255f, 84f / 255f, 1f);
					}
					catch (Exception e)
					{
						Debug.LogException(e);
					}
				}
				else
				{
					module_c.Grayization = 0f;
				}
			}

		}

		public static void Player_Update(ref bool Execute, ref On.Player.orig_Update orig, ref Player player, ref bool eu)
		{
			int N = player.playerState.playerNumber;
			if (player.GetModule(out var module) && module.SpawnNecrophytes)
			{
				//Console.WriteLine($"JmpCounter_({module.JmpCounter})_[0].jmp_({player.input[0].jmp})_[1].jmp({player.input[1].jmp})");
				if (module.JmpCounter >= 60 && !player.input[0].jmp && player.input[1].jmp)
				{
					if (!player.dead)
					{
						Room room = player.room;
						for (int i = room.abstractRoom.creatures.Count - 1; i >= 0; i--)
						{
							Creature creature = room.abstractRoom.creatures[i].realizedCreature;
							if (creature != null)
							{
								if (creature.abstractCreature.GetModule(out var module_c) && !module_c.IsNecrophyte && !module_c.NecrophyteDying)
								{
									if (creature is Lizard || creature is Scavenger)
									{
										try
										{
											Health.ReviveCreature(creature);
											Tame.TameCreature(player.room.game, creature);
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
												if (scavenger.graphicsModule is ScavengerGraphics scavGraphics)
												{
												}
												// 拾荒者专属好感设置
												if (scavenger.AI is ScavengerAI scavAI)
												{
												}
											}

											// 标记为亡灵生物
											module_c.IsNecrophyte = true;
											module_c.Grayization = 0;

											//creature.Template.shortcutColor = new Color(84f / 255f, 84f / 255f, 84f / 255f, 1f);
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


		/// <summary>
		/// 是否为亡灵生物
		/// </summary>
		public static bool IsNecrophyte(Creature creature)
		{
			if (creature == null)
			{
				return false;
			}
			bool isNecrophyte = false;

			if (creature.abstractCreature.GetModule(out var module_c) && module_c.IsNecrophyte)
			{
				isNecrophyte = true;
			}
			return isNecrophyte;
		}

		/// <summary>
		/// 查找当前房间中距离自身最近的非亡灵生物
		/// </summary>
		public static Creature? FindNearestUnNecrophyte(Vector2 selfPos, Room room, bool IncludePlayer, Creature? creature, bool IncludeDeadCreature, int select)
		{
			Creature? nearest = null;        // 最近非亡灵生物对象
			float minSqrDistance = float.MaxValue;  // 最小平方距离（初始设为最大浮点数）
													//List<Creature> UnUnCreatures = new List<Creature>();

			if (!(room.abstractRoom.creatures.Count > 0))
			{
				return null;
			}
			//if (selfPos == null || room == null || room.abstractRoom == null || room.abstractRoom.creatures == null || !(room.abstractRoom.creatures.Count > 0))
			if (room == null || room.abstractRoom?.creatures == null || !(room.abstractRoom.creatures.Count > 0))
			{
				return null;
			}

			// 遍历当前房间所有生物
			foreach (AbstractCreature abstractCreature in room.abstractRoom.creatures)
			{

				Creature c = abstractCreature.realizedCreature;
				// 排除检查：玩家、无效引用、自身、或没有身体部位的对象
				if (c == null ||             // 确保生物存在
					c == creature ||             // 排除自身
					c.mainBodyChunk == null) // 确保有有效的mainBodyChunk
				{
					continue; // 跳过无效项，继续检查下一个
				}
				if (!IncludePlayer)
				{
					var player1 = c as Player;
					if (player1 != null)
					{
						continue; // 跳过无效项，继续检查下一个
					}
				}
				if (Extension.DisabledCreature(c) && select == 1)// 禁用生物
				{
					continue; // 跳过无效项，继续检查下一个
				}
				if (Extension.IsHarmlessCreature(c) && select == 2)// 无害生物
				{
					continue; // 跳过无效项，继续检查下一个
				}
				if ((IsDying(c) || c.dead == true) && !IncludeDeadCreature)// 死亡的生物
				{
					continue; // 跳过无效项，继续检查下一个
				}
				if (IsNecrophyte(c))
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
				//Console.WriteLine("RandomlySelectedUnUndeadCreature: No UnUndeadCreatures found");
				return null;
			}
			return nearest; // 返回最近生物（可能为null）
		}

		/// <summary>
		/// 查找当前房间中距离自身最近的非亡灵生物
		/// </summary>
		public static Creature? FindNearestUnNecrophyte1(Vector2 selfPos, Room room, bool IncludePlayer, Creature? creature, bool IncludeDeadCreature, int select)
		{
			if (room == null) return null;

			Creature? nearest = null;
			float minSqr = float.MaxValue;

			// 直接拿物理层数组，避免抽象层装箱
			List<PhysicalObject> physList = room.physicalObjects[2];
			for (int i = 0; i < physList.Count; i++)
			{
				if (physList[i] is not Creature c) continue;

				if (c == creature) continue;
				if (IsNecrophyte(c)) continue;
				if (c.mainBodyChunk == null) continue;
				if (!IncludeDeadCreature && (c.dead || IsDying(c))) continue;
				if (!IncludePlayer)
				{
					var player1 = c as Player;
					if (player1 != null)
					{
						continue; // 跳过无效项，继续检查下一个
					}
				}
				if (Extension.DisabledCreature(c) && select == 1)// 禁用生物
				{
					continue; // 跳过无效项，继续检查下一个
				}
				if (Extension.IsHarmlessCreature(c) && select == 2)// 无害生物
				{
					continue; // 跳过无效项，继续检查下一个
				}

				float sqr = (c.mainBodyChunk.pos - selfPos).sqrMagnitude;
				if (sqr < minSqr)
				{
					minSqr = sqr;
					nearest = c;
				}
			}
			return nearest;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsDying(Creature c) =>
			c.abstractCreature.GetModule(out var m) && m.NecrophyteDying;

	}
}
