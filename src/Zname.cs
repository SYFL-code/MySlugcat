using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using UnityEngine;
using MoreSlugcats;
using SlugBase.Features;
using static SlugBase.Features.FeatureTypes;
using RWCustom;
using static Player;
using BepInEx.Logging;
using System.Reflection;

namespace MySlugcat
{
	internal class Zname
	{
		private static readonly string dnSpy = "In fact, this is open source. 其实这是开源的. Code at https://github.com/SYFL-code/MySlugcat";
		private static float zname;

		/*//*/

		/// <summary>
		/// 其他
		/// </summary>
		private static void 其他()
		{//shioldcotClass.Lockodtarget
			try
			{

			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}

			/*ArtificialIntelligence self = creature.abstractCreature.abstractAI.RealAI;

Player player;
foreach (AbstractCreature ac in self.creature.world.game.Players)
{
	if (ac.realizedCreature is Player && player.slugcatStats.name == Plugin.YourSlugID)
	{
		player = (Player)ac.realizedCreature;
		if (self is LizardAI ai &&
			ai.lizard.Template.type == CreatureTemplate.Type.CyanLizard &&
			player.room == self.creature.Room.realizedRoom)
		{
			Lizard cyanLizard = ai.lizard;
			cyanLizard.abstractCreature.world.game.session.creatureCommunities.
				SetLikeOfPlayer(cyanLizard.abstractCreature.creatureTemplate.communityID,
				cyanLizard.abstractCreature.world.RegionNumber,
				(player.State as PlayerState).playerNumber,
				-1.0f);
			//self.tracker.SeeCreature(player.abstractCreature);
		}
	}
}

creature.abstractCreature.world.game.session.creatureCommunities.SetLikeOfPlayer
	(creature.abstractCreature.creatureTemplate.communityID, creature.abstractCreature.world.RegionNumber, (player.State as PlayerState).playerNumber, 1.0f);*/

			/*if (self.GetModule(out var module))
			{
				module.Hungry_Update(self);
			}*/
		}

		private static void Hook()
		{
			// ref 

			// ref bool Execute, ref 

			// ref bool Execute, ref bool return_, ref 
		}

		private static void Plugin()
		{
			//using static PlayerModuleManager;

			//public static ManualLogSource? Logger { get; private set; }
			/*-----------------------------------------------------挂钩-----------------------------------------------------*/

			/*public static readonly PlayerFeature<float> SuperJump = PlayerFloat("slugtemplate/super_jump");
			public static readonly PlayerFeature<bool> ExplodeOnDeath = PlayerBool("slugtemplate/explode_on_death");
			public static readonly GameFeature<float> MeanLizards = GameFloat("slugtemplate/mean_lizards");*/

			//private FixedSkill FixedSkillHook = new FixedSkill();


			// Add hooks-添加钩子
			//private PointerSkill PointerSkillHook = new PointerSkill();


			// Put your custom hooks here!-在此放置你自己的钩子
			//On.Player.Jump += Player_Jump;
			//在玩家触发跳跃时执行Player_Jump
			//On.Player.Die += Player_Die;
			//On.Lizard.ctor += Lizard_ctor;
			//On.Player.Update += Player_Update;

			// Implement MeanLizards-实现激怒蜥蜴的效果
			/*private void Lizard_ctor(On.Lizard.orig_ctor orig, Lizard self, AbstractCreature abstractCreature, World world)
			{
				orig(self, abstractCreature, world);

				if(MeanLizards.TryGet(world.game, out float meanness))
				{
					self.spawnDataEvil = Mathf.Min(self.spawnDataEvil, meanness);
				}
			}*/


			// Implement SuperJump-实现超高跳跃的效果
			/*private void Player_Jump(On.Player.orig_Jump orig, Player self)
			{
				orig(self);//总不能挂完钩子把原本该执行的东西给弄丢吧 这一句就是为了再把它塞进来让它正常运行

				if (SuperJump.TryGet(self, out var power))
				{
					self.jumpBoost *= 1f + power;
				}
			}*/

			// Implement ExlodeOnDeath-实现死亡自爆效果
			/*        private void Player_Die(On.Player.orig_Die orig, Player self)
					{
						if (self.slugcatStats.name == Plugin.YourSlugID && !self.dead)
						{
							Creature obj = FrameSkill.Frame(self, false, self, 12);
							if (obj != null)
							{
								self.dead = false;
								//obj.Die();
								var hs = obj.State as HealthState;
								if (hs != null)
								{
									hs.health -= 1.01f;
								}
							}
							else
							{
								orig(self);
							}
						}
						else
						{
							orig(self);
						}

						bool wasDead = self.dead;
						//布尔值wasDead判断玩家是否死亡

						/orig(self);

						//if(!wasDead && self.dead
						//    && ExplodeOnDeath.TryGet(self, out bool explode)
						//    && explode)
						if(!wasDead
							&& ExplodeOnDeath.TryGet(self, out bool explode)
							&& explode)
						{
							// Adapted from ScavengerBomb.Explode-改编自ScavengerBomb.Explode，即拾荒者炸弹的爆炸效果
							var room = self.room;
							var pos = self.mainBodyChunk.pos;
							var color = self.ShortCutColor();
							//这三行分别获取了房间 身体位置和ShortCutColor，也就是这个生物通过管道时显示的颜色
							room.AddObject(new Explosion(room, self, pos, 7, 250f, 6.2f, 2f, 280f, 0.25f, self, 0.7f, 160f, 1f));
							//在 当前房间 从自己身体 在当前身体的位置 生成一个 持续时长7 半径250，力度6.2，伤害2，眩晕280，致聋0.25，判定击杀由自己造成，伤害乘数0.7，最小眩晕160，背景噪声1的爆炸
							room.AddObject(new Explosion.ExplosionLight(pos, 280f, 1f, 7, color));
							//玩家位置 半径280，透明度1，持续时长7，发光颜色就是上面的shortcutcolor的爆炸光效
							room.AddObject(new Explosion.ExplosionLight(pos, 230f, 1f, 3, new Color(1f, 1f, 1f)));
							//和上面差不多，玩家位置 半径230，透明度1，持续时长3，发光颜色1f1f1f
							room.AddObject(new ExplosionSpikes(room, pos, 14, 30f, 9f, 7f, 170f, color));
							//自己解释下这玩意罢咱累了（

							room.AddObject(new ShockWave(pos, 330f, 0.045f, 5, false));
							//或许没那么累 在自身位置生成冲击波效果 大小330 强度0.045 时长5 false表示绘制顺序（绘制到HUD图层，True就是HUD2
							//思考一下 如果把持续时长改成180会怎么样
							//再想想如何让这玩意影响面积更大（？

							room.ScreenMovement(pos, default, 1.3f);
							//屏幕震动
							room.PlaySound(SoundID.Bomb_Explode, pos);
							//播放爆炸音效
							room.InGameNoise(new Noise.InGameNoise(pos, 9000f, self, 1f));
							//游戏内噪声效果
						}
					}*/
		}

