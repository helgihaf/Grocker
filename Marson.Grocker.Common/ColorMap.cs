using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marson.Grocker.Common
{
    public static class ColorMap
    {
        private static Dictionary<ConsoleColor, Color> map = new Dictionary<ConsoleColor, Color>()
        {
            // Map based on http://stackoverflow.com/questions/28211009/what-are-the-actual-rgb-values-of-the-consolecolors
            [ConsoleColor.Black] = Color.FromArgb(0x000000),
            [ConsoleColor.DarkBlue] = Color.FromArgb(0x000080),
            [ConsoleColor.DarkGreen] = Color.FromArgb(0x008000),
            [ConsoleColor.DarkCyan] = Color.FromArgb(0x008080),
            [ConsoleColor.DarkRed] = Color.FromArgb(0x800000),
            [ConsoleColor.DarkMagenta] = Color.FromArgb(0x800080),
            [ConsoleColor.DarkYellow] = Color.FromArgb(0x808000),
            [ConsoleColor.Gray] = Color.FromArgb(0xC0C0C0),
            [ConsoleColor.DarkGray] = Color.FromArgb(0x808080),
            [ConsoleColor.Blue] = Color.FromArgb(0x0000FF),
            [ConsoleColor.Green] = Color.FromArgb(0x00FF00),
            [ConsoleColor.Cyan] = Color.FromArgb(0x00FFFF),
            [ConsoleColor.Red] = Color.FromArgb(0xFF0000),
            [ConsoleColor.Magenta] = Color.FromArgb(0xFF00FF),
            [ConsoleColor.Yellow] = Color.FromArgb(0xFFFF00),
            [ConsoleColor.White] = Color.FromArgb(0xFFFFFF),
        };

        public static Color GetDrawingColor(ConsoleColor consoleColor)
        {
            return map[consoleColor];
        }

        public static ConsoleColor ToConsoleColor(string color)
        {
            ConsoleColor result;
            if (!Enum.TryParse<ConsoleColor>(color, out result))
            {
                throw new ArgumentException("Invalid color name", nameof(color));
            }
            return result;
        }

        public static string ToString(ConsoleColor color)
        {
            return color.ToString();
        }

        public static Color ToDrawingColor(string color)
        {
            if (string.IsNullOrEmpty(color))
            {
                throw new ArgumentException("Invalid color name", nameof(color));
            }

            if (color.StartsWith("#"))
            {
                return HexToColor(color.Substring(1));
            }
            else if (color.StartsWith("0x"))
            {
                return HexToColor(color.Substring(2));
            }
            else
            {
                Color? drawingColor = FindDrawingColor(color);
                if (drawingColor == null)
                {
                    drawingColor = GetDrawingColor(ToConsoleColor(color));
                }
                return drawingColor.Value;
            }
        }

        private static Color HexToColor(string hex)
        {
            return Color.FromArgb(int.Parse(hex, System.Globalization.NumberStyles.HexNumber));
        }


        private static Color? FindDrawingColor(string color)
        {
            try
            {
                return Color.FromName(color);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

    }
}
