using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marson.Grocker.Common
{
    public class LogFile
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
            lines = new List<Line>();
            using (var reader = new LogReader(FilePath))
            {
                encoding = reader.Encoding;
                IndexLines(reader);
            }
        }

        private void AddLine(long currentLineIndex, LogReader reader)
        {
            lines.Add(new Line { Index = currentLineIndex, Length = (int)(reader.Index - currentLineIndex) });
        }

        private void UpdateIndex()
        {
            if (lines.Count == 0 || lines[lines.Count - 1].Index == 0)
            {
                IndexLines();
                return;
            }

            using (var reader = new LogReader(FilePath, lines[lines.Count - 1].Index, encoding))
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

        private void HandleLineLength(int length)
        {
            if (length > MaxAllowedLineLength)
                throw new FormatException(string.Format("Maximum line length of {0} exceeded in file, line length {1}.", MaxAllowedLineLength, length));

            if (maxLineLength < length)
                maxLineLength = length;
        }

        public void Update()
        {
            UpdateIndex();
        }

        private string ClipText(string text)
        {
            if (text != null)
            {
                return text.Substring(0, Math.Min(text.Length, 25));
            }
            else
            {
                return "";
            }
        }

        internal void LoadLines(int lineIndex, string[] destLines)
        {
            var startLine = Lines[lineIndex];
            //var totalBytes = 0;
            //for (int i = 0; i < destLines.Length && i < Lines.Count; i++)
            //{
            //    totalBytes += Lines[lineIndex + i].Length;
            //}
            //if (totalBytes == 0)
            //{
            //    return;
            //}
            //var buffer = new byte[totalBytes];
            using (var stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
            {
                stream.Seek(startLine.Index, SeekOrigin.Begin);
                using (var reader = new StreamReader(stream, encoding))
                {
                    string line;
                    for (int i = 0; (line = reader.ReadLine()) != null && i < destLines.Length; i++)
                    {
                        destLines[i] = line;
                    }
                }
            }


        }

        private int LinesToArray(List<string> blockLines, string[] destLines)
        {
            int index;
            for (index = 0; index < blockLines.Count && index < destLines.Length; index++)
            {
                destLines[index] = blockLines[index];
            }
            return index;
        }


    }
}