		private static void AI_Behavior()
		{
			//ScavengerAI.Behavior.Attack;                  // 攻击
			//ScavengerAI.Behavior.CommunicateWithPlayer;   // 与玩家交流
			//ScavengerAI.Behavior.EscapeRain;              // 避雨
			//ScavengerAI.Behavior.FindPackLeader;          // 寻找首领
			//ScavengerAI.Behavior.Flee;                    // 逃跑
			//ScavengerAI.Behavior.GuardOutpost;            // 守卫前哨
			//ScavengerAI.Behavior.Idle;                    // 待机
			//ScavengerAI.Behavior.Injured;                 // 受伤
			//ScavengerAI.Behavior.Investigate;             // 调查
			//ScavengerAI.Behavior.LeaveRoom;               // 离开房间
			//ScavengerAI.Behavior.Scavange;                // 搜刮
			//ScavengerAI.Behavior.Travel;                  // 移动

			//LizardAI.Behavior.ActingOutMission;  // 执行使命?
			//LizardAI.Behavior.EscapeRain;        // 避雨
			//LizardAI.Behavior.Fighting;          // 战斗
			//LizardAI.Behavior.Flee;              // 逃离 逃跑
			//LizardAI.Behavior.FollowFriend;      // 跟随伙伴 跟随朋友
			//LizardAI.Behavior.Frustrated;        // 受挫 沮丧
			//LizardAI.Behavior.GoToSpitPos;       // 前往喷射点 吐口水
			//LizardAI.Behavior.Hunt;              // 狩猎
			//LizardAI.Behavior.Idle;              // 待机
			//LizardAI.Behavior.Injured;           // 受伤
			//LizardAI.Behavior.InvestigateSound;  // 调查声响
			//LizardAI.Behavior.Lurk;              // 潜伏
			//LizardAI.Behavior.ReturnPrey;        // 带回猎物 返回猎物 归还猎物
			//LizardAI.Behavior.Travelling;        // 移动 旅行
		}

		private static void CreatureTemplate_Relationship_Type()
		{
            //CreatureTemplate.Relationship.Type.Afraid;              // 害怕
            //CreatureTemplate.Relationship.Type.AgressiveRival;      // 敌对竞争者
            //CreatureTemplate.Relationship.Type.Antagonizes;         // 挑衅
            //CreatureTemplate.Relationship.Type.Attacks;             // 攻击
            //CreatureTemplate.Relationship.Type.DoesntTrack;         // 不追踪
            //CreatureTemplate.Relationship.Type.Eats;                // 捕食
            //CreatureTemplate.Relationship.Type.Ignores;             // 忽视
            //CreatureTemplate.Relationship.Type.Pack;                // 群居
            //CreatureTemplate.Relationship.Type.PlaysWith;           // 与之玩耍
            //CreatureTemplate.Relationship.Type.SocialDependent;     // 社交依赖
            //CreatureTemplate.Relationship.Type.StayOutOfWay;        // 避而远之
            //CreatureTemplate.Relationship.Type.Uncomfortable;       // 感到不适

            //CreatureTemplate.Relationship.Type.valueDictionary;     // 值字典
            //CreatureTemplate.Relationship.Type.values;              // 值集合
            //CreatureTemplate.Relationship.Type.valuesVersion;       // 值版本

        }

		private static void MOD启用()
		{
			//ModManager.ActiveMods             //激活Mods
			//ModManager.InstalledMods          //已存在的Mods
			//ModManager.FailedRequirementIds   //需求ID失败的Mods
			//ModManager.PrePackagedModIDs      //预包装ModsID


			if (ModManager.GameVersionChangedOnThisLaunch)
			{
				//游戏版本在这次发布中发生了变化
			}
			if (ModManager.NonPrepackagedModsInstalled)
			{
				//安装了非预打包的Mods
			}
			if (ModManager.InitializationScreenFinished)
			{
				//初始化屏幕完成
			}
			if (ModManager.DLCShared)
			{
				//DLC共享?
			}
			if (ModManager.MSC)
			{
				//更多蛞蝓猫?_1
			}
			if (ModManager.MMF)
			{
				//更多蛞蝓猫?_2
			}
			if (ModManager.CoopAvailable)
			{
				//联机模式是否可用?
			}
			if (ModManager.JollyCoop)
			{
				//联机模式
			}
			if (ModManager.Expedition)
			{
				//远征模式
			}
			if (ModManager.DevTools)
			{
				//开发者工具
			}
			if (ModManager.Watcher)
			{
				//观望者
			}
		}

