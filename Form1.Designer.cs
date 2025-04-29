namespace Minesweeper
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            groupBox1 = new GroupBox();
            tableGrid = new TableLayoutPanel();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(tableGrid);
            groupBox1.Location = new Point(11, 249);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(407, 489);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "FieldContainer";
            // 
            // tableGrid
            // 
            tableGrid.AutoSize = true;
            tableGrid.BackgroundImageLayout = ImageLayout.Zoom;
            tableGrid.ColumnCount = 9;
            tableGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 11.1111107F));
            tableGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 11.1111107F));
            tableGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 11.1111107F));
            tableGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 11.1111107F));
            tableGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 11.1111107F));
            tableGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 11.1111107F));
            tableGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 11.1111107F));
            tableGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 11.1111107F));
            tableGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 11.1111107F));
            tableGrid.Dock = DockStyle.Fill;
            tableGrid.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            tableGrid.Location = new Point(3, 23);
            tableGrid.Name = "tableGrid";
            tableGrid.RowCount = 9;
            tableGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 11.1111107F));
            tableGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 11.1111107F));
            tableGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 11.1111107F));
            tableGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 11.1111107F));
            tableGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 11.1111107F));
            tableGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 11.1111107F));
            tableGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 11.1111107F));
            tableGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 11.1111107F));
            tableGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 11.1111107F));
            tableGrid.Size = new Size(401, 463);
            tableGrid.TabIndex = 0;
            tableGrid.Paint += tableGrid_Paint;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1024, 768);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private TableLayoutPanel tableGrid;
    }
}
