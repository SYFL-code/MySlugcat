using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using MySlugcat;

internal static class PlayerModuleManager
{
	public static ConditionalWeakTable<Player, PlayerModule> playerModules = new ConditionalWeakTable<Player, PlayerModule>();

	internal class PlayerModule
	{
		WeakReference<Player> playerRef;

        //超级跳设置


		int HungryCoolDown = 12000;//冷却计时器

        public PlayerModule(Player player)
		{
			playerRef = new WeakReference<Player>(player);
			SetUp(player.SlugCatClass);
		}

		void SetUp(SlugcatStats.Name name)
		{

		}

		public void Hungry_Update(Player player)
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



		}

	}
}

internal static class PlayerHooks
{

	public static void HookOn()
	{
		On.Player.ctor += Player_ctor;
		On.Player.Update += Player_Update;
	}



	private static void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
	{
		orig.Invoke(self, eu);
		if (PlayerModuleManager.playerModules.TryGetValue(self, out var module))
		{
			module.Hungry_Update(self);
		}
	}

	private static void Player_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
	{
		orig.Invoke(self, abstractCreature, world);
		PlayerModuleManager.playerModules.Add(self, new PlayerModuleManager.PlayerModule(self));
	}
}
