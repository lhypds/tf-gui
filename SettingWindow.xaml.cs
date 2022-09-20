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

            StringDictionary configs = SimpleConfigUtils.ReadConfigs();
            if (configs.ContainsKey("tf_executable_path")) textBoxTfPath.Text = configs["tf_executable_path"];
            if (configs.ContainsKey("collection_url")) textBoxCollectionUrl.Text = configs["collection_url"];
            if (configs.ContainsKey("workspace")) textBoxWorkspace.Text = configs["workspace"];
            if (configs.ContainsKey("user_name")) textBoxUserName.Text = configs["user_name"];
            if (configs.ContainsKey("password")) textBoxPassword.Text = configs["password"];
            if (configs.ContainsKey("project_path")) textBoxProjectPath.Text = configs["project_path"];
            if (configs.ContainsKey("tfs_path")) textBoxTfsPath.Text = configs["tfs_path"];

            // 0 = system default, 1 = notepad, 2 = vscode, 3 = sublime text, 4 = vim
            if (configs.ContainsKey("default_text_editor")) comboBoxDefaultEditor.SelectedIndex = int.Parse(configs["default_text_editor"]); else comboBoxDefaultEditor.SelectedIndex = 0;
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            switch (comboBoxDefaultEditor.SelectedIndex)
            {
                case 2: if (!File.Exists(Const.VSCODE_PATH)) { MessageBox.Show("VS Code not found in your system.", "Message"); comboBoxDefaultEditor.SelectedIndex = 0; return; } break;
                case 3: if (!File.Exists(Const.SUBLIME_TEXT_3_PATH)) { MessageBox.Show("Sublime Text 3 not found in your system.", "Message"); comboBoxDefaultEditor.SelectedIndex = 0; return; } break;
                case 4: if (!File.Exists(Const.VIM_PATH)) { MessageBox.Show("Vim not found in your system.", "Message"); comboBoxDefaultEditor.SelectedIndex = 0; return; } break;
                default: break;
            }

            StringDictionary configs = SimpleConfigUtils.ReadConfigs();

            if (!string.IsNullOrEmpty(textBoxTfPath.Text.ToString()))
            {
                if (configs.ContainsKey("tf_executable_path")) configs.Remove("tf_executable_path");
                configs.Add("tf_executable_path", textBoxTfPath.Text.ToString());
            }

            if (!string.IsNullOrEmpty(textBoxCollectionUrl.Text.ToString()))
            {
                if (configs.ContainsKey("collection_url")) configs.Remove("collection_url");
                configs.Add("collection_url", textBoxCollectionUrl.Text.ToString());
            }

            if (!string.IsNullOrEmpty(textBoxWorkspace.Text.ToString()))
            {
                if (configs.ContainsKey("workspace")) configs.Remove("workspace");
                configs.Add("workspace", textBoxWorkspace.Text.ToString());
            }

            if (!string.IsNullOrEmpty(textBoxUserName.Text.ToString()))
            {
                if (configs.ContainsKey("user_name")) configs.Remove("user_name");
                configs.Add("user_name", textBoxUserName.Text.ToString());
            }

            if (!string.IsNullOrEmpty(textBoxPassword.Text.ToString()))
            {
                if (configs.ContainsKey("password")) configs.Remove("password");
                configs.Add("password", textBoxPassword.Text.ToString());
            }

            if (!string.IsNullOrEmpty(textBoxProjectPath.Text.ToString()))
            {
                if (configs.ContainsKey("project_path")) configs.Remove("project_path");
                configs.Add("project_path", textBoxProjectPath.Text.ToString());
            }

            if (!string.IsNullOrEmpty(textBoxTfsPath.Text.ToString()))
            {
                if (configs.ContainsKey("tfs_path")) configs.Remove("tfs_path");
                configs.Add("tfs_path", textBoxTfsPath.Text.ToString());
            }

            if (!string.IsNullOrEmpty(comboBoxDefaultEditor.SelectedIndex.ToString()))
            {
                if (configs.ContainsKey("default_text_editor")) configs.Remove("default_text_editor");
                configs.Add("default_text_editor", comboBoxDefaultEditor.SelectedIndex.ToString());
            }

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
