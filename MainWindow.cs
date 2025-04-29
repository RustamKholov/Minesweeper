namespace Minesweeper
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeGameGrid(9, 9);
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableGrid_Paint(object sender, PaintEventArgs e)
        {

        }
        private void InitializeGameGrid(int rows, int cols)
        {

            tableGrid.Controls.Clear();
            tableGrid.ColumnStyles.Clear();
            tableGrid.RowStyles.Clear();
            tableGrid.RowCount = rows;
            tableGrid.ColumnCount = cols;

            for (int i = 0; i < cols; i++)
                tableGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / cols));

            for (int i = 0; i < rows; i++)
                tableGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / rows));

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    Button btn = new Button();
                    btn.Dock = DockStyle.Fill;
                    btn.BackColor = Color.LightGray;
                    btn.Margin = new Padding(0);
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 1;
                    btn.FlatAppearance.BorderColor = Color.Gray;
                    btn.Tag = (row, col); // optional
                    btn.Click += Cell_Click;

                    tableGrid.Controls.Add(btn, col, row);
                    btn.MouseDown += (s, e) =>
                    {
                        if (e.Button == MouseButtons.Right)
                        {
                            btn.Text = "💣";
                        }
                    };
                    btn.MouseEnter += (s, e) =>
                    {
                        btn.BackColor = Color.Gray;
                    };
                    btn.MouseLeave += (s, e) =>
                    {
                        btn.BackColor = Color.LightGray;
                    };
                }
            }
        }
        private void Cell_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is ValueTuple<int, int> coords)
            {
                int row = coords.Item1;
                int col = coords.Item2;
                MessageBox.Show($"Clicked cell at ({row}, {col})");

                // TODO: call GameEngine.RevealCell(row, col);
                // update button UI based on result
            }
        }

        private void Menu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}
