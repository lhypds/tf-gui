using System;
using System.Collections.Generic;
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
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxTfPath.Text.ToString())) { MessageBox.Show("TF.exe Path cannot be empty.", "Message"); return; }
            if (string.IsNullOrEmpty(textBoxCollectionUrl.Text.ToString())) { MessageBox.Show("Collection URL cannot be empty.", "Message"); return; }
            if (string.IsNullOrEmpty(textBoxWorkspace.Text.ToString())) { MessageBox.Show("Workspace cannot be empty.", "Message"); return; }
            if (string.IsNullOrEmpty(textBoxUserName.Text.ToString())) { MessageBox.Show("User Name cannot be empty.", "Message"); return; }
            if (string.IsNullOrEmpty(textBoxPassword.Text.ToString())) { MessageBox.Show("Password cannot be empty.", "Message"); return; }
            if (string.IsNullOrEmpty(textBoxProjectPath.Text.ToString())) { MessageBox.Show("Project Path cannot be empty.", "Message"); return; }

            File.Delete("config.txt");
            File.Create("config.txt").Close();
            List<string> configStrings = new List<string>();
            configStrings.Add("tf_executable_path" + "," + textBoxTfPath.Text.ToString());
            configStrings.Add("collection_url" + "," + textBoxCollectionUrl.Text.ToString());
            configStrings.Add("workspace" + "," + textBoxWorkspace.Text.ToString());
            configStrings.Add("user_name" + "," + textBoxUserName.Text.ToString());
            configStrings.Add("password" + "," + textBoxPassword.Text.ToString());
            configStrings.Add("project_path" + "," + textBoxProjectPath.Text.ToString());
            File.AppendAllLines("config.txt", configStrings);
            Close();
        }
    }
}
