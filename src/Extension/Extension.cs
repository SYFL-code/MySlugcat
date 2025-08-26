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
using System.Runtime.CompilerServices;


namespace MySlugcat
{
	/// <summary>
	/// 扩展插件
	/// </summary>
	public static class Extension
	{

		public static int Random(int min, int max)
		{
			return UnityEngine.Random.Range(min, max);
		}

		public static float Random(float min, float max)
		{
			return UnityEngine.Random.Range(min, max);
		}

		public static Vector2 Random(float xMin, float xMax, float yMin, float yMax)
		{
			return new Vector2(UnityEngine.Random.Range(xMin, xMax), UnityEngine.Random.Range(yMin, yMax));
		}

		/// <summary>
		/// 玩家胃里的食物格数
		/// </summary>
		public static float? PlayerStomachFood(Player player)
		{
			if (player == null || player.playerState == null) return null;
			int FoodInt = 0;
			FoodInt = player.FoodInStomach;
			float FoodFloat = player.playerState.quarterFoodPoints * 0.25f;
			return FoodInt + FoodFloat;
			/*if (player.FoodInStomach == player.playerState.foodInStomach)
			{
				FoodInt = player.FoodInStomach;
			}*/
		}

		/// <summary>
		/// 从玩家胃里扣除指定数量的 ¼ 格食物
		/// </summary>
		/// <param name="player">目标玩家</param>
		/// <param name="quartersToRemove">要扣掉的 ¼ 格总数</param>
		public static bool SubtractQuarterFood(Player player, int quartersToRemove)
		{
			if (quartersToRemove <= 0) return false;          // 健壮性
			if (player == null || player.room == null) return false;

			int Deduct = 0;

			var hud = player.room.game.cameras[0]?.hud;
			var meter = hud?.foodMeter;
			var sound = SoundID.HUD_Food_Meter_Deplete_Plop_A;

			for (int i = 0; i < quartersToRemove; i++)
			{
				if (player.playerState.quarterFoodPoints > 0)
				{
					// 扣 ¼ 格
					player.playerState.quarterFoodPoints--;
				}
				else if (player.FoodInStomach > 0)
				{
					// 官方整格扣除
					player.SubtractFood(1);
					player.playerState.quarterFoodPoints = 3;   // 直接补 3 个 ¼ 格
				}
				else
				{
					break;  // 已经空了
				}

				// 每扣一次都刷新 UI
				Deduct += 1;
				hud?.PlaySound(sound);
				meter?.Update();
				meter?.quarterPipShower?.Reset();
			}
			return Deduct == quartersToRemove;
		}

		/*/// <summary>
		/// 把任意 Color 按指定强度转成黑白灰
		/// </summary>
		/// <param name="c">原始 Color</param>
		/// <param name="strength">0~1，0 保持原色，1 完全灰度</param>
		public static Color ToGrayscale(this Color c, float strength = 1f)
		{
			float g = 0.299f * c.r + 0.587f * c.g + 0.114f * c.b;
			strength = Mathf.Clamp01(strength);          // 保险
			float r = Mathf.Lerp(c.r, g, strength);
			float g2 = Mathf.Lerp(c.g, g, strength);
			float b2 = Mathf.Lerp(c.b, g, strength);
			return new Color(r, g2, b2, c.a);            // 保留 alpha
		}*/

		private static readonly Vector3 LumaCoeff = new(0.299f, 0.587f, 0.114f);

		/// <summary>
		/// 把任意 HSLColor 按指定强度转成黑白灰
		/// </summary>
		/// <param name="hsl">原始 HSLColor</param>
		/// <param name="strength">0~1，0 保持原色，1 完全灰度</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static HSLColor ToGrayscale(this HSLColor hsl, float strength = 1f)
		{
			strength = Mathf.Clamp(strength, 0f, 1f);

			// 用 Vector3 一次性做乘加，JIT 会自动展开为 SIMD
			Vector3 rgb = new(hsl.rgb.r, hsl.rgb.g, hsl.rgb.b);
			float grayLightness = Vector3.Dot(rgb, LumaCoeff);

			return new HSLColor(
				hue: hsl.hue,
				saturation: Mathf.Lerp(hsl.saturation, 0f, strength),
				lightness: Mathf.Lerp(hsl.lightness, grayLightness, strength)
			);
		}

