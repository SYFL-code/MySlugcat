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
	public class ArcLightning
	{
		private Creature initialCreature;    // 初始生物（闪电起点）
		private Vector2 direction;
		private Room room;
		private Creature thrownBy;
		private Creature sourceCreature;
		private float maxAngle;
		private float maxDistance;
		private int chainCount; // 连锁次数计数器
		private int maxChains; // 最大连锁次数

		private LightningMachine? arcEmitter;
		private List<Creature> hitCreatures; // 记录已经击中的生物

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
					int percentage = 2;
					if (weapon is Spear)
					{
						percentage = 8;
					}
					else if (weapon is Rock)
					{
						percentage = 3;
					}
					else if (weapon is ScavengerBomb)
					{
						percentage = 1;
					}
					else if (weapon is PuffBall)
					{
						percentage = 2;
					}
					else if (ModManager.MSC && weapon is LillyPuck)
					{
						percentage = 6;
					}
					else if (ModManager.Watcher && weapon is Boomerang)
					{
						percentage = 4;
					}

					if (percentage > UnityEngine.Random.Range(0, 100))
					{
						// 创建雷电实例
						var lightning = new ArcLightning(creature, weapon.firstChunk.vel, room, player, creature, 60f, 500f, 8);

						// 执行雷电效果（可以多次调用）
						lightning.Execute();
					}
				}
			}
			return return_;
		}

		/// <summary>
		/// 创建雷电链实例
		/// </summary>
		public ArcLightning(Creature initialCreature, Vector2 direction, Room room, Creature thrownBy,
						   Creature sourceCreature, float maxAngle = 60f, float maxDistance = 300f,
						   int maxChains = 3) // 默认最多连锁3次
		{
			this.initialCreature = initialCreature;
			this.direction = direction;
			this.room = room;
			this.thrownBy = thrownBy;
			this.sourceCreature = sourceCreature;
			this.maxAngle = maxAngle;
			this.maxDistance = maxDistance;
			this.maxChains = maxChains;
			this.chainCount = 0;
			this.hitCreatures = new List<Creature>();

			// 添加初始排除的生物
			if (sourceCreature != null)
			{
				hitCreatures.Add(sourceCreature);
			}
		}

		/// <summary>
		/// 执行雷电链效果
		/// </summary>
		public void Execute()
		{
			Log.OutputLog($"Spear _OK1 - Chain {chainCount + 1}");

			Vector2 start = initialCreature.firstChunk.pos;

			// 寻找最近的生物目标（排除所有已经击中的生物）
			Creature? c = FindNextTarget(start, direction.normalized);

			if (c != null)
			{
				Log.OutputLog($"Spear _OK2 - Hit {c.GetType()}");

				Vector2 end = c.firstChunk.pos;

				// 创建闪电效果
				CreateLightningEffect(start, end, c); // 添加目标生物参数

				// 对目标造成伤害
				ApplyDamageToTarget(start, end, c);

				// 添加水中效果
				ApplyUnderwaterEffects(end, c);

				// 添加音效和光效
				AddSoundAndLightEffects(start, c);

				// 记录击中的生物
				hitCreatures.Add(c);
				chainCount++;

				// 如果还有连锁次数，继续连锁
				if (chainCount < maxChains)
				{
					// 从当前目标继续连锁
					ChainToNextTarget(c, end);
				}

				Log.OutputLog($"Spear _OK6 - Chain completed");
			}

			Log.OutputLog($"Spear _OK5 - Chain search ended");
		}

		/// <summary>
		/// 寻找下一个目标（排除所有已击中生物）
		/// </summary>
		private Creature? FindNextTarget(Vector2 startPos, Vector2 searchDirection)
		{
			// 使用扩展方法寻找目标，排除所有已经击中的生物
			var target = Extension.FindNearestCreatureDirection(
				startPos,
				room,
				false,
				hitCreatures,
				true,
				searchDirection,
				maxAngle,
				maxDistance
			);
			if (target != null && !hitCreatures.Contains(target))
			{
				return target;
			}

			return null;
		}

		/// <summary>
		/// 创建闪电视觉效果
		/// </summary>
		private void CreateLightningEffect(Vector2 start, Vector2 end, Creature target)
		{
			arcEmitter = new LightningMachine(
				pos: start,
				startPoint: Vector2.zero,
				endPoint: end - start,
				chance: 0.4f,
				permanent: false,
				radial: false,
				width: 0.2f,
				intensity: 1f,
				lifeTime: 20f
			);

			arcEmitter.lightningType = 0.66f;
			room.AddObject(arcEmitter);

			// 启动5秒后销毁的协程，并传递目标生物和初始生物
			room.AddObject(new DestroyLightningAfterDelay(arcEmitter, thrownBy, target, initialCreature, 300));
		}

		/// <summary>
		/// 对目标造成伤害
		/// </summary>
		private void ApplyDamageToTarget(Vector2 start, Vector2 end, Creature target)
		{
			if (target is not BigEel && !CheckElectricCreature(target))
			{
				target.Violence(thrownBy.firstChunk, new Vector2?(Custom.DirVec(start, end) * 5f),
							   target.firstChunk, null, Creature.DamageType.Electric, 0.8f,
							   (target is not Player) ? (320f * Mathf.Lerp(target.Template.baseStunResistance, 1f, 0.5f)) : 140f);
				target.stun = Math.Max(target.stun, 300);
				room.AddObject(new CreatureSpasmer(target, false, target.stun));
			}
		}

		private static void ApplyDamageToTarget(Vector2 start, Vector2 end, Creature target, Room room, Creature thrownBy)
		{
			if (target is not BigEel && !CheckElectricCreature(target))
			{
				target.Violence(thrownBy.firstChunk, new Vector2?(Custom.DirVec(start, end) * 5f),
							   target.firstChunk, null, Creature.DamageType.Electric, 0.1f,
							   (target is not Player) ? (320f * Mathf.Lerp(target.Template.baseStunResistance, 1f, 0.5f)) : 140f);
				target.stun = Math.Max(target.stun, 10);
				room.AddObject(new CreatureSpasmer(target, false, target.stun));
			}
		}

		/// <summary>
		/// 添加水中效果
		/// </summary>
		private void ApplyUnderwaterEffects(Vector2 end, Creature target)
		{
			if (target.Submersion > 0.5f)
			{
				room.AddObject(new UnderwaterShock(room, null, end, 10, 800f, 2f, thrownBy, new Color(0.8f, 0.8f, 1f)));
			}
		}

		/// <summary>
		/// 添加音效和光效
		/// </summary>
		private void AddSoundAndLightEffects(Vector2 start, Creature target)
		{
			room.PlaySound(SoundID.Jelly_Fish_Tentacle_Stun, target.firstChunk);
			room.AddObject(new Explosion.ExplosionLight(start, 200f, 1f, 4, new Color(0.7f, 1f, 1f)));
		}

		/// <summary>
		/// 继续连锁到下一个目标
		/// </summary>
		private void ChainToNextTarget(Creature currentTarget, Vector2 currentPos)
		{
			// 稍微延迟一下再连锁，让效果更自然
			room.AddObject(new ChainDelay(this, currentTarget, currentPos, 10)); // 10帧后继续连锁
		}

		/// <summary>
		/// 延迟连锁的协程
		/// </summary>
		private class ChainDelay : UpdatableAndDeletable
		{
			private ArcLightning parent;
			private Creature currentTarget;
			private Vector2 currentPos;
			private int delay;
			private int timer;

			public ChainDelay(ArcLightning parent, Creature currentTarget, Vector2 currentPos, int delayFrames)
			{
				this.parent = parent;
				this.currentTarget = currentTarget;
				this.currentPos = currentPos;
				this.delay = delayFrames;
				this.timer = 0;
			}

			public override void Update(bool eu)
			{
				base.Update(eu);

				timer++;
				if (timer >= delay)
				{
					// 从当前目标的位置和方向创建新的连锁
					Vector2 newDirection = (currentPos - parent.initialCreature.firstChunk.pos).normalized;
					var nextLightning = new ArcLightning(
						currentTarget,
						newDirection,
						parent.room,
						parent.thrownBy,
						parent.sourceCreature,
						parent.maxAngle,
						parent.maxDistance * 0.8f, // 每次连锁距离减少
						parent.maxChains
					);

					// 复制已经击中的生物列表
					foreach (var hit in parent.hitCreatures)
					{
						nextLightning.hitCreatures.Add(hit);
					}

					nextLightning.chainCount = parent.chainCount;
					nextLightning.Execute();

					this.Destroy();
				}
			}
		}

		/// <summary>
		/// 检查生物是否对电击免疫
		/// </summary>
		public static bool CheckElectricCreature(Creature otherObject)
		{
			return otherObject is Centipede || otherObject is BigJellyFish || otherObject is Inspector;
		}

		/// <summary>
		/// 延迟销毁LightningMachine的协程类（增加生物状态检查和移动跟随）
		/// </summary>
		private class DestroyLightningAfterDelay : UpdatableAndDeletable
		{
			private LightningMachine lightning;
			private Creature thrownBy;
			private Creature targetCreature;     // 目标生物（闪电终点）
			private Creature initialCreature;    // 初始生物（闪电起点）
			private int delay;
			private int timer;
			private Vector2 originalStartPos;    // 原始起始位置
			private int lastFrameSkillTick = -100;

			public DestroyLightningAfterDelay(LightningMachine lightning, Creature thrownBy, Creature targetCreature, Creature initialCreature, int delayFrames)
			{
				this.lightning = lightning;
				this.thrownBy = thrownBy;
				this.targetCreature = targetCreature;
				this.initialCreature = initialCreature;
				this.delay = delayFrames;
				this.timer = 0;
				this.originalStartPos = lightning.pos;
			}

			public override void Update(bool eu)
			{
				base.Update(eu);

				// 每帧更新闪电的位置，让闪电跟随生物移动
				if (lightning != null && !lightning.slatedForDeletetion)
				{
					Vector2 currentStartPos = originalStartPos;
					Vector2 currentEndPos = originalStartPos;

					// 更新起始点（跟随初始生物）
					if (initialCreature != null && initialCreature.firstChunk != null &&
						!initialCreature.dead && !initialCreature.slatedForDeletetion)
					{
						currentStartPos = initialCreature.firstChunk.pos;
					}

					// 更新终点（跟随目标生物）
					if (targetCreature != null && targetCreature.firstChunk != null &&
						!targetCreature.dead && !targetCreature.slatedForDeletetion)
					{
						currentEndPos = targetCreature.firstChunk.pos;
					}

					// 更新闪电位置
					lightning.pos = currentStartPos;
					lightning.startPoint = Vector2.zero;
					lightning.endPoint = currentEndPos - currentStartPos;

					// 强制刷新闪电（如果需要）
					// lightning.Reset();

					int now = room?.world?.game?.clock ?? -1;
					if (Math.Abs(now - lastFrameSkillTick) > 10 && targetCreature != null && initialCreature != null && room != null)
					{
						lastFrameSkillTick = now;
						ApplyDamageToTarget(currentStartPos, currentEndPos, initialCreature, room, thrownBy);
						ApplyDamageToTarget(currentStartPos, currentEndPos, targetCreature, room, thrownBy);
					}
				}

				bool shouldDestroy = false;
				string reason = "";

				// 检查初始生物状态
				if (initialCreature != null)
				{
					/*if (initialCreature.dead)
					{
						shouldDestroy = true;
						reason = "initial creature died";
					}*/
					if (initialCreature.slatedForDeletetion || initialCreature.room == null)
					{
						shouldDestroy = true;
						reason = "initial creature removed";
					}
				}

				// 检查目标生物状态
				if (targetCreature != null)
				{
					/*if (targetCreature.dead)
					{
						shouldDestroy = true;
						reason = "target died";
					}*/
					if (targetCreature.stun <= 0)
					{
						shouldDestroy = true;
						reason = "target woke up";
					}
					else if (targetCreature.slatedForDeletetion || targetCreature.room == null)
					{
						shouldDestroy = true;
						reason = "target removed";
					}
					else if (targetCreature.firstChunk != null &&
							 Vector2.Distance(originalStartPos, targetCreature.firstChunk.pos) > 500f)
					{
						shouldDestroy = true;
						reason = "target moved too far";
					}
				}

				// 时间检查
				timer++;
				if (timer >= delay)
				{
					shouldDestroy = true;
					reason = "time expired";
				}

				// 闪电本身已经被销毁
				if (lightning == null || lightning.slatedForDeletetion)
				{
					shouldDestroy = true;
					reason = "lightning already destroyed";
				}

				if (shouldDestroy)
				{
					if (lightning != null && !lightning.slatedForDeletetion)
					{
						lightning.Destroy();
						Log.OutputLog($"Lightning destroyed: {reason}");
					}
					this.Destroy();
				}
			}
		}

	}
}