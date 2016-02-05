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
            Options options = Options.Parse(args);
            if (options == null)
            {
                return;
            }

            Watcher watcher;
            try
            {
                watcher = CreateWatcher(options);
            }
            catch (ApplicationException ex)
            {
                Console.Error.WriteLine(ex.Message);
                return;
            }

            watcher.Start();

            bool doRun = true;
            while (doRun)
            {
                var keyInfo = Console.ReadKey();
                doRun = keyInfo.KeyChar != 'q';
            }
            watcher.Stop();
        }

        private static Watcher CreateWatcher(Options options)
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

        private static List<ColorSchema> CreateColorSchemas()
        {
            var soaSchema = new ColorSchema
            {
                Name = "SOA",
                SelectorPattern = @"^\[\d{1,2}\.\d{1,2}\.\d{4} \d{2}:\d{2}:\d{2}.\d{3}\]",
            };
            soaSchema.Filters.AddRange(new[]
                {
                    new ColorFilter
                    {
                        Pattern = @"\[Warning\]",
                        ForegroundColor = ConsoleColor.Yellow,
                        BackgroundColor = ConsoleColor.Black,
                    },
                    new ColorFilter
                    {
                        Pattern = @"\[Error\]",
                        ForegroundColor = ConsoleColor.White,
                        BackgroundColor = ConsoleColor.Red,
                    },
                    new ColorFilter
                    {
                        Pattern = @"\[Debug\]",
                        ForegroundColor = ConsoleColor.Gray,
                        BackgroundColor = ConsoleColor.Black,
                    },
                    new ColorFilter
                    {
                        Pattern = @"\[Info\]",
                        ForegroundColor = ConsoleColor.Cyan,
                        BackgroundColor = ConsoleColor.Black,
                    },
                    //new ColorFilter
                    //{
                    //    Pattern = @"\[Activity.*\]",
                    //    ForegroundColor = ConsoleColor.Black,
                    //    BackgroundColor = ConsoleColor.Gray,
                    //},
                });

            var coreSchema = new ColorSchema
            {
                Name = "Core",
                SelectorPattern = @"^\d{1,2}\.\d{1,2}\.\d{4} \d{2}:\d{2}:\d{2}.\d{3}\|TRID",
            };
            coreSchema.Filters.AddRange(new[]
                {
                    new ColorFilter
                    {
                        Pattern = @"\|WARNING\>",
                        ForegroundColor = ConsoleColor.Black,
                        BackgroundColor = ConsoleColor.Yellow,
                    },
                    new ColorFilter
                    {
                        Pattern = @"\|ERROR\>",
                        ForegroundColor = ConsoleColor.White,
                        BackgroundColor = ConsoleColor.Red,
                    },
                    new ColorFilter
                    {
                        Pattern = @"\|DEBUG\>",
                        ForegroundColor = ConsoleColor.Black,
                        BackgroundColor = ConsoleColor.Gray,
                    },
                    new ColorFilter
                    {
                        Pattern = @"\|INFO\>",
                        ForegroundColor = ConsoleColor.Black,
                        BackgroundColor = ConsoleColor.Cyan,
                    },
                });

            var iisSchema = new ColorSchema
            {
                Name = "IIS",
                SelectorPattern = @"^(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\s)|(#\w+:)",
            };
            iisSchema.Filters.AddRange(new[]
                {
                    new ColorFilter
                    {
                        Pattern = @"POST .*- [3-5]\d\d ",
                        ForegroundColor = ConsoleColor.White,
                        BackgroundColor = ConsoleColor.Red,
                    },
                    new ColorFilter
                    {
                        Pattern = @"POST .*- 2\d\d ",
                        ForegroundColor = ConsoleColor.Black,
                        BackgroundColor = ConsoleColor.Cyan,
                    },
                });

            return new List<ColorSchema>(new[] { soaSchema, coreSchema, iisSchema });
        }
    }
}
