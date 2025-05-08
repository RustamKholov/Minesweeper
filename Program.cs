namespace Minesweeper
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            GameDependencies GameDependencies = new GameDependencies();
            ApplicationConfiguration.Initialize();
            Application.Run(GameDependencies.CreateMainWindow());
        }
    }
}