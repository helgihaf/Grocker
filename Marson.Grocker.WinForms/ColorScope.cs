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
        private ColorFilter<Color> colorFilter;

        public ColorScope(RichTextBox richTextBox, ColorFilter<Color> colorFilter)
        {
            this.box = richTextBox;
            this.colorFilter = colorFilter;
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;
            if (colorFilter != null)
            {
                box.SelectionColor = colorFilter.ForegroundColor;
                box.SelectionBackColor = colorFilter.BackgroundColor;
            }
        }

        public void Dispose()
        {
            if (colorFilter != null)
            {
                // hmmm....
                box.SelectionColor = box.ForeColor;
                box.SelectionBackColor = box.BackColor;
            }
        }

    }
}
