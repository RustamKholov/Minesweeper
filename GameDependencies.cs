using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    public class GameDependencies
    {
        
        public GameEngine GameEngine { get; private set; }
        public Settings Settings { get; private set; }
        
        public GameDependencies()
        {
            Settings = new Settings();
            GameEngine = new GameEngine(Settings);
        }
        public MainWindow CreateMainWindow()
        {
            return new MainWindow(Settings, GameEngine); 
        }
    }
}
