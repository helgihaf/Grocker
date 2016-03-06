using Marson.Grocker.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Marson.Grocker.WinForms
{
    internal class ColorScope : IDisposable
    {
        private readonly RichTextBox box;
        private ColorFilter colorFilter;

        public ColorScope(RichTextBox richTextBox, ColorFilter colorFilter)
        {
            this.box = richTextBox;
            this.colorFilter = colorFilter;
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;
            if (colorFilter != null)
            {
                box.SelectionColor = ColorMap.ToDrawingColor(colorFilter.ForegroundColor);
                box.SelectionBackColor = ColorMap.ToDrawingColor(colorFilter.BackgroundColor);
            }
        }

        public void Dispose()
        {
            if (colorFilter != null)
            {
                box.SelectionColor = box.ForeColor;
                box.SelectionBackColor = box.BackColor;
            }
        }

    }
}