		/*public static HSLColor ToGrayscale(this HSLColor hsl, float strength = 1f)
		{
			strength = Mathf.Clamp01(strength);

			//float grayLightness =
			//	0.299f * hsl.RgbR + 0.587f * hsl.RgbG + 0.114f * hsl.RgbB; // 如果你 HSLColor 有 RgbR/G/B
			// 如果没有，就先转 Color 再算：
			float grayLightness = 0.299f * hsl.rgb.r + 0.587f * hsl.rgb.g + 0.114f * hsl.rgb.b;

			return new HSLColor(
				hue: hsl.hue,                           // 色相不变
				saturation: Mathf.Lerp(hsl.saturation, 0f, strength),
				lightness: Mathf.Lerp(hsl.lightness, grayLightness, strength)
			);
		}*/

		// BT.601 亮度系数
		private const float RCoef = 0.299f;
		private const float GCoef = 0.587f;
		private const float BCoef = 0.114f;

		/// <summary>
		/// 把任意 Color 按指定强度转成黑白灰
		/// </summary>
		/// <param name="col">原始 Color</param>
		/// <param name="strength">0~1，0 保持原色，1 完全灰度</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Color ToGrayscale(this Color col, float strength = 1f)
		{
			strength = Mathf.Clamp01(strength);

			// 计算灰度亮度（0-1）
			float luma = col.r * RCoef + col.g * GCoef + col.b * BCoef;

			// 逐分量插值
			float r = Mathf.Lerp(col.r, luma, strength);
			float g = Mathf.Lerp(col.g, luma, strength);
			float b = Mathf.Lerp(col.b, luma, strength);

			// 返回新 Color（alpha 不变）
			return new Color(r, g, b, col.a);
		}


		/// <summary>
		/// 是否为驯服的生物
		/// </summary>
		public static bool IsTameCreature(Player player, Creature creature)
		{
			return friends_of_friends.Friends(creature.abstractCreature, player.abstractCreature);
		}

		/// <summary>
		/// 是否为无害的生物
		/// </summary>
		public static bool IsHarmlessCreature(Creature creature)
		{
			// 玩家 监视者 蝉乌贼 垃圾虫 波动龟 光鼠 蛙鱼 管虫 蝠蝇 蛋虫 雨鹿 幼年面条蝇 幼年蜈蚣 射线虫 墨鱼 水母 跃客
			// 监察者 天空鲸 藤壶 水熊虫 火精灵 箱虫 
			if (creature == null || creature is Player || creature is Overseer || creature is Cicada ||
				creature is GarbageWorm || creature is Snail || creature is LanternMouse ||//segments
				creature is JetFish || creature is TubeWorm || creature is Fly || creature is EggBug ||
				creature is Deer || creature is SmallNeedleWorm || (creature is Centipede centipede && centipede.Small) ||
				creature is VultureGrub || creature is Hazer || creature is JellyFish || creature is Yeek ||
				(creature is Inspector inspector && inspector.Consious == false) ||
				creature is SkyWhale || creature is Barnacle || creature is FireSprite || creature is Tardigrade ||
				(creature is BoxWorm boxWorm && boxWorm.Consious == false))
			{
				return true;// creature is Leech || 
			}
			return false;
		}

		/// <summary>
		/// 是否为有害的生物(不完全)
		/// </summary>
		public static bool IsHarmfulCreature(Creature creature)
		{
			if (creature == null || creature is SandGrub || creature is TentaclePlant || creature is Lizard ||
				creature is BigEel || creature is DaddyLongLegs || creature is Vulture || creature is MirosBird ||
				(creature is Centipede centipede && !centipede.Small) || creature is Spider || 
				creature is Scavenger || creature is BigNeedleWorm || creature is DropBug || creature is BigMoth ||
				(creature is Inspector inspector && inspector.Consious == true) || creature is PoleMimic ||
				creature is BigJellyFish || creature is StowawayBug || creature is Loach || creature is Frog ||
				(creature is BoxWorm boxWorm && boxWorm.Consious == true) || creature is DrillCrab)
			{
				return true;// creature is Leech || 
			}
			return false;
		}

