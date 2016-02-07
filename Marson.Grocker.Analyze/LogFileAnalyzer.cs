using System;
using System.IO;

namespace Marson.Grocker.Analyze
{
    public class LogFileAnalyzer
    {

        public LineLengths LineLengths { get; } = new LineLengths();
        public Frequencies<char> CharacterFrequencies { get; } = new Frequencies<char>();

        public LogFileResults Analyze(string logFilePath)
        {
            using (var stream = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
            using (var reader = new StreamReader(stream, true))
            {
                var results = new LogFileResults();
                results.FilePath = logFilePath;

                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    foreach (var c in line)
                    {
                        results.CharacterFrequencies.Add(c);
                    }
                    results.LineCount++;
                    results.LineLengths.Add(line.Length);
                }

                results.Encoding = reader.CurrentEncoding.EncodingName;
                LineLengths.Add(results.LineLengths);
                CharacterFrequencies.Add(results.CharacterFrequencies);

                return results;
            }
        }
    }
}