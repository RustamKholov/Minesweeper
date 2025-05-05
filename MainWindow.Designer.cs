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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            fieldBox = new GroupBox();
            tableGrid = new TableLayoutPanel();
            InfoBox = new GroupBox();
            NavigationMenu = new MenuStrip();
            newGameToolStripMenuItem = new ToolStripMenuItem();
            optionsToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripMenuItem();
            toolStripMenuItem2 = new ToolStripMenuItem();
            toolStripMenuItem3 = new ToolStripMenuItem();
            statisticToolStripMenuItem = new ToolStripMenuItem();
            fieldBox.SuspendLayout();
            NavigationMenu.SuspendLayout();
            SuspendLayout();
            // 
            // fieldBox
            // 
            resources.ApplyResources(fieldBox, "fieldBox");
            fieldBox.Controls.Add(tableGrid);
            fieldBox.Name = "fieldBox";
            fieldBox.TabStop = false;
            // 
            // tableGrid
            // 
            resources.ApplyResources(tableGrid, "tableGrid");
            tableGrid.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            tableGrid.Name = "tableGrid";
            // 
            // InfoBox
            // 
            resources.ApplyResources(InfoBox, "InfoBox");
            InfoBox.Name = "InfoBox";
            InfoBox.TabStop = false;
            // 
            // NavigationMenu
            // 
            NavigationMenu.ImageScalingSize = new Size(20, 20);
            NavigationMenu.Items.AddRange(new ToolStripItem[] { newGameToolStripMenuItem, optionsToolStripMenuItem, statisticToolStripMenuItem });
            resources.ApplyResources(NavigationMenu, "NavigationMenu");
            NavigationMenu.Name = "NavigationMenu";
            // 
            // newGameToolStripMenuItem
            // 
            newGameToolStripMenuItem.Name = "newGameToolStripMenuItem";
            resources.ApplyResources(newGameToolStripMenuItem, "newGameToolStripMenuItem");
            newGameToolStripMenuItem.Click += newGameToolStripMenuItem_Click;
            // 
            // optionsToolStripMenuItem
            // 
            optionsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuItem1, toolStripMenuItem2, toolStripMenuItem3 });
            optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            resources.ApplyResources(optionsToolStripMenuItem, "optionsToolStripMenuItem");
            // 
            // toolStripMenuItem1
            // 
            resources.ApplyResources(toolStripMenuItem1, "toolStripMenuItem1");
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Click += toolStripMenuItem1_Click;
            // 
            // toolStripMenuItem2
            // 
            resources.ApplyResources(toolStripMenuItem2, "toolStripMenuItem2");
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Click += toolStripMenuItem2_Click;
            // 
            // toolStripMenuItem3
            // 
            resources.ApplyResources(toolStripMenuItem3, "toolStripMenuItem3");
            toolStripMenuItem3.Name = "toolStripMenuItem3";
            toolStripMenuItem3.Click += toolStripMenuItem3_Click;
            // 
            // statisticToolStripMenuItem
            // 
            statisticToolStripMenuItem.Name = "statisticToolStripMenuItem";
            resources.ApplyResources(statisticToolStripMenuItem, "statisticToolStripMenuItem");
            // 
            // MainWindow
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(InfoBox);
            Controls.Add(fieldBox);
            Controls.Add(NavigationMenu);
            DoubleBuffered = true;
            Name = "MainWindow";
            Load += MainWindow_Load;
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
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripMenuItem toolStripMenuItem3;
    }
}
