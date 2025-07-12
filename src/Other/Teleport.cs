using System;
using System.Collections.Generic;
using UnityEngine;


namespace MySlugcat
{
    public static class Teleport
    {

        public static void SetObjectPosition(PhysicalObject obj, Vector2 newPos)
        {
            if (!(obj is Creature))
            {
                ReleaseAllGrasps(obj);
            }
            int num = 0;
            for (; ; )
            {
                int num2 = num;
                int? num3;
                if (obj == null)
                {
                    num3 = null;
                }
                else
                {
                    BodyChunk[] bodyChunks = obj.bodyChunks;
                    num3 = ((bodyChunks != null) ? new int?(bodyChunks.Length) : null);
                }
                int? num4 = num3;
                if (!(num2 < num4.GetValueOrDefault() & num4 != null))
                {
                    break;
                }
                if (obj.bodyChunks[num] != null)
                {
                    obj.bodyChunks[num].pos = newPos;
                    obj.bodyChunks[num].lastPos = newPos;
                    obj.bodyChunks[num].lastLastPos = newPos;
                    obj.bodyChunks[num].vel = default(Vector2);
                    if (obj is PlayerCarryableItem)
                    {
                        (obj as PlayerCarryableItem).lastOutsideTerrainPos = null;
                    }
                }
                num++;
            }
        }

        public static void ReleaseAllGrasps(PhysicalObject obj)
        {
            if (((obj != null) ? obj.grabbedBy : null) != null)
            {
                for (int i = obj.grabbedBy.Count - 1; i >= 0; i--)
                {
                    Creature.Grasp grasp = obj.grabbedBy[i];
                    if (grasp != null)
                    {
                        grasp.Release();
                    }
                }
            }
            if (obj is Creature)
            {
                if (obj is Player)
                {
                    Player.SlugOnBack slugOnBack = (obj as Player).slugOnBack;
                    if (slugOnBack != null)
                    {
                        slugOnBack.DropSlug();
                    }
                    Player onBack = (obj as Player).onBack;
                    if (onBack != null)
                    {
                        Player.SlugOnBack slugOnBack2 = onBack.slugOnBack;
                        if (slugOnBack2 != null)
                        {
                            slugOnBack2.DropSlug();
                        }
                    }
                    (obj as Player).slugOnBack = null;
                    (obj as Player).onBack = null;
                    Player.SpearOnBack spearOnBack = (obj as Player).spearOnBack;
                    if (spearOnBack != null)
                    {
                        spearOnBack.DropSpear();
                    }
                }
                (obj as Creature).LoseAllGrasps();
            }
        }


    }
}