		private static void 通行证()
		{

			//"The Survivor"        //"求生者"
			//"The Hunter"          //"猎手"
			//"The Saint"           //"圣徒"
			//"The Wanderer"        //"漫游者"
			//"The Chieftain"       //"酋长"
			//"The Monk"            //"僧侣"
			//"The Outlaw"          //"暴徒"
			//"The Dragon Slayer"   //"屠龙者"
			//"The Scholar"         //"学者"
			//"The Friend"          //"朋友"
			// ModManager.MSC
			//"The Nomad"           //"流浪者"
			//"The Martyr"          //"殉道者"
			//"The Pilgrim"         //"朝圣者"
			//"The Mother"          //"慈母"

			// The Vanguard
			//"The Dragonlord"      //"龙王"
			// Rotund World
			//"The Glutton"         //"贪食者"；暴食者；贪吃者；嗜食者；贪婪者



			//"The Chieftain"       //"酋长" // 线状
			/*return new WinState.FloatTracker(
				this.PassageID,   // ID
				dflt: 0f,          // 默认值
				min: 0f,           // 最小值
				showFrom: 0f,      // 开始显示的进度
				max: 1f           // 最大值
			);*/

			//"The Survivor"        //"求生者" // 点状
			/*return new WinState.IntegerTracker(
				this.PassageID,   // ID
				dflt: 0,          // 默认值
				min: 0,           // 最小值
				showFrom: 1,      // 开始显示的进度
				max: 10           // 最大值
			);*/

			//"The Wanderer"        //"漫游者" // 布尔状 (点状不可逆?)
			/*return new WinState.BoolArrayTracker(
				this.PassageID,   // ID
				SlugcatStats.SlugcatStoryRegions(RainWorld.lastActiveSaveSlot).Count
			);*/

			//"The Dragon Slayer"   //"屠龙者"   // 列表状 (点状不可逆?) 多个布尔条件（如任务清单）
			/*return new WinState.ListTracker(
				this.PassageID,   // ID
				6
			);*/                                 // 布尔状 (点状不可逆?)
			/*return new WinState.BoolArrayTracker(
				this.PassageID,   // ID
				6
			);*/

			//"The Pilgrim"         //"朝圣者" 
			/*int num = 0;
			List<string> list = SlugcatStats.SlugcatStoryRegions(RainWorld.lastActiveSaveSlot);
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i] != "MS" && World.CheckForRegionGhost(RainWorld.lastActiveSaveSlot, list[i]))
				{
					num++;
				}
			}
			endgameTracker = new WinState.BoolArrayTracker(ID, num);*/

			// 美味佳肴 HUD状
			/*return new WinState.GourFeastTracker(
				this.PassageID,   // ID
				WinState.GourmandPassageTracker.Length
			);*/

		}

		private static void CosmeticSprite()//装饰品精灵集
		{
			Vector2 pos = new Vector2(0f, 0f);

			MoreSlugcats.LightningMachine activateLightning = 
				new MoreSlugcats.LightningMachine
				(pos, new Vector2(pos.x, pos.y), new Vector2(pos.x, pos.y + 10f), 0f, false, true, 0.3f, 1f, 1f);
		}

