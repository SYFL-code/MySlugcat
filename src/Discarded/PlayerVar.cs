/*using MySlugcat;
using System.Collections.Generic;
using UnityEngine;

namespace MySlugcat
{
    public class PlayerVar
    {
        public MyDebug? myDebug = null;//调试图像
        //public TriangleMesh tailMesh = null;//尾巴
        //public FlyAbility flyAbility = new FlyAbility();//飞行能力变量
        //public bool InGrabUpdateProc = false;//冰矛合成能力变量
        //public List<MyIceShield> iceShieldList = new List<MyIceShield>();//冰盾列表
        //public int iceShield_ReadyCraft = 40;//合成预备时间
        //public int iceShield_craft = 120;//合成时间
        //public MyCloak cloak = null;
    }
}*/





// ---------------------------------------------------------
//  Rain World  Mod: MoreSlugcats
//  文件名: LightningMachine.cs
//  功能  : 闪电生成器（支持永久、径向、定向三种模式）
//  备注  : 已逐行添加中文注释，可直接替换原文件使用
// ---------------------------------------------------------

using System;
using RWCustom;
using UnityEngine;

namespace MoreSlugcats
{
	// Token: 0x020003A9 RID: 937
	public class LightningMachine : UpdatableAndDeletable
	{
		/* =====================================================
         * 构造方法：初始化闪电参数
         * ===================================================== */
		// Token: 0x06002BA5 RID: 11173 RVA: 0x003347B8 File Offset: 0x003329B8
		public LightningMachine(Vector2 pos, Vector2 startPoint, Vector2 endPoint,
								float chance, bool permanent, bool radial,
								float width, float intensity, float lifeTime)
		{
			this.pos = pos;                     // 闪电生成器在房间中的中心坐标
			this.startPoint = startPoint;       // 起始偏移（相对于 pos）
			this.endPoint = endPoint;           // 终止偏移（相对于 pos）
			this.chance = chance;               // 每帧触发闪电的概率(0~1)
			this.permanent = permanent;         // true=永久闪电，不消失
			this.radial = (!permanent && radial); // 径向模式：从中心向四周随机发射（永久模式下强制关闭）
			this.width = width;                 // 闪电宽度
			this.intensity = intensity;         // 闪电亮度
			this.lifeTime = lifeTime;           // 非永久闪电的生命周期（秒）

			this.random = false;                // 是否使用随机触发（与 counter 触发二选一）
			this.soundLoop = new DisembodiedDynamicSoundLoop(this); // 持续电流声
			this.soundLoop.sound = SoundID.Zapper_LOOP;
			this.soundLoop.Pitch = 2f;
			this.soundLoop.Volume = 0f;
		}

		/* =====================================================
         * 永久闪电更新逻辑：保持一条持续存在的闪电
         * ===================================================== */
		// Token: 0x06002BA6 RID: 11174 RVA: 0x0033485C File Offset: 0x00332A5C
		public void PermaLightning()
		{
			if (this.permaLightning == null)
			{
				// 首次创建
				this.Strike();
				return;
			}

			// 更新已存在闪电的位置、宽度、亮度等
			this.soundLoop.Volume = this.volume * 0.5f;
			this.permaLightning.from = this.Source;
			this.permaLightning.target = this.Target;
			this.permaLightning.intensity = this.intensity;
			this.permaLightning.width = this.width * 30f;
			this.permaLightning.lightningParam = this.lightningParam;
			this.permaLightning.lightningType = this.lightningType;
		}

		/* =====================================================
         * 径向闪电：每次随机新方向后立即触发一次
         * ===================================================== */
		// Token: 0x06002BA7 RID: 11175 RVA: 0x003348FB File Offset: 0x00332AFB
		public void RadialLightning()
		{
			this.rPoint = this.rSecPoint; // 随机新方向
			this.Strike();                // 立即触发
		}

