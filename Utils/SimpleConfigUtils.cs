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

        public static StringDictionary ReadConfigs()
        {
            List<string> configStrings = File.ReadAllLines(CONFIG).ToList();
            StringDictionary configs = new StringDictionary();
            foreach (var configString in configStrings)
            {
                if (configString.Contains(SPLITER))
                {
                    List<string> configKeyValue = configString.Split(SPLITER).ToList();
                    configs.Add(configKeyValue[0], configKeyValue[1]);
                }
            }
            return configs;
        }

        public static string GetConfig(string key)
        {
            return ReadConfigs()[key];
        }
    }
}
