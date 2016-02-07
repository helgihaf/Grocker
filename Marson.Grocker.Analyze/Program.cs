using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Marson.Grocker.Analyze
{
    class Program
    {
        static void Main(string[] args)
        {
            //string[] logFiles = new[]
            //{
            //    @"C:\logs\LIService_5.1\LIService20160115_9.log",
            //    @"C:\inetpub\logs\LogFiles\W3SVC1\u_ex150724.log",
            //    @"C:\logs\STS-2.4\LogFile.log",
            //};

            string[] dirs = new[]
            {
                @"C:\logs\LIService_5.1",
                @"C:\inetpub\logs\LogFiles\W3SVC1",
                @"C:\logs\STS-2.4",
            };

            var analyzer = new LogFileAnalyzer();

            foreach (var dir in dirs)
            {
                foreach (var logFile in Directory.GetFiles(dir, "*.log", SearchOption.AllDirectories))
                {
                    Console.Write(logFile + "...");
                    var result = analyzer.Analyze(logFile);
                    //ObjectDumper.Write(result, 3, Console.Out);
                    Console.WriteLine("done.");
                }
            }

            WriteToCsv(@"c:\temp\CharacterFrequencies.csv", analyzer.CharacterFrequencies);
            //WriteToCsv(@"c:\temp\LineLengths.csv", result.LineLengths);

        }

        private static void WriteToCsv<T>(string filePath, Frequencies<T> frequencies)
        {
            using (var streamWriter = new StreamWriter(filePath))
            {
                foreach (var item in frequencies.GetItems().OrderBy(t => t.Value))
                {
                    streamWriter.WriteLine("\"" + item.Value + "\"" + ";" + item.Count);
                }
            }
        }

    }
}