		/* =====================================================
         * 触发一次闪电（非永久/非径向模式）
         * ===================================================== */
		// Token: 0x06002BA8 RID: 11176 RVA: 0x00334910 File Offset: 0x00332B10
		public void Strike()
		{
			// 对概率做指数映射，使低概率更平滑
			float num = Custom.ExponentMap(this.chance, 0f, 1f, 2f);
			this.counter += num;

			// 计算闪电颜色（HSL -> RGB）
			this.color = Custom.HSL2RGB(this.lightningType - 0.0001f, 1f, 0.6f);

			/* ---------- 永久模式 ---------- */
			if (this.permanent)
			{
				this.room.AddObject(this.permaLightning = new LightningBolt(
					this.Source, this.Target, 1, this.width, this.lifeTime,
					this.lightningParam, this.lightningType, this.light));
				return;
			}

			/* ---------- 随机触发 ---------- */
			if (UnityEngine.Random.value <= num && this.random)
			{
				this.ready = true;
			}

			/* ---------- 计数器触发 ---------- */
			if (this.counter > 1f && !this.random)
			{
				this.ready = true;
				this.counter = 0f;
			}

			/* ---------- 真正生成闪电 ---------- */
			if (this.ready)
			{
				// 关闭持续电流声
				this.soundLoop.Volume = 0f;

				// 播放单次音效
				if (this.soundType == 0)
				{
					this.room.PlaySound(SoundID.Death_Lightning_Spark_Spontaneous,
										this.Target, this.volume * 0.5f,
										1.4f - UnityEngine.Random.value * 0.4f);
				}
				if (this.soundType == 1)
				{
					this.room.PlaySound(SoundID.Zapper_Zap,
										this.Target, this.volume * 0.5f,
										1.4f - UnityEngine.Random.value * 0.4f);
				}

				// 创建闪电
				LightningBolt lightningBolt;
				this.room.AddObject(lightningBolt = new LightningBolt(
					this.Source, this.Target, 0, this.width, this.lifeTime,
					this.lightningParam, this.lightningType, this.light));
				lightningBolt.intensity = this.intensity;
				lightningBolt.lightningParam = this.lightningParam;
				lightningBolt.lightningType = this.lightningType;

				// 命中特效
				if (this.impactType == 1 && this.isTerrain)
				{
					this.room.AddObject(new LightningMachine.Impact(
						this.Target, this.intensity * this.width * 0.5f, this.color, this.isTerrain));
				}
				if (this.impactType > 1)
				{
					this.room.AddObject(new LightningMachine.Impact(
						this.Target, this.intensity * this.width * 1f, this.color, this.isTerrain));
					if (this.impactType == 3)
					{
						this.room.AddObject(new LightningMachine.Impact(
							this.Source, this.intensity * this.width * 1f, this.color, this.isTerrain));
					}
				}

				this.ready = false;
			}
		}

		/* =====================================================
         * 射线检测：返回闪电实际落点（如果命中地形会偏移）
         * ===================================================== */
		// Token: 0x06002BA9 RID: 11177 RVA: 0x00334BB0 File Offset: 0x00332DB0
		public Vector2 Trace(Vector2 start, Vector2 end)
		{
			Vector2 a = Custom.DegToVec(Custom.AimFromOneVectorToAnother(start, end));
			IntVector2? intVector = SharedPhysics.RayTraceTilesForTerrainReturnFirstSolid(this.room, start, end);
			if (intVector != null)
			{
				this.isTerrain = true; // 命中地形
				return this.room.MiddleOfTile(intVector.Value) - a * 7f; // 向内偏移，避免穿模
			}
			this.isTerrain = false;
			return end; // 未命中地形，直接返回端点
		}

