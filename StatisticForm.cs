using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper
{
    public partial class StatisticForm : Form
    {
        private GameEngine _gameEngine;

        public StatisticForm(GameEngine gameEngine)
        {
            _gameEngine = gameEngine;
            InitializeComponent();
        }

        private void OptionsForm_Load(object sender, EventArgs e)
        {
            foreach (var record in _gameEngine.DataBase.RecordsList)
            {
                recordBindingSource.Add(record);
            }
        }

        
    }
}
