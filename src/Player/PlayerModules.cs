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
			_rwLock.EnterReadLock();   // 写锁即可，读锁已够用
			try
			{
				_snapshot.Clear();
				_snapshot.AddRange(_activePlayers);
				_dirty = false;
			}
			finally { _rwLock.ExitReadLock(); }
		}
		return _snapshot;

		/*_rwLock.EnterReadLock();
		try { return new List<Player>(_activePlayers); }
		finally { _rwLock.ExitReadLock(); }*/
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
		/// <summary> 暴食能力 </summary>
		public bool DigestionSkill = false;
		/// <summary> 死灵能力 死灵法师 </summary>
		public bool SpawnNecrophytes = false;
		/// <summary> 定身能力 </summary>
		public bool FixedSkill = false;
		/// <summary> 杀戮光环 </summary>
		public bool KillingAuraSkill = false;

		//public int HungryCoolDown = 12000;//冷却计时器
		#endregion

		#region 通行证字段
		// Vanilla
		/// <summary> "求生者" </summary>
		private const string TheSurvivorPassage = "The Survivor";
		/// <summary> "猎手" </summary>
		private const string TheHunterPassage = "The Hunter";
		/// <summary> "圣徒" </summary>
		private const string TheSaintPassage = "The Saint";
		/// <summary> "漫游者" </summary>
		private const string TheWandererPassage = "The Wanderer";
		/// <summary> "酋长" </summary>
		private const string TheChieftainPassage = "The Chieftain";
		/// <summary> "僧侣" </summary>
		private const string TheMonkPassage = "The Monk";
		/// <summary> "暴徒" </summary>
		private const string TheOutlawPassage = "The Outlaw";
		/// <summary> "屠龙者" </summary>
		private const string TheDragonSlayerPassage = "The Dragon Slayer";
		/// <summary> "学者" </summary>
		private const string TheScholarPassage = "The Scholar";
		/// <summary> "朋友" </summary>
		private const string TheFriendPassage = "The Friend";

		// ModManager.MSC
		/// <summary> "流浪者" </summary>
		private const string TheNomadPassage = "The Nomad";
		/// <summary> "殉道者" </summary>
		private const string TheMartyrPassage = "The Martyr";
		/// <summary> "朝圣者" </summary>
		private const string ThePilgrimPassage = "The Pilgrim";
		/// <summary> "慈母" </summary>
		private const string TheMotherPassage = "The Mother";

		// The Vanguard
		/// <summary> "龙王" </summary>
		private const string TheDragonlordPassage = "The Dragonlord";
		// Rotund World
		/// <summary> "贪食者" </summary>
		private const string TheGluttonPassage = "The Glutton";
		#endregion

		#region 其他字段
		#endregion


		public PlayerModule(Player player)
		{
			//playerRef = new WeakReference<Player>(player);
			this.player = player;
			Console.WriteLine("Passages_1: " + string.Join(", ", Passages));
			SetSkill(player);
			//Log.Logger();
			Console.WriteLine("Passages_2: " + string.Join(", ", Passages));
		}

		public void SetSkill(Player player)
		{
			MySlugcatStats = 0;
			Exhausted = false;
			VisionSystem = false;

			FrameSkill = false;
			DeflagrationSkill = false;
			KnitmeshSkill = false;
			PerceptionSkill = false;
			DigestionSkill = false;
			SpawnNecrophytes = false;
			FixedSkill = false;
			KillingAuraSkill = false;

			if (player.slugcatStats.name == Plugin.YourSlugID || Control.AllPlayerSkill)
			{
				MySlugcatStats = -1;
				Exhausted = true;
				VisionSystem = true;

				FrameSkill = false;
				DeflagrationSkill = false;
				KnitmeshSkill = false;
				PerceptionSkill = false;
				DigestionSkill = true;//
				SpawnNecrophytes = true;//
				FixedSkill = false;
				KillingAuraSkill = true;//

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

				var passSet = new HashSet<string>(Passages);
				if (Survivor && passSet != null && passSet.Count > 0)
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
				}
			}
		}
	}
}

internal static class PlayerHooks
{

	public static void HookOn()
	{
		On.Player.ctor += Player_ctor;
		On.Player.Destroy += Player_Destroy;
	}

	private static void Player_ctor(On.Player.orig_ctor orig, Player player,
		AbstractCreature ac, World world)
	{
		orig(player, ac, world);
		PlayerModuleManager.RegisterPlayer(player);        // ① 注册
	}

	private static void Player_Destroy(On.Player.orig_Destroy orig, Player player)
	{
		orig(player);
		if (player.dead || player.slatedForDeletetion)
			PlayerModuleManager.UnregisterPlayer(player);  // ② 注销
	}


}
