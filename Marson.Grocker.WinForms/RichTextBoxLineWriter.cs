using Marson.Grocker.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Marson.Grocker.WinForms
{
    internal class RichTextBoxLineWriter : ILineWriter
    {
        private readonly RichTextBox box;
        private readonly List<ColorSchema<Color>> colorSchemas = new List<ColorSchema<Color>>();

        private bool autoDetectColorSchema = true;
        private bool autoDetectionPending = true;
        private ColorSchema<Color> colorSchema;

        public RichTextBoxLineWriter(RichTextBox box)
        {
            this.box = box;
        }

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

        public List<ColorSchema<Color>> ColorSchemas
        {
            get { return colorSchemas; }
            set
            {
                colorSchemas.Clear();
                colorSchemas.AddRange(value);
            }
        }

        public ColorSchema<Color> ColorSchema
        {
            get { return colorSchema; }
            set { colorSchema = value; }
        }

        public void WriteLine(string line)
        {
            if (box.InvokeRequired)
            {
                box.Invoke((MethodInvoker)delegate { DoWriteLine(line); });
            }
            else
            {
                DoWriteLine(line);
            }
        }

        private void DoWriteLine(string line)
        {
            if (autoDetectionPending)
            {
                TryDetectSchema(new[] { line });
            }
            using (var colorScope = CreateColorScope(line))
            {
                box.AppendText(line + Environment.NewLine);
            }
        }

        public void WriteLines(string[] lines)
        {
            if (box.InvokeRequired)
            {
                box.Invoke((MethodInvoker)delegate { DoWriteLines(lines); });
            }
            else
            {
                DoWriteLines(lines);
            }
        }

        private void DoWriteLines(string[] lines)
        {
            if (autoDetectionPending)
            {
                TryDetectSchema(lines);
            }
            foreach (var line in lines)
            {
                using (var colorScope = CreateColorScope(line))
                {
                    box.AppendText(line + Environment.NewLine);
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
        }


        private ColorScope CreateColorScope(string line)
        {
            return new ColorScope(box, ColorSchema?.GetMatchingFilter(line));
        }
    }
}
