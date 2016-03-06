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
        public const int DefaultLineCount = 100;
        private const int FilePollingMs = 2500;
        private readonly Queue<WatcherEvent> eventQueue = new Queue<WatcherEvent>();
        private readonly object eventQueueLock = new object();

        private FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();
        private Task readTask;

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

        public Watcher()
        {
            LineCount = DefaultLineCount;
            fileSystemWatcher.Changed += FileSystemWatcher_Changed;
            fileSystemWatcher.Created += FileSystemWatcher_Created;
            fileSystemWatcher.Renamed += FileSystemWatcher_Renamed;
            fileSystemWatcher.Deleted += FileSystemWatcher_Deleted;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (fileSystemWatcher != null)
                {
                    fileSystemWatcher.EnableRaisingEvents = false;
                    fileSystemWatcher.Dispose();
                    fileSystemWatcher = null;
                }

                if (readTask != null)
                {
                    ShutdownReadTask();
                }
            }
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
        /// Number of lines to read from the end of the file. Default is DefaultLineCount.
        /// </summary>
        public int LineCount { get; set; }

        public bool IsStarted
        {
            get { return fileSystemWatcher.EnableRaisingEvents; }
        }

        /// <summary>
        /// Fires when the watcher starts watching a new file.
        /// </summary>
        public event EventHandler<FilePathEventArgs> FileFound;

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

            readTask = Task.Factory.StartNew(() => ReadTaskMain(), TaskCreationOptions.LongRunning);
            fileSystemWatcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            fileSystemWatcher.EnableRaisingEvents = false;
            ShutdownReadTask();
        }

        private void ReadTaskMain()
        {
            bool keepOnReading = true;
            StreamReader reader = null;

            while (keepOnReading)
            {
                // If not already reading, find something to read
                if (reader == null)
                {
                    reader = GetStreamReader();
                }

                // Check for data on our current reader
                if (reader != null)
                {
                    string line;
                    try
                    {
                        while ((line = reader.ReadLine()) != null)
                        {
                            LineWriter.WriteLine(line);
                        }
                    }
                    catch (IOException ex)
                    {
                        LineWriter.WriteLine(string.Format(">>>> {0} <<<<", ex.Message));
                        reader = null;
                        continue;
                    }
                }

                // Check for file system events
                WatcherEvent watcherEvent = null;
                lock (eventQueueLock)
                {
                    if (eventQueue.Count == 0)
                    {
                        Monitor.Wait(eventQueueLock, FilePollingMs);
                    }
                    if (eventQueue.Count > 0)
                    {
                        watcherEvent = eventQueue.Dequeue();
                        keepOnReading = watcherEvent != null;
                    }
                }
                if (watcherEvent != null)
                {
                    reader = HandleWatcherEvent(reader, watcherEvent);
                }
            }
        }

        private StreamReader GetStreamReader()
        {
            StreamReader streamReader = null;

            var filePath = FindFilePath();
            if (filePath != null)
            {
                streamReader = FindTailOf(filePath);
                if (streamReader != null)
                {
                    LineWriter.WriteLine(string.Format("++++++ File: {0} ++++++", filePath));
                    OnFileFound(filePath);
                }
            }
            return streamReader;
        }

        protected virtual void OnFileFound(string filePath)
        {
            if (FileFound != null)
            {
                FileFound(this, new FilePathEventArgs { FilePath = filePath });
            }
        }

        private StreamReader FindTailOf(string filePath)
        {
            var logFile = LogFile.LoadFrom(filePath);
            if (logFile.Lines.Count == 0)
            {
                return null;
            }

            // Output the last LineCount (or less) lines of the file
            var logLineIndex = Math.Max(0, logFile.Lines.Count - LineCount);
            return logFile.CreateReader(logLineIndex);
        }

        private string FindFilePath()
        {
            var filePaths = 
                from filePath in Directory.GetFiles(DirectoryPath, Filter)
                orderby (new FileInfo(filePath)).LastWriteTimeUtc descending
                select filePath;
            return filePaths.FirstOrDefault();
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

        private void ShutdownReadTask()
        {
            lock (eventQueueLock)
            {
                eventQueue.Clear();
                eventQueue.Enqueue(null);
                Monitor.Pulse(eventQueueLock);
            }
            readTask.Wait();
            readTask.Dispose();
            readTask = null;
        }



        private StreamReader HandleWatcherEvent(StreamReader currentStreamReader, WatcherEvent watcherEvent)
        {
            StreamReader resultStreamReader = currentStreamReader;

            var fileSystemEventArgs = watcherEvent.EventArgs as FileSystemEventArgs;
            var renamedEventArgs = watcherEvent.EventArgs as RenamedEventArgs;

            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:dd.fffff") + " " + watcherEvent.EventType);
            switch (watcherEvent.EventType)
            {
                case WatcherEventType.Create:
                    // Do nothing. A new empty file does not interest us. We'll read it once we get a Change event.
                    break;
                case WatcherEventType.Change:
                    if (!ArePathsEqual(fileSystemEventArgs.FullPath, FullPathOf(currentStreamReader)))
                    {
                        // A change in some other file. Chase that one.
                        resultStreamReader = null;
                    }
                    break;
                case WatcherEventType.Rename:
                    var currentFullPath = FullPathOf(currentStreamReader);
                    if (ArePathsEqual(renamedEventArgs.FullPath, currentFullPath) || ArePathsEqual(renamedEventArgs.OldFullPath, currentFullPath))
                    {
                        // File names are changing and our stream reader happily ignores further data.
                        resultStreamReader = null;
                    }
                    break;
                case WatcherEventType.Delete:
                    if (ArePathsEqual(fileSystemEventArgs.FullPath, FullPathOf(currentStreamReader)))
                    {
                        // We are being deleted.
                        resultStreamReader = null;
                    }
                    break;
            }

            return resultStreamReader;
        }

        private bool ArePathsEqual(string path1, string path2)
        {
            return string.Equals(path1, path2, StringComparison.CurrentCultureIgnoreCase);
        }

        private string FullPathOf(StreamReader streamReader)
        {
            var fileStream = streamReader.BaseStream as FileStream;
            return fileStream?.Name;
        }
    }
}
