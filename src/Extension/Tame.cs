using System;
using System.Collections.Generic;

namespace MySlugcat
{

    public static class Tame
    {

        public static bool IsTamable(RainWorldGame game, PhysicalObject obj)
        {
            if (!(obj is Creature))
            {
                return false;
            }
            if (((game != null) ? game.FirstAlivePlayer : null) == null)
            {
                return false;
            }
            Creature creature = obj as Creature;
            ArtificialIntelligence? artificialIntelligence;
            if (creature == null)
            {
                artificialIntelligence = null;
            }
            else
            {
                AbstractCreature abstractCreature = creature.abstractCreature;
                if (abstractCreature == null)
                {
                    artificialIntelligence = null;
                }
                else
                {
                    AbstractCreatureAI abstractAI = abstractCreature.abstractAI;
                    artificialIntelligence = ((abstractAI != null) ? abstractAI.RealAI : null);
                }
            }
            ArtificialIntelligence? artificialIntelligence2 = artificialIntelligence;
            return artificialIntelligence2 is IUseARelationshipTracker || artificialIntelligence2 is FriendTracker.IHaveFriendTracker;
        }


        public static void TameCreature(RainWorldGame game, PhysicalObject obj)
        {
            if (!(obj is Creature) || game.world == null)
            {
                return;
            }
            AbstractCreature abstractCreature = (game != null) ? game.FirstAlivePlayer : null;
            AbstractCreature abstractCreature2 = (obj as Creature).abstractCreature;
            if (!(((abstractCreature != null) ? abstractCreature.realizedCreature : null) is Player) || abstractCreature2 == null)
            {
                return;
            }
            AbstractCreatureAI abstractAI = abstractCreature2.abstractAI;
            ArtificialIntelligence artificialIntelligence = (abstractAI != null) ? abstractAI.RealAI : null;
            if (artificialIntelligence is IUseARelationshipTracker)
            {
                CreatureState state = abstractCreature2.state;
                SocialMemory.Relationship relationship;
                if (state == null)
                {
                    relationship = null;
                }
                else
                {
                    SocialMemory socialMemory = state.socialMemory;
                    relationship = ((socialMemory != null) ? socialMemory.GetOrInitiateRelationship(abstractCreature.ID) : null);
                }
                SocialMemory.Relationship relationship2 = relationship;
                if (relationship2 != null && relationship2.tempLike < 1f)
                {
                    relationship2.InfluenceTempLike(2f);
                }
                if (relationship2 != null && relationship2.like < 1f)
                {
                    relationship2.InfluenceLike(2f);
                }
                if (relationship2 != null && relationship2.know < 1f)
                {
                    relationship2.InfluenceKnow(0.9f);
                }
                if (relationship2 != null)
                {

                    /*Configurable<bool> tameIncreasesRep = Options.tameIncreasesRep;//***
                    if (tameIncreasesRep != null && tameIncreasesRep.Value)
                    {
                        GameSession session = game.session;
                        if (session != null)
                        {
                            CreatureCommunities creatureCommunities = session.creatureCommunities;
                            if (creatureCommunities != null)
                            {
                                creatureCommunities.InfluenceLikeOfPlayer(abstractCreature2.creatureTemplate.communityID, game.world.RegionNumber, (abstractCreature.state as PlayerState).playerNumber, 0.03f, 0.15f, 0.2f);
                            }
                        }
                    }*/
                    /*Configurable<bool> logDebug = Options.logDebug;
                    if (logDebug == null || logDebug.Value)
                    {
                        Plugin.Logger.LogDebug(string.Concat(new string[]
                        {
                            "TameCreature, SocialMemory.Relationship data set: like=",
                            relationship2.like.ToString(),
                            ", tempLike=",
                            relationship2.tempLike.ToString(),
                            ", know=",
                            relationship2.know.ToString()
                        }));
                    }*/
                }
                if (artificialIntelligence is FriendTracker.IHaveFriendTracker && artificialIntelligence.friendTracker != null)
                {
                    artificialIntelligence.friendTracker.friend = abstractCreature.realizedCreature;
                    if (artificialIntelligence.friendTracker.friendRel == null)
                    {
                        artificialIntelligence.friendTracker.friendRel = relationship2;
                    }
                    /*Configurable<bool> logDebug2 = Options.logDebug;
                    if (logDebug2 == null || logDebug2.Value)
                    {
                        Plugin.Logger.LogDebug("TameCreature, FriendTracker data set");
                    }*/
                }
            }
        }


