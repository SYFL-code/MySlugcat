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


namespace MySlugcat
{
    // SkillControl 能力控制中心

    public class SC
    {
        public static bool Exhausted = true;          // 精疲力竭
        public static bool Frame​​Skill = false;        // 嫁祸能力
        public static bool Deflagration​​Skill = false; // 爆燃能力
        public static bool KnitmeshSkill = false;     // 缠绕能力
        public static bool PerceptionSkill = false;   // 感知能力
        public static bool DigestionSkill = false;    // 暴食能力
        public static bool FixedSkill = false;        // 定身能力

        public static void Hook()
        {
#if MYDEBUG
            try
            {
#endif
            On.Player.ctor += Player_ctor;

#if MYDEBUG
            }
            catch (Exception e)
            {
                StackTrace st = new StackTrace(new StackFrame(true));
                StackFrame sf = st.GetFrame(0);
                var sr = sf.GetFileName().Split('\\');
                MyDebug.outStr = sr[sr.Length - 1] + "\n";
                MyDebug.outStr += sf.GetMethod() + "\n";
                MyDebug.outStr += e;
                UnityEngine.Debug.Log(e);
            }
#endif
        }

        private static void Player_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
        {
            orig.Invoke(self, abstractCreature, world);

            Exhausted = true;
            Frame​​Skill = true;
            Deflagration​​Skill = true;
            KnitmeshSkill = true;
            PerceptionSkill = true;
            DigestionSkill = true;
            FixedSkill = true;
        }


    }
}
