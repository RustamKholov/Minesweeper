using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minesweeper.Application.DTO;
using Minesweeper.Domain.Entities;

namespace Minesweeper.Application.Interfaces
{
    public  interface IGameOverService
    {
        void SaveRecord(Record record);
    }
}
