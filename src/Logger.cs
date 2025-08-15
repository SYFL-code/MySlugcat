using BepInEx.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;


namespace MySlugcat
{
	public static class Log
	{
		public static bool LogReset = true;


		public static void Logger(float needloglevel, string about, string location, string message)
		{
			bool logDebug = Control.LogDebug;
			float loglevel = Control.Loglevel;

			string[] Enable = new string[] { "FixedSkill", "..."};
			bool EnableOutputLog = Enable.Contains(about);

			if (logDebug && loglevel >= needloglevel && EnableOutputLog)
			{
				string newContent = $"loglevel: {needloglevel.ToString()}, about: {about}, location: {location}\n   message: {message}";

				OutputLog(newContent);
			}

			if (logDebug && needloglevel < 0 && loglevel >= -needloglevel && EnableOutputLog)
			{
				string newContent = "\n";

				OutputLog(newContent);
			}
		}

		private static readonly object _lock = new object();

		private static void OutputLog(string newContent)
		{
			string filePath = "LH_MySlugcat_log.txt";

			lock (_lock)
			{
				try
				{
					UnityEngine.Debug.Log(newContent);
					Debug.Log(newContent);
					Console.WriteLine(newContent);

					// 如果文件不存在，直接创建并写入 或 新进游戏
					if (!File.Exists(filePath) || LogReset)
					{
						File.WriteAllText(filePath, $"=== 新日志 {DateTime.Now} ===\n\n");
						LogReset = false;
					}
					File.AppendAllText(filePath, newContent + "\n");
				}
				catch (Exception ex)
				{
					Console.WriteLine($"操作失败: {ex}");
					Debug.Log(newContent);
				}
			}

		}


	}
}
