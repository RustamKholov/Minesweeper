namespace Minesweeper
{
    partial class MainWindow
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
            fieldBox = new GroupBox();
            tableGrid = new TableLayoutPanel();
            InfoBox = new GroupBox();
            Menu = new ToolStrip();
            SizeLabal = new ToolStripLabel();
            SizeComboBox = new ToolStripComboBox();
            SizeButton = new ToolStripButton();
            fieldBox.SuspendLayout();
            Menu.SuspendLayout();
            SuspendLayout();
            // 
            // fieldBox
            // 
            fieldBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            fieldBox.Controls.Add(tableGrid);
            fieldBox.Location = new Point(16, 164);
            fieldBox.Name = "fieldBox";
            fieldBox.Size = new Size(408, 530);
            fieldBox.TabIndex = 0;
            fieldBox.TabStop = false;
            fieldBox.Text = "FieldContainer";
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
            tableGrid.Size = new Size(402, 504);
            tableGrid.TabIndex = 0;
            tableGrid.Paint += tableGrid_Paint;
            // 
            // InfoBox
            // 
            InfoBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            InfoBox.Location = new Point(16, 89);
            InfoBox.Name = "InfoBox";
            InfoBox.Size = new Size(405, 69);
            InfoBox.TabIndex = 1;
            InfoBox.TabStop = false;
            InfoBox.Text = "Info";
            // 
            // Menu
            // 
            Menu.GripStyle = ToolStripGripStyle.Hidden;
            Menu.ImageScalingSize = new Size(20, 20);
            Menu.Items.AddRange(new ToolStripItem[] { SizeLabal, SizeComboBox, SizeButton });
            Menu.Location = new Point(10, 10);
            Menu.Name = "Menu";
            Menu.RenderMode = ToolStripRenderMode.Professional;
            Menu.Size = new Size(420, 28);
            Menu.TabIndex = 3;
            Menu.Text = "toolStrip1";
            Menu.ItemClicked += Menu_ItemClicked;
            // 
            // SizeLabal
            // 
            SizeLabal.Name = "SizeLabal";
            SizeLabal.Size = new Size(36, 25);
            SizeLabal.Text = "Size";
            // 
            // SizeComboBox
            // 
            SizeComboBox.Items.AddRange(new object[] { "Small (440x720)", "Medium", "Big" });
            SizeComboBox.Name = "SizeComboBox";
            SizeComboBox.Size = new Size(121, 28);
            // 
            // SizeButton
            // 
            SizeButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            SizeButton.ImageTransparentColor = Color.Magenta;
            SizeButton.Name = "SizeButton";
            SizeButton.Size = new Size(52, 25);
            SizeButton.Text = "Apply";
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(440, 720);
            Controls.Add(Menu);
            Controls.Add(InfoBox);
            Controls.Add(fieldBox);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Name = "MainWindow";
            Padding = new Padding(10);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Minesweeper";
            Load += Form1_Load;
            fieldBox.ResumeLayout(false);
            fieldBox.PerformLayout();
            Menu.ResumeLayout(false);
            Menu.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GroupBox fieldBox;
        private TableLayoutPanel tableGrid;
        private GroupBox InfoBox;
        private ToolStrip Menu;
        private ToolStripLabel SizeLabal;
        private ToolStripComboBox SizeComboBox;
        private ToolStripButton SizeButton;
    }
}
