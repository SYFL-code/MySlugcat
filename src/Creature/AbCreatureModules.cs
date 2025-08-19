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
using static MySlugcat.PlayerModuleManager;
using static MySlugcat.AbCreatureModuleManager;
using On;
using IL;
using RewiredConsts;
using SlugBase.Features;
using static HarmonyLib.Code;


namespace MySlugcat;

internal static class AbCreatureModuleManager
{
	//Player

	private static readonly ReaderWriterLockSlim _rwLock = new(LockRecursionPolicy.SupportsRecursion);
	private static readonly HashSet<AbstractCreature> _activeAbCreatures = new();
	public static readonly ConditionalWeakTable<AbstractCreature, AbCreatureModule> AbCreatureModules = new();

	private static readonly List<AbstractCreature> _snapshot = new(4);
	private static volatile bool _dirty = true;

	public static int ActiveAbCreatureCount
	{
		get
		{
			_rwLock.EnterReadLock();
			try { return _activeAbCreatures.Count; }
			finally { _rwLock.ExitReadLock(); }
		}
	}

	public static void RegisterAbCreature(AbstractCreature abCreature)
	{
		if (abCreature == null) return;

		_rwLock.EnterWriteLock();
		try
		{
			_activeAbCreatures.Add(abCreature);
			AbCreatureModules.Add(abCreature, new AbCreatureModule(abCreature));
			_dirty = true;
		}
		finally { _rwLock.ExitWriteLock(); }
	}

	public static void UnregisterAbCreature(AbstractCreature abCreature)
	{
		if (abCreature == null) return;

		_rwLock.EnterWriteLock();
		try
		{
			_activeAbCreatures.Remove(abCreature);
			AbCreatureModules.Remove(abCreature);
			_dirty = true;
		}
		finally { _rwLock.ExitWriteLock(); }
	}

	public static IReadOnlyList<AbstractCreature> GetActiveAbCreatures()
	{
		if (_dirty)
		{
			_rwLock.EnterReadLock();
			try
			{
				_snapshot.Clear();
				_snapshot.AddRange(_activeAbCreatures);
				_dirty = false;
			}
			finally { _rwLock.ExitReadLock(); }
		}
		return _snapshot;

		/*_rwLock.EnterReadLock();
		try
		{ 
			return new List<AbstractCreature>(_activeAbCreatures); 
		}
		finally { _rwLock.ExitReadLock(); }*/
	}

	public static AbCreatureModule GetModule(this AbstractCreature abCreature)
	{
		_rwLock.EnterReadLock();
		try
		{
			if (AbCreatureModules.TryGetValue(abCreature, out var module_))
			{
				return module_;
			}
		}
		finally { _rwLock.ExitReadLock(); }

		_rwLock.EnterWriteLock();
		try
		{
			// 玩家不存在于 AbCreatureModules 中时，创建并注册模块
			AbCreatureModule module = new AbCreatureModule(abCreature);
			AbCreatureModules.Add(abCreature, module);
			_activeAbCreatures.Add(abCreature);
			_dirty = true;
			return module;
		}
		finally { _rwLock.ExitWriteLock(); }
	}

	public static bool GetModule(this AbstractCreature abCreature, out AbCreatureModule module)
	{
		_rwLock.EnterReadLock();
		try
		{
			if (AbCreatureModules.TryGetValue(abCreature, out var module_))
			{
				module = module_;
				return true;
			}
		}
		finally { _rwLock.ExitReadLock(); }

		_rwLock.EnterWriteLock();
		try
		{
			// 玩家不存在于 AbCreatureModules 中时，创建并注册模块
			AbCreatureModule module__ = new AbCreatureModule(abCreature);
			AbCreatureModules.Add(abCreature, module__);
			if (!_activeAbCreatures.Contains(abCreature))
			{
				_activeAbCreatures.Add(abCreature);
				_dirty = true;
			}
			module = module__;
			return true;
		}
		finally { _rwLock.ExitWriteLock(); }
	}



	internal class AbCreatureModule
	{
		//WeakReference<Creature> creatureRef;
		public readonly AbstractCreature abCreature;

		#region 字段
		/// <summary> 是否为亡灵生物 </summary>
		public bool IsNecrophyte = false;
		/// <summary> 灰度化 </summary>
		public float Grayization = 0f;
		/// <summary> 亡灵生物死亡中(True 表示该亡灵生物正在执行 Despawn 动画，不要再触发 Die。) </summary>
		public bool NecrophyteDying = false;
		#endregion



		public AbCreatureModule(AbstractCreature abCreature)
		{
			this.abCreature = abCreature;
			//creatureRef = new WeakReference<Creature>(creature);
		}
	}

}


internal static class AbCreatureHooks
{

	public static void HookOn()
	{
		//On.Creature.ctor += Creature_ctor;
		//On.Creature.Die += Creature_Die;
		On.AbstractCreature.ctor += AbstractCreature_ctor;
		On.AbstractPhysicalObject.Destroy += AbstractPhysicalObject_Destroy;
	}

	/*private static void Creature_ctor(On.Creature.orig_ctor orig, Creature creature,
		AbstractCreature ac, World world)
	{
		orig(creature, ac, world);
		CreatureModuleManager.RegisterCreature(creature);        // ① 注册
	}*/

	private static void AbstractCreature_ctor(On.AbstractCreature.orig_ctor orig, AbstractCreature ac, World world, CreatureTemplate template, Creature realizedCreature, WorldCoordinate pos, EntityID ID)
	{
		orig(ac, world, template, realizedCreature, pos, ID);
		AbCreatureModuleManager.RegisterAbCreature(ac);        // ① 注册
	}

	private static void AbstractPhysicalObject_Destroy(On.AbstractPhysicalObject.orig_Destroy orig, AbstractPhysicalObject abPhysicalObject)
	{
		orig(abPhysicalObject);
		if (abPhysicalObject is AbstractCreature abcreature)
		{
			if (abPhysicalObject.slatedForDeletion)
			{
				AbCreatureModuleManager.UnregisterAbCreature(abcreature);  // ② 注销
			}
		}

	}


}

