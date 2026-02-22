using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace DpiTool.Loader.Utils
{
    public static class Config
    {
        public static readonly string AppDirConfigPath =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Config\Settings.json");
        private static readonly object file_lock= new object();

        public static object Lock
        {
            get{ 
                return file_lock;
            }
        }
        public static void Load()
        {
            lock (Lock)
            {
                if (!File.Exists(AppDirConfigPath))
                {
                    MessageBox.Show("配置文件不存在", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    Application.Current.Shutdown();
                }
                string jsonStr;
                using (var sr = new StreamReader(AppDirConfigPath, false))
                {
                    jsonStr = sr.ReadToEnd();
                }

                var jsonObj = JObject.Parse(jsonStr);

                Filter.ProcessNames = jsonObj["Filter"]["ProcessNames"].Select(x => (string)x).ToList();
            }


        }

        public static void Save()
        {
            lock (Lock)
            { 
                if (!File.Exists(AppDirConfigPath))
                {
                    MessageBox.Show("配置文件不存在", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    Application.Current.Shutdown();
                }
                var jsonObj = new JObject();

                jsonObj["Filter"] = new JObject();
                jsonObj["Filter"]["ProcessNames"] = JToken.FromObject(Filter.ProcessNames);

                var jsonStr = jsonObj.ToString();
                using (var sw = new StreamWriter(AppDirConfigPath))
                {
                    sw.WriteLine(jsonStr);
                }
            }
        }

        public static bool Contains(string name)
        {
            lock(Lock)
            {
                return Filter.ProcessNames.Contains(name);
            }
        }
        public static void Add(string name)
        {
            lock (Lock)
            {
                Filter.ProcessNames.Add(name);
            }
        }

        public static void Remove(string name)
        {
            lock (Lock)
            {
                Filter.ProcessNames.Remove(name);
            }
        }

        public static void Edit(string name,string value)
        {
            lock (Lock)
            {
                int index = Config.Filter.ProcessNames.IndexOf(name);
                Filter.ProcessNames[index] = value;
            }
        }

        public static void Clear()
        {
            lock (Lock)
            {
                Filter.ProcessNames.Clear();
            }
        }


        public static class Filter
        {
            public static List<string> ProcessNames;
        }
    }

}
