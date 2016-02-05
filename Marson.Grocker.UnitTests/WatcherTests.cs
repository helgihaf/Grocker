using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Marson.Grocker.Common;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Marson.Grocker.UnitTests
{
    [TestClass]
    public class WatcherTests
    {
        [TestMethod]
        public void ShouldWriteTail()
        {
            var logFile = LogFile.LoadFrom(FileNames.LogFile196407);

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
            string[] expectedLines = new string[]
                {
                    "++++++ File: ++++++",
                    "[23.10.2015 09:26:44.560] [LAIHEH] [LAIHEH] [ActivityStart] [Generating token] [88f66500-7e1f-4a78-831a-c012a35a656f] ParentId:[269a7a4c-e235-4640-98a7-08b081dc4130] ActivityChainId:[66a7eef2-c314-40dd-a865-038de097ca9f]",
                    "[23.10.2015 09:26:45.482] [LAIHEH] [LAIHEH] [ActivityStop ] [Generating token](925 ms) [88f66500-7e1f-4a78-831a-c012a35a656f] ParentId:[269a7a4c-e235-4640-98a7-08b081dc4130] ActivityChainId:[66a7eef2-c314-40dd-a865-038de097ca9f]",
                    "[23.10.2015 09:26:45.482] [LAIHEH] [LAIHEH] [ActivityStop ] [Issue](973 ms) [269a7a4c-e235-4640-98a7-08b081dc4130] ParentId:[b9327fce-3da4-4182-abbf-f75493f5f4ea] ActivityChainId:[66a7eef2-c314-40dd-a865-038de097ca9f]",
                    "[23.10.2015 09:27:29.877] [LAIHEH] [LAIHEH] [ActivityStart] [Issue] [35cbea68-867d-48e8-9bf2-625d103a6e9d] ParentId:[09dcdfe7-b3d3-4ff2-9187-c197370f1134] ActivityChainId:[09f5ff36-8ab0-419b-b11b-865478da041b]",
                };

            var filePath = FileNames.LogFile9;
            var watcher = new Watcher();
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
                writtenLines = stringWriter.ToString().ToLines();
            }
            writtenLines.ForEach(i => Debug.WriteLine(i));
            CompareLines(expectedLines, writtenLines);
        }

        [TestMethod]
        public void ShouldDetectNewWrite()
        {
            string[] newLines = new string[]
            {
                "[11.11.2015 15:57:05.278] [LAASSI] [LAASSI] [ActivityStart] [Generating token] [c4b769b7-10f4-4889-bc08-a0c7bea81a84] ParentId:[8f572baf-8458-4a9d-8204-5e48d9bb9ce0] ActivityChainId:[f836410c-efd1-444e-9189-2902db1d782d]",
            };

            string[] expectedLines = new string[]
                {
                    "++++++ File: ++++++",
                    "[23.10.2015 09:26:44.560] [LAIHEH] [LAIHEH] [ActivityStart] [Generating token] [88f66500-7e1f-4a78-831a-c012a35a656f] ParentId:[269a7a4c-e235-4640-98a7-08b081dc4130] ActivityChainId:[66a7eef2-c314-40dd-a865-038de097ca9f]",
                    "[23.10.2015 09:26:45.482] [LAIHEH] [LAIHEH] [ActivityStop ] [Generating token](925 ms) [88f66500-7e1f-4a78-831a-c012a35a656f] ParentId:[269a7a4c-e235-4640-98a7-08b081dc4130] ActivityChainId:[66a7eef2-c314-40dd-a865-038de097ca9f]",
                    "[23.10.2015 09:26:45.482] [LAIHEH] [LAIHEH] [ActivityStop ] [Issue](973 ms) [269a7a4c-e235-4640-98a7-08b081dc4130] ParentId:[b9327fce-3da4-4182-abbf-f75493f5f4ea] ActivityChainId:[66a7eef2-c314-40dd-a865-038de097ca9f]",
                    "[23.10.2015 09:27:29.877] [LAIHEH] [LAIHEH] [ActivityStart] [Issue] [35cbea68-867d-48e8-9bf2-625d103a6e9d] ParentId:[09dcdfe7-b3d3-4ff2-9187-c197370f1134] ActivityChainId:[09f5ff36-8ab0-419b-b11b-865478da041b]",
                    "[11.11.2015 15:57:05.278] [LAASSI] [LAASSI] [ActivityStart] [Generating token] [c4b769b7-10f4-4889-bc08-a0c7bea81a84] ParentId:[8f572baf-8458-4a9d-8204-5e48d9bb9ce0] ActivityChainId:[f836410c-efd1-444e-9189-2902db1d782d]",
                };

            var tempDir = CreateTempDirectory();
            var tempFileName = Path.Combine(tempDir, "log.log");
            Debug.WriteLine(tempFileName);
            using (var stringWriter = new StringWriter())
            {
                var writer = new LineTextWriter(stringWriter);
                string[] writtenLines;
                var watcher = new Watcher();
                using (var logger = new LoggerEmulator(FileNames.LogFile9, tempFileName))
                {
                    logger.WriteToEnd();

                    watcher.LineWriter = writer;
                    watcher.DirectoryPath = tempDir;
                    watcher.Filter = "*.log";
                    watcher.LineCount = 4;
                    watcher.Start();

                    logger.WriteLines(newLines);
                }

                Thread.Sleep(1000);
                watcher.Stop();
                writer.Flush();
                writtenLines = stringWriter.ToString().ToLines();
                CompareLines(expectedLines, writtenLines);
            }
        }

        [TestMethod]
        public void ShouldDetectFileRollover()
        {
            string[] newLines = new string[]
            {
                "[11.11.2015 15:57:05.278] [LAASSI] [LAASSI] [ActivityStart] [Generating token] [c4b769b7-10f4-4889-bc08-a0c7bea81a84] ParentId:[8f572baf-8458-4a9d-8204-5e48d9bb9ce0] ActivityChainId:[f836410c-efd1-444e-9189-2902db1d782d]",
            };

            string[] expectedLines = new string[]
                {
                    "[23.10.2015 09:26:44.560] [LAIHEH] [LAIHEH] [ActivityStart] [Generating token] [88f66500-7e1f-4a78-831a-c012a35a656f] ParentId:[269a7a4c-e235-4640-98a7-08b081dc4130] ActivityChainId:[66a7eef2-c314-40dd-a865-038de097ca9f]",
                    "[23.10.2015 09:26:45.482] [LAIHEH] [LAIHEH] [ActivityStop ] [Generating token](925 ms) [88f66500-7e1f-4a78-831a-c012a35a656f] ParentId:[269a7a4c-e235-4640-98a7-08b081dc4130] ActivityChainId:[66a7eef2-c314-40dd-a865-038de097ca9f]",
                    "[23.10.2015 09:26:45.482] [LAIHEH] [LAIHEH] [ActivityStop ] [Issue](973 ms) [269a7a4c-e235-4640-98a7-08b081dc4130] ParentId:[b9327fce-3da4-4182-abbf-f75493f5f4ea] ActivityChainId:[66a7eef2-c314-40dd-a865-038de097ca9f]",
                    "[23.10.2015 09:27:29.877] [LAIHEH] [LAIHEH] [ActivityStart] [Issue] [35cbea68-867d-48e8-9bf2-625d103a6e9d] ParentId:[09dcdfe7-b3d3-4ff2-9187-c197370f1134] ActivityChainId:[09f5ff36-8ab0-419b-b11b-865478da041b]",
                    "[11.11.2015 15:57:05.278] [LAASSI] [LAASSI] [ActivityStart] [Generating token] [c4b769b7-10f4-4889-bc08-a0c7bea81a84] ParentId:[8f572baf-8458-4a9d-8204-5e48d9bb9ce0] ActivityChainId:[f836410c-efd1-444e-9189-2902db1d782d]",
                };

            var tempDir = CreateTempDirectory();
            var logTempFilePath = Path.Combine(tempDir, "log.log");
            var oldLogTempFilePath = Path.Combine(tempDir, "log.old");
            Debug.WriteLine(logTempFilePath);

            var writer = new LineTextWriter(Console.Out);
            File.Copy(FileNames.LogFile9, logTempFilePath);
            var watcher = new Watcher();
            watcher.LineWriter = writer;
            watcher.DirectoryPath = tempDir;
            watcher.Filter = "*.log";
            watcher.LineCount = 4;
            watcher.Start();

            File.Move(logTempFilePath, oldLogTempFilePath);
            EmulateLog(FileNames.LogFile9, logTempFilePath);

            Thread.Sleep(1000);
            watcher.Stop();
            //writer.Flush();
            //writtenLines = writer.ToString().ToLines();
            //CompareLines(expectedLines, writtenLines);
        }


        [TestMethod]
        public void ShouldDetectFileDelete()
        {
            var tempDir = CreateTempDirectory();
            var logTempFilePath = Path.Combine(tempDir, "log.log");
            Debug.WriteLine(logTempFilePath);

            File.Copy(FileNames.LogFile9, logTempFilePath);
            var watcher = new Watcher();
            watcher.LineWriter = new LineTextWriter(Console.Out);
            watcher.DirectoryPath = tempDir;
            watcher.Filter = "*.log";
            watcher.LineCount = 4;
            watcher.Start();

            File.Delete(logTempFilePath);

            Thread.Sleep(1000);
            watcher.Stop();
            //writer.Flush();
            //writtenLines = writer.ToString().ToLines();
            //CompareLines(expectedLines, writtenLines);
        }

        private void EmulateLog(string sourceFilePath, string targetFilePath)
        {
            using (var logger = new LoggerEmulator(sourceFilePath, targetFilePath))
            {
                logger.WriteToEnd();
            }
        }

        private string CreateTempDirectory()
        {
            var dateTimeDir = DateTime.Now.ToString("yyyy-MM-ddTHH.mm.ss.fff");
            var tempPath = Path.Combine(Path.GetTempPath(), dateTimeDir);
            Directory.CreateDirectory(tempPath);
            return tempPath;
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
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                var exp = expected[i];
                var act = actual[i];
                if (!(exp.StartsWith(FileNameMarker) && act.StartsWith(FileNameMarker)))
                {
                    Assert.AreEqual(exp, act, "i = " + i.ToString());
                }
            }
        }

        //private void DumpLines(string[] lines)
        //{
        //    foreach (var line in lines)
        //    {
        //        Debug.WriteLine(line);
        //    }
        //}

        //[TestMethod]
        //public void ShouldPeek()
        //{
        //    string filePath = FileNames.LogFile9;
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