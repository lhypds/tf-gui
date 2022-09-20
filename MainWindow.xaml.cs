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
        private List<FileItem> FileList = new List<FileItem>();

        public MainWindow()
        {
            InitializeComponent();
            listViewFiles.ItemsSource = FileList;
            checkboxDragAndDropFileToCheckout.IsChecked = SimpleConfigUtils.GetConfig("drag_and_drop_to_checkout").Equals("true");
        }

        private void IsEnableAllControls(bool isEnable)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                buttonAdd.IsEnabled = isEnable;
                buttonChanges.IsEnabled = isEnable;
                buttonCheckin.IsEnabled = isEnable;
                buttonCheckout.IsEnabled = isEnable;
                buttonGet.IsEnabled = isEnable;
                buttonUndoSelect.IsEnabled = isEnable;
                buttonSettings.IsEnabled = isEnable;
                buttonUndoAll.IsEnabled = isEnable;
            }));
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
                if (!filePath.Contains(SimpleConfigUtils.GetConfig("project_path")))
                {
                    Status("File not in project path.");
                    return;
                }

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

            // Proform next step
            if (SimpleConfigUtils.GetConfig("drag_and_drop_to_checkout").Equals("true"))
            {
                if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    Add();
                }
                else
                {
                    Checkout();
                }
            }
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            Add();
        }

        private void Add()
        {
            if (!SimpleConfigUtils.ConfigVerification()) { MessageBox.Show("Please check settings.", "Message"); return; }
            Status("Adding files...");

            if (listViewFiles.Items.Count == 0) { Status("File list empty."); return; }

            IsEnableAllControls(false);
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                int checkoutCounter = 0;
                int failedCounter = 0;
                foreach (var file in FileList)
                {
                    string cmd = SimpleConfigUtils.GetConfig("tf_executable_path") + " add "
                        + "/login:" + SimpleConfigUtils.GetConfig("user_name") + "," + SimpleConfigUtils.GetConfig("password") + " ";
                    cmd += "\"" + file.Path + "\"";
                    CommandUtils.Run(cmd, out string output, out string error);
                    Debug.WriteLine(output);
                    if (error.Length > 0)
                    {
                        SimpleLogUtils.Write("ERROR: " + error);
                        Debug.WriteLine(error);
                    }

                    if (output.Contains(System.IO.Path.GetDirectoryName(file.Path) + ":\r\n" + file.Name))
                        checkoutCounter++;
                    else failedCounter++;
                }

                if (failedCounter > 0)
                    Status(checkoutCounter + " file(s) added, " + failedCounter + " file(s) error.");
                else Status(checkoutCounter + " file(s) added.");

                Thread.Sleep(600);  // avoid too fast
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    buttonChanges_Click(null, null);
                }));

                IsEnableAllControls(true);
            }).Start();
        }

        private void buttonUndoSelect_Click(object sender, RoutedEventArgs e)
        {
            UndoSelect();
        }

        private void UndoSelect()
        {
            if (!SimpleConfigUtils.ConfigVerification()) { MessageBox.Show("Please check settings.", "Message"); return; }
            Status("Undoing select file changes...");

            List<FileItem> selectedFiles = new List<FileItem>();
            foreach (var selectedItem in listViewFiles.SelectedItems)
            {
                selectedFiles.Add(selectedItem as FileItem);
            }
            if (listViewFiles.Items.Count == 0 || selectedFiles.Count == 0) { Status("No file selected."); return; }

            IsEnableAllControls(false);
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                int undoCounter = 0;
                foreach (var file in selectedFiles)
                {
                    string cmd = SimpleConfigUtils.GetConfig("tf_executable_path") + " undo "
                            + "/collection:" + SimpleConfigUtils.GetConfig("collection_url") + " "
                            + "/workspace:" + SimpleConfigUtils.GetConfig("workspace") + " "
                            + "/login:" + SimpleConfigUtils.GetConfig("user_name") + "," + SimpleConfigUtils.GetConfig("password") + " ";
                    cmd += "\"" + file.Path + "\"";
                    CommandUtils.Run(cmd, out string output);
                    Debug.WriteLine(output);

                    if (output.Contains("Undoing edit"))
                        undoCounter++;
                }

                Status(undoCounter + " file(s) undo.");
                Thread.Sleep(600);  // avoid too fast

                IsEnableAllControls(true);
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    buttonChanges_Click(null, null);
                }));
            }).Start();
        }

        private void buttonUndoAll_Click(object sender, RoutedEventArgs e)
        {
            UndoAll();
        }

        private void UndoAll()
        {
            if (!SimpleConfigUtils.ConfigVerification()) { MessageBox.Show("Please check settings.", "Message"); return; }
            Status("Undoing changes...");

            if (listViewFiles.Items.Count == 0) { Status("File list empty."); return; }

            IsEnableAllControls(false);
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
                    cmd += "\"" + file.Path + "\"";
                    CommandUtils.Run(cmd, out string output);
                    Debug.WriteLine(output);

                    if (output.Contains("Undoing edit"))
                        undoCounter++;
                }

                Status(undoCounter + " file(s) undo.");
                Thread.Sleep(600);  // avoid too fast

                IsEnableAllControls(true);
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    buttonChanges_Click(null, null);
                }));
            }).Start();
        }

        private void buttonCheckout_Click(object sender, RoutedEventArgs e)
        {
            Checkout();
        }

        private void Checkout()
        {
            if (!SimpleConfigUtils.ConfigVerification()) { MessageBox.Show("Please check settings.", "Message"); return; }
            Status("Checking out files...");

            if (listViewFiles.Items.Count == 0) { Status("File list empty."); return; }

            IsEnableAllControls(false);
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                int checkoutCounter = 0;
                int failedCounter = 0;
                foreach (var file in FileList)
                {
                    string cmd = SimpleConfigUtils.GetConfig("tf_executable_path") + " checkout "
                        + "/login:" + SimpleConfigUtils.GetConfig("user_name") + "," + SimpleConfigUtils.GetConfig("password") + " ";
                    cmd += "\"" + file.Path + "\"";
                    CommandUtils.Run(cmd, out string output, out string error);
                    Debug.WriteLine(output);
                    if (error.Length > 0)
                    {
                        SimpleLogUtils.Write("ERROR: " + error);
                        Debug.WriteLine(error);
                    }

                    if (output.Contains(System.IO.Path.GetDirectoryName(file.Path) + ":\r\n" + file.Name))
                        checkoutCounter++;
                    else failedCounter++;
                }

                if (failedCounter > 0)
                    Status(checkoutCounter + " file(s) checkout, " + failedCounter + " file(s) error.");
                else Status(checkoutCounter + " file(s) checkout.");

                Thread.Sleep(600);  // avoid too fast

                IsEnableAllControls(true);
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    buttonChanges_Click(null, null);
                }));
            }).Start();
        }

        private void buttonChanges_Click(object sender, RoutedEventArgs e)
        {
            Changes();
        }

        private void Changes()
        {
            if (!SimpleConfigUtils.ConfigVerification()) { MessageBox.Show("Please check settings.", "Message"); return; }
            Status("Loading changes...");

            FileList.Clear();
            listViewFiles.Items.Refresh();

            IsEnableAllControls(false);
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
                    Status("No pending changes.");
                    IsEnableAllControls(true);
                    return;
                }

                int fileEditCounter = 0;
                int fileAddCounter = 0;
                for (int i = 3; i < lines.Count; i++)
                {
                    string line = lines[i];
                    if (!line.Contains(" ! edit ") && !line.Contains(" ! add ")) continue;

                    List<string> buffer = new List<string>();
                    if (line.Contains(" ! edit "))
                    {
                        buffer = line.Split(" ! edit ").ToList();
                        fileEditCounter++;
                    }
                    else if (line.Contains(" ! add "))
                    {
                        buffer = line.Split(" ! add ").ToList();
                        fileAddCounter++;
                    }

                    string name = buffer[0].Trim();
                    string path = buffer[1].Trim();

                    // .Add can trigger the listViewFiles.Items.Refresh
                    FileList.Add(new FileItem()
                    {
                        Name = name,
                        Path = path,
                    });
                }

                Status(fileEditCounter + " file(s) edit, " + fileAddCounter + " file(s) add.");
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    listViewFiles.Items.Refresh();
                }));

                IsEnableAllControls(true);
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

        private void buttonGet_Click(object sender, RoutedEventArgs e)
        {
            Get();
        }

        private void Get()
        {
            if (!SimpleConfigUtils.ConfigVerification()) { MessageBox.Show("Please check settings.", "Message"); return; }
            Status("Getting latest code...");

            IsEnableAllControls(false);
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                // Get pending file changes
                string cmd = SimpleConfigUtils.GetConfig("tf_executable_path") + " get "
                    + SimpleConfigUtils.GetConfig("tfs_path") + " "
                    + "/recursive "
                    + "/login:" + SimpleConfigUtils.GetConfig("user_name") + "," + SimpleConfigUtils.GetConfig("password") + " ";
                CommandUtils.Run(cmd, out string output);
                Debug.WriteLine(output);

                int replacingCounter = 0;
                int deletingCounter = 0;
                List<string> lines = output.Split("\r\n").ToList();
                for (int i = 0; i < lines.Count; i++)
                {
                    string line = lines[i];
                    if (line.Contains("Replacing ")) replacingCounter++;
                    if (line.Contains("Deleting ")) deletingCounter++;
                }

                string statusString = "";
                if (output.Contains("All files are up to date."))
                {
                    statusString += "All files are up to date.";
                    statusString += " (" + replacingCounter + " replace, " + deletingCounter + " delete)";
                }
                else
                {
                    statusString += "Error";
                }
                Status(statusString);

                IsEnableAllControls(true);
            }).Start();
        }

        private void StatusBarItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (File.Exists("log.txt")) OpenFileWithDefaultEditor("log.txt");
        }

        private void checkboxDragAndDropFileToCheckout_Checked(object sender, RoutedEventArgs e)
        {
            SimpleConfigUtils.SetConfig("drag_and_drop_to_checkout", "true");
        }

        private void checkboxDragAndDropFileToCheckout_Unchecked(object sender, RoutedEventArgs e)
        {
            SimpleConfigUtils.SetConfig("drag_and_drop_to_checkout", "false");
        }

        private void listViewFiles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FileItem file = listViewFiles.SelectedItem as FileItem;
            if (file == null) return;
            if (Keyboard.Modifiers == ModifierKeys.Control) OpenFileWithDefaultEditor(file.Path);
            else Diff(file.Path);
        }

        private void Diff(string filePath)
        {
            if (!SimpleConfigUtils.ConfigVerification()) { MessageBox.Show("Please check settings.", "Message"); return; }
            Status("Differing file...");

            IsEnableAllControls(false);
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                // Get pending file changes
                string cmd = SimpleConfigUtils.GetConfig("tf_executable_path") + " diff "
                    + "\"" + filePath + "\" "
                    + "/login:" + SimpleConfigUtils.GetConfig("user_name") + "," + SimpleConfigUtils.GetConfig("password") + " ";
                CommandUtils.Run(cmd, out string output);
                Debug.WriteLine(output);
                File.WriteAllText("diff_result.diff", output);
                Status("File diff exported.");

                IsEnableAllControls(true);
                OpenFileWithDefaultEditor("diff_result.diff");
            }).Start();
        }

        private void OpenFileWithDefaultEditor(string filePath)
        {
            using Process fileopener = new Process();
            string defaultEditor = "";
            switch (SimpleConfigUtils.GetConfig("default_text_editor"))
            {
                case "0": defaultEditor = "explorer"; break;
                case "1": defaultEditor = "notepad"; break;
                case "2": defaultEditor = Const.VSCODE_PATH; break;
                case "3": defaultEditor = Const.SUBLIME_TEXT_3_PATH; break;
                case "4": defaultEditor = Const.VIM_PATH; break;
                default: defaultEditor = "explorer"; break;
            }
            fileopener.StartInfo.FileName = defaultEditor;
            fileopener.StartInfo.Arguments = "\"" + filePath + "\"";
            fileopener.Start();
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
