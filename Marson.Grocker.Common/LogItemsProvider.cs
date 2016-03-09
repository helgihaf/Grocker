using Marson.Grocker.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marson.Grocker.Common
{
    public class LogItemsProvider : IItemsProvider<string>
    {
        private readonly LogFile logFile;

        public LogItemsProvider(LogFile logFile)
        {
            if (logFile == null)
            {
                throw new ArgumentNullException(nameof(logFile));
            }

            this.logFile = logFile;
        }

        public int GetCount()
        {
            return logFile.Lines.Count;
        }

        public IList<string> GetRange(int startIndex, int count)
        {
            if (startIndex < logFile.Lines.Count)
            {
                return logFile.LoadLines(startIndex, count);
            }
            else
            {
                return new List<string>();
            }
        }

    }
}
