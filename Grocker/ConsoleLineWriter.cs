using Marson.Grocker.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grocker
{
    public class ConsoleLineWriter : ILineWriter
    {
        private readonly TextWriter implTextWriter = Console.Out;
        private readonly List<ColorSchema> colorSchemas = new List<ColorSchema>();

        private bool autoDetectColorSchema = true;
        private bool autoDetectionPending = true;
        private ColorSchema colorSchema;

        public bool AutoDetectColorSchema
        {
            get
            {
                return autoDetectColorSchema;
            }
            set
            {
                autoDetectColorSchema = value;
                autoDetectionPending = value;
            }
        }

        public List<ColorSchema> ColorSchemas
        {
            get { return colorSchemas; }
            set
            {
                colorSchemas.Clear();
                colorSchemas.AddRange(value);
            }
        }

        public ColorSchema ColorSchema
        {
            get { return colorSchema; }
            set { colorSchema = value; }
        }

        public void WriteLine(string line)
        {
            if (autoDetectionPending)
            {
                TryDetectSchema(new[] { line });
            }
            using (var colorScope = CreateColorScope(line))
            {
                implTextWriter.WriteLine(line);
            }
        }

        public void WriteLines(string[] lines)
        {
            if (autoDetectionPending)
            {
                TryDetectSchema(lines);
            }
            foreach (var line in lines)
            {
                using (var colorScope = CreateColorScope(line))
                {
                    implTextWriter.WriteLine(line);
                }
            }
        }

        private void TryDetectSchema(string[] lines)
        {
            if (colorSchemas == null || colorSchemas.Count == 0)
            {
                throw new InvalidOperationException("Cannot detect color schema when none have been made available");
            }

            int maxScore = 0;
            int indexOfMax = 0;

            for (int i = 0; i < colorSchemas.Count; i++)
            {
                var cs = colorSchemas[i];
                if (!string.IsNullOrEmpty(cs.SelectorPattern))
                {
                    int score = 0;
                    foreach (var line in lines)
                    {
                        if (line != null && cs.IsMatch(line))
                        {
                            score++;
                        }
                    }
                    if (maxScore < score)
                    {
                        maxScore = score;
                        indexOfMax = i;
                    }
                }
            }

            if (maxScore > 0)
            {
                colorSchema = colorSchemas[indexOfMax];
                autoDetectionPending = false;
            }
        }

        public void Flush()
        {
            implTextWriter.Flush();
        }


        private ColorScope CreateColorScope(string line)
        {
            return new ColorScope(ColorSchema?.GetMatchingFilter(line));
        }

    }
}
