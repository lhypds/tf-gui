using System;
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
        };

        public static StringDictionary ReadConfigs()
        {
            StringDictionary configs = new StringDictionary();
            if (File.Exists(CONFIG))
            {
                List<string> configStrings = File.ReadAllLines(CONFIG).ToList();
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
            return ReadConfigs()[key];
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
