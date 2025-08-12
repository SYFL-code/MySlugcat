using System;
using System.Collections.Generic;
using HUD;
using MoreSlugcats;
using RWCustom;
using UnityEngine;
using Watcher;


namespace MySlugcat
{
	public class Health
	{

        public static void KillCreature(RainWorldGame game, PhysicalObject obj)
        {
            if (!(obj is Creature))
            {
                return;
            }
            // 当使用工具杀死生物时，将杀死标记设置为第一个玩家，这样生物可以传承。\n不通过击杀而销毁生物不会产生传承。
            /*Configurable<bool> lineageKill = Options.lineageKill;
            if (lineageKill != null && lineageKill.Value && ((game != null) ? game.FirstAlivePlayer : null) != null)
            {
                (obj as Creature).SetKillTag(game.FirstAlivePlayer);
            }*/
			if (obj is Creature creature)
			{
                creature.Die();
                AbstractCreature abstractCreature = creature.abstractCreature;
                if (((abstractCreature != null) ? abstractCreature.state : null) is HealthState healthState)
                {
                    healthState.health = 0f;
                }
                // 使用此工具击杀精英拾荒者或秃鹫会释放它们的面具。
                //Configurable<bool> killReleasesMask = Options.killReleasesMask;
                //if (killReleasesMask != null && killReleasesMask.Value)
                if (true)
                {
                    if (obj is Scavenger scavenger && scavenger.Elite)
                    {
                        scavenger.Violence(obj.firstChunk, null, obj.firstChunk, null, Creature.DamageType.Stab, 0f, 0f);
                    }
                    Vulture? vulture = obj as Vulture;
                    if (vulture == null)
                    {
                        return;
                    }
                    vulture.DropMask(default(Vector2));
                }
            }

        }

