using DpiTool.Launcher.Actions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DpiTool.Launcher
{
    internal class Program
    {
        static void Main(string[] args)
        {

            if (!AutoStartup.GetStatus())
                AutoStartup.Enable();

            // 要启动的程序路径
            string programPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"DpiTool.Loader.exe");

            // 创建进程启动信息
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = programPath,
                Verb = "runas" // 设置为以管理员身份运行
            };

            // 启动进程
            try
            {
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
