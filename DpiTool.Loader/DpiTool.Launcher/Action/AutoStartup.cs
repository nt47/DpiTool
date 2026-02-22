using DpiTool.Loader.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DpiTool.Launcher.Actions
{
    internal class AutoStartup
    {
        private const string RegeditAutoStartDir = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string RegeditAutoStartKey = "DpiTool";

        public static bool GetStatus()
        {
            var value = Regedit.GetValue(RegeditAutoStartDir, RegeditAutoStartKey);
            if (value == null) return false;
            if (!value.Equals(Process.GetCurrentProcess().MainModule.FileName))
                Regedit.SetValue(RegeditAutoStartDir, RegeditAutoStartKey, Process.GetCurrentProcess().MainModule.FileName);
            return true;
        }

        public static void Enable()
        {
            var result = Regedit.SetValue(RegeditAutoStartDir, RegeditAutoStartKey, Process.GetCurrentProcess().MainModule.FileName);
            if (!result) MessageBox.Show(null, "开启启动设置失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void Disable()
        {
            var result = Regedit.DeleteValue(RegeditAutoStartDir, RegeditAutoStartKey);
            if (!result) MessageBox.Show(null, "开启启动设置失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
