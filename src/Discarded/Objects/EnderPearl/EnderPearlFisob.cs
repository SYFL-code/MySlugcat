using On;
using IL;
using System;
using Mono.Cecil;
using MoreSlugcats;
using RWCustom;
using HUD;
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
using Fisobs.Items;
using Fisobs.Core;
using Fisobs.Sandbox;


namespace MySlugcat
{
    //末影珍珠
    sealed class EnderPearlFisob : Fisob
    {
#nullable enable

        public static readonly AbstractPhysicalObject.AbstractObjectType EnderPearls = new("EnderPearl", true);
        public static readonly MultiplayerUnlocks.SandboxUnlockID EnderPearl = new("EnderPearl", true);

        public EnderPearlFisob() : base(EnderPearls)
        {
            // Fisobs auto-loads the `icon_EnderPearl` embedded resource as a texture.
            // See `EnderPearls.csproj` for how you can add embedded resources to your project.

            // If you want a simple grayscale icon, you can omit the following line.
            Icon = new EnderPearlIcon();

            SandboxPerformanceCost = new(linear: 0.35f, exponential: 0f);


            RegisterUnlock(EnderPearl, parent: MultiplayerUnlocks.SandboxUnlockID.KingVulture, data: 0);
        }

        public override AbstractPhysicalObject Parse(World world, EntitySaveData saveData, SandboxUnlock? unlock)
        {

            var result = new EnderPearlAbstract(world, saveData.Pos, saveData.ID);

            return result;
        }


    }
}
