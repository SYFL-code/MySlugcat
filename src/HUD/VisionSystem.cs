using UnityEngine;
using RWCustom;
using HUD;
using MoreSlugcats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using UnityEngine.Rendering;
using static MySlugcat.PlayerModuleManager;


namespace MySlugcat
{

	// VisionSystem 视觉系统

	public class VisionSystem : HudPart
	{

		public static void Hook()
		{
			// 睡眠HUD初始化
			//On.HUD.HUD.InitSleepHud += HUD_InitSleepHud;
			// 单人模式HUD初始化
			On.HUD.HUD.InitSinglePlayerHud += HUD_InitSinglePlayerHud;
			// 多人模式HUD初始化
			On.HUD.HUD.InitMultiplayerHud += HUD_InitMultiplayerHud;
			// 狩猎模式HUD初始化
			On.HUD.HUD.InitSafariHud += HUD_InitSafariHud;
			// 快速瞬移通行证HUD初始化
			//On.HUD.HUD.InitTeleportHud += HUD_InitTeleportHud;
		}

		/*private static void HUD_InitSleepHud(On.HUD.HUD.orig_InitSleepHud orig, HUD.HUD HUD, Menu.SleepAndDeathScreen sleepAndDeathScreen, HUD.Map.MapData mapData, SlugcatStats charStats)
		{
			//Log.Logger(7, "IntelHUD", "MySlugcat:IntelHUD:HUD_InitSleepHud", $"st");
			if (charStats.name == Plugin.YourSlugID)
			{
				SpawnHUD(HUD);
			}
			orig.Invoke(HUD, sleepAndDeathScreen, mapData, charStats);
		}*/

		private static void HUD_InitSinglePlayerHud(On.HUD.HUD.orig_InitSinglePlayerHud orig, HUD.HUD HUD, RoomCamera cam)
		{
			if (cam.room.game.StoryCharacter == Plugin.YourSlugID)
			{
				SpawnHUD(HUD);
			}
			orig.Invoke(HUD, cam);
		}

		private static void HUD_InitMultiplayerHud(On.HUD.HUD.orig_InitMultiplayerHud orig, HUD.HUD HUD, ArenaGameSession session)
		{
			if (session.room.game.StoryCharacter == Plugin.YourSlugID)
			{
				SpawnHUD(HUD);
			}
			orig.Invoke(HUD, session);
		}

		private static void HUD_InitSafariHud(On.HUD.HUD.orig_InitSafariHud orig, HUD.HUD HUD, RoomCamera cam)
		{
			if (cam.room.game.StoryCharacter == Plugin.YourSlugID)
			{
				SpawnHUD(HUD);
			}
			orig.Invoke(HUD, cam);
		}

		private static void SpawnHUD(HUD.HUD HUD)
		{
			HUD.AddPart(new VisionSystem(HUD));
			if (ActivePlayerCount > 0)
			{
				for (int i = 0; i < ActivePlayerCount; i++)
				{
					HUD.AddPart(new Perception(HUD, i));
				}
			}
			else
			{
				for (int i = 0; i < 4; i++)
				{
					HUD.AddPart(new Perception(HUD, i));
				}
			}
		}


		public FContainer container;
		//public FSprite background;
		public FSprite[,] pixelGrid;

		public float gridX = 700;
		public float gridY = 400;

		public float pixelSize = 50; // 每个"像素"的大小
		public int gridWidth = 28; // 网格宽度(像素数)
		public int gridHeight = 16; // 网格高度(像素数)

		public bool[] holeActive = new bool[100]; // 新增状态标志数组
		public Vector2[] holeCenters = new Vector2[100]; // 洞的中心位置
		public float[] holeRadii = new float[100]; // 洞的半径
		public float[] holeEdge = new float[100]; // 洞的半径
		public Color[] holeColor = new Color[100]; // 洞的颜色
		//public List<bool, Vector2, float, Color> values = new List<bool, Vector2, float, Color>;

		public float Clear = 0;

		public static float Alpha = 0.9f; // 初始透明度
		// 添加常量定义（替代硬编码）
		private const float HOLE_RADIUS = 80f; // 洞半径
		private const float HOLE_EDGE = 120f; // 洞边缘
		public static Color HOLE_COLOR = new Color(0f, 0f, 0f); // 洞颜色

