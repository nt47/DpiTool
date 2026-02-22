using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DpiTool.Loader.Utils;

namespace DpiTool.Loader.Actions
{
    internal class Inject
    {
        static object g_lock = new object();


        static async Task CacheTaskAsync(Dictionary<int, List<IntPtr>> map_fwPID)
        {
            lock (g_lock)
            {
                List<int> badPID = new List<int>();
                foreach (var kvp in map_fwPID)
                {
                    int fwPID = kvp.Key;
                    if (!NativeMethod.isExistProcess(fwPID))
                    {
                        badPID.Add(fwPID);

                    }
                }

                foreach (var v in badPID)
                {
                    map_fwPID.Remove(v);
                }
            }
            //NativeMethod.FlushMemory();
            await Task.Delay(500);
        }


        static async Task MainTaskAsync(Dictionary<int, List<IntPtr>> map_fwPID)
        {
            int fwPID = 0;    //进程ID
            int fwTID = 0;    //线程ID

            IntPtr hwnd;
            Process process;

            Config.Load();

            lock (g_lock)
            {
                hwnd = NativeMethod.GetForegroundWindow();
                fwTID = NativeMethod.GetWindowThreadProcessId(hwnd, out fwPID);
                process = Process.GetProcessById(fwPID);

                if (process.ProcessName == "explorer"
                    || process.ProcessName == "devenv"
                     || process.ProcessName == "chrome"
                     || process.ProcessName == "msedge"
                     || process.ProcessName == "msedgewebview2"
                     || Config.Contains(process.ProcessName)
                     || NativeMethod.IsDebuggerAttached(fwPID)

                    )
                    return;

                if (hwnd == IntPtr.Zero)
                    return;

                if (map_fwPID.ContainsKey(fwPID) && !map_fwPID[fwPID].Contains(hwnd))//存在PID缓存,但不存在窗口句柄缓存
                {
                    map_fwPID[fwPID].Add(hwnd);
                    return;
                }

                if (map_fwPID.ContainsKey(fwPID) || !NativeMethod.IsWindowVisible(hwnd))
                    return;

                map_fwPID.Add(fwPID, new List<IntPtr> { hwnd });

                //遍历字典
                foreach (var kvp in map_fwPID)
                {
                    foreach (var v in kvp.Value)
                    {
                        Console.WriteLine("PID = {0}, hwnd = {1}", kvp.Key, v);
                    }
                }

                if (NativeMethod.isX86(fwPID))
                {
                    CLR.Injector.Inject(fwPID, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"DpiTool.Core.X86.dll"));//开机启动必须绝对路径注入
                }
                else
                {
                    CLR.Injector.Inject(fwPID, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"DpiTool.Core.X64.dll"));//开机启动必须绝对路径注入
                }
            }
            //NativeMethod.FlushMemory();
            await Task.Delay(500);
        }

        public static async Task CacheLoop(Dictionary<int, List<IntPtr>> map_fwPID)
        {

            do
            {
                await CacheTaskAsync(map_fwPID);
                await Task.Delay(500);


            } while (true);
        }
        public static async Task MainLoop(Dictionary<int, List<IntPtr>> map_fwPID)
        {
            //Console.WriteLine("Inject MainLoop Start");
            do
            {
                await MainTaskAsync(map_fwPID);
                await Task.Delay(500);

            } while (true);
        }
    }
}
