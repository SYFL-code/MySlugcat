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


namespace MySlugcat
{
	/// <summary> 穿透能力 </summary>
	public class PenetrationSkill
	{
		public static void Weapon_Update(ref bool Execute, ref Creature? thrownBy, ref On.Weapon.orig_Update orig, ref Weapon weapon, ref bool eu)
		{
			if (thrownBy != null && weapon.thrownBy == null && thrownBy is Player player && player.GetModule().PenetrationSkill)
			{
				weapon.thrownBy = player;
			}
		}

		public static bool Weapon_HitSomething(ref bool Execute, ref bool return_, ref On.Weapon.orig_HitSomething orig, ref Weapon weapon, ref SharedPhysics.CollisionResult result, ref bool eu)
		{
			if (weapon.thrownBy is Player player && player.GetModule().PenetrationSkill)
			{
				if (result.obj is Creature creature)
				{
					if (creature.State is HealthState hs)
					{
						weapon.room.PlaySound(SoundID.Rock_Hit_Creature, weapon.firstChunk);
						/*if (hs.health < 0.25f)
						{
							creature.Die();
						}*/
						hs.health -= 0.25f;
						if (creature is Scavenger scavenger)
						{
							creature.Stun(30);
						}
						else
						{
							creature.Stun(10);
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
					if (creature.State is HealthState hs)
					{
						rock.room.PlaySound(SoundID.Rock_Hit_Creature, rock.firstChunk);
						/*if (hs.health < 0.25f)
						{
							creature.Die();
						}*/
						hs.health -= 0.1f;
						if (creature is Scavenger scavenger)
						{
							creature.Stun(80);
						}
						else
						{
							creature.Stun(20);
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
					if (creature is Player player_)
					{
						player_.Die();
					}
					if (creature.State is HealthState hs)
					{
						spear.room.PlaySound(SoundID.Spear_Stick_In_Creature, spear.firstChunk);

						float spearDamageBonus = 1f;
						switch (player.slugcatStats.throwingSkill)
						{
							case 0:
								spearDamageBonus = 0.6f + 0.3f * Mathf.Pow(UnityEngine.Random.value, 4f);
								BodyChunk firstChunk = spear.firstChunk;
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
							spearDamageBonus = 0.6f;
						}

						if (hs.health < spearDamageBonus)
						{
							creature.Die();
						}
						hs.health -= (spearDamageBonus / 2f) + 0.2f;
						if (creature is Scavenger scavenger)
						{
							creature.Stun(40);
						}
						else
						{
							creature.Stun(10);
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

	}
}
