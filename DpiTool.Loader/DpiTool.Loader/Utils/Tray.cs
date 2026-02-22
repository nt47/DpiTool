using DpiTool.Loader.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace DpiTool.Loader.Utils
{
    internal class Tray
    {
        static SettingsWindow wnd=null;
        void About_Click(object sender, EventArgs e)
        {
            System.Windows.MessageBox.Show("修复大多数程序高分辨率Dpi问题","提示",MessageBoxButton.OK,MessageBoxImage.Information);
        }

        void Reload_Click(object sender, EventArgs e)
        {
            // 获取当前进程的路径
            string processPath = Process.GetCurrentProcess().MainModule.FileName;

            // 启动新进程
            Process.Start(processPath);

            // 终止当前进程
            Environment.Exit(0);
        }

        void Show_Click(object sender, EventArgs e)
        {
            if(wnd==null)
            {  
                wnd = new SettingsWindow(); 
                wnd.Show();
            }

            wnd.WindowState = WindowState.Normal;
            wnd.ShowInTaskbar = true;
        }

        void Exit_Click(object sender, EventArgs e)//implicit call
        {
            if(wnd!=null)
            {
                wnd.m_bImplicitClose = true;
                wnd.Close();
            }
            MainWindow.instance.Close();
        }


        private readonly NotifyIcon _notifyIcon = new NotifyIcon();
        public void InitializeTrayA()
        {
            _notifyIcon.BalloonTipText = "DpiTool is running";
            _notifyIcon.Text = "DpiTool";


            _notifyIcon.Icon = Properties.Resources.DpiTool;

            _notifyIcon.Visible = true;
             _notifyIcon.ShowBalloonTip(0);

            _notifyIcon.DoubleClick += Show_Click;

            var childen = new ContextMenuStrip();
            childen.Items.Add("关于",null,About_Click);
            childen.Items.Add("重载", null, Reload_Click);
            childen.Items.Add("设置", null, Show_Click);
            childen.Items.Add("退出", null, Exit_Click);


            _notifyIcon.ContextMenuStrip = childen;
        }
    }
}
