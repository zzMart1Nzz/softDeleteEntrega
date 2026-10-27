using System.Drawing;
using System.Windows.Forms;

namespace softDeleteEntrega.Ui
{
    internal static class Itxura
    {
        public static readonly Color AtzekoKolorea = Color.FromArgb(243, 246, 249);
        public static readonly Color Iluna = Color.FromArgb(22, 42, 61);
        public static readonly Color Nagusia = Color.FromArgb(0, 111, 119);
        public static readonly Color Bigarrena = Color.FromArgb(45, 80, 133);
        public static readonly Color Testua = Color.FromArgb(32, 50, 68);
        public static readonly Color Apala = Color.FromArgb(90, 109, 127);

        public static Label Etiketa(string text, float size, bool bold = false)
        {
            return new Label
            {
                Text = text, AutoSize = false, ForeColor = Testua,
                Font = new Font("Segoe UI", size, bold ? FontStyle.Bold : FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill
            };
        }

        public static Button Botoia(string text, Color color, int width = 150)
        {
            var button = new Button
            {
                Text = text, Width = width, Height = 42, BackColor = color, ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand, Margin = new Padding(0, 0, 12, 0), UseVisualStyleBackColor = false
            };
            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        public static Button Txartela(string text, Color color)
        {
            var button = Botoia(text, color);
            button.Dock = DockStyle.Fill;
            button.Margin = new Padding(10);
            button.Font = new Font("Segoe UI", 23, FontStyle.Bold);
            button.TextAlign = ContentAlignment.MiddleCenter;
            return button;
        }

        public static DataGridView Taula()
        {
            var grid = new DataGridView
            {
                Dock = DockStyle.Fill, AutoGenerateColumns = false,
                AllowUserToAddRows = false, AllowUserToDeleteRows = false, AllowUserToResizeRows = false,
                ReadOnly = true, MultiSelect = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None, CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.FromArgb(229, 235, 241), RowHeadersVisible = false,
                EnableHeadersVisualStyles = false, ColumnHeadersHeight = 46,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            };
            grid.ColumnHeadersDefaultCellStyle.BackColor = Iluna;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 0, 0, 0);
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            grid.DefaultCellStyle.ForeColor = Testua;
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(218, 239, 239);
            grid.DefaultCellStyle.SelectionForeColor = Testua;
            grid.DefaultCellStyle.Padding = new Padding(10, 0, 0, 0);
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(247, 249, 251);
            grid.RowTemplate.Height = 46;
            return grid;
        }

        public static void Zutabea(DataGridView grid, string property, string title, int weight = 100)
        {
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = property, DataPropertyName = property, HeaderText = title,
                FillWeight = weight, MinimumWidth = 60
            });
        }

        public static void Ekintza(DataGridView grid, string name, string text, Color color)
        {
            var column = new DataGridViewButtonColumn
            {
                Name = name, HeaderText = text, Text = text, UseColumnTextForButtonValue = true,
                FlatStyle = FlatStyle.Flat, AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                Width = 138, MinimumWidth = 125, SortMode = DataGridViewColumnSortMode.NotSortable
            };
            column.DefaultCellStyle.BackColor = color;
            column.DefaultCellStyle.ForeColor = Color.White;
            column.DefaultCellStyle.SelectionBackColor = color;
            column.DefaultCellStyle.SelectionForeColor = Color.White;
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.DefaultCellStyle.Padding = new Padding(4);
            grid.Columns.Add(column);
        }
    }
}
