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
using static Menu.Remix.InternalOI;


namespace MySlugcat
{
	/// <summary> 钩子 </summary>
	public class Hook
	{

		public static void HookOn()
		{
			On.Player.Update += Player_Update;

			On.Spear.HitSomething += Spear_HitSomething;
			On.Rock.HitSomething += Rock_HitSomething;
			On.Weapon.HitSomething += Weapon_HitSomething;
			On.Weapon.Update += Weapon_Update;
		}

		public static void Player_Update(On.Player.orig_Update orig, Player player, bool eu)
		{
			bool Execute = true;
			try
			{
				//PenetrationSkill.Weapon_HitSomething(ref Execute, ref ret, ref orig, ref weapon, ref result, ref eu);
				if (!Execute) return;

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}

			orig(player, eu);

			try
			{
				//PenetrationSkill.Weapon_HitSomething(ref Execute, ref ret, ref orig, ref weapon, ref result, ref eu);
				if (!Execute) return;

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}

		public static bool Spear_HitSomething(On.Spear.orig_HitSomething orig, Spear spear, SharedPhysics.CollisionResult result, bool eu)
		{
			Log.OutputLog($"spear({spear != null})_thrownBy({spear?.thrownBy?.GetType()})_r({result.obj?.GetType()})_");
			bool Execute = true;
			bool ret = false;
			try
			{
				ret = FrameSkill.Spear_HitSomething(ref Execute, ref ret, ref orig, ref spear, ref result, ref eu);
				if (!Execute) return ret;
				ret = DeflagrationSkill.Spear_HitSomething(ref Execute, ref ret, ref orig, ref spear, ref result, ref eu);
				if (!Execute) return ret;
				ret = PenetrationSkill.Spear_HitSomething(ref Execute, ref ret, ref orig, ref spear, ref result, ref eu);
				if (!Execute) return ret;

				ret = orig(spear, result, eu);
				Log.OutputLog($"Spear ret({ret})_");
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
			return ret;
		}

		public static bool Rock_HitSomething(On.Rock.orig_HitSomething orig, Rock rock, SharedPhysics.CollisionResult result, bool eu)
		{
			Log.OutputLog($"rock({rock != null})_thrownBy({rock?.thrownBy?.GetType()})_r({result.obj?.GetType()})_");
			bool Execute = true;
			bool ret = false;
			try
			{
				ret = PenetrationSkill.Rock_HitSomething(ref Execute, ref ret, ref orig, ref rock, ref result, ref eu);
				if (!Execute) return ret;

				ret = orig(rock, result, eu);
				Log.OutputLog($"Rock ret({ret})_");
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
			return ret;
		}

		public static bool Weapon_HitSomething(On.Weapon.orig_HitSomething orig, Weapon weapon, SharedPhysics.CollisionResult result, bool eu)
		{
			Log.OutputLog($"weapon({weapon != null})_r({result.obj?.GetType()})_");
			bool Execute = true;
			bool ret = false;
			try
			{
				ret = PenetrationSkill.Weapon_HitSomething(ref Execute, ref ret, ref orig, ref weapon, ref result, ref eu);
				if (!Execute) return ret;

				ret = orig(weapon, result, eu);
				Log.OutputLog($"Weaponret({ret})_");
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
			return ret;
		}

		private static void Weapon_Update(On.Weapon.orig_Update orig, Weapon weapon, bool eu)
		{
			bool Execute = true;
			Creature? thrownBy = weapon.thrownBy;
			try
			{
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}

			orig(weapon, eu);

			try
			{
				PenetrationSkill.Weapon_Update(ref Execute, ref thrownBy, ref orig, ref weapon, ref eu);
				if (!Execute) return;

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}



	}
}
