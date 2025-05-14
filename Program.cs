using Microsoft.Extensions.DependencyInjection;
using Minesweeper.Controllers;
using Minesweeper.Interfaces;
using Minesweeper.Models;

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

            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();
            ApplicationConfiguration.Initialize();
            var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
            Application.Run(mainWindow);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IGameServiceGenerator, GameServiceGenerator>();
            services.AddSingleton<IGameSettings, Settings>();
            services.AddSingleton<MainWindow>();
        }   
    }
}