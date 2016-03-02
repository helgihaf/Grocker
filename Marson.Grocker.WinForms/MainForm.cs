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
        private List<ColorSchema<Color>> colorSchemas;

        public MainForm()
        {
            InitializeComponent();
            colorSchemas = LoadColorSchemas();
        }

        private static List<ColorSchema<Color>> LoadColorSchemas()
        {
            IColorSchemaSerializer<Color> serializer = GetSerializer();
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

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderDialog.ShowDialog(this) == DialogResult.OK)
            {
                AddMonitor(folderDialog.Folder);
            }
        }

        private void AddMonitor(string directoryPath)
        {
            TabPage page = new TabPage(directoryPath);
            WatcherView view = new WatcherView();
            page.Controls.Add(view);
            page.Tag = view;
            view.Dock = DockStyle.Fill;
            view.DirectoryPath = directoryPath;
            view.ColorSchemas = colorSchemas;
            tabControl.TabPages.Add(page);
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

        private static IColorSchemaSerializer<Color> GetSerializer()
        {
            return new XmlColorSchemaSerializer<Color>();
        }

    }
}
