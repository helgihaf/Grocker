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

            var result = new Options();
            result.DirectoryPath = clp.DetachNamed("d");
            result.Filter = clp.DetachNamed("f");
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
            Console.WriteLine();
            Console.WriteLine("Marson Grocker version " + Assembly.GetExecutingAssembly().GetName().Version.ToString());
            Console.WriteLine("Usage:");
            Console.WriteLine(" grocker [-d directory] [-f filter]");
            Console.WriteLine();
            Console.WriteLine("   Default directory is current directory");
            Console.WriteLine("   Default filter is *.log");
            Console.WriteLine();
        }
    }
}
