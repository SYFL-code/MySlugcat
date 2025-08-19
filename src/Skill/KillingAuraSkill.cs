using On;
using IL;
using System;
using Mono.Cecil;
using MoreSlugcats;
using Noise;
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
using static MonoMod.InlineRT.MonoModRule;


namespace MySlugcat
{
	// 杀戮光环

	public class KillingAuraFSprite : UpdatableAndDeletable
	{
		public static float Radius = 300f;

		private readonly FContainer pointerContainer;
		private readonly FSprite circleSprite;

		private int N;

		// 新增：防止重复销毁
		private bool destroyed;

		public KillingAuraFSprite(int N)
		{
			this.N = N;

			try
			{
				// 1. 创建显示容器
				pointerContainer = new FContainer();
				Futile.stage.AddChild(pointerContainer);

				circleSprite = new FSprite("Circle20")
				{
					scale = Radius / 20f,
					color = new Color(1f, 1f, 1f, 0.3f),
					anchorX = 0.5f,
					anchorY = 0.5f
				};

				// 确保初始状态完全透明且不可见
				circleSprite.alpha = 0f;

				pointerContainer.AddChild(circleSprite);
				pointerContainer.alpha = 0f;
				pointerContainer.isVisible = false;
			}
			catch (Exception e)
			{
				throw new InvalidOperationException("无法初始化circleSprite", e);
			}
		}

		public override void Update(bool eu)
		{
			base.evenUpdate = eu;
			if (!eu) return;

			Player? player = null;
			bool isShow = false;

			var Players = PlayerModuleManager.GetActivePlayers();
			foreach (var player_ in Players)
			{
				if (player_.playerState.playerNumber == N && player_.room != null && !player_.inShortcut && !player_.dead)
				{
					if (player_.GetModule(out var module) && module.PerceptionSkill)
					{
						isShow = true;
						player = player_;
						break;
					}
				}
			}

			if (isShow && player != null && !slatedForDeletetion)
			{
				var cam = player.room.game.cameras.FirstOrDefault(c => c.room == player.room);
				if (cam == null) return;

				circleSprite.alpha = 1f;
				circleSprite.x = player.mainBodyChunk.pos.x - cam.pos.x;
				circleSprite.y = player.mainBodyChunk.pos.y - cam.pos.y;
				pointerContainer.isVisible = true;
			}
			else
			{
				circleSprite.alpha = 0f;
				pointerContainer.isVisible = false;
			}
		}

		public override void Destroy()
		{
			base.slatedForDeletetion = true;

			if (pointerContainer != null)
			{
				pointerContainer.isVisible = false;
				pointerContainer.RemoveFromContainer();
			}
		}

		public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
		{
		}
		public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
		{
		}
		public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
		{

		}
		public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer? newContatiner)
		{
		}

		    // 必须实现 RemoveFromContainer
    /*public void RemoveFromContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites[0].RemoveFromContainer();
    }*/

}


	public class KillingAuraHook
	{
		//public static bool[] HaveFSprite = new bool[100];
		//public static int[] CoolDown = Enumerable.Repeat(400, 100).ToArray();//冷却计时器

		public static readonly Dictionary<int, bool> HaveFSprite = new();
		private static readonly Dictionary<int, int> CoolDown = new();


		public static void Hook()
		{
			On.Player.ctor += Player_ctor;
			On.Player.Update += Player_Update;
			On.Player.Destroy += Player_Destroy;
		}

		private static void Player_ctor(On.Player.orig_ctor orig, Player player, AbstractCreature ac, World world)
		{
			orig(player, ac, world);

			Room room = player.room;
			if (room == null) return;

			int N = player.playerState.playerNumber;
			if (player.GetModule().KillingAuraSkill)
			{
				// 确保只创建一次
				if (!HaveFSprite.ContainsKey(N) || !HaveFSprite[N])
				{
					player.room.AddObject(new KillingAuraFSprite(N));
					HaveFSprite[N] = true;
					CoolDown[N] = 400;
				}
			}
		}

		private static void Player_Update(On.Player.orig_Update orig, Player player, bool eu)
		{
			orig(player, eu);

			Room room = player.room;
			if (room == null || player.dead || player.inShortcut) return;

			int N = player.playerState.playerNumber;
			if (player.GetModule().KillingAuraSkill)
			{
				// 不存在就初始化
				if (!CoolDown.ContainsKey(N)) CoolDown[N] = 0;

				if (--CoolDown[N] <= 0)
				{
					CoolDown[N] = 40;

					List<Creature>? creatures = Extension.CreaturesInRange(room, player.mainBodyChunk.pos, KillingAuraFSprite.Radius, false, player, true, false, player, false);
					if (creatures != null && creatures.Count > 0)
					{
						foreach (Creature creature in creatures)
						{
							if (creature != null)
							{
								if (creature.State is HealthState hs)
								{
									hs.health -= 1f / 10f;
								}
							}
						}
					}

					// 用 SpatialHash 范围查询，避免全房间遍历
					/*foreach (var obj in player.room.physicalObjects.SelectMany(x => x))
					{
						if (obj is Creature creature &&
							Vector2.Distance(player.mainBodyChunk.pos, creature.bodyChunks[0].pos) <= KillingAuraFSprite.Radius)
						{
							creature.Die(); // 安全死亡，触发所有事件
						}
					}*/
				}

				/*if (CoolDown[N] > 0)
				{
					CoolDown[N]--;
				}
				else
				{
					CoolDown[N] = 400;
					List<Creature>? creatures = Extension.CreaturesInRange(room, player.mainBodyChunk.pos, KillingAuraFSprite.Radius, false, player, true, false, player, false);
					if (creatures != null && creatures.Count > 0)
					{
						foreach (Creature creature in creatures)
						{
							if (creature != null)
							{
								if (creature.State is HealthState hs)
								{
									hs.health -= 1;
								}
							}
						}
					}
				}
				if (!HaveFSprite[N] && player.room != null && !player.inShortcut && !player.dead)
				{
					room.AddObject(new KillingAuraFSprite(N));
					HaveFSprite[N] = true;
				}*/
			}
		}

		private static void Player_Destroy(On.Player.orig_Destroy orig, Player player)
		{
			orig(player);

			int N = player.playerState.playerNumber;
			HaveFSprite[N] = false;
			CoolDown[N] = 400;
		}



	}
}
