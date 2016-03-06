using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marson.Grocker.WinForms
{
    public interface IWatcherViewHost
    {
        void Attach(WatcherView watcherView);
    }
}
