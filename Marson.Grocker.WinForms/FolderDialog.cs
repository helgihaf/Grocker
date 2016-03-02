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
    public partial class FolderDialog : Form
    {
        public FolderDialog()
        {
            InitializeComponent();
        }

        public string Folder
        {
            get
            {
                return comboBoxFolder.Text;
            }
            set
            {
                comboBoxFolder.Text = value;
            }
        }

        private void FolderDialog_Load(object sender, EventArgs e)
        {
            UpdateActions();
        }

        private void UpdateActions()
        {
            buttonOk.Enabled = comboBoxFolder.Text.Length > 0;
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.SelectedPath = comboBoxFolder.Text;
            if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
            {
                comboBoxFolder.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            AddComboTextIfNew(comboBoxFolder);
            DialogResult = DialogResult.OK;
        }

        private void AddComboTextIfNew(ComboBox comboBox)
        {
            bool found = false;
            foreach (string folder in comboBox.Items)
            {
                if (comboBox.Text.Equals(folder, StringComparison.CurrentCultureIgnoreCase))
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                comboBox.Items.Insert(0, comboBox.Text);
            }
        }

        private void comboBoxFolder_TextChanged(object sender, EventArgs e)
        {
            UpdateActions();
        }
    }
}
