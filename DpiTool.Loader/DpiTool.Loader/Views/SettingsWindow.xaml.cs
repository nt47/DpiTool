using DpiTool.Loader.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace DpiTool.Loader.Views
{
    /// <summary>
    /// SettingsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public static SettingsWindow instance;
        public SettingsWindow()
        {
            InitializeComponent();

            instance=this;

            DataContext = new SettingsWindowVM(); // 替换成你的ViewModel实例
        }


        public bool m_bImplicitClose=false;
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!m_bImplicitClose)
            {
                e.Cancel = true;
                this.WindowState = WindowState.Minimized; //最小化
                this.ShowInTaskbar = false; //在任务栏中不显示窗体
                m_bImplicitClose = false;
            }
        }

    }
}
