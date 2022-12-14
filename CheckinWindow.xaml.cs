using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
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

        private void IsEnableAllControls(bool isEnable)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                buttonCheckin.IsEnabled = isEnable;
            }));
        }

        private void buttonCheckin_Click(object sender, RoutedEventArgs e)
        {
            Checkin();
        }

        private void Checkin()
        {
            if (!SimpleConfigUtils.ConfigVerification()) { MessageBox.Show("Please check settings.", "Message"); return; }

            string checkinComment = textBoxComment.Text.ToString();

            IsEnableAllControls(false);
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                string cmd = SimpleConfigUtils.GetConfig("tf_executable_path") + " checkin "
                + "/login:" + SimpleConfigUtils.GetConfig("user_name") + "," + SimpleConfigUtils.GetConfig("password") + " ";

                foreach (var file in FileList)
                {
                    cmd += "\"" + file.Path + "\"" + " ";
                }
                cmd += "/comment:\"" + checkinComment + "\" "
                    + "/noprompt";
                CommandUtils.Run(cmd, out string output);
                SimpleLogUtils.Write(output);
                Debug.WriteLine(output);

                IsEnableAllControls(true);
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (output.Contains("checked in"))
                    {
                        labelCheckinResult.Content = "Check-in successfully.";
                        Thread.Sleep(600);  // avoid too fast
                        Close();
                    }
                    else
                    {
                        labelCheckinResult.Content = "Check-in failed.";
                    }
                }));
            }).Start();
        }

        private void textBoxComment_KeyUp(object sender, KeyEventArgs e)
        {
            // Ctrl enter to checkin
            if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Checkin();
            }
        }
    }
}
