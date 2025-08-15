using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using IL.Menu;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.Utils;
using On;
using SlugBase;
using SlugBase.Features;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using Mono.Collections.Generic;
using MonoMod.RuntimeDetour;


namespace MySlugcat
{
	// 包含了一些控制插件行为的静态字段和方法
	public static class Intros
	{
		public static bool mscCheckbox = false;// 允许 MSC 开场动画
		public static bool slugbaseCheckbox = false;// 允许 Slugbase 蛞蝓猫的开场动画（实验性功能）
		public static bool onlySlugbaseCheckbox = true;// 仅启用 Slugbase 蛞蝓猫的开场动画

		// 定义了标题名称数组
		/// <summary>
		/// 插件自己的标题卡名称列表。当完全禁用 MSC/SlugBase 时可单独使用。
		/// </summary>
		public static readonly string[] titles = new string[]
		{
			"MySlugcat"
		};

		// 注册钩子函数
		/// <summary>
		/// 注册 IL 钩子，修改 Menu.IntroRoll 的构造函数，把最终标题卡字符串替换掉。
		/// </summary>
		public static void Hook()
		{
			// ILContext 钩子：在构造函数里找到生成标题卡的路径，插入自定义逻辑
			IntroRoll.ctor += IntroRoll_ctor;
		}

		// 钩子函数，用于修改游戏的开场滚动标题
		/// <summary>
		/// 实际的 IL 钩子实现。
		/// 流程：
		/// 1. 找到加载字符串 "Intro_Roll_C_" 的位置。
		/// 2. 在其后插入 Delegate，动态决定最终标题卡名。
		/// 3. 支持三种模式：
		///    • 原版 + 插件自定义
		///    • 原版 + MSC
		///    • 纯 SlugBase（若启用）
		/// </summary>
		/// <param name="il">MonoMod 注入的 ILContext</param>
		public static void IntroRoll_ctor(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);

			/* ---------- 处理 MSC 路径 ---------- */
			// 定位 ldstr "Intro_Roll_C_"  -> call string.Concat(...)
			ILCursor ilcursor2 = ilcursor;
			Func<Instruction, bool>[] array = new Func<Instruction, bool>[1];
			array[0] = ((Instruction i) => i.MatchLdstr("Intro_Roll_C_"));

			bool MSCILHook = false;
			if (ilcursor2.TryGotoNext(array))
			{
				ILCursor ilcursor3 = ilcursor;
				MoveType moveType = MoveType.After;
				Func<Instruction, bool>[] array2 = new Func<Instruction, bool>[1];
				array2[0] = ((Instruction i) => i.MatchCallOrCallvirt<string>("Concat"));
				MSCILHook = ilcursor3.TryGotoNext(moveType, array2);
			}
			if (MSCILHook)
			{
				ilcursor.Emit(Mono.Cecil.Cil.OpCodes.Ldloc_3); // 把旧的 string[] 压栈
				ilcursor.EmitDelegate<Func<string, string[], string>>(delegate (string titleImage, string[] oldTitleImages)
				{
					// 如果需要加入 MSC 标题，则合并；否则仅用自己的 titles
					oldTitleImages = (mscCheckbox ? titles.Concat(oldTitleImages).ToArray<string>() : titles);
					return GetVanillaIntros(oldTitleImages);
				});
			}
			else
			{
				//Debug.LogError("IL hook IntroRoll_ctor, MSC, failed!");
			}

			/* ---------- 处理非 MSC 路径 ---------- */
			ilcursor.Index = 0; // 重置游标
			ILCursor ilcursor4 = ilcursor;
			Func<Instruction, bool>[] array3 = new Func<Instruction, bool>[1];
			array3[0] = ((Instruction i) => i.MatchLdstr("Intro_Roll_C_"));
			bool flag3;
			if (ilcursor4.TryGotoNext(array3))
			{
				ILCursor ilcursor5 = ilcursor;
				Func<Instruction, bool>[] array4 = new Func<Instruction, bool>[1];
				array4[0] = ((Instruction i) => i.MatchLdstr("Intro_Roll_C"));
				flag3 = ilcursor5.TryGotoPrev(array4);
			}
			else
			{
				flag3 = false;
			}
			bool flag4 = flag3;
			if (flag4)
			{
				ilcursor.EmitDelegate<Func<string, string>>((string titleImage) => GetVanillaIntros(titles));
			}
			else
			{
				//Debug.LogError("IL hook IntroRoll_ctor, no MSC, failed!");
			}

		}

