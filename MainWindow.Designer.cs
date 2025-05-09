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
            tableGrid = new TableLayoutPanel();
            InfoBox = new GroupBox();
            SmileButton = new Button();
            Mines_Panel = new Panel();
            Mines_Label = new Label();
            Timer_Panel = new Panel();
            Game_Timer_Label = new Label();
            NavigationMenu = new MenuStrip();
            newGameToolStripMenuItem = new ToolStripMenuItem();
            optionsToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripMenuItem();
            toolStripMenuItem2 = new ToolStripMenuItem();
            toolStripMenuItem3 = new ToolStripMenuItem();
            statisticToolStripMenuItem = new ToolStripMenuItem();
            recordsToolStripMenuItem = new ToolStripMenuItem();
            InfoBox.SuspendLayout();
            Mines_Panel.SuspendLayout();
            Timer_Panel.SuspendLayout();
            NavigationMenu.SuspendLayout();
            SuspendLayout();
            // 
            // tableGrid
            // 
            resources.ApplyResources(tableGrid, "tableGrid");
            tableGrid.BackColor = Color.Transparent;
            tableGrid.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            tableGrid.Name = "tableGrid";
            // 
            // InfoBox
            // 
            resources.ApplyResources(InfoBox, "InfoBox");
            InfoBox.Controls.Add(SmileButton);
            InfoBox.Controls.Add(Mines_Panel);
            InfoBox.Controls.Add(Timer_Panel);
            InfoBox.Name = "InfoBox";
            InfoBox.TabStop = false;
            // 
            // SmileButton
            // 
            resources.ApplyResources(SmileButton, "SmileButton");
            SmileButton.BackgroundImage = Properties.Resources.Smile;
            SmileButton.Name = "SmileButton";
            SmileButton.UseVisualStyleBackColor = true;
            SmileButton.Click += SmileButton_Click;
            // 
            // Mines_Panel
            // 
            resources.ApplyResources(Mines_Panel, "Mines_Panel");
            Mines_Panel.BackColor = Color.Black;
            Mines_Panel.BorderStyle = BorderStyle.FixedSingle;
            Mines_Panel.Controls.Add(Mines_Label);
            Mines_Panel.Name = "Mines_Panel";
            // 
            // Mines_Label
            // 
            resources.ApplyResources(Mines_Label, "Mines_Label");
            Mines_Label.BackColor = Color.Transparent;
            Mines_Label.ForeColor = Color.Red;
            Mines_Label.Name = "Mines_Label";
            // 
            // Timer_Panel
            // 
            Timer_Panel.BackColor = Color.Black;
            Timer_Panel.BorderStyle = BorderStyle.FixedSingle;
            Timer_Panel.Controls.Add(Game_Timer_Label);
            resources.ApplyResources(Timer_Panel, "Timer_Panel");
            Timer_Panel.Name = "Timer_Panel";
            // 
            // Game_Timer_Label
            // 
            resources.ApplyResources(Game_Timer_Label, "Game_Timer_Label");
            Game_Timer_Label.BackColor = Color.Transparent;
            Game_Timer_Label.ForeColor = Color.Red;
            Game_Timer_Label.Name = "Game_Timer_Label";
            // 
            // NavigationMenu
            // 
            NavigationMenu.BackColor = SystemColors.Control;
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
            statisticToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { recordsToolStripMenuItem });
            statisticToolStripMenuItem.Name = "statisticToolStripMenuItem";
            resources.ApplyResources(statisticToolStripMenuItem, "statisticToolStripMenuItem");
            // 
            // recordsToolStripMenuItem
            // 
            recordsToolStripMenuItem.Name = "recordsToolStripMenuItem";
            resources.ApplyResources(recordsToolStripMenuItem, "recordsToolStripMenuItem");
            recordsToolStripMenuItem.Click += recordsToolStripMenuItem_Click;
            // 
            // MainWindow
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableGrid);
            Controls.Add(NavigationMenu);
            Controls.Add(InfoBox);
            DoubleBuffered = true;
            MainMenuStrip = NavigationMenu;
            Name = "MainWindow";
            FormClosing += MainWindow_FormClosing;
            Load += MainWindow_Load;
            InfoBox.ResumeLayout(false);
            Mines_Panel.ResumeLayout(false);
            Timer_Panel.ResumeLayout(false);
            NavigationMenu.ResumeLayout(false);
            NavigationMenu.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private GroupBox InfoBox;
        private MenuStrip NavigationMenu;
        private ToolStripMenuItem newGameToolStripMenuItem;
        private ToolStripMenuItem optionsToolStripMenuItem;
        private ToolStripMenuItem statisticToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripMenuItem toolStripMenuItem3;
        private Label Game_Timer_Label;
        private TableLayoutPanel tableGrid;
        private Panel Timer_Panel;
        private Panel Mines_Panel;
        private Label Mines_Label;
        private Button SmileButton;
        private ToolStripMenuItem recordsToolStripMenuItem;
    }
}