		private static void 着色器()
		{
			//Basic 默认无特效，最普通的纹理绘制
			//Hologram 全息 / 幽灵：带扫描线、轻微噪点、半透明
			//LightSource 发光体：把贴图当作光源，周围产生光晕
			//Water 水流：波纹扭曲、折射
			//Waterfall 瀑布：比
			//Water 更剧烈的扭曲和滚动
			//LensDistortion 镜头畸变：边缘放大、中心收缩的“鱼眼”效果
			//Blur 高斯模糊
			//Fog 雾：颜色叠加 + 深度雾
			//Fire 火焰：滚动噪声、红黄调色
			//HeatDistortion 热扭曲：透过火焰看背景时的空气抖动
			//Lightning 闪电：高亮、闪烁
			//Rain 雨滴：垂直条纹 + 滚动
			//Sand 沙：细小滚动的颗粒
			//Scavenger 拾荒者盔甲：金属高光、反射贴图
			//Slugcat 蛞蝓猫：边缘描边（用于雨眠过场）
			//SkyAndPressureGradient 天空：日夜颜色渐变
			//Bloom 泛光：高亮区域向外晕染
			//Multiply 正片叠底：将贴图与背景颜色相乘

			//Grayscale 去色：变黑白
			// 把精灵的所有颜色信息强行转成灰度亮度，只保留明暗关系。

			//Palette 调色板映射：把灰度贴图按当前房间调色板重新上色  
			// 把一张灰度贴图按当前房间的 调色板（palette） 重新上色。
			// 灰度值 0 → 调色板最暗色
			// 灰度值 1 → 调色板最亮色
			// 介于 0~1 之间 → 插值颜色

			//Shadow 阴影：半透明黑色 + 模糊
			//Ice 冰：高光 + 反射 + 轻微扭曲
			//Centipede 蜈蚣：节段滚动纹理
			//Vulture 秃鹫：金属 + 虹彩
			//JetFish 喷气鱼：带流线滚动
			//SmallNeedleWorm 针虫：皮肤光泽
			//TubeWorm 管虫：内部发光
			//Lantern 灯笼：中心黄白光晕
			//Overseer 观察者：脉冲扫描线 + 发光
			//Glow 纯粹发光，可用于 UI 或特效

			//Basic 基础

			//LevelColor 关卡颜色

			//Background 背景

			//WaterSurface 水面

			//DeepWater 深水区

			//Shortcuts 捷径

			//DeathRain 死亡之雨

			//LizardLaser 蜥蜴激光

			//WaterLight 水光

			//WaterFall 瀑布

			//ShockWave 冲击波

			//Smoke 烟雾

			//Spores 孢子

			//Steam 蒸汽

			//ColoredSprite 彩色精灵

			//ColoredSprite2 彩色精灵2

			//LightSource 光源

			//LightSourceBothSides 双面光源

			//LightSourceRippleSide 波纹面光源

			//LightBloom 光晕

			//SkyBloom 天光晕染

			//Adrenaline 肾上腺素

			//AdrenalineBothSides 双面肾上腺素

			//CicadaWing 蝉翼

			//BulletRain 弹雨

			//CustomDepth 自定义深度

			//CustomDepthBothSides 双面自定义深度

			//UnderWaterLight 水下光源

			//FlatLight 平面光

			//FlatLightRippleSide 波纹面平面光

			//FlatLightBothSides 双面平面光

			//FlatLightBehindTerrain 地形后平面光

			//VectorCircle 矢量圆环

			//VectorCircleBothSides 双面矢量圆环

			//VectorCircleRippleSide 波纹面矢量圆环

			//VectorCircleFadable 可渐隐矢量圆环

			//FlareBomb 闪光弹

			//FlareBombBothSides 双面闪光弹

			//Fog 雾气

			//WaterSplash 水花

			//EelFin 鳗鱼鳍

			//EelBody 鳗鱼身体

			//JaggedCircle 锯齿圆环

			//JaggedCircleBothSides 双面锯齿圆环

			//JaggedCircleRippleSide 波纹面锯齿圆环

			//JaggedSquare 锯齿方块

			//TubeWorm 管虫

			//LizardAntenna 蜥蜴触须

			//TentaclePlant 触手植物

			//TentaclePlantBothSides 双面触手植物

			//TentaclePlantRippleSide 波纹面触手植物

			//LevelMelt 关卡溶解

			//LevelMelt2 关卡溶解2

			//CoralCircuit 珊瑚电路

			//CoralCircuitBothSides 双面珊瑚电路

			//DeadCoralCircuit 死珊瑚电路

			//DeadCoralCircuitBothSides 双面死珊瑚电路

			//CoralNeuron 珊瑚神经元

			//CoralNeuronBothSides 双面珊瑚神经元

			//Bloom 泛光

			//GravityDisruptor 重力干扰器

			//GlyphProjection 符文投影

			//BlackGoo 黑色粘液

			//BlackGooBothSides 双面黑色粘液

			//Map 地图

			//MapAerial 航拍地图

			//MapShortcut 捷径地图

			//LightAndSkyBloom 光与天光晕染

			//SceneBlur 场景模糊

			//EdgeFade 边缘褪色

			//HeatDistortion 热浪扭曲

			//Projection 投影

			//SingleGlyph 单体符文

			//DeepProcessing 深度处理

			//Cloud 云层

			//CloudDistant 远景云层

			//DistantBkgObject 远景背景物体

			//BkgFloor 背景地板

			//House 房屋

			//DistantBkgObjectRepeatHorizontal 水平重复远景物体

			//Dust 尘埃

			//RoomTransition 房间过渡

			//VoidCeiling 虚空天花板

			//FlatLightNoisy 噪点平面光

			//VoidWormBody 虚空蠕虫身体

			//VoidWormFin 虚空蠕虫鳍

			//VoidWormPincher 虚空蠕虫钳

			//FlatWaterLight 平面水光

			//FlatWaterLightBothSides 双面平面水光

			//FlatWaterLightRippleSpawn 波纹生成平面水光

			//FlatWaterLightRippleSpawnRippleSide 波纹面生成平面水光

			//WormLayerFade 蠕虫层渐隐

			//OverseerZip 监视者瞬移

			//GhostSkin 幽灵表皮

			//GhostBall 幽灵球体

			//GhostDistortion 幽灵扭曲

			//GhostSkinRipple 波纹幽灵表皮

			//GhostBallRipple 波纹幽灵球体

			//GhostDistortionRipple 波纹幽灵扭曲

			//GateHologram 门全息投影

			//OutPostAntler 前哨鹿角

			//WaterNut 水坚果

			//Hologram 全息投影

			//HologramBothSides 双面全息投影

			//FireSmoke 火焰烟雾

			//HoldButtonCircle 按钮保持圆环

			//GoldenGlow 金色辉光

			//ElectricDeath 电击死亡

			//VoidSpawnBody 虚空孵化体

			//SceneLighten 场景增亮

			//SceneBlurLightEdges 场景光边模糊

			//SceneRain 场景雨

			//SceneOverlay 场景叠加

			//SceneSoftLight 场景柔光

			//SceneMultiply 场景相乘

			//HologramImage 全息图像

			//HologramBehindTerrain 地形后全息

			//Decal 贴花

			//SpecificDepth 特定深度

			//LocalBloom 局部泛光

			//MenuText 菜单文字

			//DeathFall 坠落死亡

			//DeathFallHeavy 重型坠落死亡

			//KingTusk 帝王獠牙

			//HoloGrid 全息网格

			//SootMark 煤烟痕迹

			//NewVultureSmoke 新秃鹫烟雾

			//SmokeTrail 烟雾轨迹

			//RedsIllness 红色病态

			//HazerHaze 薄雾朦胧

			//Rainbow 彩虹

			//LightBeam 光束

			//SlopedTerrainSurface 斜坡地形表面

			//SlopedTerrainStain 斜坡地形污渍

			//Rubble 碎石

			//Whirlpool 漩涡

			//GeyserWater 间歇泉水

			//BackgroundAdditive 附加背景

			//BackgroundJaggedCircle 背景锯齿圆环

			//BackgroundNoHoles 无孔背景

			//WaterCurrent 水流

			//BlackSpot 黑斑

			//SkyWhaleBody 天鲸身体

			//SkyWhaleCuticle 天鲸角质层

			//KarmicShield 业力护盾

			//TemplarCircle 圣堂圆环

			//TemplarCloak 圣堂斗篷

			//Sandstorm 沙暴

			//SlopedTerrainMask 斜坡地形遮罩

			//SlopedTerrainMaskGrab 斜坡地形遮罩抓取

			//DistantBkgObjectAlpha 远景物体透明度

			//WaterSlush 雪泥水

			//SporesSnow 孢子雪

			//SnowFall 落雪

			//OESphereTop 奥术球顶部

			//OESphereLight 奥术球光源

			//OESphereBase 奥术球基底

			//MoonProjection 月亮投影

			//LocalBlizzard 局部暴风雪

			//LightningBolt 闪电束

			//LevelHeat 关卡热度

			//FastSnowFall 快速落雪

			//FastLocalBlizzard 快速局部暴风雪

			//FastBlizzard 快速暴风雪

			//EnergySwirl 能量漩涡

			//EnergyCell 能量细胞

			//DisplaySnowShader 显示雪着色器

			//BlizzardMapPrerender 暴风雪地图预渲染

			//Blizzard 暴风雪

			//LevelSnowShader 关卡雪着色器

			//InterpolateWindMap 插值风图

			//DustWaveLow 低强度尘埃波

			//DustFlowRenderer 尘埃流渲染器

			//DustWave 尘埃波

			//DustWaveLevel 关卡尘埃波

			//DustWaveLevelLow 低强度关卡尘埃波

			//BlizzardReduction 暴风雪减弱

			//BlizzardMap 暴风雪地图

			//CellDist 细胞距离

			//DisplayWind 显示风

			//SingleGlyphHologram 单体符文全息

			//WaterFallInverted 反向瀑布

			//AquapedeBody 水蜈蚣身体

			//MenuTextGold 金色菜单文字

			//MenuTextCustom 自定义菜单文字

			//WarpPointHoldFrame 跃迁点保持帧

			//Warp 跃迁

			//WarpNoRing 无环跃迁

			//WarpCircleBasic 基础跃迁圆环

			//WarpCircleRipple 波纹跃迁圆环

			//WarpPlayerDistortion 玩家跃迁扭曲

			//HugeTurbine 巨型涡轮

			//RotWormBody 腐烂蠕虫身体

			//RotWormFin 腐烂蠕虫鳍

			//DynamicLevelElementGrab 动态关卡元素抓取

			//DynamicLevelElement 动态关卡元素

			//DevUI_TwinArrowVertical 开发UI垂直双箭头

			//DevUI_TwinArrowHorizontal 开发UI水平双箭头

			//DevUIDepthPreview 开发UI深度预览

			//DynamicLevelRock 动态关卡岩石

			//DynamicLevelRock_NoMovement 静态动态关卡岩石

			//DynamicLevelBlob 动态关卡斑点

			//DynamicLevelTubeSegment 动态关卡管段

			//DynamicLevelWire 动态关卡线缆

			//DynamicLevelUrbanCandle 动态关卡都市烛台

			//DynamicLevelBowl 动态关卡碗

			//DynamicLevelPole 动态关卡杆

			//Aurora 极光

			//AuroraForeground 前景极光

			//AuroraRipple 波纹极光

			//AuroraForegroundRipple 前景波纹极光

			//RippleSpawnBody 波纹生成体

			//RippleSpawnBodyRippleSide 波纹面生成体

			//RippleGlow 波纹辉光

			//RippleGlowRippleSide 波纹面辉光

			//RippleDeath 波纹死亡

			//BrainBall 脑球

			//BrainStem 脑干

			//BrainMold 脑霉菌

			//BrainBallDark 暗黑脑球

			//BrainStemDark 暗黑脑干

			//LocustCluster 蝗虫群

			//LocustClusterShadow 蝗虫群阴影

			//AncientUrbanBuilding 远古都市建筑

			//DustGradient 尘埃渐变

			//BoxWormBody 箱虫身体

			//BoxWormLarvaHolder 箱虫幼虫容器

			//BoxWormBox 箱虫箱体

			//BoxWormOpenBox 箱虫开箱

			//BoxWormFakeLarva 箱虫假幼虫

			//BoxWormLarvaFood 箱虫幼虫食物

			//FireSpriteWing 火精灵翅膀

			//FireSpriteBody 火精灵身体

			//RippleHybrid 混合波纹

			//RippleHybridRipple 混合波纹涟漪

			//RippleHybridBoth 双面混合波纹

			//FlameJet 火焰喷射

			//FlameJetGlow 火焰喷射辉光

			//SaltFlake 盐片

			//SaltFlakeShadow 盐片阴影

			//AetherRainbow 以太彩虹

			//GildedWind 镀金之风

			//Stardust 星尘

			//BackgroundDune 背景沙丘

			//OuterRimBackgroundBuilding 外环背景建筑

			//OuterRimDustGradient 外环尘埃渐变

			//FallingStar 流星

			//WallLight 墙光

			//WallLightSoftEdge 柔边墙光

			//WallLightStaticNoise 静态噪点墙光

			//WallLightHardShadow 硬阴影墙光

			//WallLightFlat 平面墙光

			//WarpTearMask 跃迁撕裂遮罩

			//WarpTearBlocker 跃迁撕裂阻挡器

			//WarpTear 跃迁撕裂

			//WarpTearBad 不良跃迁撕裂

			//WarpTearOuter 外部跃迁撕裂

			//WarpTearRippleSide 波纹面跃迁撕裂

			//WarpTearGrab 跃迁撕裂抓取

			//SKLightning 天空闪电

			//SKLightningForeground 前景天空闪电

			//SKLightningBGFlash 背景天空闪电闪光

			//Darken 暗化

			//SKLightningFlash 天空闪电闪光

			//PrinceStem 王子茎干

			//SkinkStripes 石龙子条纹

			//MothWing 蛾翼

			//MudDecal 泥浆贴花

			//MudPit 泥潭

			//MudOverlay 泥浆覆盖

			//UrbanLife 都市生命

			//UrbanLifeShadow 都市生命阴影

			//UrbanLifeFirstLayer 都市生命首层

			//UrbanShadowsGrab 都市阴影抓取

			//UrbanShadowsBlur 都市阴影模糊

			//UrbanShadowsBlurGrab 都市阴影模糊抓取

			//UrbanShadowGradient 都市阴影渐变

			//SpinToy 旋转玩具

			//SpinToyGlyph 旋转玩具符文

			//BallToy 球玩具

			//SoftToyBody 软玩具身体

			//SoftToyEye 软玩具眼睛

			//GreebleGrid 细节网格

			//PlaceholderBackgroundElement 占位背景元素

			//FirmamentCloud 苍穹云

			//DustDunes 尘埃沙丘

			//CamoMeter 伪装计量器

			//UrbanCandleSSS 都市烛台次表面散射

			//UrbanCandleFlame 都市烛台火焰

			//DeepLightSource 深层光源

			//PoisonSpearTip 毒矛尖

			//ARZapper AR电击器

			//ARZapperOmni AR全向电击器

			//ARZapperOneSide AR单面电击器

			//ARZapperGlow AR电击器辉光

			//ShiftMask 位移遮罩

			//WavesShiftMask 波浪位移遮罩

			//RippleTearMask 波纹撕裂遮罩

			//RippleRingMask 波纹环遮罩

			//RippleBubbleMask 波纹气泡遮罩

			//TransitionRippleMask 过渡波纹遮罩

			//RippleFlow 波纹流

			//RippleGrab 波纹抓取

			//GameplayRippleGrab 游戏波纹抓取

			//RippleBasic 基础波纹

			//RippleBasicRippleSide 基础波纹面

			//RippleBasicRippleSideAlt 基础波纹面替代

			//RippleBasicBothSides 双面基础波纹

			//RippleBasicClipDistortion 基础波纹裁剪扭曲

			//PlayerCamoMask 玩家伪装遮罩

			//PlayerCamoMaskBeforePlayer 玩家前伪装遮罩

			//PlayerRippleTrail 玩家波纹轨迹

		}

