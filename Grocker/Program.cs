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
            if (watcher != null)
            {
                return;
            }

            watcher.Start();
            WaitForStopSignal();
            watcher.Stop();
        }

        private static Watcher CreateWatcher(string[] args)
        {
            Watcher watcher = null;
            try
            {
                Options options = Options.Parse(args);
                watcher = CreateWatcherWithOptions(options);
            }
            catch (ArgumentException ex)
            {
                Console.Error.WriteLine(ex.Message);
                Console.WriteLine(Options.GetHelpText());
            }
            catch (ApplicationException ex)
            {
                Console.Error.WriteLine(ex.Message);
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
            using (var stream = new FileStream("ColorSchemas.Grocker.xml", FileMode.Open))
            {
                return serializer.Deserialize(stream).ToList();
            }
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
    }
}
