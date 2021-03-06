﻿using Marson.Grocker.Common;
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
        private const int LineCountMax = 512;
        private const int LineCountToClean = 128;

        private readonly RichTextBox box;
        private readonly List<ColorSchema> colorSchemas = new List<ColorSchema>();
        private readonly IFilterCounter filterCounter;

        private bool autoDetectColorSchema = true;
        private bool autoDetectionPending = true;
        private ColorSchema colorSchema;

        public RichTextBoxLineWriter(RichTextBox box, IFilterCounter filterCounter)
        {
            this.box = box;
            this.filterCounter = filterCounter;
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
            CheckAndClean();
            using (var colorScope = CreateColorScope(line))
            {
                box.AppendText(line + Environment.NewLine);
            }
            box.SelectionStart = box.Text.Length;
            box.ScrollToCaret();
        }

        private void CheckAndClean()
        {
            string[] lines = box.Lines;
            if (lines.Length > LineCountMax)
            {
                string[] newLines = new string[lines.Length - LineCountToClean];
                lines.CopyTo(newLines, LineCountToClean);
                box.Lines = newLines;
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
                if (filterCounter != null)
                {
                    filterCounter.AnnounceColorSchema(colorSchema);
                }
            }
        }

        public void Flush()
        {
        }


        private ColorScope CreateColorScope(string line)
        {
            var filter = ColorSchema?.GetMatchingFilter(line);
            if (filter != null && filterCounter != null)
            {
                filterCounter.Increment(filter.Name);
            }
            return new ColorScope(box, filter);
        }
    }
}
