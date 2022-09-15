using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TfGuiTool.Utils;

namespace TfGuiTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<FileItem> _FileList = new List<FileItem>();

        private List<FileItem> FileList
        {
            get { return _FileList; }
            set { 
                _FileList = value;
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    listViewFiles.Items.Refresh();
                }));
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            listViewFiles.ItemsSource = FileList;
        }

        private void buttonSettings_Click(object sender, RoutedEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control) 
            {
                if (File.Exists("config.txt"))
                    Process.Start("notepad.exe", "config.txt");
                return;
            }

            SettingWindow settingWindow = new SettingWindow();
            settingWindow.Owner = this;
            settingWindow.ShowDialog();
        }

        private void listViewFiles_Drop(object sender, DragEventArgs e)
        {
            string[] filePathList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach (var filePath in filePathList)
            {
                if (FileList.Find(f => f.Path == filePath) == null)
                {
                    FileList.Add(new FileItem()
                    {
                        Name = System.IO.Path.GetFileName(filePath),
                        Path = filePath
                    });
                    Status("File(s) added.");
                }
            }
            listViewFiles.Items.Refresh();
        }

        private void buttonUndoAll_Click(object sender, RoutedEventArgs e)
        {
            Status("Undoing changes...");
            if (!SimpleConfigUtils.ConfigVerification()) { MessageBox.Show("Please check settings.", "Message"); return; }
            if (listViewFiles.Items.Count == 0) { Status("File list empty."); return; }

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                int undoCounter = 0;
                foreach (var file in FileList)
                {
                    string cmd = SimpleConfigUtils.GetConfig("tf_executable_path") + " undo "
                        + "/collection:" + SimpleConfigUtils.GetConfig("collection_url") + " "
                        + "/workspace:" + SimpleConfigUtils.GetConfig("workspace") + " "
                        + "/login:" + SimpleConfigUtils.GetConfig("user_name") + "," + SimpleConfigUtils.GetConfig("password") + " ";
                    cmd += file.Path;
                    CommandUtils.Run(cmd, out string output);
                    Debug.WriteLine(output);

                    if (output.Contains("Undoing edit"))
                        undoCounter++;
                }
                
                Status(undoCounter + " file(s) undo.");
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    buttonChanges_Click(null, null);
                }));
            }).Start();
        }

        private void buttonCheckout_Click(object sender, RoutedEventArgs e)
        {
            Status("Checking out files...");
            if (!SimpleConfigUtils.ConfigVerification()) { MessageBox.Show("Please check settings.", "Message"); return; }
            if (listViewFiles.Items.Count == 0) { Status("File list empty."); return; }

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                int checkoutCounter = 0;
                foreach (var file in FileList)
                {
                    string cmd = SimpleConfigUtils.GetConfig("tf_executable_path") + " checkout "
                        + "/login:" + SimpleConfigUtils.GetConfig("user_name") + "," + SimpleConfigUtils.GetConfig("password") + " ";
                    cmd += file.Path;
                    CommandUtils.Run(cmd, out string output);
                    Debug.WriteLine(output);

                    if (output.Contains(file.Name))
                        checkoutCounter++;
                }
                Status(checkoutCounter + " file(s) checkout.");
            }).Start();
        }

        private void buttonChanges_Click(object sender, RoutedEventArgs e)
        {
            Status("Loading changes...");
            if (!SimpleConfigUtils.ConfigVerification()) { MessageBox.Show("Please check settings.", "Message"); return; }
            FileList.Clear();
            listViewFiles.Items.Refresh();

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                // Get pending file changes
                string cmd = SimpleConfigUtils.GetConfig("tf_executable_path") + " stat "
                    + "/collection:" + SimpleConfigUtils.GetConfig("collection_url") + " "
                    + "/workspace:" + SimpleConfigUtils.GetConfig("workspace") + " "
                    + "/login:" + SimpleConfigUtils.GetConfig("user_name") + "," + SimpleConfigUtils.GetConfig("password") + " ";
                CommandUtils.Run(cmd, out string output);
                Debug.WriteLine(output);

                List<string> lines = output.Split("\r\n").ToList();
                if (lines[0].Contains("There are no pending changes."))
                {
                    Status("No pending changes detected.");
                    return;
                }

                int fileChangeCounter = 0;
                for (int i = 3; i < lines.Count; i++)
                {
                    string line = lines[i];
                    if (!line.Contains(" ! edit ")) continue;

                    List<string> buffer = line.Split(" ! edit ").ToList();
                    string name = buffer[0].Trim();
                    string path = buffer[1].Trim();
                    FileList.Add(new FileItem()
                    {
                        Name = name,
                        Path = path,
                    });
                    fileChangeCounter++;
                }
                Status(fileChangeCounter + " file(s) pending changes detected.");
            }).Start();
        }

        private void buttonCheckin_Click(object sender, RoutedEventArgs e)
        {
            if (!SimpleConfigUtils.ConfigVerification()) { MessageBox.Show("Please check settings.", "Message"); return; }
            if (FileList.Count == 0)
            {
                Status("File list empty.");
                return;
            }

            CheckinWindow checkinWindow = new CheckinWindow(FileList);
            checkinWindow.Owner = this;
            checkinWindow.Closed += (s, e) => { buttonChanges_Click(null, null); };
            checkinWindow.ShowDialog();
        }

        private void Status(string status)
        {
            Dispatcher.BeginInvoke(new Action(() => 
            {
                this.labelStatus.Text = status;
            }));
        }
    }

    public class FileItem
    {
        public string Name;

        public string Path;

        public override string ToString()
        {
            return this.Name + " | " + Path.Replace(SimpleConfigUtils.GetConfig("project_path"), "");
        }
    }
}
