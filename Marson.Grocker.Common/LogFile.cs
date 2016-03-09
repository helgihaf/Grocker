using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marson.Grocker.Common
{
    public class LogFile : IItemsProvider<string>
    {
        private const int MaxAllowedLineLength = 1024 * 1024;

        private List<Line> lines;
        private int maxLineLength;

        private Encoding encoding;

        public static LogFile LoadFrom(string filePath)
        {
            var logFile = new LogFile(filePath);
            logFile.IndexLines();
            return logFile;
        }

        public static Task<LogFile> LoadFromAsync(string filePath)
        {
            return Task.Factory.StartNew<LogFile>(() => LoadFrom(filePath));
        }

        private LogFile(string filePath)
        {
            this.FilePath = filePath;
        }

        public string FilePath { get; private set; }

        public List<Line> Lines
        {
            get
            {
                return lines;
            }
        }

        public int MaxLineLength
        {
            get { return maxLineLength; }
        }

        public LogWindow CreateWindow()
        {
            return new LogWindow(this);
        }

        private void IndexLines()
        {
            encoding = LogReader.DetectEncoding(FilePath);
            lines = new List<Line>();
            using (var reader = new LogReader(FilePath, encoding))
            {
                encoding = reader.Encoding;
                IndexLines(reader);
            }
        }

        private void AddLine(long currentLineIndex, LogReader reader)
        {
            var line = new Line { Index = currentLineIndex, Length = (int)(reader.Index - currentLineIndex) };
            lines.Add(line);
            if (line.Length > maxLineLength)
            {
                maxLineLength = line.Length;
            }
        }

        private void UpdateIndex()
        {
            if (lines.Count == 0 || lines[lines.Count - 1].Index == 0)
            {
                IndexLines();
                return;
            }

            using (var reader = new LogReader(FilePath, encoding, lines[lines.Count - 1].Index))
            {
                lines.RemoveAt(lines.Count - 1);
                IndexLines(reader);
            }
        }

        private void IndexLines(LogReader reader)
        {
            long currentLineIndex = reader.Index;
            reader.ReadLine();
            while (reader.CurrentByte != -1)
            {
                AddLine(currentLineIndex, reader);
                currentLineIndex = reader.Index;
                reader.ReadLine();
            }

            if (reader.Index > currentLineIndex)
            {
                // Last data is a line, even if there is no newline
                AddLine(currentLineIndex, reader);
            }
        }

        public void Update()
        {
            UpdateIndex();
        }

        internal void LoadLines(int lineIndex, string[] destLines)
        {
            LoadLines(lineIndex, destLines.Length, (index, line) => destLines[index] = line);
        }

        public IList<string> LoadLines(int lineIndex, int count)
        {
            var list = new List<string>(count);
            LoadLines(lineIndex, count, (index, line) => list.Add(line));
            return list;
        }

        private void LoadLines(int lineIndex, int count, Action<int, string> action)
        {
            var startLine = Lines[lineIndex];
            using (var stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
            {
                stream.Seek(startLine.Index, SeekOrigin.Begin);
                using (var reader = new StreamReader(stream, encoding))
                {
                    string line;
                    for (int i = 0; (line = reader.ReadLine()) != null && i < count; i++)
                    {
                        action(i, line);
                    }
                }
            }
        }


        internal StreamReader CreateReader(int lineIndex)
        {
            var startLine = Lines[lineIndex];
            var stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
            try
            {
                stream.Seek(startLine.Index, SeekOrigin.Begin);
                return new StreamReader(stream, encoding);
            }
            catch
            {
                stream.Dispose();
                throw;
            }
        }

        int IItemsProvider<string>.GetCount()
        {
            return lines.Count();
        }

        IList<string> IItemsProvider<string>.GetRange(int startIndex, int count)
        {
            throw new NotImplementedException();
        }
    }
}
