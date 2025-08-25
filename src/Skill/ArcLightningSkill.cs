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
using System.ComponentModel;
using static MySlugcat.PlayerModuleManager;
using System.Runtime.CompilerServices;
using static MySlugcat.AbPhysicalObjectModuleManager;
using Noise;
using static Menu.Remix.InternalOI;
using System.Threading;


namespace MySlugcat
{
	/// <summary> 电弧连锁 </summary>
	public class ArcLightning : UpdatableAndDeletable
	{
		private LightningMachine? arcEmitter;
		public HashSet<Creature> ElectricCreatures = new HashSet<Creature>();

		public int depth;
		private Vector2 start;
		private Creature? startCreature;
		private Creature? endCreature;
		private Vector2 direction;
		private float maxAngleDeg;
		private Creature thrownBy;
		private int lifetime;

		const int MAX_LIFETIME = 400;
		const float CHAIN_RADIUS = 300f;      // 半径
		const int MAX_RECURSION_DEPTH = 8;    // 深度
		const int FORKS_PER_HIT = 3;          // 分叉
		const int MAX_TOTAL_FORKS = 50;
		static int totalForks = 0;

		public ArcLightning(Vector2 start_, Creature? startCreature_, Vector2 direction_, float maxAngleDeg_, Creature thrownBy_, ref HashSet<Creature> ElectricCreatures_, int depth_ = 0)
		{
			depth = depth_;
			start = start_;
			startCreature = startCreature_;
			direction = direction_;
			maxAngleDeg = maxAngleDeg_;
			thrownBy = thrownBy_;
			lifetime = MAX_LIFETIME;

			// 添加深度限制
			/*if (depth >= MAX_RECURSION_DEPTH || room == null || slatedForDeletetion || totalForks > MAX_TOTAL_FORKS)
			{
				Destroy();
				slatedForDeletetion = true;
				return;
			}*/
			ElectricCreatures = ElectricCreatures_;
			if (depth == 0)
			{
				ElectricCreatures.Clear();
			}
			Interlocked.Increment(ref totalForks);
			//totalForks += 1;
			if (startCreature != null)
			{
				start = startCreature.firstChunk.pos;
			}

			Creature? c = Extension.FindNearestCreatureDirection(start, room, true, thrownBy, true, direction, maxAngleDeg, CHAIN_RADIUS);
			if (c != null && !ElectricCreatures.Contains(c))
			{
				Vector2 end = c.firstChunk.pos;
				float radius = (end - start).sqrMagnitude;

				if (radius < CHAIN_RADIUS * CHAIN_RADIUS)
				{
					endCreature = c;
					ElectricCreatures.Add(c);

					//if (c is not BigEel && !CheckElectricCreature(c))
					//{
					//	c.Violence(thrownBy.firstChunk, new Vector2?(Custom.DirVec(start, end) * 5f), c.firstChunk, null, Creature.DamageType.Electric, 0.8f, (c is not Player) ? (320f * Mathf.Lerp(c.Template.baseStunResistance, 1f, 0.5f)) : 140f);
					//	room.AddObject(new CreatureSpasmer(c, false, c.stun));
					//	lifetime = c.stun;
					//}
					//if (c.Submersion > 0.5f)
					//{
					//	room.AddObject(new UnderwaterShock(room, null, end, 10, 800f, 2f, thrownBy, new Color(0.8f, 0.8f, 1f)));
					//}

					//room.PlaySound(SoundID.Jelly_Fish_Tentacle_Stun, c.firstChunk);
					//room.AddObject(new Explosion.ExplosionLight(start, 200f, 1f, 4, new Color(0.7f, 1f, 1f)));

					// 在房间里生成一个一次性电弧放射器
					arcEmitter = new LightningMachine(
						pos: start,
						startPoint: Vector2.zero,
						endPoint: end - start,  // 200px 半径
						chance: 0.8f,                    // 高概率
						permanent: false,
						radial: true,                    // 放射
						width: 0.5f,
						intensity: 1f,
						lifeTime: 9999f);                  // 20 帧后消失

					arcEmitter.lightningType = 0.66f;

					room.AddObject(arcEmitter);
				}
			}


		}

