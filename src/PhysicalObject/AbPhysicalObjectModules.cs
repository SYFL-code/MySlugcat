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
using static MySlugcat.AbPhysicalObjectModuleManager;
using On;
using IL;
using RewiredConsts;


namespace MySlugcat;

internal static class AbPhysicalObjectModuleManager
{
	//Player

	private static readonly ReaderWriterLockSlim _rwLock = new(LockRecursionPolicy.SupportsRecursion);
	private static readonly HashSet<AbstractPhysicalObject> _activeAbPhysicalObjects = new();
	public static readonly ConditionalWeakTable<AbstractPhysicalObject, AbPhysicalObjectModule> AbPhysicalObjectModules = new();

	private static readonly List<AbstractPhysicalObject> _snapshot = new(4);
	private static volatile bool _dirty = true;

	public static int ActiveAbPhysicalObjectCount
	{
		get
		{
			_rwLock.EnterReadLock();
			try { return _activeAbPhysicalObjects.Count; }
			finally { _rwLock.ExitReadLock(); }
		}
	}

	public static void RegisterAbPhysicalObject(AbstractPhysicalObject abPhysicalObject)
	{
		if (abPhysicalObject == null) return;

		_rwLock.EnterWriteLock();
		try
		{
			_activeAbPhysicalObjects.Add(abPhysicalObject);
			AbPhysicalObjectModules.Add(abPhysicalObject, new AbPhysicalObjectModule(abPhysicalObject));
			_dirty = true;
		}
		finally { _rwLock.ExitWriteLock(); }
	}

	public static void UnregisterAbPhysicalObject(AbstractPhysicalObject abPhysicalObject)
	{
		if (abPhysicalObject == null) return;

		_rwLock.EnterWriteLock();
		try
		{
			_activeAbPhysicalObjects.Remove(abPhysicalObject);
			AbPhysicalObjectModules.Remove(abPhysicalObject);
			_dirty = true;
		}
		finally { _rwLock.ExitWriteLock(); }
	}

	public static IReadOnlyList<AbstractPhysicalObject> GetActiveAbPhysicalObjects()
	{
		if (_dirty)
		{
			_rwLock.EnterReadLock();
			try
			{
				_snapshot.Clear();
				_snapshot.AddRange(_activeAbPhysicalObjects);
				_dirty = false;
			}
			finally { _rwLock.ExitReadLock(); }
		}
		return _snapshot;

		/*_rwLock.EnterReadLock();
		try
		{ 
			return new List<AbstractPhysicalObject>(_activeAbPhysicalObjects); 
		}
		finally { _rwLock.ExitReadLock(); }*/
	}

	public static AbPhysicalObjectModule GetModule(this AbstractPhysicalObject abPhysicalObject)
	{
		_rwLock.EnterReadLock();
		try
		{
			if (AbPhysicalObjectModules.TryGetValue(abPhysicalObject, out var module_))
			{
				return module_;
			}
		}
		finally { _rwLock.ExitReadLock(); }

		_rwLock.EnterWriteLock();
		try
		{
			// 玩家不存在于 AbPhysicalObjectModules 中时，创建并注册模块
			AbPhysicalObjectModule module = new AbPhysicalObjectModule(abPhysicalObject);
			AbPhysicalObjectModules.Add(abPhysicalObject, module);
			_activeAbPhysicalObjects.Add(abPhysicalObject);
			_dirty = true;
			return module;
		}
		finally { _rwLock.ExitWriteLock(); }
	}

	public static bool GetModule(this AbstractPhysicalObject abPhysicalObject, out AbPhysicalObjectModule module)
	{
		_rwLock.EnterReadLock();
		try
		{
			if (AbPhysicalObjectModules.TryGetValue(abPhysicalObject, out var module_))
			{
				module = module_;
				return true;
			}
		}
		finally { _rwLock.ExitReadLock(); }

		_rwLock.EnterWriteLock();
		try
		{
			// 玩家不存在于 AbPhysicalObjectModules 中时，创建并注册模块
			AbPhysicalObjectModule module__ = new AbPhysicalObjectModule(abPhysicalObject);
			AbPhysicalObjectModules.Add(abPhysicalObject, module__);
			if (!_activeAbPhysicalObjects.Contains(abPhysicalObject))
			{
				_activeAbPhysicalObjects.Add(abPhysicalObject);
				_dirty = true;
			}
			module = module__;
			return true;
		}
		finally { _rwLock.ExitWriteLock(); }
	}



	internal class AbPhysicalObjectModule
	{
		//WeakReference<PhysicalObject> PhysicalObjectRef;
		public readonly AbstractPhysicalObject abPhysicalObject;

		#region 字段
		/// <summary> 武器穿透对象 </summary>
		public PhysicalObject? stuckInObject = null;
		/// <summary> 武器穿透时长 </summary>
		public int stuckInObjectTime = 0;
		/// <summary> 武器穿透次数 </summary>
		public int penetrateCount = 0;
		#endregion



		public AbPhysicalObjectModule(AbstractPhysicalObject abPhysicalObject)
		{
			this.abPhysicalObject = abPhysicalObject;
			//PhysicalObjectRef = new WeakReference<PhysicalObject>(PhysicalObject);
		}
	}

}


internal static class AbPhysicalObjectHooks
{

	/*public static void HookOn()
	{
		//On.PhysicalObject.ctor += PhysicalObject_ctor;
		//On.PhysicalObject.Die += PhysicalObject_Die;
		On.AbstractPhysicalObject.ctor += AbstractPhysicalObject_ctor;
		On.AbstractPhysicalObject.Destroy += AbstractPhysicalObject_Destroy;
	}*/

	/*private static void PhysicalObject_ctor(On.PhysicalObject.orig_ctor orig, PhysicalObject PhysicalObject,
		AbstractPhysicalObject ac, World world)
	{
		orig(PhysicalObject, ac, world);
		PhysicalObjectModuleManager.RegisterPhysicalObject(PhysicalObject);        // ① 注册
	}*/

	public static void AbstractPhysicalObject_ctor(ref bool Execute, ref On.AbstractPhysicalObject.orig_ctor orig, ref AbstractPhysicalObject ac, ref World world, ref AbstractPhysicalObject.AbstractObjectType type, ref PhysicalObject realizedPhysicalObject, ref WorldCoordinate pos, ref EntityID ID)
	{
		AbPhysicalObjectModuleManager.RegisterAbPhysicalObject(ac);        // ① 注册
	}

	public static void AbstractPhysicalObject_Destroy(ref bool Execute, ref On.AbstractPhysicalObject.orig_Destroy orig, ref AbstractPhysicalObject abPhysicalObject)
	{
		if (abPhysicalObject != null)
		{
			if (abPhysicalObject.slatedForDeletion)
			{
				AbPhysicalObjectModuleManager.UnregisterAbPhysicalObject(abPhysicalObject);  // ② 注销
			}
		}

	}


}

