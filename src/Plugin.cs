using System;
using BepInEx;
using UnityEngine;
using SlugBase.Features;
using static SlugBase.Features.FeatureTypes;
using BepInEx.Logging;
using System.Security;
using System.Security.Permissions;
using Noise;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System.Linq;
using MoreSlugcats;
using MonoMod.RuntimeDetour;
using CustomPassages = CustomPassageSupport.CustomPassageSupport;
using System.Reflection;

// Allows access to private members 允许访问私有成员
#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618

namespace MySlugcat
{
	[BepInPlugin(MOD_ID, "The Accommodator", "0.1.0")]
	[BepInDependency("fluffball.custompassages", BepInDependency.DependencyFlags.HardDependency)]
	// Hard Dependency 硬依赖
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

			//Content.Register(new EnderPearlFisob());
			//玩家能力
			Hook.HookOn();
			//Perception.Hook();
			//IntelHUD.Hook();
			//PointerSkillHook.Hook();
			FrameSkill.Hook();
			KillingAuraHook.Hook();

			//Intros.Hook();
			FixedSkill.Hook();

			//Intros.DoPatching();
		}

		private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
		{
			orig.Invoke(self);
			//加载设置菜单
			MachineConnector.SetRegisteredOI(MOD_ID, Options.Instance);

			// 注册自定义通行证
			On.RainWorld.Start += (orig, self) =>
			{
				orig(self);
				CustomPassages.RegisterPassage(new MyPassage());
			};
		}


		// Load any resources, such as sprites or sounds-加载任何资源 包括图像素材和音效
		private void LoadResources(RainWorld rainWorld)
		{

		}

		private void Player_Update(On.Player.orig_Update orig, Player player, bool eu)
		{
			orig(player, eu);

		}

	}
}