using Marson.Grocker.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrockerWpf
{
    class LogLinesItemProvider : IItemsProvider<ModelLine>
    {
        private readonly LogFile logFile;

        public LogLinesItemProvider(LogFile logFile)
        {
            this.logFile = logFile;
        }

        public int FetchCount()
        {
            return logFile.Lines.Count;
        }

        public IList<ModelLine> FetchRange(int startIndex, int count)
        {
            var window = logFile.CreateWindow();
            window.Length = count;
            window.CurrentIndex = startIndex;
            var list = new List<ModelLine>();
            for (int i = 0; i < window.Lines.Length; i++)
            {
                list.Add(new ModelLine { Index = startIndex + i, Text = window.Lines[i] });
            }
            return list;
        }
    }
}
