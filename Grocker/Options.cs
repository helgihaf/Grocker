using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Grocker
{
    class Options
    {
        public string DirectoryPath { get; set; }
        public string Filter { get; set; }
        public bool EnableColorSchema { get; set; }
        public string ColorSchemaName { get; set; }

        public static Options Parse(string[] args)
        {
            CommandLineParameters clp;
            try
            {
                clp = CommandLineParameters.Parse(args);
            }
            catch (ArgumentException ex)
            {
                Console.Error.WriteLine(ex.Message);
                ShowUsage();
                return null;
            }

            if (clp.DetachNamedBool("?"))
            {
                ShowUsage();
                return null;
            }

            var result = new Options();

            result.DirectoryPath = clp.DetachPositional();
            result.Filter = clp.DetachNamed("f");
            result.EnableColorSchema = !clp.DetachNamedBool("nc");
            if (result.EnableColorSchema)
            {
                result.ColorSchemaName = clp.DetachNamed("c");
            }

            if (clp.Named.Count > 0)
            {
                foreach (var name in clp.Named.Keys)
                {
                    Console.Error.WriteLine("Invalid option /" + name);
                }
                ShowUsage();
                return null;
            }

            if (clp.Positionals.Count > 0)
            {
                Console.Error.WriteLine("Invalid argument");
                ShowUsage();
            }

            return result;
        }

        private static void ShowUsage()
        {
            var assembly = Assembly.GetExecutingAssembly();
            Console.WriteLine("Marson Grocker. Version " + assembly.GetName().Version.ToString());
            var copyrightAttribute = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>();
            if (copyrightAttribute != null)
            {
                Console.WriteLine(copyrightAttribute.Copyright);
            }
            Console.WriteLine();
            Console.WriteLine("Usage: Grocker [directory] [-f filter] [-c [colorSchemaName]]");
            Console.WriteLine();
            Console.WriteLine("  [directory]");
            Console.WriteLine("    The directory to monitor. Default is current directory.");
            Console.WriteLine("  [-f filter]");
            Console.WriteLine("    File filter wildcard. Default is *.log");
            Console.WriteLine("  [-c colorSchemaName]");
            Console.WriteLine("    Color schema. Default is auto detect. Incompatible with -nc");
            Console.WriteLine("  [-nc]");
            Console.WriteLine("    No color schema. Incompatible with -c");
            Console.WriteLine();
        }
    }
}
