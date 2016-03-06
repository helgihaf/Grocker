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
using System.Diagnostics;

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

        public List<ColorSchema> ColorSchemas { get; set; }

        public bool IsStarted
        {
            get
            {
                return watcher != null;
            }
        }

        public async Task Start()
        {
            if (watcher != null)
            {
                throw new InvalidOperationException("Watcher already started");
            }

            UpdateActions();
            watcher = await StartNewWatcherAsync();
            UpdateActions();
        }

        public void Stop()
        {
            if (watcher == null)
            {
                throw new InvalidOperationException("Watcher is not started");
            }
            if (watcher.IsStarted)
            {
                watcher.Stop();
            }
            watcher.Dispose();
            watcher = null;
        }

        private Task<Watcher> StartNewWatcherAsync()
        {
            return Task.Factory.StartNew<Watcher>(() => StartNewWatcher());
        }

        private Watcher StartNewWatcher()
        {
            var newWatcher = new Watcher();
            try
            {
                newWatcher.DirectoryPath = DirectoryPath;
                newWatcher.Filter = "*.log";
                newWatcher.LineWriter = CreateWriter();
                newWatcher.FileFound += WatcherFileFound;
                newWatcher.Start();
            }
            catch
            {
                try
                {
                    newWatcher.Dispose();
                }
                catch
                {
                }
                throw;
            }
            return newWatcher;
        }

        private void WatcherFileFound(object sender, FilePathEventArgs e)
        {
            RunOnMainThread(() => textBoxFilePath.Text = e.FilePath);
        }

        private void RunOnMainThread(Action action)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { action(); });
            }
            else
            {
                action();
            }
        }

        private void UpdateActions()
        {
            buttonPause.Visible = watcher == null || (watcher != null && watcher.IsStarted);
            buttonPause.Enabled = watcher != null;

            buttonPlay.Visible = watcher != null && !watcher.IsStarted;
        }

        private ILineWriter CreateWriter()
        {
            var writer = new RichTextBoxLineWriter(richTextBox, colorFilterSummary);
            writer.ColorSchemas = ColorSchemas;
            return writer;
        }

        private void buttonPause_Click(object sender, EventArgs e)
        {
            watcher.Stop();
            UpdateActions();
        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            watcher.Start();
            UpdateActions();
        }

    }
}

