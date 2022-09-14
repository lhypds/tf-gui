using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
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
        private List<FileItem> _FileList = new List<FileItem>();

        public List<FileItem> FileList
        { 
            get { return _FileList; } 
            set { _FileList = value; }
        }
        
        private FileItem _SelectedFile;

        public MainWindow()
        {
            InitializeComponent();
            buttonChanges.ToolTip = "buttonChanges";
            buttonCheckin.ToolTip = "buttonCheckin";
            buttonCheckout.ToolTip = "buttonCheckout";
            buttonUndoAll.ToolTip = "buttonUndoAll";

            _FileList.Add(new FileItem() { Name = "test", Path = "Path" });
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
                if (_FileList.Find(f => f.Path == filePath) == null)
                {
                    _FileList.Add(new FileItem()
                    {
                        Name = System.IO.Path.GetFileName(filePath),
                        Path = filePath
                    });
                }
            }
            labelStatus.Text = "File list updated.";
        }
    }

    public class FileItem : INotifyPropertyChanged
    {
        private string _Name;

        public string Name
        {
            get {return _Name;  }
            set
            {
                _Name = value;
                OnPropertyChanged("Name");
            }
        }

        private string _Path;

        public string Path
        {
            get { return _Path; }
            set
            {
                _Path = value;
                OnPropertyChanged("Path");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public override string ToString()
        {
            return this.Name.ToString() + " path:" + this.Path.ToString();
        }
    }
}
