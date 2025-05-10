namespace Minesweeper
{
    partial class StatusPie
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
            winRatePanel = new Panel();
            diffCheckBox = new CheckedListBox();
            DifficultyLabel = new Label();
            SuspendLayout();
            // 
            // winRatePanel
            // 
            winRatePanel.Location = new Point(439, 24);
            winRatePanel.Name = "winRatePanel";
            winRatePanel.Size = new Size(339, 166);
            winRatePanel.TabIndex = 0;
            // 
            // diffCheckBox
            // 
            diffCheckBox.BackColor = SystemColors.Control;
            diffCheckBox.BorderStyle = BorderStyle.None;
            diffCheckBox.CheckOnClick = true;
            diffCheckBox.FormattingEnabled = true;
            diffCheckBox.Items.AddRange(new object[] { "Easy", "Medium", "Hard" });
            diffCheckBox.Location = new Point(32, 66);
            diffCheckBox.Name = "diffCheckBox";
            diffCheckBox.Size = new Size(71, 72);
            diffCheckBox.TabIndex = 1;
            diffCheckBox.SelectedIndexChanged += diffCheckBox_SelectedIndexChanged;
            // 
            // DifficultyLabel
            // 
            DifficultyLabel.AutoSize = true;
            DifficultyLabel.Location = new Point(32, 38);
            DifficultyLabel.Name = "DifficultyLabel";
            DifficultyLabel.Size = new Size(58, 15);
            DifficultyLabel.TabIndex = 2;
            DifficultyLabel.Text = "Difficulty:";
            // 
            // StatusPie
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(DifficultyLabel);
            Controls.Add(diffCheckBox);
            Controls.Add(winRatePanel);
            Name = "StatusPie";
            Text = "StatusPie";
            Load += StatusPie_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel winRatePanel;
        private CheckedListBox diffCheckBox;
        private Label DifficultyLabel;
    }
}