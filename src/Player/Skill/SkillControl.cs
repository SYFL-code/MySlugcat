﻿using On;
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


namespace MySlugcat
{
    // SkillControl 能力控制中心
    public class SC
    {
        private static bool StartRunning = true;

        public static int  MySlugcatStats = 0;        // 蛞蝓猫数据
        public static bool Exhausted = true;          // 精疲力竭
        public static bool Frame​​Skill = false;        // 嫁祸能力
        public static bool Deflagration​​Skill = false; // 爆燃能力
        public static bool KnitmeshSkill = false;     // 缠绕能力
        public static bool PerceptionSkill = false;   // 感知能力
        public static bool DigestionSkill = false;    // 暴食能力
        public static bool FixedSkill = false;        // 定身能力

        public static void Hook()
        {
            On.RainWorldGame.Update += RainWorldGame_Update;
            //On.Player.ctor += Player_ctor;
        }

        public static void RainWorldGame_Update(On.RainWorldGame.orig_Update orig, RainWorldGame rainWorldGame)
        {
            orig(rainWorldGame);

            if (StartRunning)
            {
                MySlugcatStats = 0;
                Exhausted = true;
                Frame​​Skill = true;
                Deflagration​​Skill = true;
                KnitmeshSkill = true;
                PerceptionSkill = true;
                DigestionSkill = true;
                FixedSkill = true;

                StartRunning = false;
            }
        }

        /*private static void Player_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
        {
            orig.Invoke(self, abstractCreature, world);


        }*/


    }
}
