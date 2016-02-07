using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Marson.Grocker.Common;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Text;

namespace Marson.Grocker.UnitTests
{
    public class TestData : IDisposable
    {
        public static readonly Statistics SoaLogLineStatistics = new Statistics
        {
            Average = 162,
            Variance = 95.2345026353084,
            Min = 13,
            Max = 325,
        };

        private readonly Random random = new Random();
        private readonly Statistics statistics;
        private string directory;

        public TestData()
        {
            statistics = SoaLogLineStatistics;
        }

        public TestData(Statistics statistics)
        {
            this.statistics = statistics;
        }

        public void Dispose()
        {
            if (directory != null && Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }
        }


        public string GetLogFile(int lineCount)
        {
            if (directory == null)
            {
                directory = CreateTempDirectory();
            }

            string filePath = Path.Combine(directory, CreateTempLogFileName());
            using (var writer = new StreamWriter(filePath))
            {
                for (int i = 0; i < lineCount; i++)
                {
                    int lineLength = GetRandomLineLength();
                    WriteLogLine(writer, lineLength);
                }
            }

            return filePath;
        }

        public static string[] LoadLines(string filePath, int startIndex, int lineCount)
        {
            if (startIndex < 0)
            {
                throw new ArgumentException(nameof(startIndex) + " must be 0 or greater");
            }
            if (lineCount <= 0)
            {
                throw new ArgumentException(nameof(lineCount) + " must be greater than 0");
            }

            var result = new string[lineCount];
            using (var reader = new StreamReader(filePath))
            {
                string line;
                int index = 0;
                while ((line = reader.ReadLine()) != null && (index - startIndex) < lineCount)
                {
                    int lineIndex = index - startIndex;
                    if (lineIndex >= 0)
                    {
                        result[lineIndex] = line;
                    }
                    index++;
                }
            }

            return result;
        }

        private int GetRandomLineLength()
        {
            double gaussian;
            double standardDeviation = Math.Sqrt(statistics.Variance);
            do
            {
                gaussian = random.NextGaussian(statistics.Average, standardDeviation);
            } while (gaussian < statistics.Min || gaussian > statistics.Max);

            return (int)gaussian;
        }

        private void WriteLogLine(StreamWriter writer, int lineLength)
        {
            writer.WriteLine(TestLineCreator.Create(random, lineLength));
        }

        private string CreateTempLogFileName()
        {
            return "Log_" + Guid.NewGuid().ToString("N") + ".log";
        }

        public static string CreateTempDirectory()
        {
            var dateTimeDir = DateTime.Now.ToString("yyyy-MM-ddTHH.mm.ss.fff");
            var tempPath = Path.Combine(Path.GetTempPath(), dateTimeDir);
            Directory.CreateDirectory(tempPath);
            return tempPath;
        }

        public string[] CreateLogLines(int count)
        {
            var result = new string[count];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = TestLineCreator.Create(random, GetRandomLineLength());
            }
            return result;
        }
    }
}
