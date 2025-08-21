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
			On.RainWorldGame.Update += RainWorldGame_Update;

			#region Player
			On.Player.ctor += Player_ctor;
			On.Player.Update += Player_Update;
			On.Player.MovementUpdate += Player_MovementUpdate;
			On.Player.SwallowObject += Player_SwallowObject;
			On.Player.ThrownSpear += Player_ThrownSpear;
			On.Player.Die += Player_Die;
			On.Player.Destroy += Player_Destroy;

			On.PlayerGraphics.InitiateSprites += PlayerGraphics_InitiateSprites;
			On.PlayerGraphics.DrawSprites += PlayerGraphics_DrawSprites;
			On.PlayerGraphics.AddToContainer += PlayerGraphics_AddToContainer;

			On.SlugcatStats.ctor += SlugcatStats_ctor;
			#endregion

			#region Creature
			On.Creature.Update += Creature_Update;
			On.Creature.Die += Creature_Die;
			On.LizardGraphics.DrawSprites += LizardGraphics_DrawSprites;
			On.ScavengerGraphics.DrawSprites += ScavengerGraphics_DrawSprites;
			#endregion

			#region Abstract
			On.AbstractCreature.ctor += AbstractCreature_ctor;
			On.AbstractPhysicalObject.ctor += AbstractPhysicalObject_ctor;
			On.AbstractPhysicalObject.Destroy += AbstractPhysicalObject_Destroy;
			#endregion

			#region Weapon
			On.Spear.HitSomething += Spear_HitSomething;
			On.Spear.SetRandomSpin += Spear_SetRandomSpin;
			On.Rock.HitSomething += Rock_HitSomething;
			On.Weapon.HitSomething += Weapon_HitSomething;
			On.Weapon.SetRandomSpin += Weapon_SetRandomSpin;
			On.PuffBall.HitSomething += PuffBall_HitSomething;
			On.PuffBall.Explode += PuffBall_Explode;
			On.FlareBomb.StartBurn += FlareBomb_StartBurn;
			On.Weapon.Update += Weapon_Update;
			On.Weapon.HitAnotherThrownWeapon += Weapon_HitAnotherThrownWeapon;
			if (ModManager.MSC)
			{
				On.MoreSlugcats.LillyPuck.HitSomething += LillyPuck_HitSomething;
				On.MoreSlugcats.LillyPuck.SetRandomSpin += LillyPuck_SetRandomSpin;
			}
			if (ModManager.Watcher)
			{
				On.Boomerang.HitSomething += Boomerang_HitSomething;
			}
			#endregion

			#region 挣脱
			//咬住挣脱
			On.Creature.Violence += Creature_Violence;
			//挣脱蜥蜴
			On.Lizard.Bite += Lizard_Bite;
			//挣脱蘑菇
			On.DaddyLongLegs.Eat += DaddyLongLegs_Eat;
			//挣脱蜈蚣
			On.Centipede.UpdateGrasp += Centipede_UpdateGrasp;
			//挣脱利维坦
			On.BigEel.JawsSnap += BigEel_JawsSnap;
			//挣脱红树
			On.TentaclePlant.Carry += TentaclePlant_Carry;
			//挣脱拟态草
			On.PoleMimic.Carry += PoleMimic_Carry;
			//挣脱火虫
			On.EggBug.CarryObject += EggBug_CarryObject;
			//冰盾转换
			//On.Spear.HitSomething += Spear_HitSomething;
			On.ScavengerBomb.HitSomething += ScavengerBomb_HitSomething;
			//挣脱魔王秃鹫
			On.Vulture.Carry += Vulture_Carry;
			#endregion

			#region Bee
			On.SporePlant.Bee.Update += Bee_Update;
			On.SporePlant.Bee.ApplyPalette += Bee_ApplyPalette;
			On.SporePlant.Bee.LookForRandomCreatureToHunt += Bee_ToHunt;
			#endregion

			#region HUD
			// 睡眠HUD初始化
			//On.HUD.HUD.InitSleepHud += HUD_InitSleepHud;
			// 单人模式HUD初始化
			On.HUD.HUD.InitSinglePlayerHud += HUD_InitSinglePlayerHud;
			// 多人模式HUD初始化
			On.HUD.HUD.InitMultiplayerHud += HUD_InitMultiplayerHud;
			// 狩猎模式HUD初始化
			On.HUD.HUD.InitSafariHud += HUD_InitSafariHud;
			// 快速瞬移通行证HUD初始化
			//On.HUD.HUD.InitTeleportHud += HUD_InitTeleportHud;
			#endregion

			#region Friends of friends
			On.RelationshipTracker.DynamicRelationship.Update += DynamicRelationship_Update;
			On.LizardAI.DoIWantToBiteThisCreature += LizardAI_DoIWantToBiteThisCreature;
			#endregion

			//打开外层空间大门
			On.RegionGate.customOEGateRequirements += RegionGate_customOEGateRequirements;
		}

		private static void RainWorldGame_Update(On.RainWorldGame.orig_Update orig, RainWorldGame rainWorldGame)
		{
			bool Execute = true;

			orig(rainWorldGame);

			try
			{
				Control.RainWorldGame_Update(ref Execute, ref orig, ref rainWorldGame);
				if (!Execute) return;

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}
		
		#region Player
		private static void Player_ctor(On.Player.orig_ctor orig, Player player, AbstractCreature abstractCreature, World world)
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

			orig(player, abstractCreature, world);

			try
			{
				Control.Player_ctor(ref Execute, ref orig, ref player, ref abstractCreature, ref world);
				if (!Execute) return;
				PlayerHooks.Player_ctor(ref Execute, ref orig, ref player, ref abstractCreature, ref world);
				if (!Execute) return;
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}

		public static void Player_Update(On.Player.orig_Update orig, Player player, bool eu)
		{
			bool Execute = true;

			orig(player, eu);

			try
			{
				MySlugcatStats.Player_Update(ref Execute, ref orig, ref player, ref eu);
				if (!Execute) return;
				Key.Player_Update(ref Execute, ref orig, ref player, ref eu);
				if (!Execute) return;
				NecrophytesCreature.Player_Update(ref Execute, ref orig, ref player, ref eu);
				if (!Execute) return;
				DeflagrationSkill.Player_Update(ref Execute, ref orig, ref player, ref eu);
				if (!Execute) return;
				DigestionSkill.Player_Update(ref Execute, ref orig, ref player, ref eu);
				if (!Execute) return;
				KnitmeshSkill.Player_Update(ref Execute, ref orig, ref player, ref eu);
				if (!Execute) return;
				KillingAuraSkill.Player_Update(ref Execute, ref orig, ref player, ref eu);
				if (!Execute) return;
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}

		public static void Player_MovementUpdate(On.Player.orig_MovementUpdate orig, Player player, bool eu)
		{
			bool Execute = true;
			try
			{
				MySlugcatStats.Player_MovementUpdate(ref Execute, ref orig, ref player, ref eu);
				if (!Execute) return;

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}

			orig(player, eu);

			try
			{
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}

		public static void Player_SwallowObject(On.Player.orig_SwallowObject orig, Player player, int grasp)
		{
			bool Execute = true;
			try
			{
				DigestionSkill.Player_SwallowObject(ref Execute, ref orig, ref player, ref grasp);
				if (!Execute) return;
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}

			orig(player, grasp);
		}

		public static void Player_ThrownSpear(On.Player.orig_ThrownSpear orig, Player player, Spear spear)
		{
			bool Execute = true;
			try
			{
				MySlugcatStats.Player_ThrownSpear(ref Execute, ref orig, ref player, ref spear);
				if (!Execute) return;

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}

			orig(player, spear);
		}

		public static void Player_Destroy(On.Player.orig_Destroy orig, Player player)
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

			orig(player);

			try
			{
				PlayerHooks.Player_Destroy(ref Execute, ref orig, ref player);
				if (!Execute) return;

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}

		private static void Player_Die(On.Player.orig_Die orig, Player player)
		{
			bool Execute = true;
			try
			{
				FrameSkill.Player_Die(ref Execute, ref orig, ref player);
				if (!Execute) return;

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}

			bool return_ = player.dead;
			orig(player);

			try
			{
				DeflagrationSkill.Player_Die(ref Execute, ref return_, ref orig, ref player);
				if (!Execute) return;

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}

		public static void PlayerGraphics_InitiateSprites(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics playerGraphics, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
		{
			bool Execute = true;

			orig.Invoke(playerGraphics, sLeaser, rCam);

			try
			{
				if (playerGraphics.player.GetModuleE(out var module).KillingAuraSkill)
				{
					module.KASkill = new KillingAuraSkill();
					module.KASkill.InitiateSprites(ref Execute, ref orig, ref playerGraphics, ref sLeaser, ref rCam);
					if (!Execute) return;
				}
				//重新添加自身的所有图像
				playerGraphics.AddToContainer(sLeaser, rCam, null);
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}

		public static void PlayerGraphics_DrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics playerGraphics, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
		{
			bool Execute = true;

			orig.Invoke(playerGraphics, sLeaser, rCam, timeStacker, camPos);

			try
			{
				if (playerGraphics.player.GetModuleE(out var module).KillingAuraSkill)
				{
					module.KASkill?.DrawSprites(ref Execute, ref orig, ref playerGraphics, ref sLeaser, ref rCam, ref timeStacker, ref camPos);
					if (!Execute) return;
				}
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}

		public static void PlayerGraphics_AddToContainer(On.PlayerGraphics.orig_AddToContainer orig, PlayerGraphics playerGraphics, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContainer)
		{
			bool Execute = true;

			orig.Invoke(playerGraphics, sLeaser, rCam, newContainer);

			try
			{
				if (playerGraphics.player.GetModuleE(out var module).KillingAuraSkill)
				{
					module.KASkill?.AddToContainer(ref Execute, ref orig, ref playerGraphics, ref sLeaser, ref rCam, ref newContainer);
					if (!Execute) return;
				}
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}

		public static void SlugcatStats_ctor(On.SlugcatStats.orig_ctor orig, SlugcatStats slugcatStats, SlugcatStats.Name slugcat, bool malnourished)
		{
			bool Execute = true;
			try
			{
				if (!Execute) return;

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}

			orig(slugcatStats, slugcat, malnourished);

			try
			{
				MySlugcatStats.SlugcatStats_ctor(ref Execute, ref orig, ref slugcatStats, ref slugcat, ref malnourished);
				if (!Execute) return;

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}
		
		
		#endregion

		#region Creature
		public static void Creature_Update(On.Creature.orig_Update orig, Creature creature, bool eu)
		{
			bool Execute = true;

			orig(creature, eu);

			try
			{
				NecrophytesCreature.Creature_Update(ref Execute, ref orig, ref creature, ref eu);
				if (!Execute) return;

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}

		public static void Creature_Die(On.Creature.orig_Die orig, Creature creature)
		{
			bool Execute = true;

			try
			{
				NecrophytesCreature.Creature_Die(ref Execute, ref orig, ref creature);
				if (!Execute) return;

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}

			orig(creature);
		}

		public static void LizardGraphics_DrawSprites(On.LizardGraphics.orig_DrawSprites orig, LizardGraphics lizardGraphics, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
		{
			bool Execute = true;

			orig(lizardGraphics, sLeaser, rCam, timeStacker, camPos);

			try
			{
				NecrophytesCreature.LizardGraphics_DrawSprites(ref Execute, ref orig, ref lizardGraphics, ref sLeaser, ref rCam, ref timeStacker, ref camPos);
				if (!Execute) return;
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}

		public static void ScavengerGraphics_DrawSprites(On.ScavengerGraphics.orig_DrawSprites orig, ScavengerGraphics scavGraphics, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
		{
			bool Execute = true;

			orig(scavGraphics, sLeaser, rCam, timeStacker, camPos);

			try
			{
				NecrophytesCreature.ScavengerGraphics_DrawSprites(ref Execute, ref orig, ref scavGraphics, ref sLeaser, ref rCam, ref timeStacker, ref camPos);
				if (!Execute) return;
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}
		#endregion

		#region Abstract
		private static void AbstractCreature_ctor(On.AbstractCreature.orig_ctor orig, AbstractCreature abstractCreature, World world, CreatureTemplate template, Creature realizedCreature, WorldCoordinate pos, EntityID ID)
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

			orig(abstractCreature, world, template, realizedCreature, pos, ID);

			try
			{
				AbCreatureHooks.AbstractCreature_ctor(ref Execute, ref orig, ref abstractCreature, ref world, ref template, ref realizedCreature, ref pos, ref ID);
				if (!Execute) return;

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}

		public static void AbstractPhysicalObject_ctor(On.AbstractPhysicalObject.orig_ctor orig, AbstractPhysicalObject abPhysicalObject, World world, AbstractPhysicalObject.AbstractObjectType type, PhysicalObject realizedPhysicalObject, WorldCoordinate pos, EntityID ID)
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

			orig(abPhysicalObject, world, type, realizedPhysicalObject, pos, ID);

			try
			{
				AbPhysicalObjectHooks.AbstractPhysicalObject_ctor(ref Execute, ref orig, ref abPhysicalObject, ref world, ref type, ref realizedPhysicalObject, ref pos, ref ID);
				if (!Execute) return;

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}

		private static void AbstractPhysicalObject_Destroy(On.AbstractPhysicalObject.orig_Destroy orig, AbstractPhysicalObject abPhysicalObject)
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

			orig(abPhysicalObject);

			try
			{
				AbPhysicalObjectHooks.AbstractPhysicalObject_Destroy(ref Execute, ref orig, ref abPhysicalObject);
				if (!Execute) return;
				AbCreatureHooks.AbstractPhysicalObject_Destroy(ref Execute, ref orig, ref abPhysicalObject);
				if (!Execute) return;

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}
		#endregion

		#region 挣脱
		private static void Creature_Violence(On.Creature.orig_Violence orig, Creature creature, BodyChunk source, Vector2? directionAndMomentum, BodyChunk hitChunk, PhysicalObject.Appendage.Pos hitAppendage, Creature.DamageType type, float damage, float stunBonus)
		{
			bool Execute = true;
			try
			{
				FrameSkill.Creature_Violence(ref Execute, ref orig, ref creature, ref source, ref directionAndMomentum, ref hitChunk, ref hitAppendage, ref type, ref damage, ref stunBonus);
				if (!Execute) return;

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}

			orig.Invoke(creature, source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);

			try
			{
				if (!Execute) return;

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}

		public static void Lizard_Bite(On.Lizard.orig_Bite orig, Lizard lizard, BodyChunk chunk)
		{
			bool Execute = true;
			try
			{
				FrameSkill.Lizard_Bite(ref Execute, ref orig, ref lizard, ref chunk);
				if (!Execute) return;

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}

			orig.Invoke(lizard, chunk);
		}

		public static void DaddyLongLegs_Eat(On.DaddyLongLegs.orig_Eat orig, DaddyLongLegs daddyLongLegs, bool eu)
		{
			bool Execute = true;
			try
			{
				FrameSkill.DaddyLongLegs_Eat(ref Execute, ref orig, ref daddyLongLegs, ref eu);
				if (!Execute) return;

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}

			orig.Invoke(daddyLongLegs, eu);
		}

		public static void Centipede_UpdateGrasp(On.Centipede.orig_UpdateGrasp orig, Centipede centipede, int g)
		{
			bool Execute = true;
			try
			{
				FrameSkill.Centipede_UpdateGrasp(ref Execute, ref orig, ref centipede, ref g);
				if (!Execute) return;

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}

			orig.Invoke(centipede, g);
		}

		public static void BigEel_JawsSnap(On.BigEel.orig_JawsSnap orig, BigEel bigEel)
		{
			bool Execute = true;
			try
			{
				FrameSkill.BigEel_JawsSnap(ref Execute, ref orig, ref bigEel);
				if (!Execute) return;

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}

			orig.Invoke(bigEel);
		}

		public static void TentaclePlant_Carry(On.TentaclePlant.orig_Carry orig, TentaclePlant tentaclePlant, bool eu)
		{
			bool Execute = true;
			try
			{
				FrameSkill.TentaclePlant_Carry(ref Execute, ref orig, ref tentaclePlant, ref eu);
				if (!Execute) return;

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}

			orig.Invoke(tentaclePlant, eu);
		}

		public static void PoleMimic_Carry(On.PoleMimic.orig_Carry orig, PoleMimic poleMimic, bool eu)
		{
			bool Execute = true;
			try
			{
				FrameSkill.PoleMimic_Carry(ref Execute, ref orig, ref poleMimic, ref eu);
				if (!Execute) return;

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}

			orig.Invoke(poleMimic, eu);
		}

		public static void EggBug_CarryObject(On.EggBug.orig_CarryObject orig, EggBug eggBug, bool eu)
		{
			bool Execute = true;
			try
			{
				FrameSkill.EggBug_CarryObject(ref Execute, ref orig, ref eggBug, ref eu);
				if (!Execute) return;

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}

			orig.Invoke(eggBug, eu);
		}

		public static void Vulture_Carry(On.Vulture.orig_Carry orig, Vulture vulture)
		{
			bool Execute = true;
			try
			{
				FrameSkill.Vulture_Carry(ref Execute, ref orig, ref vulture);
				if (!Execute) return;

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}

			orig.Invoke(vulture);
		}
		#endregion

		#region Bee
		public static void Bee_Update(On.SporePlant.Bee.orig_Update orig, SporePlant.Bee bee, bool eu)
		{
			bool Execute = true;
			try
			{
				KnitmeshSkill.Bee_Update(ref Execute, ref orig, ref bee, ref eu);
				if (!Execute) return;

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}

			orig(bee, eu);
		}

		public static void Bee_ApplyPalette(On.SporePlant.Bee.orig_ApplyPalette orig, SporePlant.Bee bee, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
		{
			bool Execute = true;
			try
			{
				KnitmeshSkill.Bee_ApplyPalette(ref Execute, ref orig, ref bee, ref sLeaser, ref rCam, ref palette);
				if (!Execute) return;

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}

			orig(bee, sLeaser, rCam, palette);
		}

		public static bool Bee_ToHunt(On.SporePlant.Bee.orig_LookForRandomCreatureToHunt orig, SporePlant.Bee bee)
		{
			bool Execute = true;
			bool ret = false;
			try
			{
				ret = KnitmeshSkill.Bee_ToHunt(ref Execute, ref ret, ref orig, ref bee);
				if (!Execute) return ret;

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
			ret = orig(bee);
			return ret;
		}
		#endregion

		#region Weapon
		public static bool Spear_HitSomething(On.Spear.orig_HitSomething orig, Spear spear, SharedPhysics.CollisionResult result, bool eu)
		{
			//Log.OutputLog($"spear({spear != null})_thrownBy({spear?.thrownBy?.GetType()})_r({result.obj?.GetType()})_");
			bool Execute = true;
			bool ret = false;
			try
			{
				if (!spear.HitThisObject(result.obj))
				{
					result.obj = null;
					ret = false;
					Execute = false;
					if (!Execute) return ret;
				}

				ret = FrameSkill.Spear_HitSomething(ref Execute, ref ret, ref orig, ref spear, ref result, ref eu);
				if (!Execute) return ret;
				ret = PenetrationSkill.PenetrateHit(ref Execute, ref ret, ref spear, ref result, ref eu);
				if (!Execute) return ret;
				ret = DeflagrationSkill.DeflagrationHit(ref Execute, ref ret, ref spear, ref result, ref eu);
				if (!Execute) return ret;

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
			ret = orig(spear, result, eu);
			Log.OutputLog($"Spear ret({ret})_");
			return ret;
		}

		public static bool ScavengerBomb_HitSomething(On.ScavengerBomb.orig_HitSomething orig, ScavengerBomb bomb, SharedPhysics.CollisionResult result, bool eu)
		{
			bool Execute = true;
			bool ret = false;
			try
			{
				ret = FrameSkill.ScavengerBomb_HitSomething(ref Execute, ref ret, ref orig, ref bomb, ref result, ref eu);
				if (!Execute) return ret;
				ret = DeflagrationSkill.DeflagrationHit(ref Execute, ref ret, ref bomb, ref result, ref eu);
				if (!Execute) return ret;
				ret = PenetrationSkill.PenetrateHit(ref Execute, ref ret, ref bomb, ref result, ref eu);
				//ret = PenetrationSkill.ScavengerBomb_HitSomething(ref Execute, ref ret, ref orig, ref bomb, ref result, ref eu);
				if (!Execute) return ret;

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
			ret = orig(bomb, result, eu);
			Log.OutputLog($"Rock ret({ret})_");
			return ret;
		}

		public static bool Rock_HitSomething(On.Rock.orig_HitSomething orig, Rock rock, SharedPhysics.CollisionResult result, bool eu)
		{
			//Log.OutputLog($"rock({rock != null})_thrownBy({rock?.thrownBy?.GetType()})_r({result.obj?.GetType()})_");
			bool Execute = true;
			bool ret = false;
			try
			{
				ret = DeflagrationSkill.DeflagrationHit(ref Execute, ref ret, ref rock, ref result, ref eu);
				if (!Execute) return ret;
				ret = PenetrationSkill.PenetrateHit(ref Execute, ref ret, ref rock, ref result, ref eu);
				//ret = PenetrationSkill.Rock_HitSomething(ref Execute, ref ret, ref orig, ref rock, ref result, ref eu);
				if (!Execute) return ret;

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
			ret = orig(rock, result, eu);
			Log.OutputLog($"Rock ret({ret})_");
			return ret;
		}

		public static bool Boomerang_HitSomething(On.Boomerang.orig_HitSomething orig, Boomerang boomerang, SharedPhysics.CollisionResult result, bool eu)
		{
			bool Execute = true;
			bool ret = false;
			try
			{
				ret = DeflagrationSkill.DeflagrationHit(ref Execute, ref ret, ref boomerang, ref result, ref eu);
				if (!Execute) return ret;
				ret = PenetrationSkill.PenetrateHit(ref Execute, ref ret, ref boomerang, ref result, ref eu);
				if (!Execute) return ret;

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
			ret = orig(boomerang, result, eu);
			Log.OutputLog($"boomerang ret({ret})_");
			return ret;
		}

		private static bool LillyPuck_HitSomething(On.MoreSlugcats.LillyPuck.orig_HitSomething orig, LillyPuck lillyPuck, SharedPhysics.CollisionResult result, bool eu)
		{
			bool Execute = true;
			bool ret = false;
			try
			{
				ret = DeflagrationSkill.DeflagrationHit(ref Execute, ref ret, ref lillyPuck, ref result, ref eu);
				if (!Execute) return ret;

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
			ret = orig(lillyPuck, result, eu);
			Log.OutputLog($"lillyPuck ret({ret})_");
			return ret;
		}

		public static bool PuffBall_HitSomething(On.PuffBall.orig_HitSomething orig, PuffBall puffBall, SharedPhysics.CollisionResult result, bool eu)
		{
			bool Execute = true;
			bool ret = false;
			try
			{
				ret = DeflagrationSkill.DeflagrationHit(ref Execute, ref ret, ref puffBall, ref result, ref eu);
				if (!Execute) return ret;

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
			ret = orig(puffBall, result, eu);
			Log.OutputLog($"PuffBall ret({ret})_");
			return ret;
		}

		public static bool Weapon_HitSomething(On.Weapon.orig_HitSomething orig, Weapon weapon, SharedPhysics.CollisionResult result, bool eu)
		{
			//Log.OutputLog($"weapon({weapon != null})_r({result.obj?.GetType()})_");
			bool Execute = true;
			bool ret = false;
			try
			{
				ret = PenetrationSkill.PenetrateHit(ref Execute, ref ret, ref weapon, ref result, ref eu);
				//ret = PenetrationSkill.Weapon_HitSomething(ref Execute, ref ret, ref orig, ref weapon, ref result, ref eu);
				if (!Execute) return ret;

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
			ret = orig(weapon, result, eu);
			Log.OutputLog($"Weaponret({ret})_");
			return ret;
		}

		public static void Spear_SetRandomSpin(On.Spear.orig_SetRandomSpin orig, Spear spear)
		{
			bool Execute = true;

			orig(spear);

			try
			{
				DeflagrationSkill.Spear_SetRandomSpin(ref Execute, ref orig, ref spear);
				if (!Execute) return;
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}

		public static void Weapon_SetRandomSpin(On.Weapon.orig_SetRandomSpin orig, Weapon weapon)
		{
			bool Execute = true;

			orig(weapon);

			try
			{
				DeflagrationSkill.Weapon_SetRandomSpin(ref Execute, ref orig, ref weapon);
				if (!Execute) return;
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}

		public static void LillyPuck_SetRandomSpin(On.MoreSlugcats.LillyPuck.orig_SetRandomSpin orig, LillyPuck lillyPuck)
		{
			bool Execute = true;

			orig(lillyPuck);

			try
			{
				DeflagrationSkill.LillyPuck_SetRandomSpin(ref Execute, ref orig, ref lillyPuck);
				if (!Execute) return;
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
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

		public static void Weapon_HitAnotherThrownWeapon(On.Weapon.orig_HitAnotherThrownWeapon orig, Weapon weapon, Weapon obj)
		{
			bool Execute = true;
			try
			{
				PenetrationSkill.Weapon_HitWeapon(ref Execute, ref weapon, ref obj);
				if (!Execute) return;
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}

			orig(weapon, obj);
		}

		private static void PuffBall_Explode(On.PuffBall.orig_Explode orig, PuffBall puffBall)
		{
			bool Execute = true;

			orig(puffBall);

			try
			{
				DeflagrationSkill.PuffBall_Explode(ref Execute, ref orig, ref puffBall);
				if (!Execute) return;
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}

		public static void FlareBomb_StartBurn(On.FlareBomb.orig_StartBurn orig, FlareBomb flareBomb)
		{
			bool Execute = true;

			orig(flareBomb);

			try
			{
				DeflagrationSkill.FlareBomb_StartBurn(ref Execute, ref orig, ref flareBomb);
				if (!Execute) return;
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}
		#endregion

		#region HUD
		private static void HUD_InitSinglePlayerHud(On.HUD.HUD.orig_InitSinglePlayerHud orig, HUD.HUD HUD, RoomCamera cam)
		{
			bool Execute = true;
			try
			{
				VisionSystem.HUD_InitSinglePlayerHud(ref Execute, ref orig, ref HUD, ref cam);
				if (!Execute) return;

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}

			orig.Invoke(HUD, cam);
		}

		private static void HUD_InitMultiplayerHud(On.HUD.HUD.orig_InitMultiplayerHud orig, HUD.HUD HUD, ArenaGameSession session)
		{
			bool Execute = true;
			try
			{
				VisionSystem.HUD_InitMultiplayerHud(ref Execute, ref orig, ref HUD, ref session);
				if (!Execute) return;

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}

			orig.Invoke(HUD, session);
		}

		private static void HUD_InitSafariHud(On.HUD.HUD.orig_InitSafariHud orig, HUD.HUD HUD, RoomCamera cam)
		{
			bool Execute = true;
			try
			{
				VisionSystem.HUD_InitSafariHud(ref Execute, ref orig, ref HUD, ref cam);
				if (!Execute) return;

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}

			orig.Invoke(HUD, cam);
		}
		#endregion

		#region Friends of friends
		public static void DynamicRelationship_Update(On.RelationshipTracker.DynamicRelationship.orig_Update orig, RelationshipTracker.DynamicRelationship self)
		{
			bool Execute = true;
			try
			{
				friends_of_friends.DynamicRelationship_Update(ref Execute, ref orig, ref self);
				if (!Execute) return;

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}

			orig(self);
		}

		public static bool LizardAI_DoIWantToBiteThisCreature(On.LizardAI.orig_DoIWantToBiteThisCreature orig, LizardAI lizardAI, Tracker.CreatureRepresentation otherCrit)
		{
			bool Execute = true;
			bool ret = true;
			try
			{
				ret = friends_of_friends.LizardAI_DoIWantToBiteThisCreature(ref Execute, ref ret, ref orig, ref lizardAI, ref otherCrit);
				if (!Execute) return ret;

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}

			ret = orig(lizardAI, otherCrit);
			return ret;
		}
		#endregion

		public static bool RegionGate_customOEGateRequirements(On.RegionGate.orig_customOEGateRequirements orig, RegionGate regionGate)
		{
			bool Execute = true;
			bool ret = false;
			try
			{
				ret = MyGame.RegionGate_customOEGateRequirements(ref Execute, ref ret, ref orig, ref regionGate);
				if (!Execute) return ret;

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}

			ret = orig(regionGate);
			return ret;
		}

	}
}
