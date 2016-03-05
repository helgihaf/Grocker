using Marson.Grocker.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Marson.Grocker.WinForms
{
    public partial class MainForm : Form, IWindowState
    {
        private readonly WindowStateManager windowStateManager;
        private readonly FolderDialog folderDialog = new FolderDialog();
        private List<ColorSchema> colorSchemas;

        public MainForm()
        {
            InitializeComponent();
            windowStateManager = new WindowStateManager(this, this);
            windowStateManager.RestoreWindowState();
            colorSchemas = LoadColorSchemas();
        }

        private Properties.Settings Settings
        {
            get
            {
                return Properties.Settings.Default;
            }
        }

        Rectangle IWindowState.WindowPosition
        {
            get
            {
                return Settings.WindowPosition;
            }

            set
            {
                Settings.WindowPosition = value;
            }
        }

        FormWindowState IWindowState.WindowState
        {
            get
            {
                return Settings.WindowState;
            }

            set
            {
                Settings.WindowState = value;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (Settings.FolderList == null)
            {
                Settings.FolderList = new System.Collections.Specialized.StringCollection();
            }
            folderDialog.FolderList = Settings.FolderList;
            PopulateRecentFolderMenu();
        }

        private void PopulateRecentFolderMenu()
        {
            for (int i = 0; i < Settings.FolderList.Count && i < 10; i++)
            {
                string folder = Settings.FolderList[i];
                var menuItem = new ToolStripMenuItem
                {
                    Text = string.Format("&{0} {1}", i + 1, folder),
                    Tag = folder,
                };
                menuItem.Click += RecentFolderMenuItemClick;
                openrecentToolStripMenuItem.DropDownItems.Add(menuItem);
            }
        }

        private static List<ColorSchema> LoadColorSchemas()
        {
            IColorSchemaSerializer serializer = GetSerializer();
            string filePath = ColorSchemaLocator.LocateSchemaFile();
            if (filePath == null)
            {
                return null;
            }
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return serializer.Deserialize(stream).ToList();
            }
        }

        private void OpenMenuItemClick(object sender, EventArgs e)
        {
            if (folderDialog.ShowDialog(this) == DialogResult.OK)
            {
                OpenTabPage(folderDialog.Folder);
            }
        }

        private void RecentFolderMenuItemClick(object sender, EventArgs e)
        {
            var menuItem = sender as ToolStripMenuItem;
            if (menuItem != null && !string.IsNullOrEmpty(menuItem.Tag as string))
            {
                OpenTabPage(menuItem.Tag as string);
            }
        }

        private void CloseMenuItemClick(object sender, EventArgs e)
        {
            int index = tabControl.SelectedIndex;
            if (index >= 0 && index < tabControl.TabPages.Count)
            {
                CloseTabPage(index);
            }
        }

        private async void OpenTabPage(string directoryPath)
        {
            TabPage page = new TabPage(Path.GetFileName(directoryPath));
            WatcherView view = new WatcherView();
            view.Name = "view";
            page.Controls.Add(view);
            page.Tag = view;
            view.Dock = DockStyle.Fill;
            view.DirectoryPath = directoryPath;
            view.ColorSchemas = colorSchemas;
            tabControl.TabPages.Add(page);
            tabControl.SelectedTab = page;
            try
            {
                await view.Start();
            }
            catch (Exception ex)
            {
                tabControl.TabPages.Remove(page);
                if (ex is ArgumentException || ex is IOException)
                {
                    ShowError("Error opening directory \"{0}\": {1}", directoryPath, ex.Message);
                }
                else
                {
                    throw;
                }
            }
        }

        private void CloseTabPage(int index)
        {
            var page = tabControl.TabPages[index];
            var view = page.Controls["view"] as WatcherView;
            if (view != null && view.IsStarted)
            {
                view.Stop();
            }
            tabControl.TabPages.RemoveAt(index);
        }

        private void ShowError(string format, params string[] args)
        {
            MessageBox.Show(this, string.Format(format, args), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static IColorSchemaSerializer GetSerializer()
        {
            return new XmlColorSchemaSerializer();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Settings.Save();
        }

        //// Handy serializing thingy
        //var sc1 = new ColorSchema();
        //sc1.Filters.Add(new ColorFilter { BackgroundColor = nameof(Color.White), ForegroundColor = nameof(Color.LightCyan) });
        //var sc = new ColorSchema[] { sc1 };
        //IColorSchemaSerializer serializer = GetSerializer();
        //using (var stream = new FileStream(@"c:\temp\color.xml", FileMode.Create))
        //{
        //    serializer.Serialize(sc, stream);
        //}
    }
}
