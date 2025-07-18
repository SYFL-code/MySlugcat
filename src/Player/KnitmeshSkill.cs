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
            On.SporePlant.Bee.Update += Bee_Update;
        }

        private static void Knitmesh(Player self, Room room, Vector2 pos)
        {
            List<Creature>? creatures = MyPlayer.CreaturesInRange(room, pos, 60f, false, self, false, true);

            Log.Logger(6, "Knitmesh", "MySlugcat:KnitmeshSkill​​:Knitmesh", $"creatures_Null ({creatures == null})");

            if (creatures != null && creatures.Count > 0)
            {
                foreach (Creature creature in creatures)
                {
                    if (creature != null)
                    {
                        Vector2 offset = creature.mainBodyChunk.pos - pos;
                        float Distance = offset.sqrMagnitude;

/*                        Vector2 V1 = new Vector2(UnityEngine.Random.Range(-60, 61), UnityEngine.Random.Range(-60, 61));
                        Vector2 V2 = new Vector2(UnityEngine.Random.Range(-60, 61), UnityEngine.Random.Range(-60, 61));
                        Vector2 V3 = new Vector2(UnityEngine.Random.Range(-60, 61), UnityEngine.Random.Range(-60, 61));
                        Vector2 V4 = new Vector2(UnityEngine.Random.Range(-60, 61), UnityEngine.Random.Range(-60, 61));*/

                        Log.Logger(6, "Knitmesh", "MySlugcat:KnitmeshSkill​​:Knitmesh", $"st");

                        for (int i = 0; i < UnityEngine.Random.Range(8, 38); i++)
                        {
                            Vector2 V1 = new Vector2(UnityEngine.Random.Range(-120, 121), UnityEngine.Random.Range(-120, 121));
                            Vector2 V2 = new Vector2(UnityEngine.Random.Range(-120, 121), UnityEngine.Random.Range(-120, 121));

                            SporePlant.Bee bee = new SporePlant.Bee(null, true, self.firstChunk.pos + V1, new Vector2(0f, 0f), SporePlant.Bee.Mode.Hunt);
                            SporePlant.Bee bee2 = new SporePlant.Bee(null, true, self.mainBodyChunk.pos + V2, new Vector2(0f, 0f), SporePlant.Bee.Mode.Hunt);
                            bee.blackColor = new Color(0.066f, 0.030f, 0.001f, 0.000f);
                            bee.ignoreCreature = self;
                            //bee.room.RoomRect
                            bee.forceAngry = true;
                            creature.room.AddObject(bee);
                            bee2.blackColor = new Color(0.066f, 0.030f, 0.001f, 0.000f);
                            bee2.ignoreCreature = self;
                            //bee.room.RoomRect
                            bee2.forceAngry = true;
                            creature.room.AddObject(bee2);
                        }
                        creature.room.PlaySound(SoundID.Spore_Bees_Emerge, creature.firstChunk);

                        Log.Logger(6, "Knitmesh", "MySlugcat:KnitmeshSkill​​:Knitmesh", $"zh");

                        float j = UnityEngine.Random.Range(0.01f, 8.00f);
                        for (int i = 0; i < 3600 / (Distance * Distance) * 1.5 * j; i++)
                        {
                            Vector2 V1 = new Vector2(UnityEngine.Random.Range(-120, 121), UnityEngine.Random.Range(-120, 121));
                            Vector2 V2 = new Vector2(UnityEngine.Random.Range(-120, 121), UnityEngine.Random.Range(-120, 121));

                            SporePlant.Bee bee = new SporePlant.Bee(null, true, creature.firstChunk.pos + V1, new Vector2(0f, 0f), SporePlant.Bee.Mode.Hunt);
                            SporePlant.Bee bee2 = new SporePlant.Bee(null, true, creature.mainBodyChunk.pos + V2, new Vector2(0f, 0f), SporePlant.Bee.Mode.Hunt);
                            bee.blackColor = new Color(0.066f, 0.030f, 0.001f, 0.000f);
                            bee.ignoreCreature = self;
                            //bee.room.RoomRect
                            bee.forceAngry = true;
                            creature.room.AddObject(bee);
                            bee2.blackColor = new Color(0.066f, 0.030f, 0.001f, 0.000f);
                            bee2.ignoreCreature = self;
                            //bee.room.RoomRect
                            bee2.forceAngry = true;
                            creature.room.AddObject(bee2);
                        }
                        Log.Logger(6, "Knitmesh", "MySlugcat:KnitmeshSkill​​:Knitmesh", $"sh");

                    }

                }
            }

        }

        private static void Player_Update(On.Player.orig_Update orig, Player player, bool eu)
        {
            orig(player, eu);

            //Log.Logger(10, "Knitmesh", "MySlugcat:KnitmeshSkill​​:Player_Update", $"({player.slugcatStats.name == Plugin.YourSlugID})");
            if (player.slugcatStats.name == Plugin.YourSlugID)
            {
                //Log.Logger(9, "Knitmesh", "MySlugcat:KnitmeshSkill​​:Player_Update", $"({player.slugcatStats.name == Plugin.YourSlugID}), ({player.input[0].mp}), ({!player.input[1].mp})");
                //Configurable<bool>? KnitmeshSkill = Options.KnitmeshSkill;
                //if (KnitmeshSkill != null&& KnitmeshSkill.Value)
                if (true)
                {
                    if (player.input[0].mp && !player.input[1].mp)
                    {
                        Knitmesh(player, player.room, player.mainBodyChunk.pos);
                    }
                }
            }
        }

        private static bool Bee_ToHunt(On.SporePlant.Bee.orig_LookForRandomCreatureToHunt orig, SporePlant.Bee bee)
        {
            if (bee.blackColor.r == 0.066f && bee.blackColor.g == 0.030f && bee.blackColor.b == 0.001f)
            {
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
                        SporePlant.SporePlantInterested(abstractCreature.realizedCreature.Template.type))
                    {
                        if ((bee.blackColor.r != 0.066f || bee.blackColor.g != 0.030f || bee.blackColor.b != 0.001f) ||
                           (bee.blackColor.r == 0.066f && bee.blackColor.g == 0.030f && bee.blackColor.b == 0.001f &&
                            abstractCreature.realizedCreature is not Player))
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
            else
            {
                return orig(bee);
            }

            //bool obj = orig(bee);

        }

        private static void Bee_Update(On.SporePlant.Bee.orig_Update orig, SporePlant.Bee bee, bool eu)
        {
            if (bee.blackColor.r == 0.066f && bee.blackColor.g == 0.030f && bee.blackColor.b == 0.001f)
            {
                if (bee.blackColor.a <= 1f)
                {
                    Log.Logger(7, "Knitmesh", "MySlugcat:KnitmeshSkill​​:Bee_Update", $"({bee.blackColor.a <= 1f}), ({bee.blackColor.a})");
                    bee.blackColor.a += 0.01f;
                }
            }

            orig(bee, eu);
        }

        private static void Bee_ApplyPalette(On.SporePlant.Bee.orig_ApplyPalette orig, SporePlant.Bee bee, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            //if (bee.blackColor != new Color(0.066f, 0.030f, 0.001f, 1.000f))
            if (bee.blackColor.r != 0.066f || bee.blackColor.g != 0.030f || bee.blackColor.b != 0.001f)
            {
                Log.Logger(6, "Knitmesh", "MySlugcat:KnitmeshSkill​​:Bee_ApplyPalette", $"({palette.blackColor.ToString()})");
                bee.blackColor = palette.blackColor;
            }
            
        }


    }
}
