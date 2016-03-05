using Marson.Grocker.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Grocker
{
    class Program
    {
        static void Main(string[] args)
        {
            Watcher watcher = CreateWatcher(args);
            if (watcher == null)
            {
                return;
            }

            watcher.Start();
            WaitForStopSignal();
            watcher.Stop();
        }

        private static Watcher CreateWatcher(string[] args)
        {
            bool showHelp = false;
            Watcher watcher = null;
            try
            {
                Options options = Options.Parse(args);
                if (!options.HelpWanted)
                {
                    watcher = CreateWatcherWithOptions(options);
                }
                else
                {
                    showHelp = true;
                }
            }
            catch (ArgumentException ex)
            {
                Console.Error.WriteLine(ex.Message);
                showHelp = true;
            }
            catch (ApplicationException ex)
            {
                Console.Error.WriteLine(ex.Message);
            }

            if (showHelp)
            {
                Console.WriteLine(Options.GetHelpText());
            }

            return watcher;
        }

        private static Watcher CreateWatcherWithOptions(Options options)
        {
            var watcher = new Watcher();

            watcher.DirectoryPath = options.DirectoryPath ?? Environment.CurrentDirectory;
            watcher.Filter = options.Filter ?? "*.log";
            watcher.LineWriter = CreateWriter(options);

            return watcher;
        }

        private static ILineWriter CreateWriter(Options options)
        {
            var writer = new ConsoleLineWriter();

            if (options.EnableColorSchema)
            {
                EnableColorSchema(writer, options);
            }
            else
            {
                writer.AutoDetectColorSchema = false;
            }

            return writer;
        }

        private static void EnableColorSchema(ConsoleLineWriter writer, Options options)
        {
            var colorSchemas = LoadColorSchemas();
            if (colorSchemas == null)
            {
                throw new ApplicationException("No color schema file found");
            }
            if (options.ColorSchemaName == null)
            {
                writer.AutoDetectColorSchema = true;
                writer.ColorSchemas = colorSchemas;
            }
            else
            {
                writer.AutoDetectColorSchema = false;
                writer.ColorSchema = colorSchemas.Find(cs => cs.Name == options.ColorSchemaName);
                if (writer.ColorSchema == null)
                {
                    throw new ApplicationException(string.Format("Color schema {0} was not found.", options.ColorSchemaName));
                }
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

        private static void WaitForStopSignal()
        {
            bool doRun = true;
            while (doRun)
            {
                var keyInfo = Console.ReadKey();
                doRun = keyInfo.KeyChar != 'q';
            }
        }

        // Handy serializing thingy
        //var sc1 = new ColorSchema<ConsoleColor>();
        //sc1.Filters.Add(new ColorFilter<ConsoleColor> { BackgroundColor = ConsoleColor.Black, ForegroundColor = ConsoleColor.DarkBlue });
        //var sc = new ColorSchema<ConsoleColor>[] { sc1 };
        //IColorSchemaSerializer<ConsoleColor> serializer = GetSerializer();
        //using (var stream = new FileStream(@"c:\temp\color.xml", FileMode.Create))
        //{
        //    serializer.Serialize(sc, stream);
        //}
        //return;

    }
}