		/* =====================================================
         * 主循环：根据模式调用不同更新函数
         * ===================================================== */
		// Token: 0x06002BAA RID: 11178 RVA: 0x00334C14 File Offset: 0x00332E14
		public override void Update(bool eu)
		{
			this.soundLoop.Update(); // 更新持续电流声

			if (this.permanent)
			{
				this.radial = false; // 永久模式下强制关闭径向
				this.PermaLightning();
			}
			else if (this.permaLightning != null)
			{
				// 从永久模式切换回来时，销毁旧闪电
				this.permaLightning.Destroy();
				this.permaLightning = null;
			}
			else if (this.radial)
			{
				this.RadialLightning();
			}
			else
			{
				this.Strike();
			}

			base.Update(eu); // 父类更新
		}

		/* =====================================================
         * 属性：径向模式下的随机端点（扇形范围内）
         * ===================================================== */
		// Token: 0x1700076A RID: 1898
		// (get) Token: 0x06002BAB RID: 11179 RVA: 0x00334C7C File Offset: 0x00332E7C
		public Vector2 rSecPoint
		{
			get
			{
				Vector2 v = this.pos + this.startPoint - this.pos; // 方向向量1
				Vector2 vec = this.pos + this.endPoint - this.pos; // 方向向量2
				float num = Custom.VecToDeg(v);                    // 起始角度
				float maxInclusive = Custom.Mod(Custom.VecToDeg(Custom.rotateVectorDeg(vec, -num)), 360f); // 扇形角度
				float minInclusive = Custom.Dist(this.pos, this.pos + this.startPoint); // 最小半径
				float maxInclusive2 = Custom.Dist(this.pos, this.pos + this.endPoint);  // 最大半径
				float y = UnityEngine.Random.Range(minInclusive, maxInclusive2);        // 随机半径
				Vector2 vector = new Vector2(0f, y);
				vector = Custom.rotateVectorDeg(vector, num);                           // 旋转到起始角度
				vector = Custom.rotateVectorDeg(vector, UnityEngine.Random.Range(0f, maxInclusive)); // 再随机扇形内角度
				return this.pos + vector;                                               // 最终世界坐标
			}
		}

		/* =====================================================
         * 属性：获取闪电目标点
         * ===================================================== */
		// Token: 0x1700076B RID: 1899
		// (get) Token: 0x06002BAC RID: 11180 RVA: 0x00334D5A File Offset: 0x00332F5A
		public Vector2 Target
		{
			get
			{
				if (this.radial)
					return this.Trace(this.pos, this.rPoint);           // 径向：中心 -> 随机点
				return this.Trace(this.Source, this.pos + this.endPoint); // 定向：起点 -> 终点
			}
		}

		/* =====================================================
         * 属性：获取闪电起始点
         * ===================================================== */
		// Token: 0x1700076C RID: 1900
		// (get) Token: 0x06002BAD RID: 11181 RVA: 0x00334D94 File Offset: 0x00332F94
		public Vector2 Source
		{
			get
			{
				if (this.radial)
					return this.pos;            // 径向：中心
				return this.pos + this.startPoint; // 定向：偏移
			}
		}

