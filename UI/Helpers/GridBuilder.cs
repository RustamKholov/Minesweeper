using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Minesweeper.Application.Interfaces;

namespace Minesweeper
{
    public class GridBuilder
    {
        private TableLayoutPanel _tableGrid;
        private IGameSettings _settings;
        private ButtonRenderer _buttonRenderer;
        public GridBuilder(TableLayoutPanel tableLayoutPanel, IGameSettings settings, ButtonRenderer buttonRenderer)
        {
            _tableGrid = tableLayoutPanel;
            _settings = settings;
            _buttonRenderer = buttonRenderer;
        }

        public void Build()
        {
            _tableGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _tableGrid.RowCount = _settings.Rows;
            _tableGrid.ColumnCount = _settings.Cols;

            _tableGrid.Padding = new Padding(0);
            _tableGrid.Margin = new Padding(0);

            for (int i = 0; i < _settings.Cols; i++)
                _tableGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, _settings.CellSize));

            for (int i = 0; i < _settings.Rows; i++)
                _tableGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, _settings.CellSize));

            FillGridWithButtons();
        }
        private void FillGridWithButtons()
        {
            for (int row = 0; row < _settings.Rows; row++)
            {
                for (int col = 0; col < _settings.Cols; col++)
                {
                    Button btn = _buttonRenderer.CreateCustomTile();

                    btn.Tag = (col, row);

                    _tableGrid.Controls.Add(btn, col, row);
                }
            }
        }
        public void Reset()
        {
            _tableGrid.Controls.Clear();
            _tableGrid.ColumnStyles.Clear();
            _tableGrid.RowStyles.Clear();
        }
        public void BindEvents(Action<Button> eventsBinder)
        {
            for (int row = 0; row < _settings.Rows; row++)
            {
                for (int col = 0; col < _settings.Cols; col++)
                {
                    if (_tableGrid.GetControlFromPosition(col, row) is Button btn)
                    {
                        eventsBinder(btn);
                    }
                }
            }
        }
        public void Rebuild(IGameSettings settings)
        {
            _settings = settings;
            Reset();
            Build();
        }
    }
}