		public VisionSystem(HUD.HUD hud) : base(hud)
		{
			pixelSize = Control.pixelSize; // 每个"像素"的大小
			gridWidth = (int)Math.Ceiling(gridX * 2 / pixelSize); // 网格宽度(像素数)
			gridHeight = (int)Math.Ceiling(gridY * 2 / pixelSize); // 网格高度(像素数)
			Alpha = Control.Alpha;

			// 创建容器
			container = new FContainer();
			hud.fContainers[1].AddChild(container);

			// 创建背景矩形
			/*background = new FSprite("pixel")
			{
				*//*width = gridWidth * pixelSize,
				height = gridHeight * pixelSize,*//*
				width = 800,
				height = 500,
				color = Color.black,
				x = gridX,
				y = gridY
			};*/
			//container.AddChild(background);

			// 创建像素网格
			pixelGrid = new FSprite[gridWidth, gridHeight];
			for (int x = 0; x < gridWidth; x++)
			{
				for (int y = 0; y < gridHeight; y++)
				{
					pixelGrid[x, y] = new FSprite("pixel")
					{
						width = pixelSize,
						height = pixelSize,
						color = new Color(0, 0, 0, 0.7f), // 初始半透明
						x = gridX + (x - gridWidth / 2) * pixelSize,
						y = gridY + (y - gridHeight / 2) * pixelSize
					};
					container.AddChild(pixelGrid[x, y]);
				}
			}

			// 初始化洞数据
			for (int i = 0; i < 100; i++)
			{
				holeRadii[i] = HOLE_RADIUS; // 默认半径
				holeEdge[i] = HOLE_EDGE;
				holeColor[i] = HOLE_COLOR;
			}
		}

		public override void Update()
		{
			//base.Update();

			// 更新洞的位置 (这里简化处理，实际应根据游戏逻辑更新)
			// 重置所有洞状态
			for (int i = 0; i < 100; i++)
			{
				holeActive[i] = false;
				holeRadii[i] = HOLE_RADIUS; // 默认半径
				holeEdge[i] = HOLE_EDGE;
				holeColor[i] = HOLE_COLOR;
			}

			bool allPlayerDead = true;
			bool NotVisionSystem = true;

			var Players = PlayerModuleManager.GetActivePlayers();
			foreach (var player in Players)
			{
				if (player.GetModule().VisionSystem)
				{
					NotVisionSystem = false;
				}
				if (!player.dead)
				{
					allPlayerDead = false;
				}
			}


			if (NotVisionSystem)
			{
				ClearSprites();
				return;
			}

			if (allPlayerDead && Clear == 0)
			{
				Clear = 0.025f;
				HOLE_COLOR = new Color(1f, 1f, 1f);
			}

			if (Clear != 0)
			{
				holeCenters[99] = new Vector2(gridX, gridY);
				holeRadii[99] = (gridX + 50) * Clear; // 默认半径
				holeEdge[99] = (gridX + 50) + 40;
				holeColor[99] = new Color(1f, 1f, 1f);
				holeActive[99] = true; // 标记有效洞
				Clear += 0.025f;
			}

			if (Clear >= 1.01f)
			{
				ClearSprites();
				return;
			}

			if (hud.owner is Creature cre)
			{
				Room room = cre.abstractCreature.world.game.cameras[0].room;
				if (room != null)
				{
					int validCount = 0;
					foreach (var crit in room.abstractRoom.creatures)
					{
						if (crit.realizedCreature != null && !crit.realizedCreature.inShortcut)
						{
							Creature creature = crit.realizedCreature;
							if (creature is Player player && !player.inShortcut)
							{
								int N = player.playerState.playerNumber;
								if (N >= 0 && N < 100 && validCount < 100)
								{
									holeCenters[N] = creature.mainBodyChunk.pos - room.game.cameras[0].pos;
									holeRadii[N] = HOLE_RADIUS; // 默认半径
									holeEdge[N] = HOLE_EDGE;
									holeColor[N] = HOLE_COLOR;
									holeActive[N] = true; // 标记有效洞
									validCount++;
								}
							}
						}
					}
				}
			}

			// 更新像素透明度
			UpdatePixelTransparency();
		}