		/* =====================================================
         * 字段：所有公开/内部字段
         * ===================================================== */
		// Token: 0x040028A7 RID: 10407
		public Vector2 pos;              // 生成器中心
										 // Token: 0x040028A8 RID: 10408
		public Vector2 startPoint;       // 起始偏移
										 // Token: 0x040028A9 RID: 10409
		public Vector2 endPoint;         // 终止偏移
										 // Token: 0x040028AA RID: 10410
		public float chance;             // 触发概率
										 // Token: 0x040028AB RID: 10411
		public bool permanent;           // 是否永久
										 // Token: 0x040028AC RID: 10412
		public bool radial;              // 是否径向
										 // Token: 0x040028AD RID: 10413
		public float width;              // 闪电宽度
										 // Token: 0x040028AE RID: 10414
		public float intensity;          // 亮度
										 // Token: 0x040028AF RID: 10415
		public float lifeTime;           // 生命周期
										 // Token: 0x040028B0 RID: 10416
		public Vector2 rPoint;           // 当前径向随机端点
										 // Token: 0x040028B1 RID: 10417
		public float lightningParam;     // 闪电参数（传给 LightningBolt）
										 // Token: 0x040028B2 RID: 10418
		public float lightningType;      // 闪电类型（影响颜色）
										 // Token: 0x040028B3 RID: 10419
		public LightningBolt permaLightning; // 永久闪电实例
											 // Token: 0x040028B4 RID: 10420
		public int impactType;           // 命中特效类型
										 // Token: 0x040028B5 RID: 10421
		public int soundType;            // 音效类型
										 // Token: 0x040028B6 RID: 10422
		public float volume;             // 音量倍率
										 // Token: 0x040028B7 RID: 10423
		private bool isTerrain;          // 本次闪电是否命中地形
										 // Token: 0x040028B8 RID: 10424
		public DynamicSoundLoop soundLoop; // 电流声循环
										   // Token: 0x040028B9 RID: 10425
		public Color color;              // 闪电颜色
										 // Token: 0x040028BA RID: 10426
		public bool random;              // 是否使用随机触发
										 // Token: 0x040028BB RID: 10427
		public bool ready;               // 当帧是否已准备好触发
										 // Token: 0x040028BC RID: 10428
		private float counter;           // 计数器（非随机触发用）
										 // Token: 0x040028BD RID: 10429
		public bool light;               // 是否产生光源

		/* =====================================================
         * 内部类：闪电命中特效（光爆/火花）
         * ===================================================== */
		// Token: 0x020009F0 RID: 2544
		public class Impact : CosmeticSprite
		{
			/* ---------------- 构造 ---------------- */
			// Token: 0x060050B6 RID: 20662 RVA: 0x0055ED48 File Offset: 0x0055CF48
			public Impact(Vector2 pos, float size, Color color)
			{
				this.pos = pos;
				this.lastPos = pos;
				this.size = size;
				this.color = color;
				this.life = 1f;
				this.lastLife = 1f;
				// 随机生命周期（2~16 秒）
				this.lifeTime = Mathf.Lerp(2f, 16f, size * UnityEngine.Random.value);
			}

			/* ---------------- 更新 ---------------- */
			// Token: 0x060050B7 RID: 20663 RVA: 0x0055EDAC File Offset: 0x0055CFAC
			public override void Update(bool eu)
			{
				// 每帧产生火花
				this.room.AddObject(new Spark(this.pos, Custom.RNV() * 60f * UnityEngine.Random.value,
											  this.color, null, 4, 50));
				if (this.life <= 0f && this.lastLife <= 0f)
				{
					this.Destroy();
					return;
				}
				this.lastLife = this.life;
				this.life = Mathf.Max(0f, this.life - 1f / this.lifeTime);
			}

			/* ---------------- 初始化精灵 ---------------- */
			// Token: 0x060050B8 RID: 20664 RVA: 0x0055EE44 File Offset: 0x0055D044
			public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
			{
				sLeaser.sprites = new FSprite[4];
				// 0: LightSource 高光
				sLeaser.sprites[0] = new FSprite("Futile_White", true);
				sLeaser.sprites[0].shader = rCam.room.game.rainWorld.Shaders["LightSource"];
				// 1: FlatLight 扁平光
				sLeaser.sprites[1] = new FSprite("Futile_White", true);
				sLeaser.sprites[1].shader = rCam.room.game.rainWorld.Shaders["FlatLight"];
				// 2: FlareBomb 火花
				sLeaser.sprites[2] = new FSprite("Futile_White", true);
				sLeaser.sprites[2].shader = rCam.room.game.rainWorld.Shaders["FlareBomb"];
				// 3: 圆环/扁平光
				sLeaser.sprites[3] = new FSprite("Futile_White", true);
				sLeaser.sprites[3].shader = rCam.room.game.rainWorld.Shaders[this.circle ? "FlareBomb" : "FlatLight"];

				// 统一颜色
				for (int i = 0; i < 4; i++)
					sLeaser.sprites[i].color = this.color;

				this.AddToContainer(sLeaser, rCam, rCam.ReturnFContainer("Water"));
			}

