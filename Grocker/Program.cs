using Marson.Grocker.Common;
using System;
using System.Collections.Generic;
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

            var watcher = new Watcher();
            watcher.DirectoryPath = options.DirectoryPath ?? Environment.CurrentDirectory;
            if (options.Filter != null)
            {
                watcher.Filter = options.Filter;
            }
            else
            {
                watcher.Filter = "*.log";
            }
            watcher.TextWriter = Console.Out;
            watcher.Start();

            bool doRun = true;
            while (doRun)
            {
                var keyInfo = Console.ReadKey();
                doRun = keyInfo.KeyChar != 'q';
            }
            watcher.Stop();
        }
    }
}
