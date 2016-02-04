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
    /// Interaction logic for LogControl.xaml
    /// </summary>
    public partial class LogControl : UserControl
    {
        private LogFile logFile;
        private LogWindow logWindow;

        public LogControl()
        {
            InitializeComponent();
        }

        public void LoadData(LogFile logFile)
        {
            this.logFile = logFile;
        }

        private void UpdateLogWindow(int? newIndex = null)
        {
            if (logWindow == null)
            {
                logWindow = logFile.CreateWindow();
                logWindow.Length = GetVisibleLineCount();
                logWindow.LastPage();
            }
            else
            {
                if (!newIndex.HasValue)
                {
                    newIndex = logWindow.CurrentIndex;
                }
                logWindow.Length = GetVisibleLineCount();
                logWindow.CurrentIndex = newIndex.Value;
            }

            UpdateScrollBar();
            UpdateText();
        }

        private void UpdateScrollBar()
        {
            scrollBar.Minimum = 0;
            scrollBar.Maximum = logFile.Lines.Count() - 1;
            scrollBar.SmallChange = 1;
            scrollBar.LargeChange = GetVisibleLineCount();
            scrollBar.Value = logWindow.CurrentIndex;
        }

        private void UpdateText()
        {
            textBox.Text = "";
            foreach (var line in logWindow.Lines)
            {
                textBox.AppendText(line + System.Environment.NewLine);
            }
        }

        private int GetVisibleLineCount()
        {
            return Convert.ToInt32(textBox.ActualHeight / textBox.FontSize);
        }


        private void textBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            bool shouldUpdateLogWindow =
                logFile != null &&
                (e.PreviousSize.IsEmpty || e.PreviousSize.Height == double.NaN || e.PreviousSize.Height == 0) &&
                !e.NewSize.IsEmpty && e.NewSize.Height > 0;

            if (shouldUpdateLogWindow)
            {
                UpdateLogWindow();
            }
        }

        private void scrollBar_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            UpdateLogWindow(Convert.ToInt32(e.NewValue));
        }
    }
}
