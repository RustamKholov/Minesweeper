using Minesweeper.Application.Interfaces;
using Minesweeper.Domain.Entities;

namespace Minesweeper.Web.Services
{
    // Satisfies GameService's constructor requirement only. The Blazor client persists
    // completed games through RecordsApiClient/Minesweeper.Api instead - there is no local
    // SQLite/CSV available in a browser sandbox, and GameService.SaveGame() is never called
    // here (persistence is triggered explicitly by GameStateService.AfterMutation).
    public sealed class NoOpGameOverService : IGameOverService
    {
        public void SaveRecord(Record record)
        {
        }
    }
}
