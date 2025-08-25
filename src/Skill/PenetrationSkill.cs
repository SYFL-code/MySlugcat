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
using System.ComponentModel;
using static MySlugcat.PlayerModuleManager;
using System.Runtime.CompilerServices;
using static MySlugcat.AbPhysicalObjectModuleManager;


namespace MySlugcat
{
	/// <summary> 穿透能力 </summary>
	public class PenetrationSkill
	{

		public static void Weapon_Update(ref bool Execute, ref Creature? thrownBy, ref On.Weapon.orig_Update orig, ref Weapon weapon, ref bool eu)
		{
			AbPhysicalObjectModule weaponModule = weapon.abstractPhysicalObject.GetModule();
			if (weaponModule != null)
			{
				if (weapon.mode != Weapon.Mode.Thrown && weapon.mode != Weapon.Mode.StuckInCreature)
				{
					weaponModule.stuckInObject = null;
					weaponModule.stuckInObjectTime = 0;
					weaponModule.penetrateCount = 0;
					//weaponModule.originalVel = Vector2.zero;
				}
			}

			if (thrownBy != null && weapon.thrownBy == null && thrownBy is Player player && player.GetModule().PenetrationSkill)
			{
				weapon.thrownBy = player;
				weapon.firstChunk.owner = player;
			}

			if (weapon.thrownBy != null && (weapon.mode != Weapon.Mode.Thrown && weapon.mode != Weapon.Mode.StuckInCreature) && weapon.thrownBy is Player player_ && player_.GetModule().PenetrationSkill)
			{
				weapon.thrownBy = null;
			}

			/*if (weapon.firstChunk.owner != null && (weapon.mode != Weapon.Mode.Thrown && weapon.mode != Weapon.Mode.StuckInCreature) && weapon.firstChunk.owner is Player player__ && player__.GetModule().PenetrationSkill)
			{
				weapon.firstChunk.owner = null;
			}*/
		}

		public static void Weapon_HitWeapon<T>(
			ref bool Execute,
			ref T weapon,
			ref Weapon obj) where T : Weapon
		{
			if (obj.firstChunk.pos.x - obj.firstChunk.lastPos.x < 0f == weapon.firstChunk.pos.x - weapon.firstChunk.lastPos.x < 0f)
			{
				return;
			}

			bool flag = weapon.thrownBy is Player player && player.GetModule().PenetrationSkill;
			bool flag_ = obj.thrownBy is Player player_ && player_.GetModule().PenetrationSkill;

			if (flag || flag_)
			{
				Room room = weapon.room;
				if (room != null)
				{
					Execute = false;

					Vector2 vector = Vector2.Lerp(obj.firstChunk.lastPos, weapon.firstChunk.lastPos, 0.5f);
					int SparkQuantity = 3;
					if (weapon is Spear)
					{
						SparkQuantity += 2;
					}
					if (obj is Spear)
					{
						SparkQuantity += 2;
					}
					for (int i = 0; i < SparkQuantity; i++)
					{
						room.AddObject(new Spark(vector + Custom.DegToVec(UnityEngine.Random.value * 360f) * 5f * UnityEngine.Random.value, 
							Custom.DegToVec(UnityEngine.Random.value * 360f) * Mathf.Lerp(2f, 7f, UnityEngine.Random.value) * SparkQuantity, 
							new Color(1f, 1f, 1f), null, 10, 170));
					}
					Vector2 vector2 = Custom.DegToVec(UnityEngine.Random.value * 360f);
					if (flag_)
					{
						weapon.WeaponDeflect(vector, vector2, weapon.firstChunk.vel.magnitude);
					}
					if (flag)
					{
						obj.WeaponDeflect(vector, -vector2, weapon.firstChunk.vel.magnitude);
					}
					room.PlaySound(SoundID.Spear_Bounce_Off_Creauture_Shell, vector, weapon.abstractPhysicalObject);
				}
			}
		}

