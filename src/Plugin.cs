using System;
using BepInEx;
using UnityEngine;
using SlugBase.Features;
using static SlugBase.Features.FeatureTypes;
using BepInEx.Logging;


namespace MySlugcat
{
	[BepInPlugin(MOD_ID, "The accommodator", "0.1.0")]
	class Plugin : BaseUnityPlugin
	{
		//spawn_raw EnderPearl
		//设置ModID
		private const string MOD_ID = "theaccommodator.LH";
		//用于检查角色id
		public static readonly SlugcatStats.Name YourSlugID = new SlugcatStats.Name("theaccommodator.LH", false);


		public void OnEnable()
		{
			//Plugin.Logger = base.Logger;

			//mod初始化
			On.RainWorld.OnModsInit += RainWorld_OnModsInit;
			On.RainWorld.OnModsInit += Extras.WrapInit(LoadResources);
			//Control.Hook();
			MySlugcatStats.Hook();
			//Content.Register(new EnderPearlFisob());
			//玩家能力
			MyPlayer.Hook();
			PlayerHooks.HookOn();
            //Exhausted.Hook();
            Control.Hook();
			VisionSystem.Hook();
			NecrophytesCreature.Hook();
			Key.Hook();
			//Perception.Hook();
			//IntelHUD.Hook();
			//PointerSkillHook.Hook();
			FrameSkill.Hook();
			DeflagrationSkill.Hook();
			KnitmeshSkill.Hook();
			DigestionSkill.Hook();

			//游戏内容设置
			MyGame.Hook();

			Intros.Hook();
			FixedSkill.Hook();
		}

		private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
		{
			orig.Invoke(self);
			//加载设置菜单
			MachineConnector.SetRegisteredOI(MOD_ID, new Options());
		}


		// Load any resources, such as sprites or sounds-加载任何资源 包括图像素材和音效
		private void LoadResources(RainWorld rainWorld)
		{
			//EnderPearl.HookTexture();
		}

		private void Player_Update(On.Player.orig_Update orig, Player player, bool eu)
		{
			orig(player, eu);


		}

	}
}