namespace Minesweeper
{
    partial class StatisticForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StatisticForm));
            winRatePanel = new Panel();
            diffCheckBox = new CheckedListBox();
            recordBindingSource1 = new BindingSource(components);
            recordBindingSource = new BindingSource(components);
            DifficultyLabel = new Label();
            bestScorePanel = new Panel();
            dateBestLabel = new Label();
            dateBestTitleLabel = new Label();
            bestTimeLabel = new Label();
            bestTimeTitleLabel = new Label();
            resultsPanel = new Panel();
            resultsFrame = new GroupBox();
            tilesResultAbLabel = new Label();
            tilesResultLostLabel = new Label();
            tilesResultWonLabel = new Label();
            abandonedResultTimeLabel = new Label();
            lostResultTimeLabel = new Label();
            wonResultTimeLabel = new Label();
            abandonedResultLabel = new Label();
            lostResultLabel = new Label();
            wonResultLabel = new Label();
            tilesResultTotalLabel = new Label();
            tilesResultTitleLabel = new Label();
            timeResultLabel = new Label();
            totalResultTimeLabel = new Label();
            totalResultLabel = new Label();
            graphicPanel = new Panel();
            difficultyPanel = new Panel();
            ((System.ComponentModel.ISupportInitialize)recordBindingSource1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)recordBindingSource).BeginInit();
            bestScorePanel.SuspendLayout();
            resultsPanel.SuspendLayout();
            resultsFrame.SuspendLayout();
            difficultyPanel.SuspendLayout();
            SuspendLayout();
            // 
            // winRatePanel
            // 
            winRatePanel.Location = new Point(486, 24);
            winRatePanel.Name = "winRatePanel";
            winRatePanel.Size = new Size(302, 166);
            winRatePanel.TabIndex = 0;
            // 
            // diffCheckBox
            // 
            diffCheckBox.BackColor = SystemColors.Control;
            diffCheckBox.BorderStyle = BorderStyle.None;
            diffCheckBox.CheckOnClick = true;
            diffCheckBox.FormattingEnabled = true;
            diffCheckBox.ImeMode = ImeMode.Off;
            diffCheckBox.Items.AddRange(new object[] { "Easy", "Medium", "Hard" });
            diffCheckBox.Location = new Point(13, 38);
            diffCheckBox.Name = "diffCheckBox";
            diffCheckBox.Size = new Size(71, 72);
            diffCheckBox.TabIndex = 1;
            diffCheckBox.ThreeDCheckBoxes = true;
            diffCheckBox.SelectedIndexChanged += diffCheckBox_SelectedIndexChanged;
            // 
            // recordBindingSource1
            // 
            recordBindingSource1.DataSource = typeof(Record);
            // 
            // recordBindingSource
            // 
            recordBindingSource.DataSource = typeof(Record);
            // 
            // DifficultyLabel
            // 
            DifficultyLabel.AutoSize = true;
            DifficultyLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            DifficultyLabel.Location = new Point(13, 10);
            DifficultyLabel.Name = "DifficultyLabel";
            DifficultyLabel.Size = new Size(62, 15);
            DifficultyLabel.TabIndex = 2;
            DifficultyLabel.Text = "Difficulty:";
            // 
            // bestScorePanel
            // 
            bestScorePanel.Controls.Add(dateBestLabel);
            bestScorePanel.Controls.Add(dateBestTitleLabel);
            bestScorePanel.Controls.Add(bestTimeLabel);
            bestScorePanel.Controls.Add(bestTimeTitleLabel);
            bestScorePanel.Location = new Point(208, 24);
            bestScorePanel.Name = "bestScorePanel";
            bestScorePanel.Size = new Size(233, 166);
            bestScorePanel.TabIndex = 3;
            // 
            // dateBestLabel
            // 
            dateBestLabel.AutoSize = true;
            dateBestLabel.Location = new Point(102, 38);
            dateBestLabel.Name = "dateBestLabel";
            dateBestLabel.Size = new Size(0, 15);
            dateBestLabel.TabIndex = 3;
            // 
            // dateBestTitleLabel
            // 
            dateBestTitleLabel.AutoSize = true;
            dateBestTitleLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dateBestTitleLabel.Location = new Point(52, 38);
            dateBestTitleLabel.Name = "dateBestTitleLabel";
            dateBestTitleLabel.Size = new Size(37, 15);
            dateBestTitleLabel.TabIndex = 2;
            dateBestTitleLabel.Text = "Date:";
            // 
            // bestTimeLabel
            // 
            bestTimeLabel.AutoSize = true;
            bestTimeLabel.Location = new Point(102, 10);
            bestTimeLabel.Name = "bestTimeLabel";
            bestTimeLabel.Size = new Size(0, 15);
            bestTimeLabel.TabIndex = 1;
            // 
            // bestTimeTitleLabel
            // 
            bestTimeTitleLabel.AutoSize = true;
            bestTimeTitleLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            bestTimeTitleLabel.Location = new Point(25, 10);
            bestTimeTitleLabel.Name = "bestTimeTitleLabel";
            bestTimeTitleLabel.Size = new Size(66, 15);
            bestTimeTitleLabel.TabIndex = 0;
            bestTimeTitleLabel.Text = "Best Time:";
            // 
            // resultsPanel
            // 
            resultsPanel.Controls.Add(resultsFrame);
            resultsPanel.Controls.Add(tilesResultTotalLabel);
            resultsPanel.Controls.Add(tilesResultTitleLabel);
            resultsPanel.Controls.Add(timeResultLabel);
            resultsPanel.Controls.Add(totalResultTimeLabel);
            resultsPanel.Controls.Add(totalResultLabel);
            resultsPanel.Location = new Point(486, 206);
            resultsPanel.Name = "resultsPanel";
            resultsPanel.Size = new Size(302, 222);
            resultsPanel.TabIndex = 4;
            // 
            // resultsFrame
            // 
            resultsFrame.Controls.Add(tilesResultAbLabel);
            resultsFrame.Controls.Add(tilesResultLostLabel);
            resultsFrame.Controls.Add(tilesResultWonLabel);
            resultsFrame.Controls.Add(abandonedResultTimeLabel);
            resultsFrame.Controls.Add(lostResultTimeLabel);
            resultsFrame.Controls.Add(wonResultTimeLabel);
            resultsFrame.Controls.Add(abandonedResultLabel);
            resultsFrame.Controls.Add(lostResultLabel);
            resultsFrame.Controls.Add(wonResultLabel);
            resultsFrame.Location = new Point(3, 27);
            resultsFrame.Name = "resultsFrame";
            resultsFrame.Size = new Size(296, 87);
            resultsFrame.TabIndex = 24;
            resultsFrame.TabStop = false;
            // 
            // tilesResultAbLabel
            // 
            tilesResultAbLabel.AutoSize = true;
            tilesResultAbLabel.ImageAlign = ContentAlignment.MiddleLeft;
            tilesResultAbLabel.Location = new Point(235, 65);
            tilesResultAbLabel.Name = "tilesResultAbLabel";
            tilesResultAbLabel.Size = new Size(55, 15);
            tilesResultAbLabel.TabIndex = 22;
            tilesResultAbLabel.Text = "1 000 000";
            tilesResultAbLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // tilesResultLostLabel
            // 
            tilesResultLostLabel.AutoSize = true;
            tilesResultLostLabel.ImageAlign = ContentAlignment.MiddleLeft;
            tilesResultLostLabel.Location = new Point(235, 37);
            tilesResultLostLabel.Name = "tilesResultLostLabel";
            tilesResultLostLabel.Size = new Size(55, 15);
            tilesResultLostLabel.TabIndex = 21;
            tilesResultLostLabel.Text = "1 000 000";
            tilesResultLostLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // tilesResultWonLabel
            // 
            tilesResultWonLabel.AutoSize = true;
            tilesResultWonLabel.ImageAlign = ContentAlignment.MiddleLeft;
            tilesResultWonLabel.Location = new Point(235, 13);
            tilesResultWonLabel.Name = "tilesResultWonLabel";
            tilesResultWonLabel.Size = new Size(55, 15);
            tilesResultWonLabel.TabIndex = 20;
            tilesResultWonLabel.Text = "1 000 000";
            tilesResultWonLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // abandonedResultTimeLabel
            // 
            abandonedResultTimeLabel.AutoSize = true;
            abandonedResultTimeLabel.ImageAlign = ContentAlignment.MiddleLeft;
            abandonedResultTimeLabel.Location = new Point(110, 63);
            abandonedResultTimeLabel.Name = "abandonedResultTimeLabel";
            abandonedResultTimeLabel.Size = new Size(49, 15);
            abandonedResultTimeLabel.TabIndex = 15;
            abandonedResultTimeLabel.Text = "00:00:00";
            abandonedResultTimeLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lostResultTimeLabel
            // 
            lostResultTimeLabel.AutoSize = true;
            lostResultTimeLabel.ImageAlign = ContentAlignment.MiddleLeft;
            lostResultTimeLabel.Location = new Point(110, 37);
            lostResultTimeLabel.Name = "lostResultTimeLabel";
            lostResultTimeLabel.Size = new Size(49, 15);
            lostResultTimeLabel.TabIndex = 14;
            lostResultTimeLabel.Text = "00:00:00";
            lostResultTimeLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // wonResultTimeLabel
            // 
            wonResultTimeLabel.AutoSize = true;
            wonResultTimeLabel.ImageAlign = ContentAlignment.MiddleLeft;
            wonResultTimeLabel.Location = new Point(110, 12);
            wonResultTimeLabel.Name = "wonResultTimeLabel";
            wonResultTimeLabel.Size = new Size(49, 15);
            wonResultTimeLabel.TabIndex = 13;
            wonResultTimeLabel.Text = "00:00:00";
            wonResultTimeLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // abandonedResultLabel
            // 
            abandonedResultLabel.AutoSize = true;
            abandonedResultLabel.Location = new Point(7, 63);
            abandonedResultLabel.Name = "abandonedResultLabel";
            abandonedResultLabel.Size = new Size(72, 15);
            abandonedResultLabel.TabIndex = 12;
            abandonedResultLabel.Text = "Abandoned:";
            // 
            // lostResultLabel
            // 
            lostResultLabel.AutoSize = true;
            lostResultLabel.Location = new Point(47, 37);
            lostResultLabel.Name = "lostResultLabel";
            lostResultLabel.Size = new Size(32, 15);
            lostResultLabel.TabIndex = 11;
            lostResultLabel.Text = "Lost:";
            // 
            // wonResultLabel
            // 
            wonResultLabel.AutoSize = true;
            wonResultLabel.Location = new Point(44, 12);
            wonResultLabel.Name = "wonResultLabel";
            wonResultLabel.Size = new Size(35, 15);
            wonResultLabel.TabIndex = 10;
            wonResultLabel.Text = "Won:";
            // 
            // tilesResultTotalLabel
            // 
            tilesResultTotalLabel.AutoSize = true;
            tilesResultTotalLabel.Location = new Point(238, 117);
            tilesResultTotalLabel.Name = "tilesResultTotalLabel";
            tilesResultTotalLabel.Size = new Size(55, 15);
            tilesResultTotalLabel.TabIndex = 23;
            tilesResultTotalLabel.Text = "1 000 000";
            tilesResultTotalLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // tilesResultTitleLabel
            // 
            tilesResultTitleLabel.AutoSize = true;
            tilesResultTitleLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            tilesResultTitleLabel.Location = new Point(199, 9);
            tilesResultTitleLabel.Name = "tilesResultTitleLabel";
            tilesResultTitleLabel.Size = new Size(97, 15);
            tilesResultTitleLabel.TabIndex = 19;
            tilesResultTitleLabel.Text = "Tiles Uncovered";
            // 
            // timeResultLabel
            // 
            timeResultLabel.AutoSize = true;
            timeResultLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            timeResultLabel.Location = new Point(91, 9);
            timeResultLabel.Name = "timeResultLabel";
            timeResultLabel.Size = new Size(71, 15);
            timeResultLabel.TabIndex = 18;
            timeResultLabel.Text = "Time Spent";
            // 
            // totalResultTimeLabel
            // 
            totalResultTimeLabel.AutoSize = true;
            totalResultTimeLabel.Location = new Point(113, 117);
            totalResultTimeLabel.Name = "totalResultTimeLabel";
            totalResultTimeLabel.Size = new Size(49, 15);
            totalResultTimeLabel.TabIndex = 17;
            totalResultTimeLabel.Text = "00:00:00";
            totalResultTimeLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // totalResultLabel
            // 
            totalResultLabel.AutoSize = true;
            totalResultLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            totalResultLabel.Location = new Point(50, 117);
            totalResultLabel.Name = "totalResultLabel";
            totalResultLabel.Size = new Size(34, 15);
            totalResultLabel.TabIndex = 16;
            totalResultLabel.Text = "Total";
            // 
            // graphicPanel
            // 
            graphicPanel.Location = new Point(45, 206);
            graphicPanel.Name = "graphicPanel";
            graphicPanel.Size = new Size(396, 222);
            graphicPanel.TabIndex = 5;
            // 
            // difficultyPanel
            // 
            difficultyPanel.Controls.Add(DifficultyLabel);
            difficultyPanel.Controls.Add(diffCheckBox);
            difficultyPanel.Location = new Point(45, 24);
            difficultyPanel.Name = "difficultyPanel";
            difficultyPanel.Size = new Size(152, 166);
            difficultyPanel.TabIndex = 6;
            // 
            // StatisticForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(difficultyPanel);
            Controls.Add(graphicPanel);
            Controls.Add(resultsPanel);
            Controls.Add(bestScorePanel);
            Controls.Add(winRatePanel);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "StatisticForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Statistic";
            Load += StatusPie_Load;
            ((System.ComponentModel.ISupportInitialize)recordBindingSource1).EndInit();
            ((System.ComponentModel.ISupportInitialize)recordBindingSource).EndInit();
            bestScorePanel.ResumeLayout(false);
            bestScorePanel.PerformLayout();
            resultsPanel.ResumeLayout(false);
            resultsPanel.PerformLayout();
            resultsFrame.ResumeLayout(false);
            resultsFrame.PerformLayout();
            difficultyPanel.ResumeLayout(false);
            difficultyPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel winRatePanel;
        private CheckedListBox diffCheckBox;
        private Label DifficultyLabel;
        private Panel bestScorePanel;
        private Label bestTimeLabel;
        private Label bestTimeTitleLabel;
        private Label dateBestTitleLabel;
        private Label dateBestLabel;
        private BindingSource recordBindingSource;
        private BindingSource recordBindingSource1;
        private Panel resultsPanel;
        private Label totalResultTimeLabel;
        private Label totalResultLabel;
        private Label abandonedResultTimeLabel;
        private Label lostResultTimeLabel;
        private Label wonResultTimeLabel;
        private Label abandonedResultLabel;
        private Label lostResultLabel;
        private Label wonResultLabel;
        private Label timeResultLabel;
        private Label tilesResultTotalLabel;
        private Label tilesResultAbLabel;
        private Label tilesResultLostLabel;
        private Label tilesResultWonLabel;
        private Label tilesResultTitleLabel;
        private GroupBox resultsFrame;
        private Panel graphicPanel;
        private Panel difficultyPanel;
    }
}