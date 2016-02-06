using System;

namespace Marson.Grocker.Common
{
    public class LogWindow
    {
        private int currentIndex = -1;
        private string[] lines;

        internal LogWindow(LogFile logFile)
        {
            this.LogFile = logFile;
        }

        public int Length
        {
            get
            {
                if (lines != null)
                {
                    return lines.Length;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                lines = new string[value];
            }
        }

        public LogFile LogFile { get; private set; }

        private void LoadLines(int lineIndex)
        {
            LogFile.LoadLines(lineIndex, lines);
        }

        public string[] Lines
        {
            get
            {
                if (currentIndex == -1)
                {
                    throw new InvalidOperationException("LogWindow has not yet been positioned");
                }
                return lines;
            }
        }

        public int CurrentIndex
        {
            get
            {
                return currentIndex;
            }
            set
            {
                LoadLines(value);
                currentIndex = value;
            }
        }

        public int PageCount
        {
            get
            {
                return (LogFile.Lines.Count + lines.Length - 1) / lines.Length;
            }
        }

        public void LastPage()
        {
            if (LogFile.Lines.Count > 0)
            {
                currentIndex = LogFile.Lines.Count - lines.Length;
                LoadLines(currentIndex);
            }
        }

        public void FirstPage()
        {
            currentIndex = 0;
            LoadLines(currentIndex);
        }

        public bool PreviousPage()
        {
            int newIndex = currentIndex - lines.Length;
            if (newIndex < 0)
            {
                newIndex = 0;
            }

            bool success = newIndex != currentIndex;
            if (success)
            {
                currentIndex = newIndex;
                LoadLines(currentIndex);
            }

            return success;
        }

        public bool NextPage()
        {
            int newIndex = currentIndex + lines.Length;
            if (newIndex >= LogFile.Lines.Count)
            {
                currentIndex = LogFile.Lines.Count - lines.Length;
            }

            bool success = newIndex != currentIndex;
            if (success)
            {
                currentIndex = newIndex;
                LoadLines(currentIndex);
            }

            return success;
        }

    }
}