		private void UpdatePixelTransparency()
		{
			// 预先构建有效玩家位置列表（避免遍历整个100元素数组）
			/*var activeHoles = new List<Vector2>();
			for (int i = 0; i < 100; i++)
			{
				if (holeActive[i]) activeHoles.Add(holeCenters[i]);
			}*/

			for (int x = 0; x < gridWidth; x++)
			{
				for (int y = 0; y < gridHeight; y++)
				{
					// 计算像素在屏幕上的位置
					Vector2 pixelPos = new Vector2(
						gridX + (x - gridWidth / 2) * pixelSize,
						gridY + (y - gridHeight / 2) * pixelSize
					);

					// 初始透明度
					Color color = HOLE_COLOR;
					float alpha = Alpha;

					// 检查是否在任意洞内
					/*foreach (var center in activeHoles) // 仅遍历有效洞
					{
						float distance = Vector2.Distance(pixelPos, center);
						if (distance < HOLE_RADIUS) // 使用常量更安全
						{
							alpha = 0f;
							break; // 完全透明，无需继续判断
						}
						else if (distance < HOLE_EDGE)
						{
							float edgeAlpha = (distance - HOLE_RADIUS) / (HOLE_EDGE - HOLE_RADIUS);
							alpha = Mathf.Min(alpha, edgeAlpha);
						}
					}*/

					for (int i = 0; i < 100; i++)
					{
						Vector2 holeCenter = holeCenters[i];

						if (holeCenter == Vector2.zero) continue;
						if (!holeActive[i]) continue;

						float distance = Vector2.Distance(pixelPos, holeCenter);
						if (distance < holeRadii[i]) // 洞半径
						{
							// 在洞内，完全透明
							color = holeColor[i];
							alpha = 0f;
							break;
						}
						else if (distance < holeEdge[i]) // 洞边缘，渐变
						{
							// 计算渐变透明度
							float edgeAlpha = (distance - holeRadii[i]) / (holeEdge[i] - holeRadii[i]);
							alpha = Mathf.Min(alpha, edgeAlpha);
							color = holeColor[i];
						}
					}


					/*foreach (var holeCenter in holeCenters)
					{
						if (holeCenter == Vector2.zero) continue;

						float distance = Vector2.Distance(pixelPos, holeCenter);
						if (distance < 60f) // 洞半径
						{
							// 在洞内，完全透明
							alpha = 0f;
							break;
						}
						else if (distance < 80f) // 洞边缘，渐变
						{
							// 计算渐变透明度
							float edgeAlpha = (distance - 60f) / 20f;
							alpha = Mathf.Min(alpha, edgeAlpha);
						}
					}*/

					// 更新像素透明度
					//Color color = pixelGrid[x, y].color;
					color.a = alpha;
					pixelGrid[x, y].color = color;
				}
			}
		}

		public override void Draw(float timeStacker)
		{
			base.Draw(timeStacker);
			// 更新已经在Update中完成
		}

		public override void ClearSprites()
		{
			container.isVisible = false;
			container.alpha = 0f;
			container.RemoveFromContainer();
			//background.RemoveFromContainer();

			for (int x = 0; x < gridWidth; x++)
			{
				for (int y = 0; y < gridHeight; y++)
				{
					pixelGrid[x, y].RemoveFromContainer();
				}
			}
		}
	}