		private static void Options()
		{
			/*public Options()
			{
				Options.FrameSkill = this.config.Bind<bool>("FrameSkill", false, new ConfigurableInfo("Enable Frame Skill (default: false)", null, "", new object[]
				{
				"FrameSkill"
				}));
				Options.DeflagrationSkill = this.config.Bind<bool>("DeflagrationSkill", false, new ConfigurableInfo("Enable Deflagration Skill (default: false)", null, "", new object[]
				{
				"DeflagrationSkill"
				}));
				Options.KnitmeshSkill = this.config.Bind<bool>("KnitmeshSkill", false, new ConfigurableInfo("Enable Knitmesh Skill (default: false)", null, "", new object[]
				{
				"KnitmeshSkill"
				}));

				Options.pixelSize = this.config.Bind<float>("PixelSize", 15f, new ConfigurableInfo("50-1", null, "", new object[]
				{
				"Pixel Size"
				}));

				Options.logDebug = this.config.Bind<bool>("logDebug", false, new ConfigurableInfo("Useful for debugging if you share your log files.", null, "", new object[]
				{
				"Log debug"
				}));
				Options.loglevel = this.config.Bind<float>("loglevel", 9f, new ConfigurableInfo("The maximum value is 10, and the minimum value is 0.", null, "", new object[]
				{
				"Log Level"
				}));

			}*/

			/*public override void Initialize()
		{
			base.Initialize();
			this.Tabs = new OpTab[]
{
				new OpTab(this, "General 1"),
				new OpTab(this, "General 2"),
				new OpTab(this, "Tools 1"),
				new OpTab(this, "Tools 2"),
				new OpTab(this, "Tool Settings")
};
			this.curTab = 0;
			this.AddTitle();
			float num = 90f;
			float num2 = 460f;
			float num3 = 40f;

			if (pixelSize != null)
			{
				this.AddTextBox<float>(Options.pixelSize, new Vector2(num, num2 -= num3), 50f);
			}

			if (FrameSkill != null)
			{
				this.AddCheckBox(Options.FrameSkill, new Vector2(num, num2 -= num3), null);
			}
			if (DeflagrationSkill != null)
			{
				this.AddCheckBox(Options.DeflagrationSkill, new Vector2(num, num2 -= num3), null);
			}
			if (KnitmeshSkill != null)
			{
				this.AddCheckBox(Options.KnitmeshSkill, new Vector2(num, num2 -= num3), null);
			}

			if (logDebug != null)
			{
				this.AddCheckBox(Options.logDebug, new Vector2(num, num2 -= num3), null);
			}*/

			/*            if (logDebug != null && copyID != null && FrameSkill != null && DeflagrationSkill != null && KnitmeshSkill != null)
						{
							this.AddCheckBox(Options.FrameSkill, new Vector2(num, num2 -= num3), null);
							this.AddCheckBox(Options.DeflagrationSkill, new Vector2(num, num2 -= num3), null);
							this.AddCheckBox(Options.KnitmeshSkill, new Vector2(num, num2 -= num3), null);
							this.AddCheckBox(Options.logDebug, new Vector2(num, num2 -= num3), null);
							this.AddCheckBox(Options.copyID, new Vector2(num, num2 -= num3), null);
						}*/

			//this.AddTextBox<float>(Options.loglevel, new Vector2(num, num2 -= num3), 50f);

			/*if (loglevel != null)
			{
				Vector2 pos = new Vector2(num, num2 -= num3);
				float width = 50f;
				OpTextBox loglevelTextBox = new OpTextBox(Options.loglevel, pos, width)
				{
					allowSpace = true,
					description = Options.loglevel.info.description
				};
				OpLabel loglevelLabel = new OpLabel(pos.x + width + 18f, pos.y + 2f, Options.loglevel.info.Tags[0] as string, false)
				{
					description = Options.loglevel.info.description
				};
				this.Tabs[this.curTab].AddItems(new UIelement[]
				{
				loglevelTextBox,
				loglevelLabel
				});
			}
		}

		public override void Update()
		{
			if (loglevelTextBox != null && loglevelLabel != null)
			{
				//if (logDebug == null || logDebug.Value)
				if (logDebug != null && logDebug.Value)
				{
					loglevelTextBox.Show();
					loglevelLabel.Show();
				}
				else
				{
					loglevelTextBox.Hide();
					loglevelLabel.Hide();
				}
			}

		}

		private void AddTitle()
		{
			OpLabel opLabel = new OpLabel(new Vector2(150f, 560f), new Vector2(300f, 30f), "Mouse Drag", FLabelAlignment.Center, true, null);
			OpLabel opLabel2 = new OpLabel(new Vector2(150f, 540f), new Vector2(300f, 30f), "Version 1.1.0", FLabelAlignment.Center, false, null);
			this.Tabs[this.curTab].AddItems(new UIelement[]
			{
				opLabel,
				opLabel2
			});
		}

		private void AddCheckBox(Configurable<bool> option, Vector2 pos, Color? c = null)
		{
			if (c == null)
			{
				c = new Color?(MenuColorEffect.rgbMediumGrey);
			}
			OpCheckBox opCheckBox = new OpCheckBox(option, pos)
			{
				description = option.info.description,
				colorEdge = c.Value
			};
			OpLabel opLabel = new OpLabel(pos.x + 40f, pos.y + 2f, option.info.Tags[0] as string, false)
			{
				description = option.info.description,
				color = c.Value
			};
			this.Tabs[this.curTab].AddItems(new UIelement[]
			{
				opCheckBox,
				opLabel
			});
		}

		private void AddTextBox<T>(Configurable<T> option, Vector2 pos, float width = 150f)
		{
			OpTextBox opTextBox = new OpTextBox(option, pos, width)
			{
				allowSpace = true,
				description = option.info.description
			};
			OpLabel opLabel = new OpLabel(pos.x + width + 18f, pos.y + 2f, option.info.Tags[0] as string, false)
			{
				description = option.info.description
			};
			this.Tabs[this.curTab].AddItems(new UIelement[]
			{
				opTextBox,
				opLabel
			});
		}

		public void PostTranslate()
		{
			IEnumerable<FieldInfo> enumerable = from f in base.GetType().GetFields(BindingFlags.Static | BindingFlags.Public)
												where f.FieldType.IsGenericType && f.FieldType.GetGenericTypeDefinition() == typeof(Configurable<>)
												select f;
			RainWorld rainWorld = Custom.rainWorld;
			if (((rainWorld != null) ? rainWorld.inGameTranslator : null) == null || enumerable == null)
			{
				return;
			}
			foreach (FieldInfo fieldInfo in enumerable)
			{
				ConfigurableBase? configurableBase = (ConfigurableBase?)((fieldInfo != null) ? fieldInfo.GetValue(null) : null);
				string? value;
				if (configurableBase == null)
				{
					value = null;
				}
				else
				{
					ConfigurableInfo info = configurableBase.info;
					value = ((info != null) ? info.description : null);
				}
				if (!string.IsNullOrEmpty(value) && configurableBase != null)
				{
					configurableBase.info.description = Custom.rainWorld.inGameTranslator.Translate(configurableBase.info.description.Replace("\n", "<LINE>")).Replace("<LINE>", "\n");
				}
				int num = 0;
				for (; ; )
				{
					int num2 = num;
					int? num3;
					if (configurableBase == null)
					{
						num3 = null;
					}
					else
					{
						ConfigurableInfo info2 = configurableBase.info;
						if (info2 == null)
						{
							num3 = null;
						}
						else
						{
							object[] tags = info2.Tags;
							num3 = ((tags != null) ? new int?(tags.Count<object>()) : null);
						}
					}
					int? num4 = num3;
					if (!(num2 < num4.GetValueOrDefault() & num4 != null))
					{
						break;
					}
					if (configurableBase != null && !string.IsNullOrEmpty(configurableBase.info.Tags[num] as string))
					{
						configurableBase.info.Tags[num] = Custom.rainWorld.inGameTranslator.Translate(((string)configurableBase.info.Tags[num]).Replace("\n", "<LINE>")).Replace("<LINE>", "\n");
					}
					num++;
				}
			}
			int num5 = 0;
			for (; ; )
			{
				int num6 = num5;
				OpTab[] tabs = this.Tabs;
				int? num4 = (tabs != null) ? new int?(tabs.Length) : null;
				if (!(num6 < num4.GetValueOrDefault() & num4 != null))
				{
					break;
				}
				if (this.Tabs[num5] != null && !string.IsNullOrEmpty(this.Tabs[num5].name))
				{
					this.Tabs[num5].name = Custom.rainWorld.inGameTranslator.Translate(this.Tabs[num5].name.Replace("\n", "<LINE>")).Replace("<LINE>", "\n");
				}
				num5++;
			}
			Configurable<bool>? configurable = Options.logDebug;
			if (configurable == null || configurable.Value)
			{
				Log.Logger(2, "Options", "Options.PostTranslate", "Options.PostTranslate, completed translation of options");
			}
		}*/



	}

