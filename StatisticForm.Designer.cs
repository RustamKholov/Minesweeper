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
            dataGridView1 = new DataGridView();
            iDDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            secondsInGameDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            difficultyDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            statusDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            tilesUncoveredDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            clicksPerformedDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            flaggsSetDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            recordBindingSource = new BindingSource(components);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)recordBindingSource).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { iDDataGridViewTextBoxColumn, secondsInGameDataGridViewTextBoxColumn, difficultyDataGridViewTextBoxColumn, statusDataGridViewTextBoxColumn, tilesUncoveredDataGridViewTextBoxColumn, clicksPerformedDataGridViewTextBoxColumn, flaggsSetDataGridViewTextBoxColumn });
            dataGridView1.DataSource = recordBindingSource;
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.Location = new Point(0, 0);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(742, 341);
            dataGridView1.TabIndex = 0;
            // 
            // iDDataGridViewTextBoxColumn
            // 
            iDDataGridViewTextBoxColumn.DataPropertyName = "ID";
            iDDataGridViewTextBoxColumn.HeaderText = "ID";
            iDDataGridViewTextBoxColumn.Name = "iDDataGridViewTextBoxColumn";
            // 
            // secondsInGameDataGridViewTextBoxColumn
            // 
            secondsInGameDataGridViewTextBoxColumn.DataPropertyName = "secondsInGame";
            secondsInGameDataGridViewTextBoxColumn.HeaderText = "secondsInGame";
            secondsInGameDataGridViewTextBoxColumn.Name = "secondsInGameDataGridViewTextBoxColumn";
            // 
            // difficultyDataGridViewTextBoxColumn
            // 
            difficultyDataGridViewTextBoxColumn.DataPropertyName = "difficulty";
            difficultyDataGridViewTextBoxColumn.HeaderText = "difficulty";
            difficultyDataGridViewTextBoxColumn.Name = "difficultyDataGridViewTextBoxColumn";
            // 
            // statusDataGridViewTextBoxColumn
            // 
            statusDataGridViewTextBoxColumn.DataPropertyName = "status";
            statusDataGridViewTextBoxColumn.HeaderText = "status";
            statusDataGridViewTextBoxColumn.Name = "statusDataGridViewTextBoxColumn";
            // 
            // tilesUncoveredDataGridViewTextBoxColumn
            // 
            tilesUncoveredDataGridViewTextBoxColumn.DataPropertyName = "tilesUncovered";
            tilesUncoveredDataGridViewTextBoxColumn.HeaderText = "tilesUncovered";
            tilesUncoveredDataGridViewTextBoxColumn.Name = "tilesUncoveredDataGridViewTextBoxColumn";
            // 
            // clicksPerformedDataGridViewTextBoxColumn
            // 
            clicksPerformedDataGridViewTextBoxColumn.DataPropertyName = "clicksPerformed";
            clicksPerformedDataGridViewTextBoxColumn.HeaderText = "clicksPerformed";
            clicksPerformedDataGridViewTextBoxColumn.Name = "clicksPerformedDataGridViewTextBoxColumn";
            // 
            // flaggsSetDataGridViewTextBoxColumn
            // 
            flaggsSetDataGridViewTextBoxColumn.DataPropertyName = "flaggsSet";
            flaggsSetDataGridViewTextBoxColumn.HeaderText = "flaggsSet";
            flaggsSetDataGridViewTextBoxColumn.Name = "flaggsSetDataGridViewTextBoxColumn";
            // 
            // recordBindingSource
            // 
            recordBindingSource.DataSource = typeof(Record);
            // 
            // StatisticForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(742, 341);
            Controls.Add(dataGridView1);
            Margin = new Padding(3, 2, 3, 2);
            Name = "StatisticForm";
            Text = "Statistic";
            Load += OptionsForm_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)recordBindingSource).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dataGridView1;
        private BindingSource recordBindingSource;
        private DataGridViewTextBoxColumn iDDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn secondsInGameDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn difficultyDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn statusDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn tilesUncoveredDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn clicksPerformedDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn flaggsSetDataGridViewTextBoxColumn;
    }
}