using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marson.Grocker.UnitTests
{
    internal class LoggerEmulator : IDisposable
    {
        private StreamReader reader;
        private StreamWriter writer;

        public LoggerEmulator(string sourceFilePath, string targetFilePath)
        {
            reader = new StreamReader(sourceFilePath);
            writer = new StreamWriter(targetFilePath);
        }

        public void Dispose()
        {
            if (writer != null)
            {
                writer.Dispose();
                writer = null;
            }
            if (reader != null)
            {
                reader.Dispose();
                reader = null;
            }
        }

        public int NextLines(int lineCount)
        {
            int i = 0;
            for (i = 0; i < lineCount; i++)
            {
                if (!NextLine())
                    break;
            }
            return i;
        }

        public bool NextLine()
        {
            var line = reader.ReadLine();
            bool endOfFile = line == null;
            if (!endOfFile)
            {
                writer.WriteLine(line);
            }
            writer.Flush();
            return !endOfFile;
        }

        public void WriteToEnd()
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                writer.WriteLine(line);
            }
            writer.Flush();
        }

        public void WriteLines(string[] lines)
        {
            foreach (var line in lines)
            {
                writer.WriteLine(line);
            }
            writer.Flush();
        }

        public void WriteLine(string line)
        {
            writer.WriteLine(line);
            writer.Flush();
        }
    }
}
