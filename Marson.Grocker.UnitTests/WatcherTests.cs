using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Marson.Grocker.Common;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace Marson.Grocker.UnitTests
{
    [TestClass]
    public class WatcherTests
    {
        private static TestData testData;
        private static string logFile196407;
        private static string logFile9;
        private static string logFile5;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            testData = new TestData();
            logFile196407 = testData.GetLogFile(196407);
            logFile9 = testData.GetLogFile(9);
            logFile5 = testData.GetLogFile(5);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            if (testData != null)
            {
                testData.Dispose();
                testData = null;
            }
        }


        [TestMethod]
        public void ShouldWriteTail()
        {
            var logFile = LogFile.LoadFrom(logFile196407);

            int logLineIndex = logFile.Lines.Count - 80;
            var logWindow = logFile.CreateWindow();
            logWindow.Length = 80;
            logWindow.LastPage();
            for (int i = 0; i < logWindow.Length; i++)
            {
                Debug.WriteLine(logWindow.Lines[i]);
            }
        }

        [TestMethod]
        public void ShouldWriteInitialTail()
        {
            const int LogFileLineCount = 9;

            var filePath = logFile9;
            using (var watcher = new Watcher())
            {
                string[] writtenLines;
                using (var stringWriter = new StringWriter())
                {
                    var writer = new LineTextWriter(stringWriter);
                    watcher.LineWriter = writer;
                    watcher.DirectoryPath = Path.GetDirectoryName(filePath);
                    watcher.Filter = Path.GetFileName(filePath);
                    watcher.LineCount = 4;
                    watcher.Start();
                    writer.Flush();
                    Thread.Sleep(200);
                    writtenLines = stringWriter.ToString().ToLines();
                }
                writtenLines.ForEach(i => Debug.WriteLine(i));

                var expectedLines = CreateLineListWithFileMarker(
                    TestData.LoadLines(logFile9,
                                       LogFileLineCount - watcher.LineCount,
                                       watcher.LineCount));
                CompareLines(expectedLines.ToArray(), writtenLines);
            }
        }

        private List<string> CreateLineListWithFileMarker(string[] strings)
        {
            var list = new List<string>();
            list.Add("++++++ File: ++++++");
            list.AddRange(strings);
            return list;
        }

        [TestMethod]
        public void ShouldDetectNewWrite()
        {
            const int LogFileLineCount = 9;
            string[] newLines = testData.CreateLogLines(1);

            var tempDir = TestData.CreateTempDirectory();
            var tempFileName = Path.Combine(tempDir, "log.log");
            Debug.WriteLine(tempFileName);
            using (var watcher = new Watcher())
            {
                using (var stringWriter = new StringWriter())
                {
                    var writer = new LineTextWriter(stringWriter);
                    string[] writtenLines;
                    using (var logger = new LoggerEmulator(logFile9, tempFileName))
                    {
                        logger.WriteToEnd();

                        watcher.LineWriter = writer;
                        watcher.DirectoryPath = tempDir;
                        watcher.Filter = "*.log";
                        watcher.LineCount = 4;
                        watcher.Start();
                        Thread.Sleep(200);
                        logger.WriteLines(newLines);
                    }

                    Thread.Sleep(1000);
                    watcher.Stop();
                    writer.Flush();
                    writtenLines = stringWriter.ToString().ToLines();

                    var expectedLines = CreateLineListWithFileMarker(
                        TestData.LoadLines(logFile9,
                                           LogFileLineCount - watcher.LineCount,
                                           watcher.LineCount));
                    expectedLines.AddRange(newLines);
                    CompareLines(expectedLines.ToArray(), writtenLines);
                }
            }
        }

        [TestMethod]
        public void ShouldDetectFileRollover()
        {
            const int FirstLogFileLineCount = 9;
            const int SecondLogFileLineCount = 5;

            // Write 9 lines in file1
            // Start watcher, will output 4 last lines of file 1
            // Rename file1 to file1old
            // Write new 5 lines to a new file1
            // Expected result:
            //   1 header
            //   4 lines from original file 1
            //   1 header
            //   4 lines from new file 1
            var tempDir = TestData.CreateTempDirectory();
            var logTempFilePath = Path.Combine(tempDir, "log.log");
            var oldLogTempFilePath = Path.Combine(tempDir, "log.old");
            Debug.WriteLine("main file: " +logTempFilePath);
            Debug.WriteLine("old file: " + oldLogTempFilePath);

            string[] writtenLines;
            using (var watcher = new Watcher())
            {
                using (var stringWriter = new StringWriter())
                {
                    var writer = new LineTextWriter(stringWriter);
                    File.Copy(logFile9, logTempFilePath);
                    watcher.LineWriter = writer;
                    watcher.DirectoryPath = tempDir;
                    watcher.Filter = "*.log";
                    watcher.LineCount = 4;
                    watcher.Start();

                    Thread.Sleep(200);
                    File.Move(logTempFilePath, oldLogTempFilePath);
                    EmulateLog(logFile5, logTempFilePath);

                    Thread.Sleep(1000);
                    watcher.Stop();
                    writer.Flush();
                    writtenLines = stringWriter.ToString().ToLines();
                }
                Debug.WriteLine("Written lines:");
                DumpLines(writtenLines);

                var firstLines = CreateLineListWithFileMarker(
                    TestData.LoadLines(logFile9,
                           FirstLogFileLineCount - watcher.LineCount,
                           watcher.LineCount));
                //firstLines.Add(">>>>>> <<<<<<");
                var secondLines = CreateLineListWithFileMarker(
                    TestData.LoadLines(logFile5,
                           SecondLogFileLineCount - watcher.LineCount,
                           watcher.LineCount));
                var expectedLines = firstLines.Concat(secondLines).ToArray();
                CompareLines(expectedLines, writtenLines);
            }
        }


        [TestMethod]
        public void ShouldDetectFileDelete()
        {
            var tempDir = TestData.CreateTempDirectory();
            var logTempFilePath = Path.Combine(tempDir, "log.log");
            Debug.WriteLine(logTempFilePath);

            File.Copy(logFile9, logTempFilePath);
            using (var watcher = new Watcher())
            {
                watcher.LineWriter = new LineTextWriter(Console.Out);
                watcher.DirectoryPath = tempDir;
                watcher.Filter = "*.log";
                watcher.LineCount = 4;
                watcher.Start();

                Thread.Sleep(200);
                File.Delete(logTempFilePath);

                Thread.Sleep(1000);
                watcher.Stop();
                //writer.Flush();
                //writtenLines = writer.ToString().ToLines();
                //CompareLines(expectedLines, writtenLines);
            }
        }

        private void EmulateLog(string sourceFilePath, string targetFilePath)
        {
            using (var logger = new LoggerEmulator(sourceFilePath, targetFilePath))
            {
                logger.WriteToEnd();
            }
        }


        private string CreateIsolatedTempCopy(string logFilePath)
        {
            var dateTimeDir = DateTime.Now.ToString("yyyy-MM-ddTHH.mm.ss.fff");
            var tempPath = Path.Combine(Path.GetTempPath(), dateTimeDir);
            Directory.CreateDirectory(tempPath);
            var tempFileName = Path.Combine(tempPath, "log.log");
            File.Copy(logFilePath, tempFileName, true);
            return tempFileName;
        }

        private static void CompareLines(string[] expected, string[] actual)
        {
            const string FileNameMarker = "++++++ ";
            const string EventMarker = ">>>>>";
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                var exp = expected[i];
                var act = actual[i];
                if (!(exp.StartsWith(FileNameMarker) && act.StartsWith(FileNameMarker)) &&
                    !(exp.StartsWith(EventMarker) && act.StartsWith(EventMarker)))
                {
                    Assert.AreEqual(exp, act, "i = " + i.ToString());
                }
            }
        }

        private void DumpLines(string[] lines)
        {
            foreach (var line in lines)
            {
                Debug.WriteLine(line);
            }
        }

        //[TestMethod]
        //public void ShouldPeek()
        //{
        //    string filePath = logFile9;
        //    using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
        //    using (var reader = new StreamReader(stream))
        //    {
        //        int chars = 0;
        //        int c;
        //        while ((c = reader.Read()))
        //    }
        //}
    }
}