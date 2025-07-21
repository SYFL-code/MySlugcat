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
    //定身能力
    public class FixedSkill
    {
        private static Dictionary<Creature, FreezeData> frozenCreature = new Dictionary<Creature, FreezeData>();

        //private static SoundID freezeCreature = new SoundID("a", false);
        //private static SoundID unfreezeCreature = new SoundID("b", false);



        public void Hook()
        {
#if MYDEBUG
            try
            {
#endif
            //On.Player.ctor += Player_ctor;
            On.Player.Update += Player_Update;
            On.Creature.Update += Creature_Update;
            On.Creature.Die += Creature_Die;

            On.RoomCamera.DrawUpdate += RoomCamera_DrawUpdate;

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

        private void RoomCamera_DrawUpdate(On.RoomCamera.orig_DrawUpdate orig, RoomCamera rCamera, float timeStacker, float timeSpeed)
        {
            orig(rCamera, timeStacker, timeSpeed);

            foreach (var pair in frozenCreature)
            {
                var creature = pair.Key;
                if (creature.room == rCamera.room && creature.abstractCreature.realizedCreature != null)
                {
                    for (int i = 0; i < creature.bodyChunks.Length; i++)
                    {
                        Vector2 pos = creature.bodyChunks[i].pos;
                        for (int j = 0; j < 8; j++)
                        {
                            Vector2 dir = Custom.DegToVec(j * 45f);
                            rCamera.room.AddObject(new Spark(pos + dir * 5f, dir * 0.1f, Color.yellow, null, 5, UnityEngine.Random.Range(8, 12)));
                        }
                    }
                }
            }
        }

        private void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
        {
            orig(self, eu);

            if (self.slugcatStats.name == Plugin.YourSlugID && SC.FixedSkill)
            {
                //if ((self.input[0].pckp || self.input[0].mp) &&
                //    self.input[0].y > 0 && self.playerState.foodInStomach > 2)
                Log.Logger(7, "FixedSkill", "MySlugcat:FixedSkill​​:Player_Update_st", $"bool1 ({self.input[0].pckp}), bool2 ({!self.input[1].pckp}), bool3 ({self.room.abstractRoom.creatures.Count > 0})");
                if (self.input[0].pckp && !self.input[1].pckp)
                {
                    //self.playerState.foodInStomach -= 2;

                    //self.room.PlaySound(freezeCreature, self.mainBodyChunk);

                    Vector2 direction = self.input[0].x != 0 ? new Vector2(self.input[0].x, 0) : Vector2.right;
                    Vector2 startPos = self.mainBodyChunk.pos;

                    if (self.room.abstractRoom.creatures.Count > 0)
                    {
                        foreach (AbstractCreature abstractCreature in self.room.abstractRoom.creatures)
                        {
                            Creature c = abstractCreature.realizedCreature;

                            if (c == null ||             // 确保生物存在
                                c == self ||             // 排除自身
                                c.dead) // 确保有有效的mainBodyChunk
                            { continue; } // 跳过无效项，继续检查下一个

                            Vector2 toCreature = c.mainBodyChunk.pos - startPos;
                            float distance = toCreature.magnitude;//magnitude n.大小；重要；光度；（地震）级数；（星星）等级

                            Log.Logger(7, "FixedSkill", "MySlugcat:FixedSkill​​:Player_Update_stt", $"bool1 ({distance <= 500f}), bool2 ({Vector2.Dot(direction.normalized, toCreature.normalized) > 0.8f})");
                            //normalized adj.标准化的；正常化的
                            if (distance <= 500f && Vector2.Dot(direction.normalized, toCreature.normalized) > 0.8f)
                            {
                                Log.Logger(7, "FixedSkill", "MySlugcat:FixedSkill​​:Player_Update_sh", $"T");
                                FreezeCreature(c);
                            }

                        }
                    }
                    
                }
            }
        }

        private void FreezeCreature(Creature creature)
        {
            if (frozenCreature.ContainsKey(creature)) return;

            // 存储原始数据
            var freezeData = new FreezeData
            {
                originalVelocities = new Vector2[creature.bodyChunks.Length],
                originalAI = creature.abstractCreature.abstractAI,
                particles = new LightSource[5],
                timer = 600 // 10秒(60帧/秒)
            };

            // 冻结生物
            for (int i = 0; i < creature.bodyChunks.Length; i++)
            {
                freezeData.originalVelocities[i] = creature.bodyChunks[i].vel;
                creature.bodyChunks[i].vel = Vector2.zero;
            }

            // 禁用AI
            creature.abstractCreature.abstractAI = null;

            // 添加粒子效果
            for (int i = 0; i < 5; i++)
            {
                freezeData.particles[i] = new LightSource(creature.mainBodyChunk.pos, false, Color.yellow, creature);
                creature.room.AddObject(freezeData.particles[i]);
            }

            frozenCreature.Add(creature, freezeData);
        }

        private void UnfreezeCreature(Creature creature)
        {
            if (!frozenCreature.TryGetValue(creature, out var freezeData)) return;
            //if (frozenCreature.ContainsKey(creature)) return;

            // 恢复速度
            if (freezeData.originalVelocities != null)
            {
                for (int i = 0; i < creature.bodyChunks.Length; i++)
                {
                    creature.bodyChunks[i].vel = freezeData.originalVelocities[i];
                }
            }

            // 恢复AI
            creature.abstractCreature.abstractAI = freezeData.originalAI;

            if (freezeData.particles != null)
            {
                foreach (var light in freezeData.particles)
                {
                    if (light != null)
                    {
                        light.Destroy();
                    }
                }
            }

            //creature.room.PlaySound(unfreezeCreature, creature.mainBodyChunk);

            frozenCreature.Remove(creature);
        }

        private void Creature_Update(On.Creature.orig_Update orig, Creature self, bool eu)
        {
            orig(self, eu);

            //if (self.slugcatStats.name == Plugin.YourSlugID)
            if (frozenCreature.TryGetValue(self, out var freezeData))
            {
                freezeData.timer--;
                if (freezeData.timer <= 0)
                {
                    UnfreezeCreature(self);
                }

                // 更新粒子位置
                if (freezeData.particles != null)
                {
                    for (int i = 0; i < freezeData.particles.Length; i++)
                    {
                        if (freezeData.particles[i] != null)
                        {
                            // 获取随机偏移
                            Vector2 offset = Custom.RNV() * UnityEngine.Random.Range(10f, 30f);

                            // 设置粒子位置
                            freezeData.particles[i].HardSetPos(self.mainBodyChunk.pos + offset);

                            // 设置粒子半径
                            freezeData.particles[i].HardSetRad(UnityEngine.Random.Range(20f, 40f));
                        }
                    }
                }

            }
        }

        private void Creature_Die(On.Creature.orig_Die orig, Creature self)
        {
            if (frozenCreature.ContainsKey(self))
            {
                UnfreezeCreature(self);
            }
            orig(self);
        }

        private class FreezeData
        {
            public Vector2[]? originalVelocities;
            public AbstractCreatureAI? originalAI;
            public LightSource[]? particles;
            public int timer;
        }

    }
}