			/* ---------------- 绘制 ---------------- */
			// Token: 0x060050B9 RID: 20665 RVA: 0x0055EFC8 File Offset: 0x0055D1C8
			public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam,
											 float timeStacker, Vector2 camPos)
			{
				base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
				float lifeFac = Mathf.Lerp(this.lastLife, this.life, timeStacker);

				// 同步位置
				for (int i = 0; i < 4; i++)
				{
					sLeaser.sprites[i].x = this.pos.x - camPos.x;
					sLeaser.sprites[i].y = this.pos.y - camPos.y;
				}

				// 根据生命周期设置缩放/透明度
				float baseSize = Mathf.Lerp(20f, 120f, Mathf.Pow(this.size, 1.5f));

				sLeaser.sprites[0].scale = Mathf.Pow(Mathf.Sin(lifeFac * Mathf.PI), 0.5f) *
										   Mathf.Lerp(0.8f, 1.2f, UnityEngine.Random.value) *
										   baseSize * 4f / 8f;
				sLeaser.sprites[0].alpha = Mathf.Pow(Mathf.Sin(lifeFac * Mathf.PI), 0.5f) *
										   Mathf.Lerp(0.6f, 1f, UnityEngine.Random.value);

				sLeaser.sprites[1].scale = Mathf.Pow(Mathf.Sin(lifeFac * Mathf.PI), 0.5f) *
										   Mathf.Lerp(0.8f, 1.2f, UnityEngine.Random.value) *
										   baseSize * 4f / 8f;
				sLeaser.sprites[1].alpha = Mathf.Pow(Mathf.Sin(lifeFac * Mathf.PI), 0.5f) *
										   Mathf.Lerp(0.6f, 1f, UnityEngine.Random.value) * 0.2f;

				sLeaser.sprites[2].scale = Mathf.Lerp(0.5f, 1f, Mathf.Sin(lifeFac * Mathf.PI)) *
										   Mathf.Lerp(0.8f, 1.2f, UnityEngine.Random.value) *
										   baseSize / 8f;
				sLeaser.sprites[2].alpha = Mathf.Sin(lifeFac * Mathf.PI) * UnityEngine.Random.value;

				sLeaser.sprites[3].scale = Mathf.Pow(Mathf.Sin(lifeFac * Mathf.PI), 0.5f) *
										   Mathf.Lerp(0.8f, 1.2f, UnityEngine.Random.value) *
										   baseSize * 0.05f * 4f / 5f;
				sLeaser.sprites[3].alpha = Mathf.Pow(Mathf.Sin(lifeFac * Mathf.PI), 0.5f) *
										   Mathf.Lerp(0.9f, 1f, UnityEngine.Random.value);
			}

			/* ---------------- 带 circle 参数的构造 ---------------- */
			// Token: 0x060050BA RID: 20666 RVA: 0x0055F249 File Offset: 0x0055D449
			public Impact(Vector2 pos, float size, Color color, bool circle)
				: this(pos, size, color)
			{
				this.circle = circle;
			}

			/* ---------------- 字段 ---------------- */
			// Token: 0x04005314 RID: 21268
			public float size;      // 尺寸
									// Token: 0x04005315 RID: 21269
			public float life;      // 当前生命
									// Token: 0x04005316 RID: 21270
			public float lastLife;  // 上一帧生命
									// Token: 0x04005317 RID: 21271
			public float lifeTime;  // 最大生命周期
									// Token: 0x04005318 RID: 21272
			public Color color;     // 颜色
									// Token: 0x04005319 RID: 21273
			public bool circle;     // 是否使用圆形贴图
		}
	}
}