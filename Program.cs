using Microsoft.Extensions.DependencyInjection;
using Minesweeper.Application.Services;
using Minesweeper.Domain.Logic;
using Minesweeper.Interfaces;

namespace Minesweeper.UI.Forms
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
            System.Windows.Forms.Application.Run(mainWindow);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IGameServiceGenerator, GameServiceGenerator>();
            services.AddSingleton<IGameSettings, Settings>();
            services.AddSingleton<MainWindow>();
        }
    }
}