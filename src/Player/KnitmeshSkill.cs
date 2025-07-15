using On;
using IL;
using System;
using Mono.Cecil;
using MoreSlugcats;
using Noise;
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
using static MonoMod.InlineRT.MonoModRule;


namespace MySlugcat
{
    //缠绕技能
    public class KnitmeshSkill
    {

        public static void Hook()
        {
            On.Player.Update += Player_Update;
            On.SporePlant.Bee.ApplyPalette += Bee_ApplyPalette;
            On.SporePlant.Bee.LookForRandomCreatureToHunt += Bee_ToHunt;
        }

        private static void Knitmesh(Player self, Room room, Vector2 pos)
        {
            List<Creature>? creatures = MyPlayer.CreaturesInRange(room, pos, 100f, false, self, false, true);

            if (creatures != null)
            {
                foreach (Creature creature in creatures)
                {
                    if (creature != null)
                    {
                        Vector2 offset = creature.mainBodyChunk.pos - pos;
                        float Distance = offset.sqrMagnitude;

                        Vector2 V1 = new Vector2(UnityEngine.Random.Range(-30, 31), UnityEngine.Random.Range(-30, 31));
                        Vector2 V2 = new Vector2(UnityEngine.Random.Range(-30, 31), UnityEngine.Random.Range(-30, 31));
                        Vector2 V3 = new Vector2(UnityEngine.Random.Range(-30, 31), UnityEngine.Random.Range(-30, 31));
                        Vector2 V4 = new Vector2(UnityEngine.Random.Range(-30, 31), UnityEngine.Random.Range(-30, 31));

                        for (int i = 0; i < 30; i++)
                        {
                            SporePlant.Bee bee = new SporePlant.Bee(null, true, self.firstChunk.pos + V1, new Vector2(0f, 0f), SporePlant.Bee.Mode.Hunt);
                            SporePlant.Bee bee2 = new SporePlant.Bee(null, true, self.mainBodyChunk.pos + V3, new Vector2(0f, 0f), SporePlant.Bee.Mode.Hunt);
                            bee.blackColor = new Color(0.0002f, 0, 0.989f);
                            bee.ignoreCreature = self;
                            //bee.room.RoomRect
                            bee.forceAngry = true;
                            creature.room.AddObject(bee);
                            bee2.blackColor = new Color(0.0002f, 0, 0.989f);
                            bee2.ignoreCreature = self;
                            //bee.room.RoomRect
                            bee2.forceAngry = true;
                            creature.room.AddObject(bee2);
                        }
                        creature.room.PlaySound(SoundID.Spore_Bees_Emerge, creature.firstChunk);

                        for (int i = 0; i < 1000 / Distance; i++)
                        {
                            SporePlant.Bee bee = new SporePlant.Bee(null, true, creature.firstChunk.pos + V2, new Vector2(0f, 0f), SporePlant.Bee.Mode.Hunt);
                            SporePlant.Bee bee2 = new SporePlant.Bee(null, true, creature.mainBodyChunk.pos + V4, new Vector2(0f, 0f), SporePlant.Bee.Mode.Hunt);
                            bee.blackColor = new Color(0.0002f,0,0.989f);
                            bee.ignoreCreature = self;
                            //bee.room.RoomRect
                            bee.forceAngry = true;
                            creature.room.AddObject(bee);
                            bee2.blackColor = new Color(0.0002f, 0, 0.989f);
                            bee2.ignoreCreature = self;
                            //bee.room.RoomRect
                            bee2.forceAngry = true;
                            creature.room.AddObject(bee2);
                        }
                        
                    }

                }
            }

        }

        private static void Player_Update(On.Player.orig_Update orig, Player player, bool eu)
        {
            orig(player, eu);

            //Log.Logger(10, "Knitmesh", "MySlugcat:KnitmeshSkill​​:Player_Update", $"({player.slugcatStats.name == Plugin.YourSlugID})");
            if (player.slugcatStats.name == Plugin.YourSlugID && Options.KnitmeshSkill != null && Options.KnitmeshSkill.Value)
            {
                if (player.input[0].mp && !player.input[1].mp)
                {
                    Knitmesh(player, player.room, player.mainBodyChunk.pos);
                }
            }
        }

        private static bool Bee_ToHunt(On.SporePlant.Bee.orig_LookForRandomCreatureToHunt orig, SporePlant.Bee bee)
        {
            //bool obj = orig(bee);

            if (ModManager.MMF && !MMF.cfgVanillaExploits.Value && bee.room.abstractRoom.gate && bee.room.regionGate.waitingForWorldLoader)
            {
                return false;
            }
            if (bee.huntChunk != null)
            {
                return false;
            }
            if (bee.room.abstractRoom.creatures.Count > 0)
            {
                AbstractCreature abstractCreature = bee.room.abstractRoom.creatures[UnityEngine.Random.Range(0, bee.room.abstractRoom.creatures.Count)];

                if (abstractCreature.realizedCreature != null && abstractCreature.realizedCreature.room == bee.room && 
                    (abstractCreature.rippleLayer == bee.rippleLayer || abstractCreature.rippleBothSides || bee.rippleBothSides) && 
                    (bee.ignoreCreature == null || abstractCreature.realizedCreature != bee.ignoreCreature) && 
                    SporePlant.SporePlantInterested(abstractCreature.realizedCreature.Template.type) && bee.blackColor != new Color(0.0002f, 0, 0.989f))
                {
                    for (int i = 0; i < abstractCreature.realizedCreature.bodyChunks.Length; i++)
                    {
                        if (Custom.DistLess(bee.pos, abstractCreature.realizedCreature.bodyChunks[i].pos, abstractCreature.realizedCreature.bodyChunks[i].rad))
                        {
                            bee.Attach(abstractCreature.realizedCreature.bodyChunks[i]);
                            return true;
                        }
                    }
                    return bee.HuntChunkIfPossible(abstractCreature.realizedCreature.bodyChunks[UnityEngine.Random.Range(0, abstractCreature.realizedCreature.bodyChunks.Length)]);
                }
            }
            if (UnityEngine.Random.value < 0.1f && bee.owner != null && bee.owner.attachedBees.Count > 0)
            {
                SporePlant.AttachedBee attachedBee = bee.owner.attachedBees[UnityEngine.Random.Range(0, bee.owner.attachedBees.Count)];
                if (!attachedBee.slatedForDeletetion && attachedBee.attachedChunk != null)
                {
                    return bee.HuntChunkIfPossible(attachedBee.attachedChunk.owner.bodyChunks[UnityEngine.Random.Range(0, attachedBee.attachedChunk.owner.bodyChunks.Length)]);
                }
            }
            return false;
        }

        private static void Bee_ApplyPalette(On.SporePlant.Bee.orig_ApplyPalette orig, SporePlant.Bee bee, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            if (bee.blackColor != new Color(0.0002f, 0, 0.989f))
            {
                Log.Logger(6, "Knitmesh", "MySlugcat:KnitmeshSkill​​:Bee_ApplyPalette", $"({palette.blackColor.ToString()})");
                bee.blackColor = palette.blackColor;
            }
            
        }


    }
}