		public static bool PenetrateHit<T>(
			ref bool Execute,
			ref bool return_,
			ref T weapon,
			ref SharedPhysics.CollisionResult result, 
			ref bool eu) where T : Weapon
		{
			if (result.obj == null)
			{
				return false;
			}
			if (result.obj.abstractPhysicalObject.rippleLayer != weapon.abstractPhysicalObject.rippleLayer && !result.obj.abstractPhysicalObject.rippleBothSides && !weapon.abstractPhysicalObject.rippleBothSides)
			{
				return false;
			}

			if (weapon.thrownBy is Player player && player.GetModule().PenetrationSkill)
			{
				Room room = weapon.room;
				if (result.obj is Creature creature && room != null)
				{
					AbPhysicalObjectModule weaponModule = weapon.abstractPhysicalObject.GetModule();
					if (weaponModule != null)
					{
						if (weaponModule.stuckInObject != creature || weaponModule.stuckInObjectTime > 30)
						{
							weaponModule.stuckInObject = creature;
							weaponModule.stuckInObjectTime = 1;
							weaponModule.penetrateCount += 1;
							if (Control.CheatMode)
							{
								weaponModule.penetrateCount = 0;
							}

							if (UnityEngine.Random.value * 10f > Mathf.Max(6.6f, 12f - weaponModule.penetrateCount))
							{
								return return_;
							}

							if (weapon is Spear spear)
							{
								spear.stuckInObject = creature;

								float spearDamageBonus = 1f;
								switch (player.slugcatStats.throwingSkill)
								{
									case 0:
										spearDamageBonus = 0.6f + 0.3f * Mathf.Pow(UnityEngine.Random.value, 4f);
										break;

									case 1:
										spearDamageBonus = 1f;
										break;

									case 2:
										spearDamageBonus = 1.25f;
										break;

									case 3:
										spearDamageBonus = 1.5f;
										break;

									default:
										spearDamageBonus = 1f;
										break;
								}
								/*if (player.slugcatStats.name == Plugin.YourSlugID)
								{
									spearDamageBonus = Mathf.Max(3f * Mathf.Pow(UnityEngine.Random.value, 10f) - 2.1f, 0.001f);
								}*/

								float MaxspearDamageBonus = Mathf.Max(spear.spearDamageBonus, spearDamageBonus);
								MaxspearDamageBonus *= Mathf.Max(0.6f, 1.1f - 0.1f * weaponModule.penetrateCount);

								//spearDamageBonus = spearDamageBonus / creature.Template.baseDamageResistance;

								if (ModManager.MSC && result.obj is Player player_ && player_.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Gourmand && UnityEngine.Random.value < 0.15f)
								{
									//MaxspearDamageBonus /= 10f;

									/*Custom.Log(new string[]
									{
										"GOURMAND SAVE!"
									});*/
								}
								if (spear.bugSpear)
								{
									MaxspearDamageBonus *= 3f;
								}

								creature.Violence(weapon.firstChunk, new Vector2?(weapon.firstChunk.vel * weapon.firstChunk.mass * 2f), result.chunk, result.onAppendagePos, Creature.DamageType.Stab, MaxspearDamageBonus, 60f);

								if (ModManager.MSC && result.obj is Player player_1)
								{
									player_1.playerState.permanentDamageTracking += (double)(MaxspearDamageBonus / player_1.Template.baseDamageResistance);
									if (player_1.playerState.permanentDamageTracking >= 1.0)
									{
										player_1.Die();
									}
								}
								room.PlaySound(SoundID.Spear_Stick_In_Creature, weapon.firstChunk);

								/*if (hs.health < spearDamageBonus)
								{
									creature.Die();
								}
								hs.health -= spearDamageBonus;
								if (creature is Scavenger scavenger)
								{
									creature.Stun(60);
								}
								else
								{
									creature.Stun(60);
								}*/
							}
							else if (weapon is Rock)
							{
								weapon.vibrate = 20;

								float stunBonus = 45f;
								if (ModManager.MMF && MMF.cfgIncreaseStuns.Value && (result.obj is Cicada || result.obj is LanternMouse || (ModManager.MSC && result.obj is Yeek)))
								{
									stunBonus = 90f;
								}
								if (ModManager.MSC && room.game.IsArenaSession && room.game.GetArenaGameSession.chMeta != null)
								{
									stunBonus = 90f;
								}
								creature.Violence(weapon.firstChunk, new Vector2?(weapon.firstChunk.vel * weapon.firstChunk.mass), result.chunk, result.onAppendagePos, Creature.DamageType.Stab, 0.12f, stunBonus);

								room.PlaySound(SoundID.Rock_Hit_Creature, weapon.firstChunk);
							}
							else if (weapon is ScavengerBomb)
							{
								weapon.vibrate = 20;

								creature.Violence(weapon.firstChunk, new Vector2?(weapon.firstChunk.vel * weapon.firstChunk.mass), result.chunk, result.onAppendagePos, Creature.DamageType.Explosion, 0.8f, 85f);

								room.PlaySound(SoundID.Rock_Hit_Creature, weapon.firstChunk);
							}
							else if (ModManager.Watcher && weapon is Boomerang)
							{
								weapon.vibrate = 20;
								float stunBonus = 45f;
								if (ModManager.MMF && MMF.cfgIncreaseStuns.Value && (result.obj is Cicada || result.obj is LanternMouse || (ModManager.MSC && result.obj is Yeek)))
								{
									stunBonus = 90f;
								}
								if (ModManager.MSC && room.game.IsArenaSession && room.game.GetArenaGameSession.chMeta != null)
								{
									stunBonus = 90f;
								}

								creature.Violence(weapon.firstChunk, new Vector2?(weapon.firstChunk.vel * weapon.firstChunk.mass), result.chunk, result.onAppendagePos, Creature.DamageType.Stab, 0.15f, stunBonus);

								room.PlaySound(WatcherEnums.WatcherSoundID.Boomerang_Collide_Creature, weapon.firstChunk);
							}
							else
							{
								creature.Violence(weapon.firstChunk, new Vector2?(weapon.firstChunk.vel * weapon.firstChunk.mass), result.chunk, result.onAppendagePos, Creature.DamageType.Stab, 0.2f, 20);

								room.PlaySound(SoundID.Rock_Hit_Creature, weapon.firstChunk);
							}

							//震动强度
							weapon.vibrate = 20;

							Log.OutputLog($"Spear _OK1");
							HashSet<Creature> hit = new HashSet<Creature>();
							hit.Add(creature);
							room.AddObject(new ArcLightning(weapon.firstChunk.pos, creature, weapon.firstChunk.vel.normalized, 60f, weapon.thrownBy, ref hit));
							Log.OutputLog($"Spear _OK2");


							// 屏幕震动
							//room.ScreenMovement(null, dir, strength);
						}
						else
						{
							weaponModule.stuckInObjectTime += 1;
						}
					}
				}
				result.obj = null;
				Execute = false;
				return false;
			}
			else
			{
				return return_;
			}
		}

	}
}


