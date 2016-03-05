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
                savedFilter = new ColorFilter { ForegroundColor = ColorMap.ToString(Console.ForegroundColor), BackgroundColor = ColorMap.ToString(Console.BackgroundColor) };
                ApplyFilter(colorFilter);
            }
        }

        private static void ApplyFilter(ColorFilter colorFilter)
        {
            Console.ForegroundColor = ColorMap.ToConsoleColor(colorFilter.ForegroundColor);
            Console.BackgroundColor = ColorMap.ToConsoleColor(colorFilter.BackgroundColor);
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