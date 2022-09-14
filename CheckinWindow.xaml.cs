using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for CheckinWindow.xaml
    /// </summary>
    public partial class CheckinWindow : Window
    {
        public CheckinWindow()
        {
            InitializeComponent();
        }

        private void buttonCheckin_Click(object sender, RoutedEventArgs e)
        {
            string cmd = SampleConfigUtils.GetConfig("tf_executable_path") + " checkin "
                + "/comment:\"" + textBoxComment.Text.ToString() + "\" "
                + "/collection:" + SampleConfigUtils.GetConfig("collection_url") + " "
                + "/workspace:" + SampleConfigUtils.GetConfig("workspace");
            CommandUtils.Run(cmd, out string output);
            Debug.WriteLine(output);
            Close();
        }
    }
}
