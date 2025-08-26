using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using UnityEngine;
using MoreSlugcats;
using RWCustom;
using System.Linq;
using Noise;
using System.Globalization;
using System.Threading;
using Watcher;


namespace MySlugcat;
internal static class PlayerModuleManager
{
	private static readonly ReaderWriterLockSlim _rwLock = new(LockRecursionPolicy.SupportsRecursion);
	private static readonly HashSet<Player> _activePlayers = new();
	public static readonly ConditionalWeakTable<Player, PlayerModule> PlayerModules = new();

	private static readonly List<Player> _snapshot = new(4);
	private static volatile bool _dirty = true;

	public static int ActivePlayerCount
	{
		get
		{
			_rwLock.EnterReadLock();
			try { return _activePlayers.Count; }
			finally { _rwLock.ExitReadLock(); }
		}
	}

	public static void RegisterPlayer(Player player)
	{
		if (player == null) return;

		_rwLock.EnterWriteLock();
		try
		{
			_activePlayers.Add(player);
			PlayerModules.Add(player, new PlayerModule(player));
			_dirty = true;
		}
		finally { _rwLock.ExitWriteLock(); }
	}

	public static void UnregisterPlayer(Player player)
	{
		if (player == null) return;

		_rwLock.EnterWriteLock();
		try
		{
			_activePlayers.Remove(player);
			PlayerModules.Remove(player);
			_dirty = true;
		}
		finally { _rwLock.ExitWriteLock(); }
	}

	//public static IEnumerable<Player> GetActivePlayers()
	public static IReadOnlyList<Player> GetActivePlayers()
	{
		if (_dirty)
		{
			_rwLock.EnterWriteLock(); // 使用写锁以确保线程安全
			try
			{
				_snapshot.Clear();
				_snapshot.AddRange(_activePlayers);
				_dirty = false;
			}
			finally { _rwLock.ExitWriteLock(); }
		}
		return _snapshot;
	}

	public static PlayerModule GetModule(this Player player)
	{
		_rwLock.EnterReadLock();
		try
		{
			if (PlayerModules.TryGetValue(player, out var module_))
			{
				return module_;
			}
		}
		finally { _rwLock.ExitReadLock(); }

		_rwLock.EnterWriteLock();
		try
		{
			// 玩家不存在于 PlayerModules 中时，创建并注册模块
			PlayerModule module = new PlayerModule(player);
			PlayerModules.Add(player, module);
			if (!_activePlayers.Contains(player))
			{
				_activePlayers.Add(player);
				_dirty = true;
			}
			return module;
		}
		finally { _rwLock.ExitWriteLock(); }
	}

	public static bool GetModule(this Player player, out PlayerModule module)
	{
		_rwLock.EnterReadLock();
		try
		{
			if (PlayerModules.TryGetValue(player, out var module_))
			{
				module = module_;
				return true;
			}
		}
		finally { _rwLock.ExitReadLock(); }

		_rwLock.EnterWriteLock();
		try
		{
			// 玩家不存在于 PlayerModules 中时，创建并注册模块
			PlayerModule module__ = new PlayerModule(player);
			PlayerModules.Add(player, module__);
			if (!_activePlayers.Contains(player))
			{
				_activePlayers.Add(player);
				_dirty = true;
			}
			module = module__;
			return true;
		}
		finally { _rwLock.ExitWriteLock(); }
	}

	public static PlayerModule GetModuleE(this Player player, out PlayerModule module)
	{
		_rwLock.EnterReadLock();
		try
		{
			if (PlayerModules.TryGetValue(player, out var module_))
			{
				module = module_;
				return module_;
			}
		}
		finally { _rwLock.ExitReadLock(); }

		_rwLock.EnterWriteLock();
		try
		{
			// 玩家不存在于 PlayerModules 中时，创建并注册模块
			PlayerModule module__ = new PlayerModule(player);
			PlayerModules.Add(player, module__);
			if (!_activePlayers.Contains(player))
			{
				_activePlayers.Add(player);
				_dirty = true;
			}
			module = module__;
			return module__;
		}
		finally { _rwLock.ExitWriteLock(); }
	}


	internal class PlayerModule
	{
		//WeakReference<Player> playerRef;
		public readonly Player player;

		public List<string> Passages = new List<string>(); // 已拥有的通行证

		#region 能力字段
		/// <summary> 蛞蝓猫数据 </summary>
		public int  MySlugcatStats = 0;
		/// <summary> 精疲力竭 </summary>
		public bool Exhausted = false;
		/// <summary> 饥饿 </summary>
		public bool Hunger = false;
		/// <summary> 视觉系统 </summary>
		public bool VisionSystem = false;

