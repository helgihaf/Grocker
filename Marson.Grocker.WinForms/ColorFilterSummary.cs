using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Marson.Grocker.WinForms
{
    public partial class ColorFilterSummary : UserControl, IFilterCounter
    {
        public ColorFilterSummary()
        {
            InitializeComponent();
        }

        public void Increment(string filterName)
        {
            ListViewItem item = FindItem(filterName);
            if (item == null)
            {
                item = new ListViewItem();
                item.Text = filterName;
                item.SubItems.Add("0");
                listView.Items.Add(item);
            }
            SetCounter(item, GetCounter(item) + 1);
        }

        private ListViewItem FindItem(string filterName)
        {
            foreach (ListViewItem item in listView.Items)
            {
                if (item.Text == filterName)
                {
                    return item;
                }
            }
            return null;
        }

        private void toolStripButtonClear_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView.Items)
            {
                SetCounter(item, 0);
            }
        }

        private void SetCounter(ListViewItem item, int value)
        {
            item.SubItems[1].Text = value.ToString();
        }

        private int GetCounter(ListViewItem item)
        {
            return int.Parse(item.SubItems[1].Text);
        }

    }
}