		public override void Update(bool eu)
		{
			base.Update(eu);

			if (slatedForDeletetion)
			{
				return;
			}
			if (room == null)
			{
				Destroy();
				slatedForDeletetion = true;
				return;
			}

			if (lifetime > 0)
			{
				lifetime--;
			}
			else
			{
				Destroy();
				slatedForDeletetion = true;
				return;
			}
			//if (MAX_LIFETIME - lifetime == 30)
			//{
			//	room.AddObject(new ArcLightning(start, startCreature, direction, maxAngleDeg, thrownBy, ref ElectricCreatures, depth + 1));
			//}

			if (arcEmitter != null)
			{
				arcEmitter.chance = 0.8f * (lifetime / MAX_LIFETIME);
				if (lifetime % 20 == 0)
				{
					if (startCreature != null)
					{
						start = startCreature.firstChunk.pos;
					}
					Creature? c = Extension.FindNearestCreatureDirection(start, room, true, thrownBy, true, direction, maxAngleDeg, CHAIN_RADIUS);
					if (c != null && (!ElectricCreatures.Contains(c) || c == endCreature))
					{
						Vector2 end = c.firstChunk.pos;
						float radius = (end - start).sqrMagnitude;

						if (radius < CHAIN_RADIUS * CHAIN_RADIUS)
						{
							endCreature = c;
							ElectricCreatures.Add(c);

							arcEmitter.pos = start;
							arcEmitter.endPoint = end - start;
							room.PlaySound(SoundID.Jelly_Fish_Tentacle_Stun, c.firstChunk);
						}
					}

				}
			}
		}

		// Token: 0x06000C13 RID: 3091 RVA: 0x0009B489 File Offset: 0x00099689
		public override void Destroy()
		{
			base.Destroy();
			slatedForDeletetion = true;
			Interlocked.Decrement(ref totalForks);
			if (arcEmitter != null)
			{
				arcEmitter.Destroy();
			}
			if (depth == 0)
			{
				ElectricCreatures.Clear();
				totalForks = 0;
			}
		}

		public static bool CheckElectricCreature(Creature otherObject)
		{
			return otherObject is Centipede || otherObject is BigJellyFish || otherObject is Inspector;
		}


