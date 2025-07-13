/*using BepInEx.Logging;
using MySlugcat;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using RWCustom;
using UnityEngine;
using Watcher;

namespace MySlugcat
{
    // Token: 0x02000009 RID: 9
    // 已抛弃
    public static class Clipboard
    {
        // Token: 0x04000028 RID: 40
        public static List<AbstractPhysicalObject> cutObjects = new List<AbstractPhysicalObject>();


        // Token: 0x06000039 RID: 57 RVA: 0x0000765C File Offset: 0x0000585C
        public static void CutObject(PhysicalObject obj)
        {
            bool flag;
            if (obj == null)
            {
                flag = (null != null);
            }
            else
            {
                Room room = obj.room;
                flag = (((room != null) ? room.game : null) != null);
            }
            if (!flag || obj.abstractPhysicalObject == null)
            {
                return;
            }
            Configurable<bool> logDebug = Options.logDebug;
            //Configurable<bool> logDebug = new Configurable<true>;
            if (logDebug == null || logDebug.Value)
            {
                Plugin.Logger.LogDebug("CutObject: " + ConsistentName(obj.abstractPhysicalObject));
            }
            Clipboard.cutObjects.Add(obj.abstractPhysicalObject);
            if (!(obj is Oracle))
            {
                if (obj is Player && (obj as Player).playerState != null)
                {
                    try
                    {
                        if ((obj as Player).objectInStomach is AbstractCreature)
                        {
                            AbstractCreature abstractCreature = (obj as Player).objectInStomach as AbstractCreature;
                            AbstractCreature abstractCreature2 = (obj as Player).abstractCreature;
                            bool flag2;
                            if (abstractCreature2 == null)
                            {
                                flag2 = (null != null);
                            }
                            else
                            {
                                World world = abstractCreature2.world;
                                flag2 = (((world != null) ? world.GetAbstractRoom(abstractCreature.pos.room) : null) != null);
                            }
                            if (!flag2)
                            {
                                abstractCreature.pos = (obj as Player).coord;
                            }
                            World world2 = abstractCreature.world;
                            bool flag3;
                            if (world2 == null)
                            {
                                flag3 = false;
                            }
                            else
                            {
                                RainWorldGame game = world2.game;
                                flag3 = ((game != null) ? new bool?(game.IsStorySession) : null).GetValueOrDefault();
                            }
                            if (flag3)
                            {
                                (obj as Player).playerState.swallowedItem = SaveState.AbstractCreatureToStringStoryWorld(abstractCreature);
                            }
                            else
                            {
                                World world3 = abstractCreature.world;
                                bool flag4;
                                if (world3 == null)
                                {
                                    flag4 = false;
                                }
                                else
                                {
                                    RainWorldGame game2 = world3.game;
                                    flag4 = ((game2 != null) ? new bool?(game2.IsArenaSession) : null).GetValueOrDefault();
                                }
                                if (flag4)
                                {
                                    (obj as Player).playerState.swallowedItem = SaveState.AbstractCreatureToStringSingleRoomWorld(abstractCreature);
                                }
                                else
                                {
                                    Plugin.Logger.LogWarning("Clipboard.CutObject, could not store swallowed creature");
                                }
                            }
                        }
                        else
                        {
                            PlayerState playerState = (obj as Player).playerState;
                            AbstractPhysicalObject objectInStomach = (obj as Player).objectInStomach;
                            playerState.swallowedItem = ((objectInStomach != null) ? objectInStomach.ToString() : null);
                        }
                    }
                    catch (Exception ex)
                    {
                        Plugin.Logger.LogWarning("Clipboard.CutObject exception: " + ((ex != null) ? ex.ToString() : null));
                    }
                }
                DestroyObject(obj);
                return;
            }
            obj.RemoveFromRoom();
            AbstractPhysicalObject abstractPhysicalObject = obj.abstractPhysicalObject;
            if (abstractPhysicalObject == null)
            {
                return;
            }
            AbstractRoom room2 = abstractPhysicalObject.Room;
            if (room2 == null)
            {
                return;
            }
            room2.RemoveEntity(obj.abstractPhysicalObject);
        }

        // Token: 0x0600003A RID: 58 RVA: 0x0000789C File Offset: 0x00005A9C
        public static void CopyObject(PhysicalObject obj)
        {
            Clipboard.CutObject(DuplicateObject(obj));
        }

        // Token: 0x0600003B RID: 59 RVA: 0x000078AC File Offset: 0x00005AAC
        public static void PasteObject(Room room, WorldCoordinate pos)
        {
            if (((room != null) ? room.world : null) == null || ((room != null) ? room.abstractRoom : null) == null)
            {
                return;
            }
            if (Clipboard.cutObjects.Count <= 0)
            {
                return;
            }
            AbstractPhysicalObject abstractPhysicalObject = Clipboard.cutObjects.Pop<AbstractPhysicalObject>();
            Configurable<bool> logDebug = Options.logDebug;
            if (logDebug == null || logDebug.Value)
            {
                Plugin.Logger.LogDebug("PasteObject: " + ConsistentName(abstractPhysicalObject));
            }
            if (abstractPhysicalObject == null)
            {
                return;
            }
            abstractPhysicalObject.pos = pos;
            abstractPhysicalObject.world = room.world;
            AbstractCreature abstractCreature = abstractPhysicalObject as AbstractCreature;
            if (abstractCreature != null)
            {
                AbstractCreatureAI abstractAI = abstractCreature.abstractAI;
                if (abstractAI != null)
                {
                    abstractAI.NewWorld(room.world);
                }
            }
            if (abstractPhysicalObject is AbstractCreature)
            {
                abstractPhysicalObject.Abstractize(pos);
            }
            if (abstractPhysicalObject.realizedObject != null)
            {
                abstractPhysicalObject.realizedObject.slatedForDeletetion = false;
                abstractPhysicalObject.realizedObject.room = room;
            }
*//*            if (abstractPhysicalObject.realizedObject is Oracle)
            {
                Oracle oracle = abstractPhysicalObject.realizedObject as Oracle;
                if (oracle.myScreen != null)
                {
                    oracle.myScreen.room = room;
                }
                if (oracle.oracleBehavior != null)
                {
                    AbstractCreature firstAlivePlayer = game.FirstAlivePlayer;
                    if (((firstAlivePlayer != null) ? firstAlivePlayer.realizedCreature : null) is Player)
                    {
                        OracleBehavior oracleBehavior = oracle.oracleBehavior;
                        AbstractCreature firstAlivePlayer2 = game.FirstAlivePlayer;
                        oracleBehavior.player = (((firstAlivePlayer2 != null) ? firstAlivePlayer2.realizedCreature : null) as Player);
                    }
                }
            }*//*
            room.abstractRoom.AddEntity(abstractPhysicalObject);
            abstractPhysicalObject.RealizeInRoom();
            if (abstractPhysicalObject.realizedObject is Player)
            {
                try
                {
                    PlayerState playerState = (abstractPhysicalObject.realizedObject as Player).playerState;
                    if (string.IsNullOrEmpty((playerState != null) ? playerState.swallowedItem : null))
                    {
                        (abstractPhysicalObject.realizedObject as Player).objectInStomach = null;
                    }
                    else
                    {
                        AbstractPhysicalObject abstractPhysicalObject2 = null;
                        string text = (abstractPhysicalObject.realizedObject as Player).playerState.swallowedItem;
                        if (text.Contains("<oA>"))
                        {
                            abstractPhysicalObject2 = SaveState.AbstractPhysicalObjectFromString(abstractPhysicalObject.world, text);
                        }
                        else if (text.Contains("<cA>"))
                        {
                            string[] array = text.Split(new string[]
                            {
                                "<cA>"
                            }, StringSplitOptions.None);
                            text = text.Replace(array[2], string.Format(CultureInfo.InvariantCulture, "{0}.{1}", pos.ResolveRoomName() ?? pos.room.ToString(), pos.abstractNode));
                            abstractPhysicalObject2 = SaveState.AbstractCreatureFromString(abstractPhysicalObject.world, text, false, default(WorldCoordinate));
                        }
                        if (abstractPhysicalObject2 != null)
                        {
                            abstractPhysicalObject2.pos = abstractPhysicalObject.pos;
                        }
                        (abstractPhysicalObject.realizedObject as Player).objectInStomach = abstractPhysicalObject2;
                        if (abstractPhysicalObject2 == null && !string.IsNullOrEmpty(text))
                        {
                            Plugin.Logger.LogWarning("Clipboard.PasteObject, swallowedItem string available but objectInStomach became null");
                        }
                        (abstractPhysicalObject.realizedObject as Player).playerState.swallowedItem = "";
                    }
                }
                catch (Exception ex)
                {
                    Plugin.Logger.LogWarning("Clipboard.PasteObject exception: " + ((ex != null) ? ex.ToString() : null));
                }
            }
*//*            if (abstractPhysicalObject.realizedObject is SandGrub)
            {
                Duplicate.BigSandGrubPostRealization(abstractPhysicalObject.realizedObject as SandGrub);
            }*//*
        }

        public static PhysicalObject DuplicateObject(PhysicalObject obj)
        {
            bool flag;
            if (obj == null)
            {
                flag = (null != null);
            }
            else
            {
                Room room = obj.room;
                flag = (((room != null) ? room.abstractRoom : null) != null);
            }
            if (!flag || obj.room.game == null)
            {
                return null;
            }
            Room room2 = obj.room;
            BodyChunk firstChunk = obj.firstChunk;
            WorldCoordinate worldCoordinate = room2.GetWorldCoordinate((firstChunk != null) ? firstChunk.pos : default(Vector2));
            AbstractPhysicalObject abstractPhysicalObject;
            if ((abstractPhysicalObject = ((obj != null) ? obj.abstractPhysicalObject : null)) == null)
            {
                Creature creature = obj as Creature;
                abstractPhysicalObject = ((creature != null) ? creature.abstractCreature : null);
            }
            AbstractPhysicalObject abstractPhysicalObject2 = abstractPhysicalObject;
            AbstractPhysicalObject abstractPhysicalObject3 = null;
            if (abstractPhysicalObject2 == null)
            {
                return null;
            }
            if (obj is Creature)
            {
                Configurable<bool> copyID = Options.copyID;
                EntityID id = (copyID != null && !copyID.Value) ? obj.room.game.GetNewID() : abstractPhysicalObject2.ID;
                abstractPhysicalObject3 = new AbstractCreature(abstractPhysicalObject2.world, (obj as Creature).Template, null, worldCoordinate, id);
                AbstractCreature abstractCreature = abstractPhysicalObject2 as AbstractCreature;
                if (((abstractCreature != null) ? abstractCreature.state : null) != null)
                {
                    CreatureState state = (abstractPhysicalObject3 as AbstractCreature).state;
                    if (state != null)
                    {
                        state.LoadFromString(Regex.Split((abstractPhysicalObject2 as AbstractCreature).state.ToString(), "<cB>"));
                    }
                }
                if (obj is Player && (obj as Player).playerState != null && !(obj as Player).isNPC)
                {
                    PlayerState playerState = (obj as Player).playerState;
                    (abstractPhysicalObject3 as AbstractCreature).state = new PlayerState(abstractPhysicalObject3 as AbstractCreature, playerState.playerNumber, playerState.slugcatCharacter, false);
                }
            }
            else
            {
                try
                {
                    if (abstractPhysicalObject2 is SeedCob.AbstractSeedCob)
                    {
                        abstractPhysicalObject3 = DuplicateObjectSeedCob(abstractPhysicalObject2);
                    }
                    else if (abstractPhysicalObject2 is Pomegranate.AbstractPomegranate)
                    {
                        abstractPhysicalObject3 = new Pomegranate.AbstractPomegranate(abstractPhysicalObject2.world, null, abstractPhysicalObject2.pos, abstractPhysicalObject2.ID, (abstractPhysicalObject2 as Pomegranate.AbstractPomegranate).originRoom, -1, null, (abstractPhysicalObject2 as Pomegranate.AbstractPomegranate).smashed, (abstractPhysicalObject2 as Pomegranate.AbstractPomegranate).disconnected, (abstractPhysicalObject2 as Pomegranate.AbstractPomegranate).spearmasterStabbed);
                    }
                    else if (abstractPhysicalObject2 is LobeTree.AbstractLobeTree)
                    {
                        PlacedObject placedObject = new PlacedObject(PlacedObject.Type.LobeTree, null);
                        if ((abstractPhysicalObject2 as LobeTree.AbstractLobeTree).placedObjectIndex >= 0)
                        {
                            int placedObjectIndex = (abstractPhysicalObject2 as LobeTree.AbstractLobeTree).placedObjectIndex;
                            RoomSettings roomSettings = obj.room.roomSettings;
                            int? num;
                            if (roomSettings == null)
                            {
                                num = null;
                            }
                            else
                            {
                                List<PlacedObject> placedObjects = roomSettings.placedObjects;
                                num = ((placedObjects != null) ? new int?(placedObjects.Count) : null);
                            }
                            int? num2 = num;
                            if (placedObjectIndex < num2.GetValueOrDefault() & num2 != null)
                            {
                                PlacedObject placedObject2 = obj.room.roomSettings.placedObjects[(abstractPhysicalObject2 as LobeTree.AbstractLobeTree).placedObjectIndex];
                                placedObject.pos = placedObject2.pos;
                                placedObject.data = placedObject2.data;
                                goto IL_332;
                            }
                        }
                        float d = 60f;
                        float ang = 0f;
                        float x = -200f;
                        float y = 0f;
                        placedObject.pos = abstractPhysicalObject2.pos.Tile.ToVector2() * 20f + new Vector2(10f, 10f);
                        (placedObject.data as LobeTree.LobeTreeData).handlePos = Custom.DegToVec(ang) * d;
                        (placedObject.data as LobeTree.LobeTreeData).rootOffset = new Vector2(x, y);
                    IL_332:
                        abstractPhysicalObject3 = new LobeTree.AbstractLobeTree(abstractPhysicalObject2.world, AbstractPhysicalObject.AbstractObjectType.LobeTree, null, abstractPhysicalObject2.pos, abstractPhysicalObject2.ID, placedObject);
                    }
                    else
                    {
                        abstractPhysicalObject3 = SaveState.AbstractPhysicalObjectFromString(abstractPhysicalObject2.world, abstractPhysicalObject2.ToString());
                    }
                    if (obj is Oracle)
                    {
                        abstractPhysicalObject3.realizedObject = new Oracle(abstractPhysicalObject3, obj.room);
                    }
                    Configurable<bool> copyID2 = Options.copyID;
                    if (copyID2 != null && !copyID2.Value)
                    {
                        abstractPhysicalObject3.ID = obj.room.game.GetNewID();
                    }
                }
                catch (Exception ex)
                {
                    Plugin.Logger.LogWarning("DuplicateObject exception: " + ((ex != null) ? ex.ToString() : null));
                    return null;
                }
                abstractPhysicalObject3.pos = worldCoordinate;
            }
            Configurable<bool> logDebug = Options.logDebug;
            if (logDebug == null || logDebug.Value)
            {
                ManualLogSource logger = Plugin.Logger;
                string str = "DuplicateObject, AddEntity ";
                AbstractPhysicalObject.AbstractObjectType type = abstractPhysicalObject3.type;
                logger.LogDebug(str + ((type != null) ? type.ToString() : null) + " at " + worldCoordinate.SaveToString());
            }
            obj.room.abstractRoom.AddEntity(abstractPhysicalObject3);
            abstractPhysicalObject3.RealizeInRoom();
*//*            if (abstractPhysicalObject3.realizedObject is SandGrub)
            {
                Duplicate.BigSandGrubPostRealization(abstractPhysicalObject3.realizedObject as SandGrub);
            }*//*
            return abstractPhysicalObject3.realizedObject;
        }

        public static void DestroyObject(PhysicalObject obj)
        {
            //Destroy.ReleaseAllGrasps(obj);
            if (obj is Oracle)
            {
                obj.Destroy();
            }
            if (obj is SporePlant && (obj as SporePlant).stalk != null)
            {
                (obj as SporePlant).stalk.sporePlant = null;
                (obj as SporePlant).stalk = null;
            }
            if (obj is Prince)
            {
                (obj as Prince).stem.Destroy();
            }
            if (obj is Spear)
            {
                (obj as Spear).resetHorizontalBeamState();
            }
            if (obj != null)
            {
                obj.RemoveFromRoom();
            }
            if (obj != null)
            {
                AbstractPhysicalObject abstractPhysicalObject = obj.abstractPhysicalObject;
                if (abstractPhysicalObject != null)
                {
                    AbstractRoom room = abstractPhysicalObject.Room;
                    if (room != null)
                    {
                        room.RemoveEntity(obj.abstractPhysicalObject);
                    }
                }
            }
            if (!(obj is Player) && obj != null)
            {
                obj.Destroy();
            }
        }

        public static string ConsistentName(AbstractPhysicalObject apo)
        {
            if (apo is AbstractCreature)
            {
                CreatureTemplate creatureTemplate = (apo as AbstractCreature).creatureTemplate;
                return ((creatureTemplate != null) ? creatureTemplate.name : null) + " " + apo.ID.ToString() + ((apo.ID.altSeed > -1) ? ("." + apo.ID.altSeed.ToString()) : "");
            }
            if (apo != null && !(apo is AbstractCreature))
            {
                AbstractPhysicalObject.AbstractObjectType type = apo.type;
                return ((type != null) ? type.ToString() : null) + " " + apo.ID.ToString() + ((apo.ID.altSeed > -1) ? ("." + apo.ID.altSeed.ToString()) : "");
            }
            return string.Empty;
        }

        private static AbstractPhysicalObject DuplicateObjectSeedCob(AbstractPhysicalObject oldApo)
        {
            AbstractPhysicalObject abstractPhysicalObject = new SeedCob.AbstractSeedCob(oldApo.world, null, oldApo.pos, oldApo.ID, oldApo.realizedObject.room.abstractRoom.index, -1, (oldApo as SeedCob.AbstractSeedCob).dead, null);
            (abstractPhysicalObject as AbstractConsumable).isConsumed = (oldApo as AbstractConsumable).isConsumed;
            oldApo.realizedObject.room.abstractRoom.entities.Add(abstractPhysicalObject);
            abstractPhysicalObject.Realize();
            string[] source = new string[]
            {
                "seedPositions",
                "seedsPopped",
                "leaves"
            };
            string[] source2 = new string[]
            {
                "totalSprites",
                "stalkSegments",
                "cobSegments",
                "placedPos",
                "rootPos",
                "rootDir",
                "cobDir",
                "stalkLength",
                "open"
            };
            FieldInfo[] fields = oldApo.realizedObject.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo fi = fields[i];
                if (source.Any((string s) => fi.Name.Contains(s)))
                {
                    fi.SetValue(abstractPhysicalObject.realizedObject, (fi.GetValue(oldApo.realizedObject) as Array).Clone());
                }
                if (source2.Any((string s) => fi.Name.Contains(s)))
                {
                    fi.SetValue(abstractPhysicalObject.realizedObject, fi.GetValue(oldApo.realizedObject));
                }
            }
            return abstractPhysicalObject;
        }


    }
}*/
