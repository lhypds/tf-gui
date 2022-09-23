using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;

namespace TfGuiTool.Utils
{
    internal class SimpleConfigUtils
    {
        const string CONFIG = "config.txt";
        const string SPLITER = ",";
        static readonly List<string> ConfigStrings = new List<string>
        {
            "tf_executable_path",
            "collection_url",
            "workspace",
            "user_name",
            "password",
            "project_path",
            "tfs_path",
            "drag_and_drop_to_checkout",
            "default_text_editor"
        };

        public static StringDictionary ReadConfigs()
        {
            StringDictionary configs = new StringDictionary();
            if (File.Exists(CONFIG))
            {
                List<string> configStrings = File.ReadAllLines(CONFIG).ToList();
                configStrings.Sort();
                foreach (var configString in configStrings)
                {
                    if (configString.Contains(SPLITER))
                    {
                        List<string> configKeyValue = configString.Split(SPLITER).ToList();
                        configs.Add(configKeyValue[0], configKeyValue[1]);
                    }
                }
            }
            return configs;
        }

        public static string GetConfig(string key)
        {
            StringDictionary configs = ReadConfigs();
            if (configs.ContainsKey(key)) return configs[key];
            else return "";
        }

        public static void AddConfig(string key, string value)
        {
            List<string> config = new List<string>();
            config.Add(key + "," + value);
            File.AppendAllLines("config.txt", config);
        }

        public static void SetConfig(string key, string newValue)
        {
            StringDictionary configs = ReadConfigs();
            if (configs.ContainsKey(key)) configs.Remove(key);
            configs.Add(key, newValue);

            List<string> configList = new List<string>();
            foreach (var configKey in configs.Keys)
            {
                configList.Add(configKey.ToString() + "," + configs[configKey.ToString()]);
            }
            configList.Sort();
            File.WriteAllLines("config.txt", configList, Encoding.UTF8);
        }

        public static void RemoveConfig(string key)
        {
            StringDictionary configs = ReadConfigs();
            if (configs.ContainsKey(key)) configs.Remove(key);
            
            List<string> configList = new List<string>();
            foreach (var configKey in configs.Keys)
            {
                configList.Add(configKey.ToString() + "," + configs[configKey.ToString()]);
            }
            configList.Sort();
            File.WriteAllLines("config.txt", configList, Encoding.UTF8);
        }

        public static bool ConfigVerification()
        {
            foreach (var configString in ConfigStrings)
            {
                if (string.IsNullOrEmpty(GetConfig(configString))) return false;
            }
            return true;
        }
    }
}
