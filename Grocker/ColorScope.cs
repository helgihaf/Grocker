using System;
using Marson.Grocker.Common;

namespace Grocker
{
    internal class ColorScope : IDisposable
    {
        private ColorFilter<ConsoleColor> savedFilter;

        public ColorScope(ColorFilter<ConsoleColor> colorFilter)
        {
            if (colorFilter != null)
            {
                savedFilter = new ColorFilter<ConsoleColor> { ForegroundColor = Console.ForegroundColor, BackgroundColor = Console.BackgroundColor };
                ApplyFilter(colorFilter);
            }
        }

        private static void ApplyFilter(ColorFilter<ConsoleColor> colorFilter)
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