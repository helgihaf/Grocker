using Marson.Grocker.Common;
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
    /// Interaction logic for LogControl2.xaml
    /// </summary>
    public partial class LogControl2 : UserControl
    {
        const int PageSize = 100;
        const int PageTimeout = 10;

        public LogControl2()
        {
            InitializeComponent();
        }

        public async void LoadData(string logFilePath)
        {
            SetStatusCurrentLine(null);
            SetStatusTotalLines(null);
            SetStatusProgress(true);
            SetStatusMessage("Processing file...");
            LogFile logFile = await LogFile.LoadFromAsync(logFilePath);
            var gridView = listView.View as GridView;
            gridView.Columns[1].Width = logFile.MaxLineLength * listView.FontSize;

            var provider = new LogLinesItemProvider(logFile);
            DataContext = new VirtualizingCollection<ModelLine>(provider, PageSize, PageTimeout * 1000);

            SetStatusTotalLines(logFile.Lines.Count);
            SetStatusProgress(false);
            SetStatusMessage("Ready");
        }

        private void SetStatusCurrentLine(int? number)
        {
            textBlockLine.Text = "Line: " + (number.HasValue ? number.Value.ToString() : "-");
        }

        private void SetStatusTotalLines(int? number)
        {
            textBlockTotalLines.Text = "Total: " + (number.HasValue ? number.Value.ToString() : "-");
        }

        private void SetStatusProgress(bool visible)
        {
            if (visible)
            {
                progressBar.IsIndeterminate = true;
                progressBar.Visibility = Visibility.Visible;
            }
            else
            {
                progressBar.Visibility = Visibility.Hidden;
            }
        }

        private void SetStatusMessage(string text)
        {
            textBlockMessage.Text = text;
        }



        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var modelLine = e.AddedItems.OfType<ModelLine>().FirstOrDefault();
            SetStatusCurrentLine(modelLine != null ? modelLine.Index : (int?)null);
        }

        private void listView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //ListView lv = sender as ListView;
            //GridView gridView = lv.View as GridView;
            //for (int i = 0; i < gridView.Columns.Count - 1; i++)
            //{
            //    var textBlock = (TextBlock)gridView.Columns[i].CellTemplate.LoadContent();
            //    gridView.Columns[i].Width = textBlock.Width;
            //}
            //gridView.Columns[gridView.Columns.Count - 1].Width = actualWidth;
        }
    }
}