		/// <summary> 嫁祸能力 </summary>
		public bool FrameSkill = false;
		/// <summary> 爆燃能力 </summary>
		public bool DeflagrationSkill = false;
		/// <summary> 缠绕能力 </summary>
		public bool KnitmeshSkill = false;
		/// <summary> 感知能力 </summary>
		public bool PerceptionSkill = false;
		/// <summary> 吞食能力 </summary>
		public bool DigestionSkill = false;
		/// <summary> 死灵能力 死灵法师 </summary>
		public bool SpawnNecrophytes = false;
		/// <summary> 定身能力 </summary>
		public bool FixedSkill = false;
		/// <summary> 杀戮光环 </summary>
		public bool KillingAuraSkill = false;
		/// <summary> 穿透能力 </summary>
		public bool PenetrationSkill = false;
		/// <summary> 电弧连锁 </summary>
		public bool ArcLightningSkill = false;
		/// <summary> 迅捷能力 </summary>
		public int Swift = 0;

		//public int HungryCoolDown = 12000;//冷却计时器
		#endregion

		#region 通行证字段
		// Vanilla
		/// <summary> "求生者" </summary>
		public const string TheSurvivorPassage = "The Survivor";
		/// <summary> "猎手" </summary>
		public const string TheHunterPassage = "The Hunter";
		/// <summary> "圣徒" </summary>
		public const string TheSaintPassage = "The Saint";
		/// <summary> "漫游者" </summary>
		public const string TheWandererPassage = "The Wanderer";
		/// <summary> "酋长" </summary>
		public const string TheChieftainPassage = "The Chieftain";
		/// <summary> "僧侣" </summary>
		public const string TheMonkPassage = "The Monk";
		/// <summary> "暴徒" </summary>
		public const string TheOutlawPassage = "The Outlaw";
		/// <summary> "屠龙者" </summary>
		public const string TheDragonSlayerPassage = "The Dragon Slayer";
		/// <summary> "学者" </summary>
		public const string TheScholarPassage = "The Scholar";
		/// <summary> "朋友" </summary>
		public const string TheFriendPassage = "The Friend";

		// ModManager.MSC
		/// <summary> "流浪者" </summary>
		public const string TheNomadPassage = "The Nomad";
		/// <summary> "殉道者" </summary>
		public const string TheMartyrPassage = "The Martyr";
		/// <summary> "朝圣者" </summary>
		public const string ThePilgrimPassage = "The Pilgrim";
		/// <summary> "慈母" </summary>
		public const string TheMotherPassage = "The Mother";

		// The Vanguard
		/// <summary> "龙王" </summary>
		public const string TheDragonlordPassage = "The Dragonlord";
		// Rotund World
		/// <summary> "贪食者" </summary>
		public const string TheGluttonPassage = "The Glutton";
		#endregion

		#region 按键Key
		// player.input[0].jmp 玩家当前帧是否按下跳跃键
		// player.input[1].jmp 玩家上一帧是否按下跳跃键

		public bool blockinput = false;

		//按下按键的时长(40次 = 1秒)?
		//jump 跳跃键
		public int JmpCounter = 0;
		//pckp 拾取键
		public int pckpCounter = 0;
		//throw 投掷键
		public int thrwCounter = 0;
		//map 地图键
		public int mpCounter = 0;
		//special 特殊键
		public int specCounter = 0;
		//y Y键  输入为正（上）、零（无输入）、负（下）
		public int yHCounter = 0;
		public int yNCounter = 0;
		public int yLCounter = 0;
		//x X键  输入为正（右）、零（无输入）、负（左）
		public int xHCounter = 0;
		public int xNCounter = 0;
		public int xLCounter = 0;
		#endregion

		#region 其他字段
		public int lastFrameSkillTick = -100;

		public KillingAuraSkill? KASkill = null;
		#endregion




		public PlayerModule(Player player)
		{
			//playerRef = new WeakReference<Player>(player);
			this.player = player;

			#region 按键Key
			// player.input[0].jmp 玩家当前帧是否按下跳跃键
			// player.input[1].jmp 玩家上一帧是否按下跳跃键

			//按下按键的时长(40次 = 1秒)
			//jump 跳跃键
			JmpCounter = 0;
			//pckp 拾取键
			pckpCounter = 0;
			//throw 投掷键
			thrwCounter = 0;
			//map 地图键
			mpCounter = 0;
			//special 特殊键
			specCounter = 0;
			//y Y键  输入为正（上）、零（无输入）、负（下）
			yHCounter = 0;
			yNCounter = 0;
			yLCounter = 0;
			//x X键  输入为正（右）、零（无输入）、负（左）
			xHCounter = 0;
			xNCounter = 0;
			xLCounter = 0;
			#endregion

			Console.WriteLine("Passages_1: " + string.Join(", ", Passages));
			SetSkill(player);
			//Log.Logger();
			Console.WriteLine("Passages_2: " + string.Join(", ", Passages));
		}

