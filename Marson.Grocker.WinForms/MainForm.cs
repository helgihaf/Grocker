using Marson.Grocker.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
        private readonly string directoryPathFromArgs;
        private List<ColorSchema> colorSchemas;
        private WatcherView currentView;

        public MainForm() : this(null)
        {
        }

        public MainForm(string[] args)
        {
            InitializeComponent();
            windowStateManager = new WindowStateManager(this, this);
            windowStateManager.RestoreWindowState();
            colorSchemas = LoadColorSchemas();
            if (args != null && args.Length >= 1)
            {
                directoryPathFromArgs = args[0];
            }
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
            if (!string.IsNullOrEmpty(directoryPathFromArgs))
            {
                OpenCurrentView(directoryPathFromArgs);
            }
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
                OpenDirectoryPath(folderDialog.Folder);
            }
        }

        private void RecentFolderMenuItemClick(object sender, EventArgs e)
        {
            var menuItem = sender as ToolStripMenuItem;
            if (menuItem != null && !string.IsNullOrEmpty(menuItem.Tag as string))
            {
                OpenDirectoryPath(menuItem.Tag as string);
            }
        }

        private void CloseMenuItemClick(object sender, EventArgs e)
        {
            CloseCurrentView();
        }

        //private async void OpenTabPage(string directoryPath)
        //{
        //    TabPage page = new TabPage(Path.GetFileName(directoryPath));
        //    WatcherView view = new WatcherView();
        //    view.Name = "view";
        //    page.Controls.Add(view);
        //    page.Tag = view;
        //    view.Dock = DockStyle.Fill;
        //    view.DirectoryPath = directoryPath;
        //    view.ColorSchemas = colorSchemas;
        //    tabControl.TabPages.Add(page);
        //    tabControl.SelectedTab = page;
        //    try
        //    {
        //        await view.Start();
        //    }
        //    catch (Exception ex)
        //    {
        //        tabControl.TabPages.Remove(page);
        //        if (ex is ArgumentException || ex is IOException)
        //        {
        //            ShowError("Error opening directory \"{0}\": {1}", directoryPath, ex.Message);
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }
        //}

        private void OpenDirectoryPath(string directoryPath)
        {
            if (currentView == null)
            {
                OpenCurrentView(directoryPath);
            }
            else
            {
                OpenExternalView(directoryPath);
            }
        }

        private async void OpenCurrentView(string directoryPath)
        {
            Debug.Assert(currentView == null);

            WatcherView view = new WatcherView();
            view.Name = "view";
            view.Dock = DockStyle.Fill;
            view.DirectoryPath = directoryPath;
            view.ColorSchemas = colorSchemas;
            Controls.Add(view);
            try
            {
                await view.Start();
            }
            catch (Exception ex)
            {
                Controls.Remove(view);
                if (ex is ArgumentException || ex is IOException)
                {
                    ShowError("Error opening directory {0}: {1}", directoryPath, ex.Message);
                }
                else
                {
                    throw;
                }
            }
            currentView = view;
        }

 
        private void CloseCurrentView()
        {
            if (currentView != null)
            {
                if (currentView.IsStarted)
                {
                    currentView.Stop();
                }
                Controls.Remove(currentView);
            }
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

        private void OpenExternalView(string directoryPath)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = new Uri(assembly.CodeBase).LocalPath,
                Arguments = "\"" + directoryPath + "\"",
                UseShellExecute = false,
                WorkingDirectory = Environment.CurrentDirectory,
            };
            System.Diagnostics.Process.Start(startInfo);
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
