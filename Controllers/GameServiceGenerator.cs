using Minesweeper.Interfaces;

namespace Minesweeper.Controllers
{
    public class GameServiceGenerator : IGameServiceGenerator
    {
        private IGameSettings _settings;
        public GameServiceGenerator(IGameSettings settings)
        {
            _settings = settings;
        }
        public IGameService CreateGameService()
        {
            return new GameService(_settings);
        }
    }
}
