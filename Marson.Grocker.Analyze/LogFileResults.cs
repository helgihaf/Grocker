namespace Marson.Grocker.Analyze
{
    public class LogFileResults
    {
        public LogFileResults()
        {
            LineLengths = new LineLengths();
            CharacterFrequencies = new Frequencies<char>();
        }

        public string FilePath { get; internal set; }
        public int LineCount { get; internal set; }
        public string Encoding { get; internal set; }
        public LineLengths LineLengths { get; private set; }
        public Frequencies<char> CharacterFrequencies { get; private set; }
    }
}