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
    public partial class MainForm : Form
    {
        private FolderDialog folderDialog = new FolderDialog();
        private List<ColorSchema> colorSchemas;

        public MainForm()
        {
            InitializeComponent();
            colorSchemas = LoadColorSchemas();
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
            string filePath = LocateSchemaFile();
            if (filePath == null)
            {
                return null;
            }
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return serializer.Deserialize(stream).ToList();
            }
        }

        private Properties.Settings Settings
        {
            get
            {
                return Properties.Settings.Default;
            }
        }

        private void OpenMenuItemClick(object sender, EventArgs e)
        {
            if (folderDialog.ShowDialog(this) == DialogResult.OK)
            {
                AddMonitor(folderDialog.Folder);
            }
        }

        private void RecentFolderMenuItemClick(object sender, EventArgs e)
        {
            var menuItem = sender as ToolStripMenuItem;
            if (menuItem != null && !string.IsNullOrEmpty(menuItem.Tag as string))
            {
                AddMonitor(menuItem.Tag as string);
            }
        }


        private async void AddMonitor(string directoryPath)
        {
            TabPage page = new TabPage(directoryPath);
            WatcherView view = new WatcherView();
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

        private void ShowError(string format, params string[] args)
        {
            MessageBox.Show(this, string.Format(format, args), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static string LocateSchemaFile()
        {
            const string fileName = "ColorSchemas.Grocker.xml";

            // 1. Try current path
            if (File.Exists(fileName))
            {
                return Path.Combine(Environment.CurrentDirectory, fileName);
            }

            // 2. Try app.config
            string filePath = System.Configuration.ConfigurationManager.AppSettings["colorSchemasFilePath"];
            if (filePath != null)
            {
                // Note: We don't check for existance. This is an explicit choice by the user.
                return filePath;
            }

            // 3. Location of current assembly
            var codeBaseDirectory = GetCodeBaseDirectory();
            filePath = Path.Combine(codeBaseDirectory, fileName);
            if (File.Exists(filePath))
            {
                return filePath;
            }

            // Give up
            return null;
        }

        private static string GetCodeBaseDirectory()
        {
            var codeBaseUri = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            return Path.GetDirectoryName(codeBaseUri.LocalPath);
        }

        private static IColorSchemaSerializer GetSerializer()
        {
            return new XmlColorSchemaSerializer();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Handy serializing thingy
            var sc1 = new ColorSchema();
            sc1.Filters.Add(new ColorFilter { BackgroundColor = nameof(Color.White), ForegroundColor = nameof(Color.LightCyan) });
            var sc = new ColorSchema[] { sc1 };
            IColorSchemaSerializer serializer = GetSerializer();
            using (var stream = new FileStream(@"c:\temp\color.xml", FileMode.Create))
            {
                serializer.Serialize(sc, stream);
            }
        }
    }
}
