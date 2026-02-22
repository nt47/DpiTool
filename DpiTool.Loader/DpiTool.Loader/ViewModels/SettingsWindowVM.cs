using DpiTool.Loader.Utils;
using DpiTool.Loader.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace DpiTool.Loader.ViewModels
{
    internal class SettingsWindowVM : INotifyPropertyChanged
    {
        //实现接口当数据源变动通知前台UI
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 前台DataGrid绑定的People集合
        /// </summary>
        public ObservableCollection<ProcessNameItem> ProcessNames { get; set; }

        /// <summary>
        /// 绑定前台DataGrid控件SelectedItem字段上，用于保存当前选中的Item所对应的数据源
        /// </summary>
        public ProcessNameItem SelectedItem
        {
            get
            {
                return m_SelectedItem;
            }
            set
            {
                m_SelectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }

        /// <summary>
        /// DataGrid控件中删除按钮命令
        /// </summary>
        public Command DelClick
        {
            get
            {
                if (m_DelClick == null)
                    m_DelClick = new Command(DeleteEvent);

                return m_DelClick;
            }
        }

        /// <summary>
        /// 前台添加小刚按钮命令
        /// </summary>
        public Command AddClick
        {
            get
            {
                if (m_AddClick == null)
                    m_AddClick = new Command(AdditionEvent);

                return m_AddClick;
            }
        }


        /// <summary>
        /// 保存内容按钮命令
        /// </summary>
        public Command SaveClick
        {
            get
            {
                if (m_SaveClick == null)
                    m_SaveClick = new Command(SaveEvent);

                return m_SaveClick;
            }
        }



        /// <summary>
        /// DataGrid控件电话信息的TextBox键盘按下回车命令
        /// </summary>
        public Command PressEnterKey
        {
            get
            {
                if (m_PressEnterKey == null)
                    m_PressEnterKey = new Command(PressEnterKeyEvent);

                return m_PressEnterKey;
            }
        }

        private ProcessNameItem m_SelectedItem;

        private Command m_DelClick;
        private Command m_AddClick;

        private Command m_SaveClick;


        private Command m_PressEnterKey;

        /// <summary>
        /// 构造方法
        /// </summary>
        public SettingsWindowVM()
        {

            ProcessNames = new ObservableCollection<ProcessNameItem>();

            //ProcessNames person1 = new ProcessNames() { ProcessName = "*" };
            //ProcessNames person2 = new ProcessNames() { ProcessName = "*" };
            //ProcessNames person3 = new ProcessNames() { ProcessName = "*" };

            //processNames.Add(person1);
            //processNames.Add(person2);
            //processNames.Add(person3);

            Config.Load();

            List<string> tmp = new List<string>(Config.Filter.ProcessNames);


            foreach (var name in tmp)
            {
                ProcessNames.Add(new ProcessNameItem() { ProcessName = name });
            }

            foreach (var processName in ProcessNames)
            {
                processName.m_bIsInitialized = true;
            }
        }

        /// <summary>
        /// DataGrid控件中删除按钮事件
        /// </summary>
        /// <param name="obj">可传入前台控件</param>
        private void DeleteEvent(object obj)
        {
            if (MessageBox.Show($"是否删除进程名{SelectedItem.ProcessName}？", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Config.Remove(SelectedItem.ProcessName);//要防止前面，否则就被移除了
                Config.Save();

                ProcessNames.Remove(SelectedItem);
            }
        }

        /// <summary>
        /// 前台添加空行按钮事件
        /// </summary>
        /// <param name="obj">可传入前台控件</param>
        private void AdditionEvent(object obj)
        {
            if (MessageBox.Show("默认添加一行空数据，请自行编辑修改", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                ProcessNames.Add(new ProcessNameItem() { ProcessName = "*", m_bIsInitialized = true });

                Config.Add("*");
                Config.Save();
            }
        }


        /// <summary>
        /// DataGrid控件中保存按钮事件
        /// </summary>
        /// <param name="obj">可传入前台控件</param>
        private void SaveEvent(object obj)
        {
            DataGrid datas = SettingsWindow.instance.DataGrid_Main;

            List<string> list_tmp = new List<string>();

            // 遍历DataGrid中所有TextBox的内容
            // 获取 DataGrid 中的所有行
            var rows = DataGridHelper.GetDataGridRows(datas);

            foreach (var row in rows)
            {
                // 通过 VisualTreeHelper 查找当前行的所有 TextBox 控件
                var textBoxes = DataGridHelper.FindVisualChildren<TextBox>(row);

                foreach (var textBox in textBoxes)
                {
                    // 在这里执行你想要的操作
                    // 例如，访问 TextBox 的 Text 属性
                    //MessageBox.Show(textBox.Text);

                    list_tmp.Add(textBox.Text);
                    // 进行其他操作...
                }
            }

            ProcessNames.Clear();
            Config.Clear();

            foreach (var name in list_tmp)
            {
                ProcessNames.Add(new ProcessNameItem() { ProcessName = name });
                Config.Add(name);
            }

            Config.Save();
            MessageBox.Show("保存数据成功！");

        }

        /// <summary>
        /// DataGrid控件电话信息的TextBox键盘按下回车事件
        /// </summary>
        /// <param name="obj">可传入前台控件</param>
        private void PressEnterKeyEvent(object obj)//CommandParameter=obj
        {
            TextBox textBox = (TextBox)obj;
            MessageBox.Show($"点击了回车！控件内容为：{textBox.Text}");
        }

        /// <summary>
        /// 数据结构
        /// </summary>
        public class ProcessNameItem
        {
            //public string ProcessName { get; set; }

            string _ProcessName;
            public bool m_bIsInitialized;

            public ProcessNameItem()
            {
                m_bIsInitialized = false;
            }

            public string ProcessName
            {
                get
                {
                    return _ProcessName;
                }

                set
                {
                    if (m_bIsInitialized)
                    {
                        //MessageBox.Show(string.Format("修改了内容{0}", _ProcessName));
                        Config.Edit(_ProcessName, value);
                        Config.Save();
                    }

                    _ProcessName = value;


                }

            }
        }
    }
}
