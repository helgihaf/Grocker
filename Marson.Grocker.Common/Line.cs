using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marson.Grocker.Common
{
    public class Line
    {
        /// <summary>
        /// Index within file
        /// </summary>
        public long Index { get; set; }

        /// <summary>
        /// Length in bytes
        /// </summary>
        public int Length { get; set; }

    }
}
