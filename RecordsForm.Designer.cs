namespace Minesweeper
{
    partial class RecordsForm
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
            csv_records_grid = new DataGridView();
            iDDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            secondsInGameDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            difficultyDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            statusDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            tilesUncoveredDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            clicksPerformedDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            flaggsSetDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            csvRecordBinding = new BindingSource(components);
            csv_radio_button = new RadioButton();
            sqlite_database_button = new RadioButton();
            sql_database_grid = new DataGridView();
            iDDataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            secondsInGameDataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            difficultyDataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            statusDataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            tilesUncoveredDataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            clicksPerformedDataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            flaggsSetDataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            sqlRecordBinding = new BindingSource(components);
            ((System.ComponentModel.ISupportInitialize)csv_records_grid).BeginInit();
            ((System.ComponentModel.ISupportInitialize)csvRecordBinding).BeginInit();
            ((System.ComponentModel.ISupportInitialize)sql_database_grid).BeginInit();
            ((System.ComponentModel.ISupportInitialize)sqlRecordBinding).BeginInit();
            SuspendLayout();
            // 
            // csv_records_grid
            // 
            csv_records_grid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            csv_records_grid.AutoGenerateColumns = false;
            csv_records_grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            csv_records_grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            csv_records_grid.Columns.AddRange(new DataGridViewColumn[] { iDDataGridViewTextBoxColumn, secondsInGameDataGridViewTextBoxColumn, difficultyDataGridViewTextBoxColumn, statusDataGridViewTextBoxColumn, tilesUncoveredDataGridViewTextBoxColumn, clicksPerformedDataGridViewTextBoxColumn, flaggsSetDataGridViewTextBoxColumn });
            csv_records_grid.DataSource = csvRecordBinding;
            csv_records_grid.ImeMode = ImeMode.NoControl;
            csv_records_grid.Location = new Point(0, 26);
            csv_records_grid.Name = "csv_records_grid";
            csv_records_grid.ReadOnly = true;
            csv_records_grid.RowHeadersWidth = 51;
            csv_records_grid.Size = new Size(499, 209);
            csv_records_grid.TabIndex = 0;
            // 
            // iDDataGridViewTextBoxColumn
            // 
            iDDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            iDDataGridViewTextBoxColumn.DataPropertyName = "ID";
            iDDataGridViewTextBoxColumn.HeaderText = "ID";
            iDDataGridViewTextBoxColumn.MinimumWidth = 6;
            iDDataGridViewTextBoxColumn.Name = "iDDataGridViewTextBoxColumn";
            iDDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // secondsInGameDataGridViewTextBoxColumn
            // 
            secondsInGameDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            secondsInGameDataGridViewTextBoxColumn.DataPropertyName = "secondsInGame";
            secondsInGameDataGridViewTextBoxColumn.HeaderText = "secondsInGame";
            secondsInGameDataGridViewTextBoxColumn.MinimumWidth = 6;
            secondsInGameDataGridViewTextBoxColumn.Name = "secondsInGameDataGridViewTextBoxColumn";
            secondsInGameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // difficultyDataGridViewTextBoxColumn
            // 
            difficultyDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            difficultyDataGridViewTextBoxColumn.DataPropertyName = "difficulty";
            difficultyDataGridViewTextBoxColumn.HeaderText = "difficulty";
            difficultyDataGridViewTextBoxColumn.MinimumWidth = 6;
            difficultyDataGridViewTextBoxColumn.Name = "difficultyDataGridViewTextBoxColumn";
            difficultyDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // statusDataGridViewTextBoxColumn
            // 
            statusDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            statusDataGridViewTextBoxColumn.DataPropertyName = "status";
            statusDataGridViewTextBoxColumn.HeaderText = "status";
            statusDataGridViewTextBoxColumn.MinimumWidth = 6;
            statusDataGridViewTextBoxColumn.Name = "statusDataGridViewTextBoxColumn";
            statusDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // tilesUncoveredDataGridViewTextBoxColumn
            // 
            tilesUncoveredDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            tilesUncoveredDataGridViewTextBoxColumn.DataPropertyName = "tilesUncovered";
            tilesUncoveredDataGridViewTextBoxColumn.HeaderText = "tilesUncovered";
            tilesUncoveredDataGridViewTextBoxColumn.MinimumWidth = 6;
            tilesUncoveredDataGridViewTextBoxColumn.Name = "tilesUncoveredDataGridViewTextBoxColumn";
            tilesUncoveredDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // clicksPerformedDataGridViewTextBoxColumn
            // 
            clicksPerformedDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            clicksPerformedDataGridViewTextBoxColumn.DataPropertyName = "clicksPerformed";
            clicksPerformedDataGridViewTextBoxColumn.HeaderText = "clicksPerformed";
            clicksPerformedDataGridViewTextBoxColumn.MinimumWidth = 6;
            clicksPerformedDataGridViewTextBoxColumn.Name = "clicksPerformedDataGridViewTextBoxColumn";
            clicksPerformedDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // flaggsSetDataGridViewTextBoxColumn
            // 
            flaggsSetDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            flaggsSetDataGridViewTextBoxColumn.DataPropertyName = "flaggsSet";
            flaggsSetDataGridViewTextBoxColumn.HeaderText = "flaggsSet";
            flaggsSetDataGridViewTextBoxColumn.MinimumWidth = 6;
            flaggsSetDataGridViewTextBoxColumn.Name = "flaggsSetDataGridViewTextBoxColumn";
            flaggsSetDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // csvRecordBinding
            // 
            csvRecordBinding.DataSource = typeof(Record);
            // 
            // csv_radio_button
            // 
            csv_radio_button.AutoSize = true;
            csv_radio_button.Checked = true;
            csv_radio_button.Location = new Point(116, 2);
            csv_radio_button.Margin = new Padding(3, 2, 3, 2);
            csv_radio_button.Name = "csv_radio_button";
            csv_radio_button.Size = new Size(97, 19);
            csv_radio_button.TabIndex = 1;
            csv_radio_button.TabStop = true;
            csv_radio_button.Text = "CSV DataBase";
            csv_radio_button.UseVisualStyleBackColor = true;
            csv_radio_button.CheckedChanged += radioButton1_CheckedChanged;
            // 
            // sqlite_database_button
            // 
            sqlite_database_button.AutoSize = true;
            sqlite_database_button.Location = new Point(261, 2);
            sqlite_database_button.Margin = new Padding(3, 2, 3, 2);
            sqlite_database_button.Name = "sqlite_database_button";
            sqlite_database_button.Size = new Size(110, 19);
            sqlite_database_button.TabIndex = 2;
            sqlite_database_button.Text = "SQLite DataBase";
            sqlite_database_button.UseVisualStyleBackColor = true;
            sqlite_database_button.CheckedChanged += sqlite_database_button_CheckedChanged;
            // 
            // sql_database_grid
            // 
            sql_database_grid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            sql_database_grid.AutoGenerateColumns = false;
            sql_database_grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            sql_database_grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            sql_database_grid.Columns.AddRange(new DataGridViewColumn[] { iDDataGridViewTextBoxColumn1, secondsInGameDataGridViewTextBoxColumn1, difficultyDataGridViewTextBoxColumn1, statusDataGridViewTextBoxColumn1, tilesUncoveredDataGridViewTextBoxColumn1, clicksPerformedDataGridViewTextBoxColumn1, flaggsSetDataGridViewTextBoxColumn1 });
            sql_database_grid.DataSource = sqlRecordBinding;
            sql_database_grid.Enabled = false;
            sql_database_grid.Location = new Point(0, 26);
            sql_database_grid.Margin = new Padding(3, 2, 3, 2);
            sql_database_grid.Name = "sql_database_grid";
            sql_database_grid.ReadOnly = true;
            sql_database_grid.RowHeadersWidth = 51;
            sql_database_grid.Size = new Size(499, 209);
            sql_database_grid.TabIndex = 3;
            sql_database_grid.Visible = false;
            // 
            // iDDataGridViewTextBoxColumn1
            // 
            iDDataGridViewTextBoxColumn1.DataPropertyName = "ID";
            iDDataGridViewTextBoxColumn1.HeaderText = "ID";
            iDDataGridViewTextBoxColumn1.MinimumWidth = 6;
            iDDataGridViewTextBoxColumn1.Name = "iDDataGridViewTextBoxColumn1";
            iDDataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // secondsInGameDataGridViewTextBoxColumn1
            // 
            secondsInGameDataGridViewTextBoxColumn1.DataPropertyName = "secondsInGame";
            secondsInGameDataGridViewTextBoxColumn1.HeaderText = "secondsInGame";
            secondsInGameDataGridViewTextBoxColumn1.MinimumWidth = 6;
            secondsInGameDataGridViewTextBoxColumn1.Name = "secondsInGameDataGridViewTextBoxColumn1";
            secondsInGameDataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // difficultyDataGridViewTextBoxColumn1
            // 
            difficultyDataGridViewTextBoxColumn1.DataPropertyName = "difficulty";
            difficultyDataGridViewTextBoxColumn1.HeaderText = "difficulty";
            difficultyDataGridViewTextBoxColumn1.MinimumWidth = 6;
            difficultyDataGridViewTextBoxColumn1.Name = "difficultyDataGridViewTextBoxColumn1";
            difficultyDataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // statusDataGridViewTextBoxColumn1
            // 
            statusDataGridViewTextBoxColumn1.DataPropertyName = "status";
            statusDataGridViewTextBoxColumn1.HeaderText = "status";
            statusDataGridViewTextBoxColumn1.MinimumWidth = 6;
            statusDataGridViewTextBoxColumn1.Name = "statusDataGridViewTextBoxColumn1";
            statusDataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // tilesUncoveredDataGridViewTextBoxColumn1
            // 
            tilesUncoveredDataGridViewTextBoxColumn1.DataPropertyName = "tilesUncovered";
            tilesUncoveredDataGridViewTextBoxColumn1.HeaderText = "tilesUncovered";
            tilesUncoveredDataGridViewTextBoxColumn1.MinimumWidth = 6;
            tilesUncoveredDataGridViewTextBoxColumn1.Name = "tilesUncoveredDataGridViewTextBoxColumn1";
            tilesUncoveredDataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // clicksPerformedDataGridViewTextBoxColumn1
            // 
            clicksPerformedDataGridViewTextBoxColumn1.DataPropertyName = "clicksPerformed";
            clicksPerformedDataGridViewTextBoxColumn1.HeaderText = "clicksPerformed";
            clicksPerformedDataGridViewTextBoxColumn1.MinimumWidth = 6;
            clicksPerformedDataGridViewTextBoxColumn1.Name = "clicksPerformedDataGridViewTextBoxColumn1";
            clicksPerformedDataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // flaggsSetDataGridViewTextBoxColumn1
            // 
            flaggsSetDataGridViewTextBoxColumn1.DataPropertyName = "flaggsSet";
            flaggsSetDataGridViewTextBoxColumn1.HeaderText = "flaggsSet";
            flaggsSetDataGridViewTextBoxColumn1.MinimumWidth = 6;
            flaggsSetDataGridViewTextBoxColumn1.Name = "flaggsSetDataGridViewTextBoxColumn1";
            flaggsSetDataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // sqlRecordBinding
            // 
            sqlRecordBinding.DataSource = typeof(Record);
            // 
            // StatisticForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BackgroundImageLayout = ImageLayout.None;
            ClientSize = new Size(503, 235);
            Controls.Add(sqlite_database_button);
            Controls.Add(csv_radio_button);
            Controls.Add(csv_records_grid);
            Controls.Add(sql_database_grid);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Margin = new Padding(3, 2, 3, 2);
            Name = "StatisticForm";
            SizeGripStyle = SizeGripStyle.Show;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Statistic";
            Load += OptionsForm_Load;
            ((System.ComponentModel.ISupportInitialize)csv_records_grid).EndInit();
            ((System.ComponentModel.ISupportInitialize)csvRecordBinding).EndInit();
            ((System.ComponentModel.ISupportInitialize)sql_database_grid).EndInit();
            ((System.ComponentModel.ISupportInitialize)sqlRecordBinding).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView csv_records_grid;
        private BindingSource csvRecordBinding;
        private RadioButton csv_radio_button;
        private RadioButton sqlite_database_button;
        private DataGridView sql_database_grid;
        private BindingSource sqlRecordBinding;
        private DataGridViewTextBoxColumn iDDataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn secondsInGameDataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn difficultyDataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn statusDataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn tilesUncoveredDataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn clicksPerformedDataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn flaggsSetDataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn iDDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn secondsInGameDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn difficultyDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn statusDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn tilesUncoveredDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn clicksPerformedDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn flaggsSetDataGridViewTextBoxColumn;
    }
}