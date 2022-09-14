﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
using TfGuiTool.Utils;

namespace TfGuiTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<FileItem> FileList = new List<FileItem>();
        
        private FileItem _SelectedFile;

        public MainWindow()
        {
            InitializeComponent();
            listViewFiles.ItemsSource = FileList;
        }

        private void buttonSettings_Click(object sender, RoutedEventArgs e)
        {
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
                    labelStatus.Text = "File(s) added.";
                }
            }
            listViewFiles.Items.Refresh();
        }

        private void buttonUndoAll_Click(object sender, RoutedEventArgs e)
        {
            int undoCounter = 0;
            foreach (var file in FileList)
            {
                string cmd = SampleConfigUtils.GetConfig("tf_executable_path") + " undo "
                + "/collection:" + SampleConfigUtils.GetConfig("collection_url") + " "
                + "/workspace:" + SampleConfigUtils.GetConfig("workspace") + " ";
                cmd += file.Path;
                CommandUtils.Run(cmd, out string output);
                Debug.WriteLine(output);

                if (output.Contains("Undoing edit"))
                    undoCounter++;
            }
            labelStatus.Text = undoCounter + " file(s) undo.";
        }

        private void buttonCheckout_Click(object sender, RoutedEventArgs e)
        {
            int checkoutCounter = 0;
            foreach (var file in FileList)
            {
                string cmd = SampleConfigUtils.GetConfig("tf_executable_path") + " checkout ";
                cmd += file.Path;
                CommandUtils.Run(cmd, out string output);
                Debug.WriteLine(output);

                if (output.Contains(file.Name))
                    checkoutCounter++;
            }
            labelStatus.Text = checkoutCounter + " file(s) checkout.";
        }

        private void buttonChanges_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonCheckin_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    public class FileItem
    {
        public string Name;

        public string Path;

        public override string ToString()
        {
            return this.Name + ", " + Path.Replace(SampleConfigUtils.GetConfig("project_path"), "");
        }
    }
}
