using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Marson.Grocker.Common
{
    public class Watcher : IDisposable
    {
        private const int FilePollingMs = 2500;
        private readonly Queue<WatcherEvent> eventQueue = new Queue<WatcherEvent>();
        private readonly object eventQueueLock = new object();

        private FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();
        private Timer timer;
        private Task eventTask;

        private LogFile logFile;
        private int logLineIndex;

        public void Dispose()
        {
            if (timer != null)
            {
                timer.Dispose();
                timer = null;
            }

            if (fileSystemWatcher != null)
            {
                fileSystemWatcher.EnableRaisingEvents = false;
                fileSystemWatcher.Dispose();
                fileSystemWatcher = null;
            }

            if (eventTask != null)
            {
                ShutdownEventTask();
            }
        }

        private enum WatcherEventType
        {
            Create,
            Change,
            Rename,
            Delete,
        }

        private class WatcherEvent
        {
            public WatcherEventType EventType { get; set; }
            public EventArgs EventArgs { get; set; }
        }

        /// <summary>
        /// All log lines to be shown will be written to this line writer.
        /// </summary>
        public ILineWriter LineWriter { get; set; }

        /// <summary>
        /// Path of directory to watch. Wildcards not allowed.
        /// </summary>
        public string DirectoryPath
        {
            get { return fileSystemWatcher.Path; }
            set { fileSystemWatcher.Path = value; }
        }

        /// <summary>
        /// Gets or sets the filter string used to determine what files are monitored in a directory.
        /// The filter string. The default is "*.*" (Watches all files.)
        /// </summary>
        public string Filter
        {
            get { return fileSystemWatcher.Filter; }
            set { fileSystemWatcher.Filter = value; }
        }

        /// <summary>
        /// Switch controlling whether to switch to a new file if one is created.
        /// </summary>
        public bool AlwaysShowLates { get; set; }

        /// <summary>
        /// Number of lines to read from the end of the file. Default 100.
        /// </summary>
        public int LineCount { get; set; }

        public Watcher()
        {
            LineCount = 80;
            timer = new Timer(new TimerCallback(TimerMain));
            fileSystemWatcher.Changed += FileSystemWatcher_Changed;
            fileSystemWatcher.Created += FileSystemWatcher_Created;
            fileSystemWatcher.Renamed += FileSystemWatcher_Renamed;
            fileSystemWatcher.Deleted += FileSystemWatcher_Deleted;
        }

        public bool IsStarted
        {
            get { return fileSystemWatcher.EnableRaisingEvents; }
        }

        public void Start()
        {
            if (IsStarted)
            {
                throw new InvalidOperationException("Already started");
            }

            if (string.IsNullOrEmpty(DirectoryPath))
            {
                throw new InvalidOperationException(nameof(DirectoryPath) + " property not set");
            }

            if (LineWriter == null)
            {
                throw new InvalidOperationException(nameof(LineWriter) + " property not set");
            }

            FindAndTail();
            eventTask = Task.Factory.StartNew(() => EventTaskMain(), TaskCreationOptions.LongRunning);
            fileSystemWatcher.EnableRaisingEvents = true;
            timer.Change(FilePollingMs, FilePollingMs);
        }

        public void Stop()
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
            fileSystemWatcher.EnableRaisingEvents = false;
            ShutdownEventTask();
        }

        private void FindAndTail(string eventText = null)
        {
            var file = FindFilePath();
            if (file != null)
            {
                TailFile(file, eventText);
            }
        }

        private string FindFilePath()
        {
            var filePaths = 
                from filePath in Directory.GetFiles(DirectoryPath, Filter)
                orderby (new FileInfo(filePath)).LastWriteTimeUtc descending
                select filePath;
            return filePaths.FirstOrDefault();
        }

        private void TailFile(string filePath, string eventText)
        {
            if (eventText != null && logFile != null && !IsCurrentLogFile(filePath))
            {
                LineWriter.WriteLine(string.Format(">>>>>> {0} <<<<<<", eventText));
            }

            if (logFile == null)
            {
                LineWriter.WriteLine(string.Format("++++++ File: {0} ++++++", filePath));
            }

            string[] lines = ReadLines(filePath);
            if (lines != null)
            {
                LineWriter.WriteLines(lines);
                logLineIndex += lines.Length;
            }
        }

        private string[] ReadLines(string filePath)
        {
            string[] lines;

            if (!IsCurrentLogFile(filePath))
            {
                logFile = LogFile.LoadFrom(filePath);
                if (logFile.Lines.Count == 0)
                {
                    return null;
                }

                var linesToLoad = Math.Min(logFile.Lines.Count, LineCount);
                lines = new string[linesToLoad];
                logLineIndex = Math.Max(0, logFile.Lines.Count - LineCount);
                logFile.LoadLines(logLineIndex, lines);
            }
            else
            {
                Debug.Assert(logLineIndex == logFile.Lines.Count);

                logFile.Update();
                var addedLineCount = logFile.Lines.Count - logLineIndex;
                if (addedLineCount <= 0)
                {
                    logLineIndex = logFile.Lines.Count;
                    return null;
                }
                lines = new string[addedLineCount];
                logFile.LoadLines(logLineIndex, lines);
            }

            return lines;
        }

        private bool IsCurrentLogFile(string filePath)
        {
            return logFile != null && string.Equals(logFile.FilePath, filePath, StringComparison.CurrentCultureIgnoreCase);
        }

        private void EnqueueEvent(WatcherEvent watcherEvent)
        {
            lock (eventQueueLock)
            {
                eventQueue.Enqueue(watcherEvent);
                Monitor.Pulse(eventQueueLock);
            }
        }

        private void FileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            EnqueueEvent(new WatcherEvent { EventType = WatcherEventType.Delete, EventArgs = e });
        }

        private void FileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            EnqueueEvent(new WatcherEvent { EventType = WatcherEventType.Rename, EventArgs = e });
        }

        private void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            EnqueueEvent(new WatcherEvent { EventType = WatcherEventType.Create, EventArgs = e });
        }

        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            EnqueueEvent(new WatcherEvent { EventType = WatcherEventType.Change, EventArgs = e });
        }

        private void EventTaskMain()
        {
            while (true)
            {
                WatcherEvent watcherEvent;
                lock (eventQueueLock)
                {
                    while (eventQueue.Count == 0)
                    {
                        Monitor.Wait(eventQueueLock);
                    }
                    watcherEvent = eventQueue.Dequeue();
                }
                if (watcherEvent == null)
                {
                    break;
                }
                HandleWatcherEvent(watcherEvent);
            }
        }

        private void ShutdownEventTask()
        {
            lock (eventQueueLock)
            {
                eventQueue.Clear();
                eventQueue.Enqueue(null);
                Monitor.Pulse(eventQueueLock);
            }
            eventTask.Wait();
            eventTask.Dispose();
            eventTask = null;
        }



        private void HandleWatcherEvent(WatcherEvent watcherEvent)
        {
            var fileSystemEventArgs = watcherEvent.EventArgs as FileSystemEventArgs;
            var renamedEventArgs = watcherEvent.EventArgs as RenamedEventArgs;

            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:dd.fffff") + " " + watcherEvent.EventType);
            switch (watcherEvent.EventType)
            {
                case WatcherEventType.Create:
                    FindAndTail("New file created");
                    break;
                case WatcherEventType.Change:
                    FindAndTail("Change in another file");
                    break;
                case WatcherEventType.Rename:
                    if (IsCurrentLogFile(renamedEventArgs.OldFullPath) || IsCurrentLogFile(renamedEventArgs.FullPath))
                    {
                        // A new file just got renamed, we can no longer assume that an existing file name is the same file we were watching
                        logFile = null;
                        FindAndTail("File renamed");
                    }
                    break;
                case WatcherEventType.Delete:
                    if (IsCurrentLogFile(fileSystemEventArgs.FullPath))
                    {
                        FindAndTail("File deleted");
                    }
                    break;
            }
        }

        private void TimerMain(object state)
        {
            EnqueueEvent(new WatcherEvent { EventType = WatcherEventType.Change });
        }

    }
}