		private static void Control()
		{
			//private static bool StartRunning = true;
			//public static List<string> ownedPassages = new List<string>(); // 已拥有的通行证


			//private static readonly object lockObject = new object();
			//private static int lockbool = 0;


			/*private static void Player_Update(On.Player.orig_Update orig, Player player, bool eu)
{
	orig.Invoke(player, eu);

	if (player.dead)
	{
		PlayerDead[player.playerState.playerNumber] = true;
	}
}*/

			/*public static void RainWorldGame_Update(On.RainWorldGame.orig_Update orig, RainWorldGame rainWorldGame)
			{
				orig(rainWorldGame);

				if (lockbool > 0)
				{
					lockbool -= 1;
				}
			}*/


			/*public static void RainWorldGame_Update(On.RainWorldGame.orig_Update orig, RainWorldGame rainWorldGame)
			{
				orig(rainWorldGame);

				if (StartRunning)
				{
					if (Options.pixelSize != null && Options.pixelSize.Value != null)
					{
						pixelSize = Options.pixelSize.Value;
					}

					AllPlayerSkill = false;
					MySlugcatStats = 0;
					Exhausted = true;
					FrameSkill = false;
					DeflagrationSkill = false;
					KnitmeshSkill = false;
					PerceptionSkill = false;
					DigestionSkill = false;
					FixedSkill = false;

					StartRunning = false;
				}
			}*/


			//private static int frameCounter = 0; // 帧计数器
			//private const int N = 12000; // 每N帧执行一次（可调整）

			/*private static void Player_Update(On.Player.orig_Update orig, Player player, bool eu)
			{
				orig.Invoke(player, eu);


				// 每N帧执行一次自定义逻辑
				if (++frameCounter >= N)
				{
					frameCounter = 0;
					SetSkill(player);
				}


				//player.room.game.GetStorySession.saveState.deathPersistentSaveData.winState
				//player.SessionRecord.
			}*/

			/*public static void SetSkill(Player player)
			{
				MySlugcatStats = 0;
				Exhausted = true;

				FrameSkill = false;
				DeflagrationSkill = false;
				KnitmeshSkill = false;
				PerceptionSkill = false;
				DigestionSkill = false;
				FixedSkill = false;

				if (player.slugcatStats.name == Plugin.YourSlugID || SC.AllPlayerSkill)
				{
					WinState winState = player.room.game.GetStorySession.saveState.deathPersistentSaveData.winState;
					ownedPassages = new List<string>();
					bool Survivor = false;

					if (winState != null && winState.endgameTrackers.Count > 0)
					{
						for (int i = 0; i < winState.endgameTrackers.Count; i++)
						{
							if (winState.endgameTrackers[i].GoalFullfilled)
							{
								ownedPassages.Add(WinState.PassageDisplayName(winState.endgameTrackers[i].ID));
								if (ownedPassages[i] == "The Survivor")
								{
									PerceptionSkill = true;
									Survivor = true;
								}
							}
						}
					}

					if (Survivor && ownedPassages != null && ownedPassages.Count > 0)
					{
						for (int i = 0; i < ownedPassages.Count; i++)
						{
							if (ownedPassages[i] == "The Outlaw")//"暴徒"
							{
								DeflagrationSkill = true;
							}

						}
					}



					//player.room.game.GetStorySession.saveState.deathPersistentSaveData.winState
					//player.SessionRecord.
				}
			}*/
		}

