using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using UnityEngine;
using MySlugcat;


namespace MySlugcat;
internal static class PlayerModuleManager
{
    //public static ConditionalWeakTable<SlugcatStats, Player> slugcatStatsPlayers = new ConditionalWeakTable<SlugcatStats, Player>();
    public static ConditionalWeakTable<Player, PlayerModule> playerModules = new ConditionalWeakTable<Player, PlayerModule>();
    public static List<WeakReference<Player>> players = new List<WeakReference<Player>>();

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
                DigestionSkill = false;
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


        //"The Survivor"        //"求生者"
        //"The Hunter"          //"猎手"
        //"The Saint"           //"圣徒"
        //"The Wanderer"        //"漫游者"
        //"The Chieftain"       //"酋长"
        //"The Monk"            //"僧侣"
        //"The Outlaw"          //"暴徒"
        //"The Dragon Slayer"   //"屠龙者"
        //"The Scholar"         //"学者"
        //"The Friend"          //"朋友"
        // ModManager.MSC
        //"The Nomad"           //"流浪者"
        //"The Martyr"          //"殉道者"
        //"The Pilgrim"         //"朝圣者"
        //"The Mother"          //"慈母"
        //



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
		//On.Player.Update += Player_Update;
	}

    private static void Player_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
    {
        orig.Invoke(self, abstractCreature, world);
        PlayerModuleManager.players.Add(new WeakReference<Player>(self));
        PlayerModuleManager.playerModules.Add(self, new PlayerModuleManager.PlayerModule(self));
    }

    /*private static void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
	{
		orig.Invoke(self, eu);
		if (PlayerModuleManager.playerModules.TryGetValue(self, out var module))
		{
			module.Hungry_Update(self);
		}
	}*/
}
