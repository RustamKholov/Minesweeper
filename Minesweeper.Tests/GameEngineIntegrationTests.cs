using Minesweeper.Domain.Logic;
using Minesweeper.Domain.Logic.MineGeneration;

namespace Minesweeper.Tests
{
    // Exercises GameEngine wired exactly as Program.cs wires it (real NoGuessMineGenerator,
    // no test doubles) through a full click/flag sequence, without ever finishing the game -
    // finishing it would route through GameService/GameOverService and touch real SQLite/CSV
    // files, which is unrelated persistence plumbing this feature doesn't touch.
    public class GameEngineIntegrationTests
    {
        [Fact]
        public void FirstClick_GeneratesMinesAndRevealsASafeOpening()
        {
            var engine = new GameEngine(rows: 9, cols: 9, mines: 10, new NoGuessMineGenerator());

            Assert.True(engine.IsFirstClick);
            engine.RevealCell(4, 4);

            Assert.False(engine.IsFirstClick);
            Assert.True(engine.GetCell(4, 4).IsRevealed);
            Assert.False(engine.GetCell(4, 4).IsMine);
            Assert.False(engine.IsGameOver);
        }

        [Fact]
        public void FlaggingAndUnflagging_TogglesStateWithoutRevealing()
        {
            var engine = new GameEngine(rows: 9, cols: 9, mines: 10, new NoGuessMineGenerator());
            engine.RevealCell(4, 4);

            var unrevealed = engine.GetCell(0, 0).IsRevealed ? engine.GetCell(8, 8) : engine.GetCell(0, 0);

            engine.FlagCell(unrevealed.Row, unrevealed.Col);
            Assert.True(unrevealed.IsFlagged);
            Assert.False(unrevealed.IsRevealed);

            engine.FlagCell(unrevealed.Row, unrevealed.Col);
            Assert.False(unrevealed.IsFlagged);
        }

        [Fact]
        public void RestartGame_ProducesAFreshIndependentLayout()
        {
            var engine = new GameEngine(rows: 9, cols: 9, mines: 10, new NoGuessMineGenerator());
            engine.RevealCell(4, 4);
            var firstLayout = MineLayout(engine);

            engine.RestartGame();
            Assert.True(engine.IsFirstClick);
            engine.RevealCell(4, 4);
            var secondLayout = MineLayout(engine);

            Assert.NotEqual(firstLayout, secondLayout);
        }

        private static HashSet<(int Row, int Col)> MineLayout(GameEngine engine)
        {
            var mines = new HashSet<(int Row, int Col)>();
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (engine.GetCell(row, col).IsMine)
                    {
                        mines.Add((row, col));
                    }
                }
            }
            return mines;
        }
    }
}
