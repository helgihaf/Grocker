using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marson.Grocker.Common
{
    public interface ILineWriter
    {
        void WriteLine(string line);
        void WriteLines(string[] lines);
        void Flush();
    }
}
