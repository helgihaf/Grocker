using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Marson.Grocker.Common;

namespace Marson.Grocker.WinForms
{
    public partial class ColorFilterSummary2 : UserControl, IFilterCounter
    {
        private readonly Dictionary<string, Label> filters = new Dictionary<string, Label>();
        private ColorSchema colorSchema;

        public ColorFilterSummary2()
        {
            InitializeComponent();
        }

        public void AnnounceColorSchema(ColorSchema colorSchema)
        {
            this.colorSchema = colorSchema;
            SetUpTable();
        }

        public void Increment(string filterName)
        {
            Label label = filters[filterName];
            SetCounter(label, GetCounter(label) + 1);
        }

        private void SetUpTable()
        {
            filters.Clear();
            if (colorSchema == null || colorSchema.Filters.Count == 0)
            {
                tableLayoutPanel.ColumnCount = 1;
                return;
            }

            tableLayoutPanel.ColumnCount = colorSchema.Filters.Count * 2;
            float percentPerFilter = 100 / colorSchema.Filters.Count;
            int column = 0;
            foreach (var filter in colorSchema.Filters)
            {
                AddTextLabel(filter.Name, percentPerFilter * 2f / 3f, column++);
                Label label = AddNumberLabel(percentPerFilter / 3f, column++);
                filters.Add(filter.Name, label);
            }
        }

        private void AddTextLabel(string text, float percent, int column)
        {
            AddLabel(text, percent, ContentAlignment.MiddleRight, column);
        }

        private Label AddNumberLabel(float percent, int column)
        {
            return AddLabel("0", percent, ContentAlignment.MiddleLeft, column);
        }

        private Label AddLabel(string text, float percent, ContentAlignment contentAlignment, int column)
        {
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, percent));
            Label label = new Label();
            label.Text = text;
            label.Dock = DockStyle.Fill;
            label.TextAlign = contentAlignment;
            tableLayoutPanel.Controls.Add(label, column, 0);
            return label;
        }

        private void SetCounter(Label label, int value)
        {
            label.Text = value.ToString();
        }

        private int GetCounter(Label label)
        {
            return int.Parse(label.Text);
        }
    }
}