	/*public class VisionSystem : HudPart
	{


		public static void Hook()
		{
			//Log.Logger(7, "IntelHUD", "MySlugcat:IntelHUD:Hook", $"sst");
			On.HUD.HUD.InitSleepHud += HUD_InitSleepHud;
			On.HUD.HUD.InitSinglePlayerHud += HUD_InitSinglePlayerHud;
		}

		private static void HUD_InitSleepHud(On.HUD.HUD.orig_InitSleepHud orig, HUD.HUD self, Menu.SleepAndDeathScreen sleepAndDeathScreen, HUD.Map.MapData mapData, SlugcatStats charStats)
		{
			//Log.Logger(7, "IntelHUD", "MySlugcat:IntelHUD:HUD_InitSleepHud", $"st");
			orig.Invoke(self, sleepAndDeathScreen, mapData, charStats);
			self.AddPart(new VisionSystem(self));
		}

		private static void HUD_InitSinglePlayerHud(On.HUD.HUD.orig_InitSinglePlayerHud orig, HUD.HUD self, RoomCamera cam)
		{
			//Log.Logger(7, "IntelHUD", "MySlugcat:IntelHUD:HUD_InitSinglePlayerHud", $"st");
			orig.Invoke(self, cam);
			self.AddPart(new VisionSystem(self));
		}


		public FContainer container;
		public FSprite rect;
		public FSprite[] circles;
		Vector2[] poss = new Vector2[100];

		// 自定义着色器
		private static FShader _holeShader;


		public VisionSystem(HUD.HUD hud) : base(hud)
		{
			container = new FContainer();
			hud.fContainers[1].AddChild(container);

			// 创建背景
			rect = new FSprite("pixel");
			rect.width = 800;
			rect.height = 500;
			rect.color = Color.black;
			rect.SetPosition(new Vector2(gridX, gridY));
			container.AddChild(rect);

			// 创建遮罩容器
			FContainer maskContainer = new FContainer();
			container.AddChild(maskContainer);

			// 创建遮罩背景
			FSprite maskBg = new FSprite("pixel")
			{
				width = 800,
				height = 500,
				color = Color.white,
				x = gridX,
				y = gridY
			};
			maskContainer.AddChild(maskBg);

			// 创建圆形洞
			circles = new FSprite[100];
			for (int i = 0; i < 100; i++)
			{
				circles[i] = new FSprite("Circle20")
				{
					scale = 120 / 20f,
					color = Color.black, // 黑色部分将"挖掉"
					anchorX = 0.5f,
					anchorY = 0.5f
				};
				maskContainer.AddChild(circles[i]);
			}

			// 应用 Futile 的遮罩效果（如果支持）
			// 注意：标准 Futile 可能不支持直接遮罩，需要检查文档

			*//*container = new FContainer();
			hud.fContainers[1].AddChild(container);

			// 创建背景
			rect = new FSprite("pixel");
			rect.width = 800;
			rect.height = 500;
			rect.color = Color.black;
			rect.SetPosition(new Vector2(gridX, gridY));
			container.AddChild(rect);

			// 创建遮罩纹理
			Texture2D maskTexture = new Texture2D(800, 500);
			Color[] maskPixels = new Color[800 * 500];

			// 填充白色（不透明）
			for (int i = 0; i < maskPixels.Length; i++)
			{
				maskPixels[i] = Color.white;
			}

			// 应用纹理
			maskTexture.SetPixels(maskPixels);
			maskTexture.Apply();

			// 创建遮罩精灵
			FSprite mask = new FSprite(maskTexture);
			mask.SetPosition(new Vector2(gridX, gridY));
			mask.shader = Custom.rainWorld.Shaders["Basic"]; // 使用基本着色器

			// 获取 Futile 的混合着色器
			FShader multiplyShader = Custom.rainWorld.Shaders["Multiply"];

			// 创建圆形洞
			circles = new FSprite[100];
			for (int i = 0; i < 100; i++)
			{
				circles[i] = new FSprite("Circle20")
				{
					scale = 120 / 20f,
					color = new Color(0f, 0f, 0f, 1f), // 黑色表示透明区域
					anchorX = 0.5f,
					anchorY = 0.5f,
					shader = multiplyShader // 应用乘法混合着色器
				};
				container.AddChild(circles[i]);
			}*/

	/*container = new FContainer();
	hud.fContainers[1].AddChild(container);

	// 创建半透明黑色背景
	rect = new FSprite("pixel");
	rect.width = 800;
	rect.height = 500;
	rect.color = new Color(0, 0, 0, 0.99f); // 70%不透明
	rect.SetPosition(new Vector2(gridX, gridY));
	container.AddChild(rect);

	// 创建"洞"（实际上是白色圆形）
	circles = new FSprite[100];
	for (int i = 0; i < 100; i++)
	{
		circles[i] = new FSprite("Circle20")
		{
			scale = 120 / 20f,
			color = new Color(1, 1, 1, 0.01f), // 浅色低透明度
			anchorX = 0.5f,
			anchorY = 0.5f
		};
		container.AddChild(circles[i]);
	}*/


