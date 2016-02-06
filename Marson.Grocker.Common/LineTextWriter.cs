using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marson.Grocker.Common
{
    public class LineTextWriter : ILineWriter
    {
        private readonly TextWriter textWriter;

        public LineTextWriter(TextWriter textWriter)
        {
            if (textWriter == null)
            {
                throw new ArgumentNullException(nameof(textWriter));
            }
            this.textWriter = textWriter;
        }

        public void WriteLine(string line)
        {
            textWriter.WriteLine(line);
        }

        public void WriteLines(string[] lines)
        {
            foreach (var line in lines)
            {
                textWriter.WriteLine(line);
            }
        }

        public void Flush()
        {
            textWriter.Flush();
        }


    }
}