		public void SetSkill(Player player)
		{
			MySlugcatStats = 0;
			Exhausted = false;
			Hunger = false;
			VisionSystem = false;

			FrameSkill = false;
			DeflagrationSkill = false;
			KnitmeshSkill = false;
			PerceptionSkill = false;
			DigestionSkill = false;
			SpawnNecrophytes = false;
			FixedSkill = false;
			KillingAuraSkill = false;
			PenetrationSkill = false;
			ArcLightningSkill = false;
			Swift = 0;

			if (player.slugcatStats.name == Plugin.YourSlugID || Control.AllPlayerSkill)
			{
				MySlugcatStats = -1;
				Exhausted = true;
				Hunger = true;
				VisionSystem = true;

				FrameSkill = true;//
				DeflagrationSkill = true;//
				KnitmeshSkill = true;//
				PerceptionSkill = true;//
				DigestionSkill = true;//
				SpawnNecrophytes = true;//
				FixedSkill = false;
				KillingAuraSkill = false;//f
				PenetrationSkill = false;//
				ArcLightningSkill = true;//
				Swift = 0;

				var session = player?.room?.game?.GetStorySession;
				if (session == null) return;
				var dpsd = session.saveState?.deathPersistentSaveData;
				if (dpsd == null) return;
				var winState = dpsd?.winState;
				if (winState == null) return;

				Passages.Clear();
				bool Survivor = false;

				if (winState != null && winState.endgameTrackers.Count > 0)
				{
					for (int i = 0; i < winState.endgameTrackers.Count; i++)
					{
						if (winState.endgameTrackers[i].GoalFullfilled)
						{
							string name = WinState.PassageDisplayName(winState.endgameTrackers[i].ID);
							Passages.Add(name);
							if (name == TheSurvivorPassage)//"求生者"
							{
								PerceptionSkill = true;
								Survivor = true;
							}
						}
					}
				}

				if (Survivor)
				{
					if (session.saveState != null && session.saveState.deathPersistentSaveData.deaths > 50)
					{
						FrameSkill = true;
					}

					var passSet = new HashSet<string>(Passages);
					if (passSet != null && passSet.Count > 0)
					{
						if (passSet.Contains(TheOutlawPassage))//"暴徒"
						{
							DeflagrationSkill = true;
						}
						if (passSet.Contains(TheHunterPassage))//"猎手"
						{
							KillingAuraSkill = true;
						}
						if (passSet.Contains(TheChieftainPassage) && passSet.Contains(TheFriendPassage))//"酋长"&"朋友"
						{
							SpawnNecrophytes = true;
						}
						if (passSet.Contains(TheGluttonPassage))//"贪食者"
						{
							DigestionSkill = true;
						}
						if (passSet.Contains(TheOutlawPassage) && passSet.Contains(TheDragonSlayerPassage))//"暴徒"&"屠龙者"
						{
							PenetrationSkill = true;
						}
					}
				}


			}
		}


		public static bool HasPassage(Player player, string passageName)
		{
			if (player?.room?.game?.GetStorySession?.saveState?.deathPersistentSaveData?.winState is not WinState winState)
				return false;

			return winState.endgameTrackers.Any(t => t.GoalFullfilled &&
													   WinState.PassageDisplayName(t.ID) == passageName);
		}

	}
}

internal static class PlayerHooks
{

	/*public static void HookOn()
	{
		On.Player.ctor += Player_ctor;
		On.Player.Destroy += Player_Destroy;
	}*/

	public static void Player_ctor(ref bool Execute, ref On.Player.orig_ctor orig, ref Player player, ref AbstractCreature ac, ref World world)
	{
		PlayerModuleManager.RegisterPlayer(player);        // ① 注册
	}

	public static void Player_Destroy(ref bool Execute, ref On.Player.orig_Destroy orig, ref Player player)
	{
		if (player.dead || player.slatedForDeletetion)
		{
			PlayerModuleManager.UnregisterPlayer(player);  // ② 注销
		}
	}


}
