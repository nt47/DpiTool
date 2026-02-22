using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DpiTool.Loader.Actions;
using DpiTool.Loader.Utils;
using System.Threading;

namespace DpiTool.Loader
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow instance;
        public MainWindow()
        {
            InitializeComponent();

            instance = this;


            Tray tray = new Tray();

            tray.InitializeTrayA();



            //CLR.Injector.Console();

            this.Hide();


            Dictionary<int, List<IntPtr>> map_fwPID = new Dictionary<int, List<IntPtr>>();


            Task.Run(() =>
            {

                _ = Inject.MainLoop(map_fwPID);
                _ = Inject.CacheLoop(map_fwPID);
            });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            NativeMethod.FlushMemory();
        }
    }
}
