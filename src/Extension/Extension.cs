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


namespace MySlugcat
{

    public class Extension
    {


        /// <summary>
        /// 减去四分之一的食物
        /// </summary>
        static public void SubtractQuarterFood(int a, Player player)
        {
            for (int i = 0; i < a; i++)
            {
                if (player.playerState.quarterFoodPoints > 0)
                {
                    player.playerState.quarterFoodPoints--;
                    player.room.game.cameras[0].hud.PlaySound(SoundID.HUD_Food_Meter_Deplete_Plop_A);
                    player.room.game.cameras[0].hud.foodMeter.quarterPipShower.Reset();
                }
                else
                {
                    player.SubtractFood(1);
                    player.room.game.cameras[0].hud.PlaySound(SoundID.HUD_Food_Meter_Deplete_Plop_A);
                    player.room.game.cameras[0].hud.foodMeter.Update();
                    player.AddQuarterFood();
                    player.AddQuarterFood();
                    player.AddQuarterFood();
                    player.room.game.cameras[0].hud.foodMeter.quarterPipShower.Reset();
                }
            }
        }

        /// <summary>
        /// 无害的生物
        /// </summary>
        public static bool HarmlessCreature(Creature creature)
        {
            // 玩家 监视者 蝉乌贼 垃圾虫 波动龟 光鼠 蛙鱼 管虫 蝠蝇 蛋虫 雨鹿 幼年面条蝇 幼年蜈蚣 射线虫 墨鱼 水母 跃客
            // 监察者 天空鲸 藤壶 水熊虫 火精灵 箱虫 
            if (creature == null || creature is Player || creature is Overseer || creature is Cicada ||
                creature is GarbageWorm || creature is Snail || creature is LanternMouse ||//segments
                creature is JetFish || creature is TubeWorm || creature is Fly || creature is EggBug ||
                creature is Deer || creature is SmallNeedleWorm || (creature is Centipede centipede && centipede.Small) ||
                creature is VultureGrub || creature is Hazer || creature is JellyFish || creature is Yeek ||
                (creature is Inspector inspector && inspector.Consious == true) ||
                creature is SkyWhale || creature is Barnacle || creature is FireSprite || creature is Tardigrade ||
                (creature is BoxWorm boxWorm && boxWorm.Consious == true))
            {
                return true;// creature is Leech || 
            }
            return false;
        }


        /// <summary>
        /// 有害的生物
        /// </summary>
        public static bool HarmfulCreature(Creature creature)
        {
            if (creature == null || creature is SandGrub || creature is TentaclePlant || creature is Lizard ||
                creature is BigEel || creature is DaddyLongLegs || creature is Vulture ||
                creature is EggBug || creature is Centipede || creature is Spider || creature is MirosBird ||
                creature is Scavenger || creature is BigNeedleWorm || creature is DropBug || creature is BigMoth ||
                (creature is Inspector inspector && inspector.Consious == false) || creature is PoleMimic ||
                creature is BigJellyFish || creature is StowawayBug || creature is Loach || creature is Frog ||
                (creature is BoxWorm boxWorm && boxWorm.Consious == false) || creature is DrillCrab)
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
        public static List<Creature>? CreaturesInRange(Room room, Vector2 centerPos, float radius, bool IncludePlayer, Creature creature, bool IncludeSpecificCreature, bool IncludeDeadCreature)
        {
            List<(Creature creature, float sqrDistance)> results = new List<(Creature, float)>();
            float radiusSquared = radius * radius;

            if (!(room.abstractRoom.creatures.Count > 0))
            {
                return null;
            }

            foreach (AbstractCreature abstractCreature in room.abstractRoom.creatures)
            {
                Creature c = abstractCreature.realizedCreature;
                // 排除检查：玩家、无效引用、自身、或没有身体部位的对象
                if (!IncludePlayer)
                {
                    var player1 = c as Player;
                    if (player1 != null)
                    {
                        continue; // 跳过无效项，继续检查下一个
                    }
                }
                if (c == null ||             // 确保生物存在
                    c == creature ||             // 排除自身
                    c.mainBodyChunk == null) // 确保有有效的mainBodyChunk
                {
                    continue; // 跳过无效项，继续检查下一个
                }
                if (DisabledCreature(c) && !IncludeSpecificCreature)// 禁用生物
                {
                    continue; // 跳过无效项，继续检查下一个
                }
                if (c.dead == true && !IncludeDeadCreature)// 死亡的生物
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

            if (selfPos == null || room == null || room.abstractRoom == null || room.abstractRoom.creatures == null || room.abstractRoom.creatures.Count == null || !(room.abstractRoom.creatures.Count > 0))
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
                if (HarmlessCreature(c) && select == 2)// 无害生物
                {
                    continue; // 跳过无效项，继续检查下一个
                }
                if (c.dead == true && !IncludeDeadCreature)// 死亡的生物
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
		/// 查找当前房间中距离自身最近的玩家
		/// </summary>
		public static Player? FindNearestPlayer(Vector2 selfPos, Room room, Player? player, bool IncludeDeadPlayer)
		{
			// 初始化变量
			Player? nearest = null;        // 最近生物对象
			float minSqrDistance = float.MaxValue;  // 最小平方距离（初始设为最大浮点数）
													//List<Creature> creatures = new List<Creature>();

			if (selfPos == null || room == null || room.abstractRoom == null || room.abstractRoom.creatures == null || room.abstractRoom.creatures.Count == null || !(room.abstractRoom.creatures.Count > 0))
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
                if (!IncludePlayer)
                {
                    var player1 = c as Player;
                    if (player1 != null)
                    {
                        continue; // 跳过无效项，继续检查下一个
                    }
                }
                if (c == null ||             // 确保生物存在
                    c == creature ||             // 排除自身
                    c.mainBodyChunk == null) // 确保有有效的mainBodyChunk
                {
                    continue; // 跳过无效项，继续检查下一个
                }
                if (DisabledCreature(c))// 禁用生物
                {
                    continue; // 跳过无效项，继续检查下一个
                }
                if (c.dead == true && !IncludeDeadCreature)// 死亡的生物
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


    }
}
