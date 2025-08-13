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
    sealed class EnderPearl : Weapon
    {
        public static float scale = 0.8f;

        public EnderPearlAbstract Abstr { get; }

        public static void HookTexture()
        {
            //Futile.atlasManager.LoadAtlas("atlases/icon_EnderPearl");
            Futile.atlasManager.LoadAtlas("icon_EnderPearl");
        }

        public EnderPearl(EnderPearlAbstract abstr, Vector2 pos) : base(abstr, abstr.world)
        {
            Abstr = abstr;

            bodyChunks = new BodyChunk[1];
            bodyChunks[0] = new BodyChunk(this, 0, pos, 4 * (Abstr.scaleX + Abstr.scaleY), 0.07f);
            bodyChunkConnections = new BodyChunkConnection[0];
            airFriction = 0.999f;
            gravity = 0.9f;
            bounce = 0.4f;
            surfaceFriction = 0.4f;
            collisionLayer = 2;
            waterFriction = 0.6f;
            buoyancy = 1f;
            firstChunk.loudness = 9f;
            firstChunk.pos = pos;

            Abstr.scaleX = scale;
            Abstr.scaleY = scale;
        }

        private void Shatter()
        {
            AllGraspsLetGoOfThisObject(true);
            abstractPhysicalObject.LoseAllStuckObjects();
            Destroy();
        }

        public override void Update(bool eu)
        {
            base.Update(eu);
        }

        public override bool HitSomething(SharedPhysics.CollisionResult result, bool eu)
        {
            if (result.obj == null)
            {
                return false;
            }

            //vibrate = 20;
            ChangeMode(Mode.Free);
/*            if (result.obj is Creature creature)
            {
                //将生物击退
                creature.firstChunk.vel += firstChunk.vel.normalized;
            }*/
            if (result.obj != null)
            {
                if (thrownBy != null && thrownBy is Player player && (player.room != null && !player.inShortcut))
                {
                    Teleport.SetObjectPosition(player, firstChunk.pos);
                }

                //WindList.WindExplosioListAdd(room, thrownBy, firstChunk.pos, true);
                //使武器消失
                Shatter();
            }

            return true;
        }

        public override void TerrainImpact(int chunk, IntVector2 direction, float speed, bool firstContact)
        {
            if (mode == Mode.Thrown)
            {
                if (thrownBy != null && thrownBy is Player player && (player.room != null && !player.inShortcut))
                {
                    Teleport.SetObjectPosition(player, firstChunk.pos);
                }

                //WindList.WindExplosioListAdd(room, thrownBy, firstChunk.pos, true);
                //使武器消失
                Shatter();
            }

            base.TerrainImpact(chunk, direction, speed, firstContact);
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[1];
            sLeaser.sprites[0] = new FSprite("icon_EnderPearl", false);
            AddToContainer(sLeaser, rCam, null);
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            Vector2 pos = Vector2.Lerp(firstChunk.lastPos, firstChunk.pos, timeStacker);
            sLeaser.sprites[0].x = pos.x - camPos.x;
            sLeaser.sprites[0].y = pos.y - camPos.y;

            sLeaser.sprites[0].scaleX = scale;
            sLeaser.sprites[0].scaleY = scale;

            if (slatedForDeletetion || room != rCam.room)
            {
                sLeaser.CleanSpritesAndRemove();
            }
        }



    }
}
