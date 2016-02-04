using Marson.Grocker.Common;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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

namespace GrockerWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuFileExit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MenuFileOpen(object sender, RoutedEventArgs e)
        {
            var openFileDialog = GetOpenFileDialog();
            bool? ok = openFileDialog.ShowDialog(this);
            if (ok.HasValue && ok.Value)
            {
                OpenFile(openFileDialog.FileName);
            }
        }

        private void OpenFile(string filePath)
        {
            string fileName = System.IO.Path.GetFileName(filePath);
            var newTab = new TabItem();
            newTab.Header = fileName;
            int newTabIndex = tabControl.Items.Add(newTab);
            var logControl = new LogControl2();
            newTab.Content = logControl;
            tabControl.SelectedIndex = newTabIndex;
            logControl.LoadData(filePath);
        }

        //private void OpenFileOld(string filePath)
        //{
        //    Task<LogFile> logFileTask = LogFile.LoadFromAsync(filePath);
        //    string fileName = System.IO.Path.GetFileName(filePath);
        //    var newTab = new TabItem();
        //    newTab.Header = fileName;
        //    int newTabIndex = tabControl.Items.Add(newTab);
        //    var logControl = new LogControl2();
        //    newTab.Content = logControl;
        //    tabControl.SelectedIndex = newTabIndex;
        //    logFileTask.Wait();
        //    logControl.LoadData(logFileTask.Result);
        //}

        private OpenFileDialog GetOpenFileDialog()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Log files (*.log)|*.log|All files|*.*";
            dialog.DefaultExt = "log";
            return dialog;
        }
    }
}
