using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TfGuiTool.Utils;

namespace TfGuiTool
{
    /// <summary>
    /// Interaction logic for SettingWindow.xaml
    /// </summary>
    public partial class SettingWindow : Window
    {
        public SettingWindow()
        {
            InitializeComponent();
            textBoxTfPath.Text = SimpleConfigUtils.GetConfig("tf_executable_path");
            textBoxCollectionUrl.Text = SimpleConfigUtils.GetConfig("collection_url");
            textBoxWorkspace.Text = SimpleConfigUtils.GetConfig("workspace");
            textBoxUserName.Text = SimpleConfigUtils.GetConfig("user_name");
            textBoxPassword.Text = SimpleConfigUtils.GetConfig("password");
            textBoxProjectPath.Text = SimpleConfigUtils.GetConfig("project_path");
            textBoxTfsPath.Text = SimpleConfigUtils.GetConfig("tfs_path");
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxTfPath.Text.ToString())) { MessageBox.Show("TF.exe Path cannot be empty.", "Message"); return; }
            if (string.IsNullOrEmpty(textBoxCollectionUrl.Text.ToString())) { MessageBox.Show("Collection URL cannot be empty.", "Message"); return; }
            if (string.IsNullOrEmpty(textBoxWorkspace.Text.ToString())) { MessageBox.Show("Workspace cannot be empty.", "Message"); return; }
            if (string.IsNullOrEmpty(textBoxUserName.Text.ToString())) { MessageBox.Show("User Name cannot be empty.", "Message"); return; }
            if (string.IsNullOrEmpty(textBoxPassword.Text.ToString())) { MessageBox.Show("Password cannot be empty.", "Message"); return; }
            if (string.IsNullOrEmpty(textBoxProjectPath.Text.ToString())) { MessageBox.Show("Project Path cannot be empty.", "Message"); return; }
            if (string.IsNullOrEmpty(textBoxTfsPath.Text.ToString())) { MessageBox.Show("TFS Path cannot be empty.", "Message"); return; }

            StringDictionary configs = SimpleConfigUtils.ReadConfigs();
            if (configs.ContainsKey("tf_executable_path")) configs.Remove("tf_executable_path"); configs.Add("tf_executable_path", textBoxTfPath.Text.ToString());
            if (configs.ContainsKey("collection_url")) configs.Remove("collection_url"); configs.Add("collection_url", textBoxCollectionUrl.Text.ToString());
            if (configs.ContainsKey("workspace")) configs.Remove("workspace"); configs.Add("workspace", textBoxWorkspace.Text.ToString());
            if (configs.ContainsKey("user_name")) configs.Remove("user_name"); configs.Add("user_name", textBoxUserName.Text.ToString());
            if (configs.ContainsKey("password")) configs.Remove("password"); configs.Add("password", textBoxPassword.Text.ToString());
            if (configs.ContainsKey("project_path")) configs.Remove("project_path"); configs.Add("project_path", textBoxProjectPath.Text.ToString());
            if (configs.ContainsKey("tfs_path")) configs.Remove("tfs_path"); configs.Add("tfs_path", textBoxTfsPath.Text.ToString());

            List<string> configList = new List<string>();
            foreach (var configKey in configs.Keys)
            {
                configList.Add(configKey.ToString() + "," + configs[configKey.ToString()]);
            }
            configList.Sort();
            File.WriteAllLines("config.txt", configList);
            Close();
        }
    }
}
