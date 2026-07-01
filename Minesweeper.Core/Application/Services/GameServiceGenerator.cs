using Minesweeper.Application.Interfaces;

namespace Minesweeper.Application.Services
{
    public class GameServiceGenerator : IGameServiceGenerator
    {
        private IGameSettings _settings;
        private IMineGenerator _mineGenerator;
        private IGameOverService _gameOverService;

        public GameServiceGenerator(IGameSettings settings, IMineGenerator mineGenerator, IGameOverService gameOverService)
        {
            _settings = settings;
            _mineGenerator = mineGenerator;
            _gameOverService = gameOverService;
        }
        public IGameService CreateGameService()
        {
            return new GameService(_settings, _mineGenerator, _gameOverService);
        }
    }
}
