using UnityEngine;
using RWCustom;
using HUD;
using MoreSlugcats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using UnityEngine.Rendering;


namespace MySlugcat
{
	
	public class Perception : HudPart//
	{




		private readonly FContainer pointerContainer;
		private readonly TriangleMesh pointerMesh; // 使用网格创建自定义形状

		private readonly int N;

		private const float PointerLength = 25f; // 缩短指针长度
		private const float PointerWidth = 10f;  // 增加指针宽度
		private const float CircleRadius = 60f;

		// 平滑旋转
		private float targetAngle;
		private const float RotationSpeed = 10f; // 度/秒

		// 动态颜色变化
		private Color startColor = Color.green;
		private Color endColor = Color.red;
		private float maxDistance = 750f; // 最大距离阈值

		// 脉冲动画
		private float pulseSpeed = 3f; // 脉冲速度
		private float pulseIntensity = 0.2f; // 脉冲强度
		private float baseLength; // 存储原始长度
		private float baseWidth; // 存储原始宽度


		// 新增淡入淡出控制变量
		private float fadeState = 0f; // 0-1表示淡入淡出进度
		private const float FadeSpeed = 2f; // 淡入淡出速度
		private bool isActive = false;

		private List<LightSource> glowEffects = new List<LightSource>();


		public Perception(HUD.HUD hud, int N) : base(hud)
		{
			this.N = N;
			try
			{
				// 1. 创建显示容器
				pointerContainer = new FContainer();
				hud.fContainers[1].AddChild(pointerContainer);
				pointerContainer.alpha = 0f; // 初始透明

				// 2. 使用可靠的"pixel"精灵创建指针
				pointerMesh = new TriangleMesh("pixel", new TriangleMesh.Triangle[]
				{
				new TriangleMesh.Triangle(0, 1, 2) // 单个三角形
				}, true, true);

				// 定义三角形顶点 (等腰三角形)
				pointerMesh.vertices[0] = new Vector2(0, PointerLength);  // 顶点
				pointerMesh.vertices[1] = new Vector2(-PointerWidth / 2, 0);    // 左下角
				pointerMesh.vertices[2] = new Vector2(PointerWidth / 2, 0);     // 右下角

				pointerMesh.color = Color.green;
				// 确保指针初始朝向正确
				pointerMesh.rotation = 0f; // 初始朝向右侧
				pointerMesh.anchorX = 0.5f;
				pointerMesh.anchorY = 0.5f; // 中心锚点便于旋转

				/*pointerSprite = new FSprite("pixel")
				{
					scaleX = PointerWidth,
					scaleY = PointerLength,
					color = Color.green,
					anchorX = 0.5f,
					anchorY = 0f // 中心锚点
				};*/

				// 4. 添加到容器
				pointerContainer.AddChild(pointerMesh);
				//pointerContainer.AddChild(pointerSprite);

				// 5. 初始化其他变量
				baseLength = PointerLength;
				baseWidth = PointerWidth;
				targetAngle = 0f;

				// 确保初始状态完全透明且不可见
				pointerContainer.alpha = 0f;
				pointerContainer.isVisible = false;
				pointerMesh.alpha = 0f;

				if (hud.owner is Creature cre)
				{
					Creature owner = cre;
					Room room = cre.abstractCreature.world.game.cameras[0].room;
					if (room != null)
					{

						// 6. 安全创建光效
						if (room != null)
						{
							for (int i = 0; i < 3; i++)
							{
								var light = new LightSource(owner.mainBodyChunk.pos, false, Color.red, owner);
								glowEffects.Add(light);
								room.AddObject(light);
							}
						}

						Creature? creature = null;
						if (owner.room != null)
						{
							creature = Extension.FindNearestCreature(owner.firstChunk.pos, owner.room, false, owner, false, 2);
							if (creature == null)
							{
								return;
							}
							if (creature != null)
							{
								Vector2? vector = creature.firstChunk.pos;
								if (vector == null)
								{
									return;
								}
							}
						}
						if (creature == null)
						{
							return;
						}

						// 1. 获取世界坐标
						Vector2 targetWorldPos = creature.mainBodyChunk.pos;
						Vector2 ownerWorldPos = owner.firstChunk.pos;

						// 2. 计算方向向量(从玩家指向目标)
						Vector2 direction = (targetWorldPos - ownerWorldPos).normalized;
						targetAngle = Custom.VecToDeg(direction);

						pointerMesh.rotation = targetAngle;

					}
				}

			}
			catch (Exception e)
			{
				Debug.LogError($"CreaturePointer初始化失败: {e.Message}");
				// 失败时确保清理资源
				if (pointerContainer != null)
				{
					pointerContainer.RemoveFromContainer();
				}
				throw; // 重新抛出异常让上层处理
			}
		}