        public static void ReviveCreature(PhysicalObject obj)
		{
			if (!(obj is Creature))
			{
				return;
			}

			if (obj is Creature creature)
			{
				AbstractCreature abstractCreature = creature.abstractCreature;
				if (((abstractCreature != null) ? abstractCreature.state : null) == null)
				{
					return;
				}
                if (abstractCreature == null || abstractCreature.state == null)
                {
                    return;
                }
                if (abstractCreature.state is HealthState healthState && healthState.health < 1f)
				{
					healthState.health = 1f;
				}
				abstractCreature.state.alive = true;
				creature.dead = false;
				creature.stun = 0;
				creature.Hypothermia = 0f;
				creature.HypothermiaExposure = 0f;
				creature.injectedPoison = 0f;
                // 使用工具治疗或复活生物也可以治疗它们的四肢/翅膀/触手。
                //Configurable<bool> healLimbs = Options.healLimbs;//***
                //if (healLimbs == null || healLimbs.Value)//***
                if (true)
				{
					if (abstractCreature.state is LizardState lizardState)
					{
						int num = 0;
						for (; ; )
						{
							int num2 = num;
							float[] limbHealth = lizardState.limbHealth;
							int? num3 = (limbHealth != null) ? new int?(limbHealth.Length) : null;
							if (!(num2 < num3.GetValueOrDefault() & num3 != null))
							{
								break;
							}
							if (lizardState.limbHealth[num] < 1f)
							{
								lizardState.limbHealth[num] = 1f;
							}
							num++;
						}
						if (lizardState.throatHealth < 1f)
						{
							lizardState.throatHealth = 1f;
						}
					}
					if (abstractCreature.state is Vulture.VultureState vultureState)
					{
						int num4 = 0;
						for (; ; )
						{
							int num5 = num4;
							float[] wingHealth = vultureState.wingHealth;
							int? num3 = (wingHealth != null) ? new int?(wingHealth.Length) : null;
							if (!(num5 < num3.GetValueOrDefault() & num3 != null))
							{
								break;
							}
							if (vultureState.wingHealth[num4] < 1f)
							{
								vultureState.wingHealth[num4] = 1f;
							}
							num4++;
						}
					}
					if (abstractCreature.state is DaddyLongLegs.DaddyState daddyState)
					{
						int num6 = 0;
						for (; ; )
						{
							int num7 = num6;
							float[] tentacleHealth = daddyState.tentacleHealth;
							int? num3 = (tentacleHealth != null) ? new int?(tentacleHealth.Length) : null;
							if (!(num7 < num3.GetValueOrDefault() & num3 != null))
							{
								break;
							}
							if (daddyState.tentacleHealth[num6] < 1f)
							{
								daddyState.tentacleHealth[num6] = 1f;
							}
							num6++;
						}
					}
					if (abstractCreature.state is Centipede.CentipedeState centipedeState)
					{
						int num8 = 0;
						for (; ; )
						{
							int num9 = num8;
							bool[] shells = centipedeState.shells;
							int? num3 = (shells != null) ? new int?(shells.Length) : null;
							if (!(num9 < num3.GetValueOrDefault() & num3 != null))
							{
								break;
							}
							centipedeState.shells[num8] = true;
							num8++;
						}
					}
					if (abstractCreature.state is Inspector.InspectorState inspectorState)
					{
						int num10 = 0;
						for (; ; )
						{
							int num11 = num10;
							float[] headHealth = inspectorState.headHealth;
							int? num3 = (headHealth != null) ? new int?(headHealth.Length) : null;
							if (!(num11 < num3.GetValueOrDefault() & num3 != null))
							{
								break;
							}
							if (inspectorState.headHealth[num10] < 3f)
							{
                                inspectorState.headHealth[num10] = 3f;
							}
							num10++;
						}
					}
				}

				if (creature.abstractPhysicalObject is AbstractCreature abstractCreature2)
				{
					AbstractCreatureAI abstractAI = abstractCreature2.abstractAI;
					if (abstractAI != null)
					{
						abstractAI.SetDestination(obj.abstractPhysicalObject.pos);
					}
				}
				if (obj is Hazer hazer)
				{
					hazer.inkLeft = 1f;
					hazer.hasSprayed = false;
					hazer.clds = 0;
				}
				//StowawayBug stowawayBug = obj as StowawayBug;
				//creature is StowawayBug stowawayBug;
				if (creature is StowawayBug stowawayBug)
				{
					if (stowawayBug.AI != null)
					{
						stowawayBug.AI.activeThisCycle = true;
						stowawayBug.AI.behavior = StowawayBugAI.Behavior.Idle;
					}
				}
				/*if (((creature is StowawayBug stowawayBug) ? stowawayBug.AI : null) != null)
				{
					stowawayBug.AI.activeThisCycle = true;
					stowawayBug.AI.behavior = StowawayBugAI.Behavior.Idle;
				}*/
				if (obj is Player player)
				{
                    // 当复活玩家时尝试退出 "游戏结束模式" 。可能与其他一些模块不兼容。
                    //Configurable<bool> exitGameOverMode = Options.exitGameOverMode;
                    //if ((exitGameOverMode == null || exitGameOverMode.Value) && !player.isNPC)
                    if (true && !player.isNPC)
					{
						int num12 = 0;
						for (; ; )
						{
							int num13 = num12;
							Room room = obj.room;
							int? num14;
							if (room == null)
							{
								num14 = null;
							}
							else
							{
								RainWorldGame game = room.game;
								if (game == null)
								{
									num14 = null;
								}
								else
								{
									RoomCamera[] cameras = game.cameras;
									num14 = ((cameras != null) ? new int?(cameras.Length) : null);
								}
							}
							int? num3 = num14;
							if (!(num13 < num3.GetValueOrDefault() & num3 != null))
							{
								break;
							}
							RoomCamera roomCamera = obj.room.game.cameras[num12];
							bool flag;
							if (roomCamera == null)
							{
								flag = (null != null);
							}
							else
							{
								HUD.HUD hud = roomCamera.hud;
								flag = (((hud != null) ? hud.textPrompt : null) != null);
							}
							if (flag)
							{
								obj.room.game.cameras[num12].hud.textPrompt.gameOverMode = false;
							}
							num12++;
						}
						Room room2 = obj.room;
						bool flag2;
						if (room2 == null)
						{
							flag2 = false;
						}
						else
						{
							RainWorldGame game2 = room2.game;
							flag2 = (((game2 != null) ? game2.arenaOverlay : null) != null);
						}
						if (flag2)
						{
							obj.room.game.arenaOverlay.ShutDownProcess();
							ProcessManager manager = obj.room.game.manager;
							if (manager != null)
							{
								List<MainLoopProcess> sideProcesses = manager.sideProcesses;
								if (sideProcesses != null)
								{
									sideProcesses.Remove(obj.room.game.arenaOverlay);
								}
							}
							obj.room.game.arenaOverlay = null;
							if (obj.room.game.session is ArenaGameSession arenaSession)
							{
								arenaSession.sessionEnded = false;
								arenaSession.challengeCompleted = false;
								arenaSession.endSessionCounter = -1;
							}
						}
					}
					player.exhausted = false;
					player.lungsExhausted = false;
					player.airInLungs = 1f;
					player.aerobicLevel = 0f;
					if (player.playerState != null)
					{
						player.playerState.permaDead = false;
						player.playerState.permanentDamageTracking = 0.0;
					}
					player.animation = Player.AnimationIndex.None;
				}
			}

		}




	}
}
