using System;
using System.Drawing;
using System.Text.RegularExpressions;

namespace Marson.Grocker.Common
{
    public class ColorFilter
    {
        private Regex regex;

        /// <summary>
        /// Optional name of the filter
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Regex pattern that is used to determine whether to apply this color filter.
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// The foreground color to apply
        /// </summary>
        public string ForegroundColor { get; set; }

        /// <summary>
        /// The background color to apply
        /// </summary>
        public string BackgroundColor { get; set; }

        /// <summary>
        /// Checks if the specified line matches the Pattern.
        /// </summary>
        /// <param name="line">A log line</param>
        /// <returns>True if the line matches the Pattern, false otherwise.</returns>
        public bool IsMatch(string line)
        {
            if (regex == null)
            {
                regex = new Regex(Pattern, RegexOptions.Compiled);
            }
            return regex.IsMatch(line);
        }
    }
}