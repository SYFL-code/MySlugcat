/*using System;
using System.Collections.Generic;
using BepInEx;
using MoreSlugcats;
using On;
using UnityEngine;


namespace MySlugcat
{
    //已废弃
    public class Passage
    {
		*//*public void OnEnable()
		{
			On.WinState.CycleCompleted += WinState_CycleCompleted;
		}

		private void WinState_CycleCompleted(On.WinState.orig_CycleCompleted orig, WinState self, RainWorldGame game)
		{
			if (self.GetTracker(WinState.EndgameID.Chieftain, addIfMissing: false) is WinState.FloatTracker chieftain && chieftain.progress >= chieftain.max)
			{
				orig(self, game);

				chieftain.progress = chieftain.max;
			}
			else
			{
				orig(self, game);
			}
		}*/


		/*private void WinStateConsumeEndGameHook(On.WinState.orig_ConsumeEndGame orig, WinState self)
        {
            orig(self);
        }*//*

		//"The Survivor"        //"幸存者"
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


		*//*private WinState.EndgameID WinStateGetNextEndGame(On.WinState.orig_GetNextEndGame orig, WinState self)
        {
            *//*SC.ownedPassages = new List<string>();
            if (self.endgameTrackers.Count > 0)
            {
                for (int i = 0; i < self.endgameTrackers.Count; i++)
                {
                    if (self.endgameTrackers[i].GoalFullfilled)
                    {
                        SC.ownedPassages.Add(WinState.PassageDisplayName(self.endgameTrackers[i].ID));
                    }
                }
            }*//*

            return orig(self);

            *//*WinState.EndgameID result;
            if (self.endgameTrackers.Count < 1)
            {
                result = orig.Invoke(self);
            }
            else
            {
                this.ownedIDs = new List<WinState.EndgameID>();
                for (int i = 0; i < self.endgameTrackers.Count; i++)
                {
                    // 消耗
                    self.endgameTrackers[i].consumed = false;

                    // 达成
                    if (self.endgameTrackers[i].GoalFullfilled && (!ModManager.MSC || self.endgameTrackers[i].ID != MoreSlugcatsEnums.EndgameID.Gourmand))
                    {
                        this.ownedIDs.Add(self.endgameTrackers[i].ID);
                    }
                }
                int index = UnityEngine.Random.Range(0, this.ownedIDs.Count);
                Debug.Log("Has " + this.ownedIDs.Count.ToString() + " Passages");
                for (int j = 0; j < 20; j++)
                {
                    int num = UnityEngine.Random.Range(0, this.ownedIDs.Count);
                    Debug.Log(num);
                    Debug.Log(this.ownedIDs[num].ToString() + " could be used to passage");
                }
                for (int k = 0; k < this.ownedIDs.Count; k++)
                {
                    Debug.Log(this.ownedIDs[k].ToString() + " is an owned passage");
                }
                Debug.Log(this.ownedIDs[index].ToString() + " has been used to passage");
                result = this.ownedIDs[index];
            }
            return result;*//*
        }*//*


	}
}
*/