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
        List<FileItem> FileList = new List<FileItem>();

        public CheckinWindow(List<FileItem> fileList)
        {
            InitializeComponent();
            FileList = fileList;
        }

        private void buttonCheckin_Click(object sender, RoutedEventArgs e)
        {
            string cmd = SimpleConfigUtils.GetConfig("tf_executable_path") + " checkin ";
                +"/login:" + SimpleConfigUtils.GetConfig("user_name") + "," + SampleConfigUtils.GetConfig("password") + " ";

            foreach (var file in FileList)
            {
                cmd += file.Path + " ";
            }
            cmd += "/comment:\"" + textBoxComment.Text.ToString() + "\" "
                + "/noprompt";
            CommandUtils.Run(cmd, out string output);
            Debug.WriteLine(output);
            Close();
        }
    }
}
