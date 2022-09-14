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
            textBoxTfPath.Text = SampleConfigUtils.GetConfig("tf_executable_path");
            textBoxCollectionUrl.Text = SampleConfigUtils.GetConfig("collection_url");
            textBoxWorkspace.Text = SampleConfigUtils.GetConfig("workspace");
            textBoxProjectPath.Text = SampleConfigUtils.GetConfig("project_path");
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            File.Delete("config.txt");
            File.Create("config.txt").Close();
            List<string> configStrings = new List<string>();
            configStrings.Add("tf_executable_path" + "," + textBoxTfPath.Text.ToString());
            configStrings.Add("collection_url" + "," + textBoxCollectionUrl.Text.ToString());
            configStrings.Add("workspace" + "," + textBoxWorkspace.Text.ToString());
            configStrings.Add("project_path" + "," + textBoxProjectPath.Text.ToString());
            File.AppendAllLines("config.txt", configStrings);
            Close();
        }
    }
}