/*public static bool Weapon_HitSomething(ref bool Execute, ref bool return_, ref On.Weapon.orig_HitSomething orig, ref Weapon weapon, ref SharedPhysics.CollisionResult result, ref bool eu)
{
	if (weapon.thrownBy is Player player && player.GetModule().PenetrationSkill)
	{
		if (result.obj is Creature creature)
		{
			AbPhysicalObjectModule weaponModule = weapon.abstractPhysicalObject.GetModule();
			if (weaponModule != null && weaponModule.WeaponPenetration.Add(creature.abstractPhysicalObject))
			{
				if (creature.State is HealthState hs)
				{
					weapon.room.PlaySound(SoundID.Rock_Hit_Creature, weapon.firstChunk);
					*//*if (hs.health < 0.25f)
					{
						creature.Die();
					}*//*
					hs.health -= Mathf.Max(0.4f * Mathf.Pow(UnityEngine.Random.value, 5f) - 0.15f, 0.001f);
					if (creature is Scavenger scavenger)
					{
						creature.Stun(30);
					}
					else
					{
						creature.Stun(20);
					}
				}
			}
		}
		result.obj = null;
		Execute = false;
		return false;
	}
	else
	{
		return return_;
	}
}

public static bool Rock_HitSomething(ref bool Execute, ref bool return_, ref On.Rock.orig_HitSomething orig, ref Rock rock, ref SharedPhysics.CollisionResult result, ref bool eu)
{
	if (rock.thrownBy is Player player && player.GetModule().PenetrationSkill)
	{
		if (result.obj is Creature creature)
		{
			AbPhysicalObjectModule weaponModule = rock.abstractPhysicalObject.GetModule();
			if (weaponModule != null && weaponModule.WeaponPenetration.Add(creature.abstractPhysicalObject))
			{
				if (creature.State is HealthState hs)
				{
					rock.room.PlaySound(SoundID.Rock_Hit_Creature, rock.firstChunk);
					*//*if (hs.health < 0.25f)
					{
						creature.Die();
					}*//*
					hs.health -= Mathf.Max(0.3f * Mathf.Pow(UnityEngine.Random.value, 5f) - 0.1f, 0.001f);
					if (creature is Scavenger scavenger)
					{
						creature.Stun(90);
					}
					else
					{
						creature.Stun(90);
					}
				}
			}
		}
		result.obj = null;
		Execute = false;
		return false;
	}
	else
	{
		return return_;
	}
}

public static bool ScavengerBomb_HitSomething(ref bool Execute, ref bool return_, ref On.ScavengerBomb.orig_HitSomething orig, ref ScavengerBomb bomb, ref SharedPhysics.CollisionResult result, ref bool eu)
{
	if (bomb.thrownBy is Player player && player.GetModule().PenetrationSkill)
	{
		if (result.obj is Creature creature)
		{
			AbPhysicalObjectModule weaponModule = bomb.abstractPhysicalObject.GetModule();
			if (weaponModule != null && weaponModule.WeaponPenetration.Add(creature.abstractPhysicalObject))
			{
				if (creature.State is HealthState hs)
				{
					bomb.room.PlaySound(SoundID.Rock_Hit_Creature, bomb.firstChunk);
					*//*if (hs.health < 0.25f)
					{
						creature.Die();
					}*//*
					hs.health -= Mathf.Max(0.5f * Mathf.Pow(UnityEngine.Random.value, 5f) - 0.1f, 0.001f);
					if (creature is Scavenger scavenger)
					{
						creature.Stun(90);
					}
					else
					{
						creature.Stun(90);
					}
				}
			}
		}
		result.obj = null;
		Execute = false;
		return false;
	}
	else
	{
		return return_;
	}
}

public static bool Spear_HitSomething(ref bool Execute, ref bool return_, ref On.Spear.orig_HitSomething orig, ref Spear spear, ref SharedPhysics.CollisionResult result, ref bool eu)
{
	if (spear.thrownBy is Player player && player.GetModule().PenetrationSkill)
	{
		if (result.obj is Creature creature)
		{
			AbPhysicalObjectModule weaponModule = spear.abstractPhysicalObject.GetModule();
			if (weaponModule != null && weaponModule.WeaponPenetration.Add(creature.abstractPhysicalObject))
			{
				if (creature.State is HealthState hs)
				{
					spear.room.PlaySound(SoundID.Spear_Stick_In_Creature, spear.firstChunk);

					float spearDamageBonus = 1f;
					switch (player.slugcatStats.throwingSkill)
					{
						case 0:
							spearDamageBonus = 0.6f + 0.3f * Mathf.Pow(UnityEngine.Random.value, 4f);
							break;

						case 1:
							spearDamageBonus = 1f;
							break;

						case 2:
							spearDamageBonus = 1.25f;
							break;

						case 3:
							spearDamageBonus = 1.5f;
							break;

						default:
							spearDamageBonus = 1f;
							break;
					}
					if (player.slugcatStats.name == Plugin.YourSlugID)
					{
						spearDamageBonus = Mathf.Max(3f * Mathf.Pow(UnityEngine.Random.value, 10f) - 2.1f, 0.001f);
					}

					if (hs.health < spearDamageBonus)
					{
						creature.Die();
					}
					hs.health -= spearDamageBonus;
					if (creature is Scavenger scavenger)
					{
						creature.Stun(90);
					}
					else
					{
						creature.Stun(90);
					}

				}
			}
		}
		result.obj = null;
		Execute = false;
		return false;
	}
	else
	{
		return return_;
	}
}*/

/*public static bool Weapon_HitSomething(On.Weapon.orig_HitSomething orig, Weapon weapon, SharedPhysics.CollisionResult result, bool eu)
{
	if (result.obj != null && weapon.thrownBy is Player player && player.GetModule().PenetrationSkill)
	{
		if (result.obj is Creature creature)
		{
			if (creature.State is HealthState hs)
			{
				if (hs.health < 1f)
				{
					creature.Die();
				}
				hs.health -= 0.1f;
				creature.Stun(120);
			}
		}

		return false;
	}
	else
	{
		return orig(weapon, result, eu);
	}
}

public static bool Spear_HitSomething(On.Spear.orig_HitSomething orig, Spear spear, SharedPhysics.CollisionResult result, bool eu)
{
	if (result.obj != null && spear.thrownBy is Player player && player.GetModule().PenetrationSkill)
	{
		if (result.obj is Creature creature)
		{
			if (creature.State is HealthState hs)
			{
				if (hs.health < 1f)
				{
					creature.Die();
				}
				hs.health -= 0.1f;
				creature.Stun(120);
			}
		}

		return false;
	}
	else
	{
		return orig(spear, result, eu);
	}
}*/


