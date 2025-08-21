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
using System.Threading;
using Expedition;
using BepInEx.Logging;


namespace MySlugcat
{
	// Control 控制中心
	public class Control
    {
		public static float pixelSize = 15f;          // 像素大小
		public static float Alpha = 0.9f;             // 像素不透明度

        public static bool AllPlayerSkill = false;

		public static bool DebugMode = false;
		public static bool CheatMode = false;

		public static bool LogDebug = false;
		public static float Loglevel = 10f;

		/*public static void Hook()
		{
			On.RainWorldGame.Update += RainWorldGame_Update;
			On.Player.ctor += Player_ctor;
            //On.Player.Update += Player_Update;
		}*/

        public static void Player_ctor(ref bool Execute, ref On.Player.orig_ctor orig, ref Player player, ref AbstractCreature abstractCreature, ref World world)
        {
            if (Options.Instance.PixelSize != null && Options.Instance.PixelSize.Value != null)
            {
                pixelSize = Options.Instance.PixelSize.Value;
            }
			if (Options.Instance.Alpha != null && Options.Instance.Alpha.Value != null)
			{
				Alpha = Options.Instance.Alpha.Value;
			}
			if (Options.Instance.AllPlayerSkill != null && Options.Instance.AllPlayerSkill.Value != null)
			{
				AllPlayerSkill = Options.Instance.AllPlayerSkill.Value;
			}
		}

		public static void RainWorldGame_Update(ref bool Execute, ref On.RainWorldGame.orig_Update orig, ref RainWorldGame rainWorldGame)
		{
			if (Options.Instance.LogDebug != null && Options.Instance.LogDebug.Value != null)
			{
				LogDebug = Options.Instance.LogDebug.Value;
			}
			if (Options.Instance.Loglevel != null && Options.Instance.Loglevel.Value != null)
			{
				Loglevel = Options.Instance.Loglevel.Value;
			}
		}



	}
}