		// 扩展方法，用于匹配特定名称的调用
		public static bool MatchCallOrCallvirt<T>(this Instruction instr, string name)
		{
			MethodReference method;
			return instr.MatchCallOrCallvirt(out method) && method.Is(typeof(T), name);
		}

		// 获取游戏的开场滚动标题
		/// <summary>
		/// 根据当前开关状态返回最终标题卡资源名。
		/// </summary>
		/// <param name="titleImages">候选标题卡名称数组</param>
		/// <returns>最终使用的资源名</returns>
		private static string GetVanillaIntros(string[] titleImages)
		{
			string result = "Intro_Roll_C_Artificer";  // 默认（失败）回退

			// 如果允许使用原版/MSC 标题池，则从中随机
			if (ShouldApplyVanillaIntros())
			{
				result = "Intro_Roll_C_" + titleImages[UnityEngine.Random.Range(0, titleImages.Length)];
			}

			// 如果允许使用 SlugBase，则进一步覆盖或混合
			if (ShouldApplySlugbaseIntros())
			{
				ApplySlugbaseIntros(ref result, titleImages);
			}
			return result;
		}

		// 应用 SlugBase 插件的开场滚动标题
		/// <summary>
		/// 扫描所有已注册的 SlugBaseCharacter，收集它们声明的 TitleCard，
		/// 并根据 onlySlugbaseCheckbox 决定是否完全替换或概率混入。
		/// </summary>
		/// <param name="titleImage">当前已决定的标题名（in/out）</param>
		/// <param name="titleImages">原版/MSC 标题池</param>
		private static void ApplySlugbaseIntros(ref string titleImage, string[] titleImages)
		{
			List<string> list = new List<string>();
			foreach (SlugBaseCharacter slugBaseCharacter in SlugBaseCharacter.Registry.Values)
			{
				if (GameFeatures.TitleCard.TryGet(slugBaseCharacter, out var text) && !string.IsNullOrEmpty(text))
				{
					list.Add(text);
				}
			}

			if (list.Count > 0)
			{
				if (onlySlugbaseCheckbox)
				{
					// 仅使用 SlugBase
					int index = UnityEngine.Random.Range(0, list.Count);
					//base.Logger.LogDebug("Only slugbase intros! Intro selected: " + list[index]);
					titleImage = list[index];
				}
				else
				{
					// 随机决定是原版还是 SlugBase
					int num = UnityEngine.Random.Range(0, list.Count + titleImages.Length);
					bool flag3 = num < list.Count;
					if (flag3)
					{
						//base.Logger.LogDebug("Changing intro: " + titleImage + " to Slugbase's intro " + list[num]);
						titleImage = list[num];
					}
				}
			}
		}

		// 检查 SlugBase 插件是否启用
		/// <summary>
		/// 检查 SlugBase 是否被 ModManager 加载并启用。
		/// </summary>
		public static bool IsSlugbaseEnabled()
		{
			foreach (ModManager.Mod mod in ModManager.ActiveMods)
			{
				bool flag = mod.id == "slime-cubed.slugbase";
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// 判断是否应该应用原版游戏的开场滚动标题
		/// <summary>
		/// 判断是否应该让原版/MSC 标题卡参与选择。
		/// </summary>
		private static bool ShouldApplyVanillaIntros()
		{
			return !IsSlugbaseEnabled() || !slugbaseCheckbox || !onlySlugbaseCheckbox;
		}

		// 判断是否应该应用 SlugBase 插件的开场滚动标题
		/// <summary>
		/// 判断是否应该让 SlugBase 介入标题卡选择。
		/// </summary>
		private static bool ShouldApplySlugbaseIntros()
		{
			return IsSlugbaseEnabled() && (!slugbaseCheckbox || onlySlugbaseCheckbox);
		}


	}
}
