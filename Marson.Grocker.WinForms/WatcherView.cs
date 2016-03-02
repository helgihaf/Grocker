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
    public partial class WatcherView : UserControl
    {
        private Watcher watcher;

        public WatcherView()
        {
            InitializeComponent();
        }

        public string DirectoryPath { get; set; }

        public List<ColorSchema<Color>> ColorSchemas { get; set; }

        private ILineWriter CreateWriter()
        {
            var writer = new RichTextBoxLineWriter(richTextBox);
            writer.ColorSchemas = ColorSchemas;
            return writer;
        }

        public void Start()
        {
            if (watcher != null)
            {
                if (watcher.IsStarted)
                {
                    watcher.Stop();
                }
                watcher.Dispose();
            }
            watcher = new Watcher();
            watcher.DirectoryPath = DirectoryPath;
            watcher.Filter = "*.log";
            watcher.LineWriter = CreateWriter();
            watcher.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Start();
        }
    }
}

