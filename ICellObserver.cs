using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    public interface ICellObserver
    {
        void UpdateRevealed(Cell cell);
        void UpdateFlagged(Cell cell);
    }
}