	/*// 1. 创建一个 FContainer 作为父容器
	container = new FContainer();
	hud.fContainers[1].AddChild(container); // 添加到HUD容器

	// 2. 添加一个长方形（背景）
	*//*rect = new FSprite("pixel")
	{
		scaleX = 400,
		scaleY = 200,
		color = Color.black,
		anchorX = 0.5f,
		anchorY = 0.5f // 中心锚点
	};*//*

	rect = new FSprite("pixel"); // 假设 "square" 是白色矩形
	rect.width = 800; // 设置宽度
	rect.height = 500; // 设置高度
	rect.color = Color.black; // 设置颜色
	rect.SetPosition(new Vector2(gridX, gridY));
	container.AddChild(rect);

	circles = new FSprite[100];
	for (int i = 0; i < 100; i++)
	{
		// 3. 添加一个圆形（作为“透明”部分）
		circles[i] = new FSprite("Circle20")
		{
			scale = 120 / 20f,
			color = new Color(0.5f, 0.5f, 0.5f, 1f),
			anchorX = 0.5f,
			anchorY = 0.5f
		};

		*//*circles[i] = new FSprite("circle20"); // 假设 "circle" 是白色圆形
		circles[i].width = 100; // 设置直径
		circles[i].height = 100;
		circles[i].x = 0; // 居中
		circles[i].y = 0;
		circles[i].color = new Color(0, 0, 0, 0); // 完全透明（如果引擎支持）*//*
		container.AddChild(circles[i]);

		// 每个精灵使用"pixel"图像和"Hologram"着色器
		*//*circles[i] = new FSprite("pixel")
		{
			shader = Custom.rainWorld.Shaders["Hologram"],
			color = Color.white
		};*//*
		//hud.fContainers[1].AddChild(circles[i]); // 添加到HUD容器
	}

	// 如果引擎不支持直接透明，可以使用混合模式（如 Futile 的 `BlendMode.Subtractive`）
	// circle.blendMode = BlendMode.Subtractive; // 可能需要调整

	//container.AddChild(circles);*//*
}

public override void Update()
{
	if (hud.owner is Creature cre)
	{
		Room room = cre.abstractCreature.world.game.cameras[0].room;
		if (room != null)
		{
			if (room.abstractRoom.creatures.Count > 0)
			{
				foreach (var crit in room.abstractRoom.creatures)
				{
					if (crit.realizedCreature != null && !crit.realizedCreature.inShortcut)
					{
						Creature creature = crit.realizedCreature;
						if (creature is Player player)
						{
							int N = player.playerState.playerNumber;
							poss[N] = player.firstChunk.pos;
						}
					}
				}
			}
		}
	}

	foreach (var pos in poss)
	{
		if (pos != null && pos != Vector2.zero)
		{

		}
	}

}

public override void Draw(float timeStacker)
{
	if (hud.owner is Creature cre)
	{
		Room room = cre.abstractCreature.world.game.cameras[0].room;
		if (room != null)
		{
			//rect.SetPosition(new Vector2(gridX, gridY) - room.game.cameras[0].pos);
			for (int i = 0; i < 100; i++)
			{
				if (poss[i] != null && poss[i] != Vector2.zero && circles[i] != null)
				{
					circles[i].SetPosition(poss[i] - room.game.cameras[0].pos);
					//circles[i].RemoveFromContainer();
				}

			}
		}
	}
}

public override void ClearSprites()
{
	//Log.Logger(9, "IntelHUD", "MySlugcat:IntelHUD:ClearSprites", $"st");
	container.isVisible = false;
	container.RemoveFromContainer();
	rect.RemoveFromContainer();
	for (int i = 0; i < 100; i++)
	{
		circles[i].RemoveFromContainer();
	}
}


}*/
}