		public override void Update()
		{
			if (hud.owner is Player owner)
			{
				/*foreach (WeakReference<Player> weakPlayerRef in PlayerModuleManager.players)
                {
                    //Player? GetPlayer = null;
                    if (weakPlayerRef.TryGetTarget(out var GetPlayer))
                    {

                    }
                }*/

				//int N = owner.playerState.playerNumber;

				Player? player = null;
				bool isShow = false;

				var Players = PlayerModuleManager.GetActivePlayers();
				foreach (var player_ in Players)
				{
					if (player_.playerState.playerNumber == N && player_.room == owner.room && !player_.dead)
					{
						if (PlayerModuleManager.PlayerModules.TryGetValue(player_, out var module) && module.PerceptionSkill)
						{
							isShow = true;
							player = player_;
							break;
						}
					}
				}

				if (isShow && player != null)
				{
					if (player.abstractCreature.world.game.GamePaused)
					{
						//pointerMesh.color = Color.black;
						return;
					}

					bool shouldBeActive = true;
					Creature? creature = null;
					float timeStacker = Time.deltaTime;//1秒60帧，那增量时间就是 1/60 秒  (Time.deltaTime)

					if (player.room == null && player.inShortcut)
					{
						shouldBeActive = false;
					}
					if (player is Player player01 && player01.Sleeping)
					{
						shouldBeActive = false;
					}
					if (player is Player player02 && player02.dead)
					{
						shouldBeActive = false;
					}
					if (player.room != null)
					{
						creature = Extension.FindNearestCreature(player.firstChunk.pos, player.room, false, player, false, 2);
						if (creature == null)
						{
							shouldBeActive = false;
						}
						if (creature != null)
						{
							Vector2? vector = creature.firstChunk.pos;
							if (vector == null)
							{
								shouldBeActive = false;
							}
						}
					}

					if (shouldBeActive != isActive)
					{
						isActive = shouldBeActive;
						fadeState = Mathf.Clamp01(fadeState); // 确保在0-1范围内
					}

					// 改进的淡入淡出控制
					float targetAlpha = isActive ? 1f : 0f;
					float fadeDelta = (isActive ? FadeSpeed : -FadeSpeed) * Time.deltaTime;

					// 更平滑的渐变过渡
					fadeState = Mathf.Clamp01(fadeState + fadeDelta * 0.5f); // 降低变化速度

					// 使用更明显的缓动函数
					float currentAlpha = EnhancedEaseInOut(fadeState);

					// 应用透明度到所有元素
					pointerContainer.alpha = currentAlpha;
					pointerMesh.alpha = currentAlpha;

					// 更新光效透明度
					foreach (var light in glowEffects)
					{
						light.setAlpha = currentAlpha * 0.7f;
					}

					// 确保当完全透明时停止更新
					if (fadeState <= 0f)
					{
						pointerContainer.isVisible = false; // 直接隐藏整个容器
						return;
					}
					else
					{
						pointerContainer.isVisible = true;
					}

					// 如果没有激活或完全透明，跳过更新
					//if (fadeState <= 0f || !shouldBeActive || player.inShortcut) return;
					if (creature == null) return;

					// 摄像机坐标
					float camX = 300f;
					float camY = 300f;
					if (player.room != null)
					{
						camX = player.room.game.cameras[0].pos.x;
						camY = player.room.game.cameras[0].pos.y;
					}

					// 1. 获取世界坐标
					Vector2 targetWorldPos = creature.mainBodyChunk.pos;
					Vector2 playerWorldPos = player.firstChunk.pos;

					// 2. 计算方向向量(从玩家指向目标)
					Vector2 direction = (targetWorldPos - playerWorldPos).normalized;
					targetAngle = Custom.VecToDeg(direction);
					//targetAngle = Custom.VecToDeg(targetWorldPos - playerWorldPos);

					// 平滑旋转
					float currentVelocity = 0f;
					float smoothTime = 0.5f; // 调整这个值（越大越慢）
					float currentAngle2 = Mathf.SmoothDampAngle(pointerMesh.rotation, targetAngle, ref currentVelocity, smoothTime);

					float degreesPerSecond = 60f; // 每秒旋转 60 度
					float maxStep = degreesPerSecond * Time.deltaTime; // 每帧最大步长
					float currentAngle = Mathf.MoveTowardsAngle(pointerMesh.rotation, targetAngle, maxStep);

					float currentAngle1 = Mathf.LerpAngle(
						pointerMesh.rotation,
						targetAngle,
						timeStacker * RotationSpeed * 0.01f);


					// 3. 计算屏幕空间位置
					Vector2 playerScreenPos = new Vector2(playerWorldPos.x - camX, playerWorldPos.y - camY);

					// 更新指针位置和旋转
					pointerContainer.SetPosition(playerScreenPos);
					pointerMesh.rotation = currentAngle;

					// 调整指针位置(尖端指向目标)
					Vector2 pointerOffset = Custom.DegToVec(currentAngle) * 30f;
					pointerMesh.SetPosition(pointerOffset);


					// 目标接近时震动效果
					float distance = Vector2.Distance(playerWorldPos, targetWorldPos);
					distance = distance * 1.5f;
					float shakeIntensity = Mathf.Clamp01(1f - distance / 300f) * fadeState;
					Vector2 exactPos = pointerOffset + Custom.RNV() * shakeIntensity * 3f;
					pointerMesh.SetPosition(exactPos);

					// 动态颜色变化
					float Lerp = Mathf.Clamp01(distance / maxDistance);
					pointerMesh.color = Color.Lerp(endColor, startColor, Lerp / 1.5f);

					// 脉冲动画
					float pulse = 0.5f + Mathf.Sin(Time.time * pulseSpeed) * 0.5f;
					float pulseIntensity_ = Mathf.Clamp01(maxDistance / distance / 5) / 3;
					if (distance >= 1100)
					{
						pulseIntensity_ = 0f;
					}
					pointerMesh.scaleX = baseLength * (1f + pulse * pulseIntensity_) * 0.075f;
					pointerMesh.scaleY = baseWidth * (1f + pulse * pulseIntensity_) * 0.075f;
				}
				else
				{
					pointerContainer.isVisible = false;
					return;
				}




			}
		}

		private float EnhancedEaseInOut(float t)
		{
			// 使用三次方缓动，效果更明显
			return t < 0.5f ?
				4f * t * t * t :
				1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
		}

		public override void ClearSprites()
		{
			// 先隐藏再移除
			//slatedForDestroy = true;
			if (pointerContainer != null)
			{
				pointerContainer.isVisible = false;
				pointerContainer.RemoveFromContainer();
			}

			// 清理光效
			foreach (var light in glowEffects)
			{
				if (light != null && light.room != null)
				{
					light.Destroy();
				}
			}
			glowEffects.Clear();
		}



	}
}
