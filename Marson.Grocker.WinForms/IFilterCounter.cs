using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marson.Grocker.WinForms
{
    public interface IFilterCounter
    {
        void Increment(string filterName);
    }
}
