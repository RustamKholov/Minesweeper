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
            NavigationMenu = new MenuStrip();
            newGameToolStripMenuItem = new ToolStripMenuItem();
            optionsToolStripMenuItem = new ToolStripMenuItem();
            statisticToolStripMenuItem = new ToolStripMenuItem();
            fieldBox.SuspendLayout();
            NavigationMenu.SuspendLayout();
            SuspendLayout();
            // 
            // fieldBox
            // 
            fieldBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            fieldBox.Controls.Add(tableGrid);
            fieldBox.Location = new Point(13, 203);
            fieldBox.Name = "fieldBox";
            fieldBox.Size = new Size(376, 395);
            fieldBox.TabIndex = 0;
            fieldBox.TabStop = false;
            fieldBox.Text = "FieldContainer";
            // 
            // tableGrid
            // 
            tableGrid.AutoSize = true;
            tableGrid.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableGrid.BackgroundImageLayout = ImageLayout.None;
            tableGrid.ColumnCount = 9;
            tableGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40F));
            tableGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40F));
            tableGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40F));
            tableGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40F));
            tableGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40F));
            tableGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40F));
            tableGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40F));
            tableGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40F));
            tableGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40F));
            tableGrid.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            tableGrid.Location = new Point(10, 29);
            tableGrid.Name = "tableGrid";
            tableGrid.RowCount = 9;
            tableGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tableGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tableGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tableGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tableGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tableGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tableGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tableGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tableGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tableGrid.Size = new Size(360, 360);
            tableGrid.TabIndex = 0;
         
            // 
            // InfoBox
            // 
            InfoBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            InfoBox.Location = new Point(22, 80);
            InfoBox.Name = "InfoBox";
            InfoBox.Size = new Size(367, 101);
            InfoBox.TabIndex = 1;
            InfoBox.TabStop = false;
            InfoBox.Text = "Info";
            // 
            // NavigationMenu
            // 
            NavigationMenu.ImageScalingSize = new Size(20, 20);
            NavigationMenu.Items.AddRange(new ToolStripItem[] { newGameToolStripMenuItem, optionsToolStripMenuItem, statisticToolStripMenuItem });
            NavigationMenu.Location = new Point(10, 10);
            NavigationMenu.Name = "NavigationMenu";
            NavigationMenu.Size = new Size(382, 28);
            NavigationMenu.TabIndex = 2;
            NavigationMenu.Text = "menuStrip1";
            
            // 
            // newGameToolStripMenuItem
            // 
            newGameToolStripMenuItem.Name = "newGameToolStripMenuItem";
            newGameToolStripMenuItem.Size = new Size(96, 24);
            newGameToolStripMenuItem.Text = "New Game";
            newGameToolStripMenuItem.Click += newGameToolStripMenuItem_Click;
            // 
            // optionsToolStripMenuItem
            // 
            optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            optionsToolStripMenuItem.Size = new Size(75, 24);
            optionsToolStripMenuItem.Text = "Options";
            // 
            // statisticToolStripMenuItem
            // 
            statisticToolStripMenuItem.Name = "statisticToolStripMenuItem";
            statisticToolStripMenuItem.Size = new Size(75, 24);
            statisticToolStripMenuItem.Text = "Statistic";
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(402, 611);
            Controls.Add(InfoBox);
            Controls.Add(fieldBox);
            Controls.Add(NavigationMenu);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Name = "MainWindow";
            Padding = new Padding(10);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Minesweeper";
            Load += Form1_Load;
            fieldBox.ResumeLayout(false);
            fieldBox.PerformLayout();
            NavigationMenu.ResumeLayout(false);
            NavigationMenu.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GroupBox fieldBox;
        private TableLayoutPanel tableGrid;
        private GroupBox InfoBox;
        private MenuStrip NavigationMenu;
        private ToolStripMenuItem newGameToolStripMenuItem;
        private ToolStripMenuItem optionsToolStripMenuItem;
        private ToolStripMenuItem statisticToolStripMenuItem;
    }
}