		private static void MagicCat(Player player)
		{
			int N = player.playerState.playerNumber;
			Vector2 pos = player.mainBodyChunk.pos;
			Room room = player.room;
			var color = player.ShortCutColor();
			//room.rainIntensity

			//

			/*                    for (int j = 0; j < room.abstractRoom.creatures.Count; j++)
					{
						Vector2 pos3 = room.abstractRoom.creatures[j].realizedCreature.mainBodyChunk.pos;
					}*/

			//
			Vector2 vel = Custom.RNV() * 4f * (1f + UnityEngine.Random.value);
			for (int i = 0; i < UnityEngine.Random.Range(5, 8); i++)
			{
				room.AddObject(new Spark(pos, vel, color, null, 20, 40));
			}
			room.PlaySound(SoundID.Bomb_Explode, pos, 0.75f, 1.25f);
			room.AddObject(new Explosion.ExplosionSmoke(pos, vel, 1.1f));
			room.AddObject(new Explosion.ExplosionLight(pos, 400f, 1f, 7, color));
			room.AddObject(new ExplosionSpikes(room, pos, 14, 30f, 9f, 7f, 170f, color));
			room.AddObject(new ShockWave(pos, 4000f, 0.05f, 20, true));

			/*                    float num = UnityEngine.Random.Range(-0.1f, 0.1f);
								room.AddObject(new Explosion.ExplosionLight(pos, 100f, 1f, 5, new Color(1f, 0.9f, 0f)));
								room.AddObject(new ExplosionSpikes(room, pos, 14, 2f, 5f, 7f, 100f, new Color(1f, 0.9f, 0f)));
								room.AddObject(new ShockWave(pos, 100f, 0.05f, 5, false));
								room.PlaySound(SoundID.Spear_Bounce_Off_Wall, pos, 2f, 0.6f + num);
								room.PlaySound(SoundID.SS_AI_Give_The_Mark_Boom, pos, 2f, 1.5f + num);*/

			//

			foreach (var item in room.updateList)
			{
				var creature = item as Creature;
				if (creature != null)
				{
					var player1 = creature as Player;
					if (player1 == null)
					{
						creature.Stun(200);
					}
				}
			}

			for (int j = 0; j < room.abstractRoom.creatures.Count; j++)
			{
				Creature creature = room.abstractRoom.creatures[j].realizedCreature;
				var player1 = creature as Player;
				if (player1 == null)
				{
					creature.Stun(200);
				}
			}

			Debug.Log("1");

			//9cf0a4/363636
		}

		/*//*/


		/*//*/
	}
}