        public static void TameCreatures(RainWorldGame game, Room room)
        {
            /*Configurable<bool> logDebug = Options.logDebug;
            if (logDebug == null || logDebug.Value)
            {
                Plugin.Logger.LogDebug("TameCreatures");
            }*/
            int num = 0;
            for (; ; )
            {
                int num2 = num;
                int? num3;
                if (room == null)
                {
                    num3 = null;
                }
                else
                {
                    List<PhysicalObject>[] physicalObjects = room.physicalObjects;
                    num3 = ((physicalObjects != null) ? new int?(physicalObjects.Length) : null);
                }
                int? num4 = num3;
                if (!(num2 < num4.GetValueOrDefault() & num4 != null))
                {
                    break;
                }
                int num5 = 0;
                for (; ; )
                {
                    int num6 = num5;
                    List<PhysicalObject> list = room.physicalObjects[num];
                    num4 = ((list != null) ? new int?(list.Count) : null);
                    if (!(num6 < num4.GetValueOrDefault() & num4 != null))
                    {
                        break;
                    }
                    Tame.TameCreature(game, room.physicalObjects[num][num5]);
                    num5++;
                }
                num++;
            }
        }


        public static void ClearRelationships(PhysicalObject obj)
        {
            if (!(obj is Creature))
            {
                return;
            }
            AbstractCreature abstractCreature = (obj as Creature).abstractCreature;
            ArtificialIntelligence artificialIntelligence;
            if (abstractCreature == null)
            {
                artificialIntelligence = null;
            }
            else
            {
                AbstractCreatureAI abstractAI = abstractCreature.abstractAI;
                artificialIntelligence = ((abstractAI != null) ? abstractAI.RealAI : null);
            }
            ArtificialIntelligence artificialIntelligence2 = artificialIntelligence;
            if (artificialIntelligence2 is IUseARelationshipTracker)
            {
                CreatureState state = abstractCreature.state;
                int? num;
                if (state == null)
                {
                    num = null;
                }
                else
                {
                    SocialMemory socialMemory = state.socialMemory;
                    if (socialMemory == null)
                    {
                        num = null;
                    }
                    else
                    {
                        List<SocialMemory.Relationship> relationShips = socialMemory.relationShips;
                        num = ((relationShips != null) ? new int?(relationShips.Count) : null);
                    }
                }
                int? num2 = num;
                int valueOrDefault = num2.GetValueOrDefault();
                CreatureState state2 = abstractCreature.state;
                if (state2 != null)
                {
                    SocialMemory socialMemory2 = state2.socialMemory;
                    if (socialMemory2 != null)
                    {
                        List<SocialMemory.Relationship> relationShips2 = socialMemory2.relationShips;
                        if (relationShips2 != null)
                        {
                            relationShips2.Clear();
                        }
                    }
                }
                /*Configurable<bool> logDebug = Options.logDebug;
                if (logDebug == null || logDebug.Value)
                {
                    Plugin.Logger.LogDebug("ClearRelationships, cleared " + valueOrDefault.ToString() + " relationships");
                }*/
                if (artificialIntelligence2 is FriendTracker.IHaveFriendTracker && artificialIntelligence2.friendTracker != null)
                {
                    artificialIntelligence2.friendTracker.friend = null;
                    artificialIntelligence2.friendTracker.friendRel = null;
                    /*Configurable<bool> logDebug2 = Options.logDebug;
                    if (logDebug2 == null || logDebug2.Value)
                    {
                        Plugin.Logger.LogDebug("ClearRelationships, FriendTracker data reset");
                    }*/
                }
            }
        }


        public static void ClearRelationships(Room room)
        {
            /*Configurable<bool> logDebug = Options.logDebug;
            if (logDebug == null || logDebug.Value)
            {
                Plugin.Logger.LogDebug("ClearRelationships");
            }*/
            int num = 0;
            for (; ; )
            {
                int num2 = num;
                int? num3;
                if (room == null)
                {
                    num3 = null;
                }
                else
                {
                    List<PhysicalObject>[] physicalObjects = room.physicalObjects;
                    num3 = ((physicalObjects != null) ? new int?(physicalObjects.Length) : null);
                }
                int? num4 = num3;
                if (!(num2 < num4.GetValueOrDefault() & num4 != null))
                {
                    break;
                }
                int num5 = 0;
                for (; ; )
                {
                    int num6 = num5;
                    List<PhysicalObject> list = room.physicalObjects[num];
                    num4 = ((list != null) ? new int?(list.Count) : null);
                    if (!(num6 < num4.GetValueOrDefault() & num4 != null))
                    {
                        break;
                    }
                    if (!(room.physicalObjects[num][num5] is Player))
                    {
                        goto IL_6F;
                    }
                    /*Configurable<bool> exceptSlugNPC = Options.exceptSlugNPC;//t
                    if (exceptSlugNPC != null && !exceptSlugNPC.Value && (room.physicalObjects[num][num5] as Player).isNPC)
                    {
                        goto IL_6F;
                    }*/
                    if (false && (room.physicalObjects[num][num5] as Player).isNPC)
                    {
                        goto IL_6F;
                    }

                IL_82:
                    num5++;
                    continue;
                IL_6F:
                    Tame.ClearRelationships(room.physicalObjects[num][num5]);
                    goto IL_82;
                }
                num++;
            }
        }
    }
}
