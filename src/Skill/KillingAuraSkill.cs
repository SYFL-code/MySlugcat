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

	public class KillingAuraSkill
	{
		//private readonly FContainer pointerContainer;
		//private readonly FSprite circleSprite;
		public static float Radius = 300f;

		private int sprite_index;

		public KillingAuraSkill()
		{
		}

		//在PlayerGraphics_InitiateSprites里调用
		public void InitiateSprites(ref bool Execute, ref On.PlayerGraphics.orig_InitiateSprites orig, ref PlayerGraphics playerGraphics, ref RoomCamera.SpriteLeaser sLeaser, ref RoomCamera rCam)
		{
			//图像扩容和设置图像
			FSprite circleSprite = new FSprite("Circle20")
			{
				scale = Radius / 20f,
				color = new Color(1f, 1f, 1f, 0.3f),
				anchorX = 0.5f,
				anchorY = 0.5f
			};

			sprite_index = sLeaser.sprites.Length;
			Array.Resize<FSprite>(ref sLeaser.sprites, sLeaser.sprites.Length + 1);
			if (sprite_index < 0 || sprite_index >= sLeaser.sprites.Length) return;
			sLeaser.sprites[sprite_index] = circleSprite;
		}

		//在PlayerGraphics_DrawSprites里调用
		public void DrawSprites(ref bool Execute, ref On.PlayerGraphics.orig_DrawSprites orig, ref PlayerGraphics playerGraphics, ref RoomCamera.SpriteLeaser sLeaser, ref RoomCamera rCam, ref float timeStacker, ref Vector2 camPos)
		{
			if (sprite_index < 0 || sprite_index >= sLeaser.sprites.Length) return;
			sLeaser.sprites[sprite_index].x = sLeaser.sprites[3].x;
			sLeaser.sprites[sprite_index].y = sLeaser.sprites[3].y;
			sLeaser.sprites[sprite_index].y -= 5f;
		}

		public int lastKillingAuraDamaged = int.MinValue;

		public static void Player_Update(ref bool Execute, ref On.Player.orig_Update orig, ref Player player, ref bool eu)
		{
			if (player.GetModuleE(out var module).KillingAuraSkill)
			{
				KillingAuraSkill? kaSkill = module.KASkill;
				if (kaSkill != null)
				{
					if (player.room.world.game.clock - kaSkill.lastKillingAuraDamaged >= 120)
					{
						kaSkill.lastKillingAuraDamaged = player.room.world.game.clock;

						// 执行你的操作
						//List<Creature> creatures = Extension.CreaturesInRange(player.room, player.mainBodyChunk.pos, Radius, false, player, true, false, player, false);
						var creatures = Extension.CreaturesInRange(player.room, player.mainBodyChunk.pos, Radius, false, player, true, false, player, false) ?? new List<Creature>();
						if (creatures != null && creatures.Count > 0)
						{
							foreach (Creature creature in creatures)
							{
								if (creature != null)
								{
									if (creature.State is HealthState hs)
									{
										hs.health -= 1f;
									}
								}
							}
						}
					}
				}

			}
		}

		//在PlayerGraphics_AddToContainer里调用
		public void AddToContainer(ref bool Execute, ref On.PlayerGraphics.orig_AddToContainer orig, ref PlayerGraphics playerGraphics, ref RoomCamera.SpriteLeaser sLeaser, ref RoomCamera rCam, ref FContainer newContatiner)
		{
			//防止重复添加
			if (!(sprite_index > 0 && sLeaser.sprites.Length > sprite_index))
				return;
			if (sprite_index < 0 || sprite_index >= sLeaser.sprites.Length) return;
			//添加到图层
			FContainer fcontainer = (newContatiner == null) ? rCam.ReturnFContainer("Midground") : newContatiner;
			fcontainer.AddChild(sLeaser.sprites[sprite_index]);
			//调整图层顺序
			sLeaser.sprites[sprite_index].MoveBehindOtherNode(sLeaser.sprites[3]);
		}
	}
}