		/// <summary>
		/// 禁用的生物
		/// </summary>
		public static bool DisabledCreature(Creature creature)
		{
			if (creature == null || creature is Fly || creature is SandGrub || creature is TentaclePlant ||
				creature is Leech || creature is BigEel || creature is DaddyLongLegs || creature is Overseer ||
				creature is GarbageWorm || creature is Deer || creature is Inspector || creature is PoleMimic ||
				creature is BigJellyFish || creature is StowawayBug || creature is Loach || creature is Frog ||
				creature is SkyWhale || creature is BoxWorm || creature is FireSprite || creature is DrillCrab)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// 查找获取一定范围内所有生物
		/// </summary>
		public static List<Creature> CreaturesInRange(Vector2 centerPos, Room room, float radius, bool IncludePlayer, Creature creature, bool IncludeSpecificCreature, bool IncludeDeadCreature, Player? player = null, bool IncludeTameCreature = false)
		{
			List<(Creature creature, float sqrDistance)> results = new List<(Creature, float)>();
			float radiusSquared = radius * radius;

			if (room == null || !(room.abstractRoom.creatures.Count > 0))
			{
				return new List<Creature>();
			}

			foreach (AbstractCreature abstractCreature in room.abstractRoom.creatures)
			{
				Creature c = abstractCreature.realizedCreature;
				// 排除检查：玩家、无效引用、自身、或没有身体部位的对象
				if (c == null ||             // 确保生物存在
					c == creature ||             // 排除自身
					c.mainBodyChunk == null) // 确保有有效的mainBodyChunk
				{
					continue; // 跳过无效项，继续检查下一个
				}
				if (!IncludePlayer)
				{
					var player1 = c as Player;
					if (player1 != null)
					{
						continue; // 跳过无效项，继续检查下一个
					}
				}
				if (player != null && !IncludeTameCreature && IsTameCreature(player, creature))
				{
					continue; // 跳过无效项，继续检查下一个
				}
				if (DisabledCreature(c) && !IncludeSpecificCreature)// 禁用生物
				{
					continue; // 跳过无效项，继续检查下一个
				}
				if (((c.abstractCreature.GetModule(out var module_c) && module_c.NecrophyteDying) || c.dead == true) && !IncludeDeadCreature)// 死亡的生物
				{
					continue; // 跳过无效项，继续检查下一个
				}
				// 计算平方距离（性能优化）
				Vector2 offset = c.mainBodyChunk.pos - centerPos;
				float sqrDist = offset.sqrMagnitude;

				// 距离检测
				if (sqrDist <= radiusSquared)
				{
					results.Add((c, sqrDist));
				}
			}
			// 按平方距离排序（无需计算真实距离）
			results.Sort((a, b) => a.sqrDistance.CompareTo(b.sqrDistance));

			// 转换为最终结果
			return results.ConvertAll(x => x.creature);
		}

		/// <summary>
		/// 查找当前房间中距离自身最近的生物
		/// </summary>
		public static Creature? FindNearestCreature(Vector2 selfPos, Room room, bool IncludePlayer, Creature? creature, bool IncludeDeadCreature, int select)
		{
			// 初始化变量
			Creature? nearest = null;        // 最近生物对象
			float minSqrDistance = float.MaxValue;  // 最小平方距离（初始设为最大浮点数）
			//List<Creature> creatures = new List<Creature>();

			if (room == null || room.abstractRoom == null || room.abstractRoom.creatures == null || !(room.abstractRoom.creatures.Count > 0))
			{
				return null;
			}

			// 遍历当前房间所有生物
			foreach (AbstractCreature abstractCreature in room.abstractRoom.creatures)
			{

				Creature c = abstractCreature.realizedCreature;
				// 排除检查：玩家、无效引用、自身、或没有身体部位的对象
				if (c == null ||             // 确保生物存在
					(creature != null && c == creature) ||             // 排除自身
					c.mainBodyChunk == null ||      // 确保有有效的mainBodyChunk
					c.mainBodyChunk.pos == null)
				{
					continue; // 跳过无效项，继续检查下一个
				}
				if (!IncludePlayer)
				{
					var player1 = c as Player;
					if (player1 != null)
					{
						continue; // 跳过无效项，继续检查下一个
					}
				}
				if (DisabledCreature(c) && select == 1)// 禁用生物
				{
					continue; // 跳过无效项，继续检查下一个
				}
				if (IsHarmlessCreature(c) && select == 2)// 无害生物
				{
					continue; // 跳过无效项，继续检查下一个
				}
				if (((c.abstractCreature.GetModule(out var module_c) && module_c.NecrophyteDying) || c.dead == true) && !IncludeDeadCreature)// 死亡的生物
				{
					continue; // 跳过无效项，继续检查下一个
				}
				// 计算位置差（目标位置 - 自身位置）
				Vector2 offset = c.mainBodyChunk.pos - selfPos;
				// 计算平方距离（比Vector2.Distance更高效）
				float sqrDistance = offset.sqrMagnitude;

				//creatures.Add(c);

				// 检查是否为更近的生物
				if (sqrDistance < minSqrDistance)
				{
					// 更新最近生物和最小距离记录
					minSqrDistance = sqrDistance;
					nearest = c;
				}
			}
			//return creatures[UnityEngine.Random.Range(0, creatures.Count)];
			return nearest; // 返回最近生物（可能为null）
		}

		/// <summary>
		/// 扇形范围内找最近生物（排除自身、可选玩家/死亡）
		/// </summary>
		public static Creature? FindNearestCreatureDirection(
			Vector2 selfPos,
			Room room,
			bool includePlayer,
			List<Creature> exclude,
			bool includeDead,
			Vector2 forward,          // 正前方向量（不必单位化）
			float maxAngleDeg,        // 扇形 半 角（度）
			float maxDist,
			int filter = 0)           // 0=全部 1=非禁用 2=非无害
		{
			// 检查房间和生物列表是否为空
			if (room == null || room.abstractRoom == null || room.abstractRoom.creatures == null)
			{
				Log.OutputLog("Room or abstractRoom or creatures is null.");
				return null;
			}

			// 检查方向向量是否有效
			if (forward.sqrMagnitude < 1E-4f)
			{
				Log.OutputLog("Forward vector is too small.");
				return null;
			}

			Creature? nearest = null;
			float minDistSq = float.MaxValue;
			Vector2 dirNorm = forward.normalized;
			float cosLimit = Mathf.Cos(maxAngleDeg * Mathf.Deg2Rad);

			Log.OutputLog($"Starting search for nearest creature from position {selfPos} in room {room}.");

			foreach (var abs in room.abstractRoom.creatures)
			{
				var c = abs.realizedCreature;
				if (c == null || exclude.Contains(c) || c.mainBodyChunk == null)
				{
					Log.OutputLog($"Skipping creature {c?.GetType()} due to null check or exclusion.");
					continue;
				}

				Vector2 toTarget = c.mainBodyChunk.pos - selfPos;
				float distSq = toTarget.sqrMagnitude;
				if (distSq > maxDist * maxDist)
				{
					Log.OutputLog($"Skipping creature {c.GetType()} due to distance check.");
					continue;
				}

				if (!includePlayer && c is Player)
				{
					Log.OutputLog($"Skipping creature {c.GetType()} because it is a player and includePlayer is false.");
					continue;
				}

				if (!includeDead && c.dead)
				{
					Log.OutputLog($"Skipping creature {c.GetType()} because it is dead and includeDead is false.");
					continue;
				}

				if (filter == 1 && DisabledCreature(c))
				{
					Log.OutputLog($"Skipping creature {c.GetType()} because it is disabled and filter is 1.");
					continue;
				}

				if (filter == 2 && IsHarmlessCreature(c))
				{
					Log.OutputLog($"Skipping creature {c.GetType()} because it is harmless and filter is 2.");
					continue;
				}

				// 扇形检测：向量夹角余弦 ≥ cosLimit
				if (toTarget.sqrMagnitude < 1E-4f || Vector2.Dot(dirNorm, toTarget.normalized) < cosLimit)
				{
					Log.OutputLog($"Skipping creature {c.GetType()} due to angle check.");
					continue;
				}

				Trace(selfPos, c.mainBodyChunk.pos, room, out bool isTerrain);
				if (isTerrain)
				{
					Log.OutputLog($"Skipping creature {c.GetType()} because there is terrain blocking the way.");
					continue;
				}

				if (distSq < minDistSq)
				{
					minDistSq = distSq;
					nearest = c;
					Log.OutputLog($"Found a closer creature: {c.GetType()} at distance {Mathf.Sqrt(distSq)}.");
				}
			}

			if (nearest == null)
			{
				Log.OutputLog("No creature found within the specified parameters.");
			}
			else
			{
				Log.OutputLog($"Nearest creature found: {nearest.GetType()} at distance {Mathf.Sqrt(minDistSq)}.");
			}

			return nearest;
		}

		/// <summary>
		/// 查找当前房间中距离自身最近的玩家
		/// </summary>
		public static Player? FindNearestPlayer(Vector2 selfPos, Room room, Player? player, bool IncludeDeadPlayer)
		{
			// 初始化变量
			Player? nearest = null;        // 最近生物对象
			float minSqrDistance = float.MaxValue;  // 最小平方距离（初始设为最大浮点数）
													//List<Creature> creatures = new List<Creature>();

			if (room == null || room.abstractRoom == null || room.abstractRoom.creatures == null || !(room.abstractRoom.creatures.Count > 0))
			{
				return null;
			}

			// 遍历当前房间所有生物
			foreach (AbstractCreature abstractCreature in room.abstractRoom.creatures)
			{

				Creature c = abstractCreature.realizedCreature;
				var p = c as Player;
				if (p == null)
				{
					continue; // 跳过无效项，继续检查下一个
				}
				// 排除检查：玩家、无效引用、自身、或没有身体部位的对象
				if (p == null ||             // 确保生物存在
					(player != null && p == player) ||             // 排除自身
					p.mainBodyChunk == null ||      // 确保有有效的mainBodyChunk
					p.mainBodyChunk.pos == null)
				{
					continue; // 跳过无效项，继续检查下一个
				}
				if (p.dead == true && !IncludeDeadPlayer)// 死亡的生物
				{
					continue; // 跳过无效项，继续检查下一个
				}
				// 计算位置差（目标位置 - 自身位置）
				Vector2 offset = p.mainBodyChunk.pos - selfPos;
				// 计算平方距离（比Vector2.Distance更高效）
				float sqrDistance = offset.sqrMagnitude;

				//creatures.Add(c);

				// 检查是否为更近的生物
				if (sqrDistance < minSqrDistance)
				{
					// 更新最近生物和最小距离记录
					minSqrDistance = sqrDistance;
					nearest = p;
				}
			}
			//return creatures[UnityEngine.Random.Range(0, creatures.Count)];
			return nearest; // 返回最近生物（可能为null）
		}

		/// <summary>
		/// 随机查找当前房间的生物
		/// </summary>
		public static Creature? RandomlySelectedCreature(Room room, bool IncludePlayer, Creature creature, bool IncludeDeadCreature)
		{
			// 初始化变量
			//Creature? nearest = null;        // 最近生物对象
			//float minSqrDistance = float.MaxValue;  // 最小平方距离（初始设为最大浮点数）
			List<Creature> creatures = new List<Creature>();

			if (!(room.abstractRoom.creatures.Count > 0))
			{
				return null;
			}

			// 遍历当前房间所有生物
			foreach (AbstractCreature abstractCreature in room.abstractRoom.creatures)
			{

				Creature c = abstractCreature.realizedCreature;
				// 排除检查：玩家、无效引用、自身、或没有身体部位的对象
				if (c == null ||             // 确保生物存在
					c == creature ||             // 排除自身
					c.mainBodyChunk == null) // 确保有有效的mainBodyChunk
				{
					continue; // 跳过无效项，继续检查下一个
				}
				if (!IncludePlayer)
				{
					var player1 = c as Player;
					if (player1 != null)
					{
						continue; // 跳过无效项，继续检查下一个
					}
				}
				if (DisabledCreature(c))// 禁用生物
				{
					continue; // 跳过无效项，继续检查下一个
				}
				if (((c.abstractCreature.GetModule(out var module_c) && module_c.NecrophyteDying) || c.dead == true) && !IncludeDeadCreature)// 死亡的生物
				{
					continue; // 跳过无效项，继续检查下一个
				}
				// 计算位置差（目标位置 - 自身位置）
				//Vector2 offset = c.mainBodyChunk.pos - selfPos;
				// 计算平方距离（比Vector2.Distance更高效）
				//float sqrDistance = offset.sqrMagnitude;

				creatures.Add(c);

				// 检查是否为更近的生物
				/*                if (sqrDistance < minSqrDistance)
								{
									// 更新最近生物和最小距离记录
									minSqrDistance = sqrDistance;
									nearest = c;
								}*/
			}
			if (creatures.Count == 0)
			{
				Console.WriteLine("MySlugcat:RandomlySelectedCreature: No valid creatures found");
				return null;
			}
			return creatures[UnityEngine.Random.Range(0, creatures.Count)];
		}


		// 方法名：路径修正（检测闪电路径是否碰撞地形）
		public static Vector2 Trace(Vector2 start, Vector2 end, Room room, out bool isTerrain)
		{
			// 计算从起点到终点的方向向量（归一化）
			Vector2 Direction = Custom.DegToVec(Custom.AimFromOneVectorToAnother(start, end));

			// 核心逻辑：射线检测起点到终点之间是否碰撞地形
			// 返回值intVector为碰撞的格子坐标（若无碰撞则返回null）
			IntVector2? intVector = SharedPhysics.RayTraceTilesForTerrainReturnFirstSolid(room, start, end);

			if (intVector != null)
			{
				// 标记闪电碰撞到地形（用于后续特效）
				isTerrain = true;

				// 计算修正后的终点位置（避免闪电穿透地形视觉效果）
				// 方案：取碰撞格子的中心坐标，并向反方向微调7单位
				return room.MiddleOfTile(intVector.Value) - Direction * 7f;
			}

			// 无碰撞时保持原始终点
			isTerrain = false;
			return start;
		}


	}

	/*internal static class MathHelper
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Clamp(float v, float min, float max) =>
			v < min ? min : v > max ? max : v;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Lerp(float a, float b, float t) =>
			a + (b - a) * t;
	}*/


}
