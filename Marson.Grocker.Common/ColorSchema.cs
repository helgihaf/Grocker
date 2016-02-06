using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Marson.Grocker.Common
{
    public class ColorSchema
    {
        private Regex regex;

        /// <summary>
        /// Unique name of the schema
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Optional Regex pattern used as criteria for auto-selecting the schema based on a selection of input.
        /// </summary>
        public string SelectorPattern { get; set; }

        /// <summary>
        /// The color filters that belong to this schema.
        /// </summary>
        public List<ColorFilter> Filters { get; set; } = new List<ColorFilter>();

        /// <summary>
        /// Checks if the specified line matches the SelectorPattern.
        /// </summary>
        /// <param name="line">A log line</param>
        /// <returns>True if the line matches the SelectorPattern, false otherwise.</returns>
        public bool IsMatch(string line)
        {
            if (regex == null)
            {
                regex = new Regex(SelectorPattern, RegexOptions.Compiled);
            }
            return regex.IsMatch(line);
        }

        /// <summary>
        /// Gets a filter that matches the specified line.
        /// </summary>
        /// <param name="line">A log line</param>
        /// <returns>A filter that matches the specified line or null if no match is found.</returns>
        public ColorFilter GetMatchingFilter(string line)
        {
            ColorFilter result = null;
            foreach (var colorFilter in Filters)
            {
                if (colorFilter.IsMatch(line))
                {
                    result = colorFilter;
                    break;
                }
            }

            return result;
        }
    }
}
