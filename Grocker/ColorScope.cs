using System;
using Marson.Grocker.Common;

namespace Grocker
{
    internal class ColorScope : IDisposable
    {
        private ColorFilter savedFilter;

        public ColorScope(ColorFilter colorFilter)
        {
            if (colorFilter != null)
            {
                savedFilter = new ColorFilter { ForegroundColor = Console.ForegroundColor, BackgroundColor = Console.BackgroundColor };
                ApplyFilter(colorFilter);
            }
        }

        private static void ApplyFilter(ColorFilter colorFilter)
        {
            Console.ForegroundColor = colorFilter.ForegroundColor;
            Console.BackgroundColor = colorFilter.BackgroundColor;
        }

        public void Dispose()
        {
            if (savedFilter != null)
            {
                ApplyFilter(savedFilter);
            }
        }
    }
}