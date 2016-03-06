using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Marson.Grocker.WinForms
{
    public partial class DetachedForm : Form
    {
        public DetachedForm()
        {
            InitializeComponent();
        }

        public IWatcherViewHost AttachHost { get; set; }

        private void buttonAttach_Click(object sender, EventArgs e)
        {
            if (AttachHost != null)
            {
                var watcherView = WatcherView;
                if (watcherView != null)
                {
                    Controls.Remove(watcherView);
                    AttachHost.Attach(watcherView);
                }
            }
            Close();
        }

        private WatcherView WatcherView
        {
            get
            {
                return Controls["view"] as WatcherView;
            }
        }

        private void DetachedForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var watcherView = WatcherView;
            if (watcherView != null && watcherView.IsStarted)
            {
                watcherView.Stop();
            }
        }
    }
}
