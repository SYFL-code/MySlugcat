using On;
using IL;
using System;
using Mono.Cecil;
using MoreSlugcats;
using RWCustom;
using HUD;
using Smoke;
using Fisobs.Core;
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


namespace MySlugcat
{
    //末影珍珠
    sealed class EnderPearlIcon : Icon
    {

        // Vanilla only gives you one int field to store all your custom data.
        // Here, that int field is used to store the shield's hue, scaled by 1000.
        // So, 0 is red and 70 is orange.
        public override int Data(AbstractPhysicalObject apo)
        {
            return 0;
        }

        public override Color SpriteColor(int data)
        {
            return new Color(128, 128, 128, 1);
        }

        public override string SpriteName(int data)
        {
            // Fisobs autoloads the file in the mod folder named "icon_{Type}.png"
            // To use that, just remove the png suffix: "icon_EnderPearl"
            return "icon_EnderPearl";
        }

    }
}
