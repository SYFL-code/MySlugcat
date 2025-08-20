using On;
using IL;
using System;
using System.Threading.Tasks;
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
using System.IO;
using Expedition;
using JollyCoop;


namespace MySlugcat
{
	// 蛞蝓猫数据

	public class MySlugcatStats
	{

		/*public static void Hook()
		{
			On.SlugcatStats.ctor += SlugcatStats_ctor;
			On.Player.MovementUpdate += Player_MovementUpdate;
			On.Player.Update += Player_Update;
			On.Player.ThrownSpear += Player_ThrownSpear;
		}*/

		public static void SlugcatStats_ctor(ref bool Execute, ref On.SlugcatStats.orig_ctor orig, ref SlugcatStats slugcatStats, ref SlugcatStats.Name slugcat, ref bool malnourished)
		{
			//if ((slugcat == Plugin.YourSlugID || SC.AllPlayerSkill) && SC.MySlugcatStats = -1)

			var Players = PlayerModuleManager.GetActivePlayers();
			foreach (var player in Players)
			{
				if (player.slugcatStats == slugcatStats)
				{
					if (player.GetModule().MySlugcatStats == -1)
					{
						slugcatStats.runspeedFac = 0.74f;
						slugcatStats.bodyWeightFac = 0.68f;
						slugcatStats.generalVisibilityBonus = 3f;
						slugcatStats.visualStealthInSneakMode = -0.5f;
						slugcatStats.loudnessFac = 2f;
						slugcatStats.lungsFac = 1.4f;
						slugcatStats.throwingSkill = 0;
					}
				}
			}
		}

		private static int HungerDegree = (int)(40 * 60 * Extension.RandomValue(5f, 10f));

		public static void Player_Update(ref bool Execute, ref On.Player.orig_Update orig, ref Player player, ref bool eu)
		{
			if (player.GetModule(out var module))
			{
				if (module.Swift != 0)
				{
					// 迅捷
					for (int i = 0; i < module.Swift; i++)
					{
						orig(player, eu);
					}
				}

				if (module.Exhausted)
				{
					// 精疲力竭
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

				if (module.Hunger)
				{
					// 饥饿
					if (!player.room.game.IsArenaSession)
					{
						if (--HungerDegree <= 0)
						{
							HungerDegree = (int)(40 * 60 * Extension.RandomValue(3f, 10f));

							if (!Extension.SubtractQuarterFood(player, 1))
							{
								player.Die();
							}
						}

					}
				}
			}

		}

		public static void Player_ThrownSpear(ref bool Execute, ref On.Player.orig_ThrownSpear orig, ref Player player, ref Spear spear)
		{
			if (player.GetModule().MySlugcatStats == -1)
			{
				Execute = false;
				spear.throwModeFrames = 18;
				spear.spearDamageBonus = 0.4f + 0.3f * Mathf.Pow(UnityEngine.Random.value, 4f);
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

				/*//风之祝福
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
		}

		public static void Player_MovementUpdate(ref bool Execute, ref On.Player.orig_MovementUpdate orig, ref Player player, ref bool eu)
		{
			if (player.GetModule().MySlugcatStats == 1)
			{
				int num2 = 0;
				for (int i = 0; i < 4; i++)
				{
					for (int j = 0; j < 2; j++)
					{
						if (player.IsTileSolid(j, Custom.eightDirections[i].x, Custom.eightDirections[i].y) && player.IsTileSolid(j, Custom.eightDirections[i + 4].x, Custom.eightDirections[i + 4].y))
						{
							num2++;
						}
					}
				}

				if ((num2 > 1 && player.bodyChunks[0].onSlope == 0 && player.bodyChunks[1].onSlope == 0 && (!player.IsTileSolid(0, 0, 0) || !player.IsTileSolid(1, 0, 0))) || (player.IsTileSolid(0, -1, 0) && player.IsTileSolid(0, 1, 0)) || (player.IsTileSolid(1, -1, 0) && player.IsTileSolid(1, 1, 0)))
				{
				}
				else
				{
					bool flag4 = player.bodyChunks[0].ContactPoint.y == -1 || player.bodyChunks[1].ContactPoint.y == -1;
					if (flag4)
					{

					}
					else if (player.jumpBoost > 0f && (player.input[0].jmp || player.simulateHoldJumpButton > 0))
					{
						player.jumpBoost += 0.9f;
						//BodyChunk bodyChunk = player.bodyChunks[0];
						//bodyChunk.vel.y = bodyChunk.vel.y + (player.jumpBoost + 1f) * 0.3f;
						//BodyChunk bodyChunk2 = player.bodyChunks[1];
						//bodyChunk2.vel.y = bodyChunk2.vel.y + (player.jumpBoost + 1f) * 0.3f;
					}
				}
			}

		}


	}
}