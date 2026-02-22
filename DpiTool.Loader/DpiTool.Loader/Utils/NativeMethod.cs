using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DpiTool.Loader.Utils
{
    internal class NativeMethod
    {
        [DllImport("Injector.Core.X64.dll")]
        public extern static bool isExistProcess(int PID);
        [DllImport("Injector.Core.X64.dll")]
        public extern static bool IsDebuggerAttached(int PID);
        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VirtualMemoryOperation = 0x00000008,
            VirtualMemoryRead = 0x00000010,
            VirtualMemoryWrite = 0x00000020,
            DuplicateHandle = 0x00000040,
            CreateProcess = 0x000000080,
            SetQuota = 0x00000100,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            QueryLimitedInformation = 0x00001000,
            Synchronize = 0x00100000
        }
        [DllImport("User32.dll")]
        public extern static IntPtr GetForegroundWindow();
        [DllImport("User32.dll")]
        public extern static bool IsWindowVisible(IntPtr hwnd);
        [DllImport("User32.dll")]
        public extern static int GetWindowThreadProcessId(IntPtr hwnd, out int ID);
        [DllImport("kernel32.dll")]
        public extern static bool IsWow64Process([In] IntPtr processHandle, out bool wow64Process);

        [DllImport("kernel32.dll")]
        public extern static IntPtr OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, int processId);
        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hHandle);
        [DllImport("kernel32.dll")]
        public static extern bool SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);

        public class SafeHandle : SafeHandleZeroOrMinusOneIsInvalid//非托管资源需要Dispose
        {
            public SafeHandle(IntPtr handle) : base(true)
            {
                this.SetHandle(handle);
            }

            protected override bool ReleaseHandle()
            {
                return CloseHandle(this.handle);
            }

            public static implicit operator IntPtr(SafeHandle alloc)
            {
                return alloc.handle;
            }
        }
        public static bool isX86(int PID)
        {
            var hProcess = new SafeHandle(OpenProcess(ProcessAccessFlags.All, false, PID));
            if (hProcess.IsInvalid)
                return false;
            bool isX86;
            IsWow64Process(hProcess, out isX86);
            if (isX86)
                return true;

            return false;
        }

        public static void FlushMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
        }
    }
}