		/*public void ArcLightning1(Weapon weapon, Vector2 start, Vector2 direction, float maxAngleDeg, ref HashSet<Creature> ElectricCreatures, int depth = 0)
		{
			if (depth == 0)
			{
				ElectricCreatures.Clear();
				totalForks = 0;
			}

			if (totalForks >= MAX_TOTAL_FORKS) return;
			totalForks++;

			// 添加深度限制
			if (depth >= MAX_RECURSION_DEPTH)
			{
				return;
			}

			for (int fork = 0; fork < FORKS_PER_HIT; fork++)
			{
				if (Extension.Random(0f, 1f) < Mathf.Min(0.05f + depth * 0.08f, 0.95f))
				{
					float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
					angle += Extension.Random(-60f, 60f);
					Vector2 newDir = Custom.DegToVec(angle);
					//ArcLightning(weapon, start, newDir, maxAngleDeg, ref ElectricCreatures, depth + 1);
				}
				else
				{
					break;
				}
			}

			Creature thrownBy = weapon.thrownBy;
			Creature? c = Extension.FindNearestCreatureDirection(start, weapon.room, false, thrownBy, false, direction, maxAngleDeg, CHAIN_RADIUS);

			if (c != null && !ElectricCreatures.Contains(c))
			{
				Vector2 end = c.firstChunk.pos;
				Room room = c.room;
				float radius = (end - start).sqrMagnitude;

				if (radius < CHAIN_RADIUS * CHAIN_RADIUS)
				{
					ElectricCreatures.Add(c);

					if (c is not BigEel && !CheckElectricCreature(c))
					{
						c.Violence(weapon.firstChunk, new Vector2?(Custom.DirVec(start, end) * 5f), c.firstChunk, null, Creature.DamageType.Electric, 0.8f, (c is not Player) ? (320f * Mathf.Lerp(c.Template.baseStunResistance, 1f, 0.5f)) : 140f);
						room.AddObject(new CreatureSpasmer(c, false, c.stun));
					}
					if (weapon.Submersion <= 0.5f && c.Submersion > 0.5f)
					{
						room.AddObject(new UnderwaterShock(room, null, end, 10, 800f, 2f, thrownBy, new Color(0.8f, 0.8f, 1f)));
					}
					room.PlaySound(SoundID.Jelly_Fish_Tentacle_Stun, c.firstChunk);
					room.AddObject(new Explosion.ExplosionLight(start, 200f, 1f, 4, new Color(0.7f, 1f, 1f)));
					for (int i = 0; i < 15; i++)
					{
						Vector2 a = Custom.DegToVec(360f * UnityEngine.Random.value);
						room.AddObject(new MouseSpark(start + a * 9f, weapon.firstChunk.vel + a * 36f * UnityEngine.Random.value, 20f, new Color(0.7f, 1f, 1f)));
					}

					for (int i = 0; i < Mathf.Clamp(radius / CHAIN_RADIUS * Extension.Random(10, 20), 4, 25); i++)
					{
						// 生成一条 0.2 秒、宽度 4、亮度 1 的电弧
						room.AddObject(new LightningMachine(start, start + Extension.Random(-8f, 8f, -8f, 8f),
															end + Extension.Random(-8f, 8f, -8f, 8f),
															1f,           // 立即触发
															false,        // 非永久
															false,        // 非径向
															4f,           // 宽度
															1f,           // 亮度
															0.2f));       // 生命周期(秒)
					}
					Vector2 dir = (end - start).normalized;
					float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
					angle += Extension.Random(-45f, 45f);
					Vector2 newDir = Custom.DegToVec(angle);
					float nextMax = Mathf.Clamp(maxAngleDeg * 0.9f + Extension.Random(-10f, 10f), 10f, 60f);
					// 递归调用
					//ArcLightning(weapon, end, newDir, nextMax, ref ElectricCreatures, depth + 1);
				}
			}

			if (depth == 0)
			{
				ElectricCreatures.Clear();
				totalForks = 0;
			}
		}

		public static float GetTerrainPercent(Weapon w) => w switch
		{
			Spear => 0.85f,
			Rock => 0.90f,
			ScavengerBomb => 0.80f,
			Boomerang => 0.88f,
			_ => 0.95f
		};

		public static void ArcLightningTerrainImpact(
			ref bool Execute,
			ref PhysicalObject physicalObject)
		{
			if (physicalObject != null && physicalObject is Weapon weapon && weapon.thrownBy is Player player && player.GetModule().ArcLightningSkill)
			{
				Room room = weapon.room;
				if (room != null)
				{
					float Percent = 0.8f;
					Percent = GetTerrainPercent(weapon);

					if (Extension.Random(0f, 1f) > Percent && true)
					{
						for (int i = 0; i < Extension.Random(5, 25); i++)
						{
							room.AddObject(new LightningMachine(weapon.firstChunk.pos, weapon.firstChunk.pos + Extension.Random(-8f, 8f, -8f, 8f),
																weapon.firstChunk.pos + new Vector2(100, 100) + Extension.Random(-8f, 8f, -8f, 8f),
																3f,           // 立即触发
																false,        // 非永久
																false,        // 非径向
																50f,           // 宽度
																5f,           // 亮度
																40f));       // 生命周期(秒)
						}

						//HashSet<Creature> hit = new HashSet<Creature>();
						//ArcLightning(weapon, weapon.firstChunk.pos, weapon.firstChunk.vel.normalized, 60f, ref hit);
					}
				}
			}
		}

		public static bool ArcLightningHit<T>(
			ref bool Execute,
			ref bool return_,
			ref T weapon,
			ref SharedPhysics.CollisionResult result,
			ref bool eu) where T : Weapon
		{
			if (result.obj == null)
			{
				return false;
			}
			if (result.obj.abstractPhysicalObject.rippleLayer != weapon.abstractPhysicalObject.rippleLayer && !result.obj.abstractPhysicalObject.rippleBothSides && !weapon.abstractPhysicalObject.rippleBothSides)
			{
				return false;
			}

			if (weapon.thrownBy is Player player && player.GetModule().ArcLightningSkill)
			{
				Room room = weapon.room;
				if (result.obj is Creature creature && room != null)
				{
					float Percent = 0.8f;
					Percent = GetTerrainPercent(weapon);

					if (Extension.Random(0f, 1f) > Percent && true)
					{

						HashSet<Creature> hit = new HashSet<Creature>();
						//ArcLightning1(weapon, weapon.firstChunk.pos, weapon.firstChunk.vel.normalized, 60f, ref hit);
					}
				}
			}
			return return_;
		}*/

	}
}
