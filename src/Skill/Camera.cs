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
using static Menu.Remix.InternalOI;


namespace MySlugcat
{
	/// <summary> 1 </summary>
	public class Camera
	{
		private static bool scopeMode;         // 是否处于望远镜
		private static Vector2 scopeAimDir;    // 当前瞄准方向（单位向量）
		private static Vector2 target;
		private static Vector2 preScopePos;    // 切出去之前摄像机的位置，用来恢复
		private const float scopeRange = 1200f; // 最大射程

		private static float step = 4f;

		/*public static void Player_Update(ref bool Execute, ref Player player, ref bool eu)
		{
			bool pressScope = player.input[0].thrw;
			if (pressScope)
			{

			}
		}*/

		public static void RoomCamera_Update(ref bool Execute, ref RoomCamera roomCamera)
		{
			//return;

			/*Player p = (roomCamera.followAbstractCreature != null && roomCamera.followAbstractCreature.realizedCreature is Player)
		   ? (Player)roomCamera.followAbstractCreature.realizedCreature : null!;
			if (p == null) return;   // 没玩家就不处理

			var module = p.GetModule();

			bool pressScope = p.input[0].thrw; // 用“投掷键”当望远镜键

			if (pressScope && !scopeMode)
			{
				// 第一次按下：记录当前摄像机位置，并切到对面
				preScopePos = roomCamera.pos;
				scopeMode = true;
				if (p.input[0].y != 0)
				{
					scopeAimDir = new Vector2(p.input[0].x, p.input[0].y);
				}
				else
				{
					scopeAimDir = new Vector2(p.ThrowDirection, 0);
				}
				if (scopeAimDir == Vector2.zero) scopeAimDir = new Vector2(-1, 0); // 防呆
				scopeAimDir = scopeAimDir.normalized;
				
				// 持续按住：镜头位置 = 玩家位置 + 方向 * 射程
				target = p.mainBodyChunk.pos + new Vector2(0, 20f) + scopeAimDir * scopeRange;
				int w = Screen.width;
				int h = Screen.height;
				target.x = target.x - (w / 2);
				target.y = target.y - (h / 2);
				if (p.input[0].y == 0)
				{
					target.y = p.mainBodyChunk.pos.y - (h / 2) + 20f;
				}
				target.x = Mathf.Clamp(target.x, 0f, roomCamera.room.PixelWidth - w);
				target.y = Mathf.Clamp(target.y, 0f, roomCamera.room.PixelHeight - h);
			}

			if (!pressScope && scopeMode)
			{
				// 松手：回退
				roomCamera.pos = preScopePos;
				scopeMode = false;
				return;
			}

			if (scopeMode)
			{
				if (p.input[0].x != 0)
				{
					target += new Vector2(p.input[0].x * step, 0);
				}
				if (p.input[0].y != 0)
				{
					target += new Vector2(0, p.input[0].y * step);
				}
				//target = Vector2.ClampMagnitude(target - p.mainBodyChunk.pos, scopeRange) + p.mainBodyChunk.pos + new Vector2(0, 20f);

				// 1) 先得出“世界”的有效矩形（单位：像素）
				World world = roomCamera.room.world;

				int w = Screen.width;
				int h = Screen.height;
				//target.y = target.y - (h / 2);

				// 把目标点夹在这个矩形里
				target.x = Mathf.Clamp(target.x, 0f, roomCamera.room.PixelWidth - w);
				target.y = Mathf.Clamp(target.y, 0f, roomCamera.room.PixelHeight - h);


				// 3) 再检查对应房间是否存在；不存在就退回到玩家附近
				WorldCoordinate wc = roomCamera.room.GetWorldCoordinate(target);
				
				*//*if (wc.room < 0 || wc.room >= world.NumberOfRooms)// 越界
				{
 				   target = p.mainBodyChunk.pos + new Vector2(0, 20f);// 直接回到玩家
				}*//*

				// 1. 通过索引拿到 AbstractRoom
				AbstractRoom targetAR = roomCamera.room.world.GetAbstractRoom(wc.room);

				// 2. 若未加载则激活
				if (targetAR != null && targetAR.realizedRoom == null)
					roomCamera.room.world.ActivateRoom(targetAR);

				// 3. 真正瞬移摄像机
				roomCamera.pos = target;
				//module.blockinput = true;
				Log.OutputLog($"target({target}_{p.mainBodyChunk.pos}_{roomCamera.room.PixelWidth}_{roomCamera.room.PixelHeight})_{(wc.room < 0 || wc.room >= world.NumberOfRooms)}");
				//roomCamera.seekPos = roomCamera.CamPos(roomCamera.currentCameraPosition);
				//roomCamera.seekPos.x = roomCamera.seekPos.x + (roomCamera.hDisplace + 8f);
				//roomCamera.seekPos.y = roomCamera.seekPos.y + 18f;
				//roomCamera.seekPos += roomCamera.leanPos * 8f;
				//roomCamera.pos = Vector2.Lerp(roomCamera.pos, roomCamera.seekPos, 0.1f);
				//roomCamera.pos.x = Mathf.Clamp(roomCamera.pos.x, roomCamera.CamPos(roomCamera.currentCameraPosition).x + roomCamera.hDisplace + 8f - 20f, roomCamera.CamPos(roomCamera.currentCameraPosition).x + roomCamera.hDisplace + 8f + 20f);
				//roomCamera.pos.y = Mathf.Clamp(roomCamera.pos.y, roomCamera.CamPos(roomCamera.currentCameraPosition).y + 8f - 7f - (roomCamera.splitScreenMode ? 192f : 0f), roomCamera.CamPos(roomCamera.currentCameraPosition).y + 33f + (roomCamera.splitScreenMode ? 192f : 0f));

				//[Info: Console] target((4873.1, 585.6)_(4873.1, 565.6)_5080_700)_
				//[Info: Unity Log] target((4873.1, 585.6)_(4873.1, 565.6)_5080_700)_
				//[Info: Console] target((4873.1, 585.6)_(4873.1, 565.6)_5080_700)_
				//[Info: Unity Log] target((4873.1, 585.6)_(4873.1, 565.6)_5080_700)_
				//[Info: Console] target((4873.1, 585.6)_(4873.1, 565.6)_5080_700)_
				//[Info :Console] target((142.4, 265.6)_(142.4, 245.6)_5080_700)_



				//询问我是否需要制作DMS
			}*/

		}

	}
}
