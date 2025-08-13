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


namespace MySlugcat
{
    //末影珍珠
    sealed class EnderPearlAbstract : AbstractPhysicalObject
    {

        public EnderPearlAbstract(World world, WorldCoordinate pos, EntityID ID) : base(world, EnderPearlFisob.EnderPearls, null, pos, ID)
        {
            scaleX = EnderPearl.scale;
            scaleY = EnderPearl.scale;
            saturation = 0.5f;
            hue = 1f;
        }

        public override void Realize()
        {
            base.Realize();
            if (realizedObject == null)
            {
                realizedObject = new EnderPearl(this, Room.realizedRoom.MiddleOfTile(pos.Tile));
            }
        }

        public float hue;
        public float saturation;
        public float scaleX;
        public float scaleY;


    }
}
