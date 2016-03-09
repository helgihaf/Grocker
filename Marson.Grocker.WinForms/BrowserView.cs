using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Marson.Grocker.Common;

namespace Marson.Grocker.WinForms
{
    public partial class BrowserView : UserControl
    {
        private LogFile logFile;
        private IList<string> lineList;

        public BrowserView()
        {
            InitializeComponent();
        }

        public void Open(string logFilePath)
        {
            logFile = LogFile.LoadFrom(logFilePath);
            int pageSize = GetVisibleLines();
            if (pageSize <= 1)
            {
                pageSize = 20;
            }
            lineList = new VirtualizingCollection<string>(new LogItemsProvider(logFile), pageSize);
            listBox.DataSource = lineList;
        }

        private int GetVisibleLines()
        {
            return listBox.ClientSize.Height / listBox.ItemHeight;
        }

        private void listBox_Resize(object sender, EventArgs e)
        {

        }
    }
}
