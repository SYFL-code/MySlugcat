using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MySlugcat
{
    public class Log
    {
        public static void Logger(float needloglevel, string message)
        {
            Configurable<bool>? logDebug = Options.logDebug;
            Configurable<float>? loglevel = Options.loglevel;
            if ((logDebug == null || logDebug.Value) && loglevel != null && loglevel.Value >= needloglevel)
            {

                string filePath = "LH_MySlugcat_log.txt";
                string newContent = $"loglevel :{needloglevel.ToString()}, message: {message}";
                //string newContent = "吃吃吃！！！";

                try
                {
                    Console.WriteLine(newContent);
                    // 如果文件不存在，直接创建并写入
                    if (!File.Exists(filePath))
                    {
                        File.WriteAllText(filePath, newContent + "\n" + $"=== 新日志 {DateTime.Now:yyyy-MM-dd} ===\n");
                        Console.WriteLine($"已创建新文件,文件路径: {Path.GetFullPath(filePath)}");
                    }
                    else
                    {
                        // 读取现有内容
                        string existingContent = File.ReadAllText(filePath);

                        // 将新内容放在顶部 + 原有内容
                        File.WriteAllText(filePath, newContent + "\n" + existingContent);
                    }

                    if ((logDebug == null || logDebug.Value) && loglevel.Value >= 10)
                    {
                        Console.WriteLine($"内容已添加到文件顶部: {Path.GetFullPath(filePath)}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"操作失败: {ex.Message}");
                }
            }

        }
    }
}
