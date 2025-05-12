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
            InfoBox = new GroupBox();
            SmileButton = new Button();
            Mines_Panel = new Panel();
            Mines_Label = new Label();
            Timer_Panel = new Panel();
            Game_Timer_Label = new Label();
            TimerPanel = new Panel();
            NavigationMenu = new MenuStrip();
            newGameToolStripMenuItem = new ToolStripMenuItem();
            optionsToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripMenuItem();
            toolStripMenuItem2 = new ToolStripMenuItem();
            toolStripMenuItem3 = new ToolStripMenuItem();
            statisticToolStripMenuItem = new ToolStripMenuItem();
            recordsStripMenuItem = new ToolStripMenuItem();
            tableGrid = new TableLayoutPanel();
            menuPanel = new Panel();
            FieldPanel = new Panel();
            InfoBox.SuspendLayout();
            Mines_Panel.SuspendLayout();
            Timer_Panel.SuspendLayout();
            NavigationMenu.SuspendLayout();
            menuPanel.SuspendLayout();
            FieldPanel.SuspendLayout();
            SuspendLayout();
            // 
            // InfoBox
            // 
            resources.ApplyResources(InfoBox, "InfoBox");
            InfoBox.Controls.Add(SmileButton);
            InfoBox.Controls.Add(Mines_Panel);
            InfoBox.Controls.Add(Timer_Panel);
            InfoBox.Controls.Add(TimerPanel);
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
            // TimerPanel
            // 
            resources.ApplyResources(TimerPanel, "TimerPanel");
            TimerPanel.BorderStyle = BorderStyle.Fixed3D;
            TimerPanel.Name = "TimerPanel";
            // 
            // NavigationMenu
            // 
            NavigationMenu.BackColor = SystemColors.Control;
            resources.ApplyResources(NavigationMenu, "NavigationMenu");
            NavigationMenu.GripMargin = new Padding(2);
            NavigationMenu.ImageScalingSize = new Size(20, 20);
            NavigationMenu.Items.AddRange(new ToolStripItem[] { newGameToolStripMenuItem, optionsToolStripMenuItem, statisticToolStripMenuItem, recordsStripMenuItem });
            NavigationMenu.Name = "NavigationMenu";
            NavigationMenu.RenderMode = ToolStripRenderMode.Professional;
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
            statisticToolStripMenuItem.Click += statisticToolStripMenuItem_Click;
            // 
            // recordsStripMenuItem
            // 
            recordsStripMenuItem.Name = "recordsStripMenuItem";
            resources.ApplyResources(recordsStripMenuItem, "recordsStripMenuItem");
            recordsStripMenuItem.Click += recordsStripMenuItem_Click;
            // 
            // tableGrid
            // 
            resources.ApplyResources(tableGrid, "tableGrid");
            tableGrid.BackColor = Color.Transparent;
            tableGrid.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            tableGrid.Name = "tableGrid";
            // 
            // menuPanel
            // 
            resources.ApplyResources(menuPanel, "menuPanel");
            menuPanel.BorderStyle = BorderStyle.Fixed3D;
            menuPanel.Controls.Add(NavigationMenu);
            menuPanel.Name = "menuPanel";
            // 
            // FieldPanel
            // 
            resources.ApplyResources(FieldPanel, "FieldPanel");
            FieldPanel.BorderStyle = BorderStyle.Fixed3D;
            FieldPanel.Controls.Add(tableGrid);
            FieldPanel.Name = "FieldPanel";
            // 
            // MainWindow
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            Controls.Add(FieldPanel);
            Controls.Add(menuPanel);
            Controls.Add(InfoBox);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.Fixed3D;
            MainMenuStrip = NavigationMenu;
            Name = "MainWindow";
            FormClosing += MainWindow_FormClosing;
            Load += MainWindow_Load;
            InfoBox.ResumeLayout(false);
            Mines_Panel.ResumeLayout(false);
            Timer_Panel.ResumeLayout(false);
            NavigationMenu.ResumeLayout(false);
            NavigationMenu.PerformLayout();
            menuPanel.ResumeLayout(false);
            menuPanel.PerformLayout();
            FieldPanel.ResumeLayout(false);
            FieldPanel.PerformLayout();
            ResumeLayout(false);
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
        private Panel Timer_Panel;
        private Panel Mines_Panel;
        private Label Mines_Label;
        private Button SmileButton;
        private ToolStripMenuItem recordsStripMenuItem;
        private Panel TimerPanel;
        private TableLayoutPanel tableGrid;
        private Panel menuPanel;
        private Panel FieldPanel;
    }
}
