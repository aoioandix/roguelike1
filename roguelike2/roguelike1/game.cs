using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RLNET;
using RogueSharp;
using RogueSharp.Random;

namespace roguelike1
{
    public class Game
    {
        public static Core.Player Player { get; private set; }
        public static DungeonMap DungeonMap { get; private set; }
        // The screen height and width are in number of tiles
        private static readonly int _screenWidth = 100;
        private static readonly int _screenHeight = 70;
        private static RLRootConsole _rootConsole;

        // The map console takes up most of the screen and is where the map will be drawn
        private static readonly int _mapWidth = 80;
        private static readonly int _mapHeight = 48;
        private static RLConsole _mapConsole;

        // Below the map console is the message console which displays attack rolls and other information
        private static readonly int _messageWidth = 80;
        private static readonly int _messageHeight = 11;
        private static RLConsole _messageConsole;

        // The stat console is to the right of the map and display player and monster stats
        private static readonly int _statWidth = 20;
        private static readonly int _statHeight = 70;
        private static RLConsole _statConsole;

        // Above the map is the inventory console which shows the players equipment, abilities, and items
        private static readonly int _inventoryWidth = 80;
        private static readonly int _inventoryHeight = 11;
        private static RLConsole _inventoryConsole;
        // Singleton of IRandom used throughout the game when generating random numbers
        public static IRandom Random { get; private set; }

        public static void Main()
        {
            // Establish the seed for the random number generator from the current time
            int seed = (int)DateTime.UtcNow.Ticks;
            Random = new DotNetRandom(seed);

            // The title will appear at the top of the console window 
            // also include the seed used to generate the level
            string consoleTitle = $"roguelike1 - Level 1 - Seed {seed}";

            // This must be the exact name of the bitmap font file we are using or it will error.
            string fontFileName = "terminal8x8.png";
            // Tell RLNet to use the bitmap font that we specified and that each tile is 8 x 8 pixels
            _rootConsole = new RLRootConsole(fontFileName, _screenWidth, _screenHeight,
              8, 8, 1f, consoleTitle);
            // ... previous code omitted After the line that starts _rootConsole = new RLRootConsole( ... );  Initialize the sub consoles that we will Blit to the root console
            _mapConsole = new RLConsole(_mapWidth, _mapHeight);
            _messageConsole = new RLConsole(_messageWidth, _messageHeight);
            _statConsole = new RLConsole(_statWidth, _statHeight);
            _inventoryConsole = new RLConsole(_inventoryWidth, _inventoryHeight);

            Player = new Core.Player();
            System.MapGenerator mapGenerator = new System.MapGenerator(_mapWidth, _mapHeight, 20, 13, 7);
            DungeonMap = mapGenerator.CreateMap();
            DungeonMap.UpdatePlayerFieldOfView();

            // Set up a handler for RLNET's Update event
            _rootConsole.Update += OnRootConsoleUpdate;
            // Set up a handler for RLNET's Render event
            _rootConsole.Render += OnRootConsoleRender;
            // Begin RLNET's game loop
            _rootConsole.Run();
            
        }

        // Event handler for RLNET's Update event
        private static void OnRootConsoleUpdate(object sender, UpdateEventArgs e)
        {
            _mapConsole.SetBackColor(0, 0, _mapWidth, _mapHeight, Core.Colors.FloorBackground);
            _mapConsole.Print(1, 1, "Map", Core.Colors.TextHeading);

            _messageConsole.SetBackColor(0, 0, _messageWidth, _messageHeight, Core.Swatch.DbDeepWater);
            _messageConsole.Print(1, 1, "Messages", Core.Colors.TextHeading);

            _statConsole.SetBackColor(0, 0, _statWidth, _statHeight, Core.Swatch.DbOldStone);
            _statConsole.Print(1, 1, "Stats", Core.Colors.TextHeading);

            _inventoryConsole.SetBackColor(0, 0, _inventoryWidth, _inventoryHeight, Core.Swatch.DbWood);
            _inventoryConsole.Print(1, 1, "Inventory", Core.Colors.TextHeading);

            
        }


        // Event handler for RLNET's Render event
        private static void OnRootConsoleRender(object sender, UpdateEventArgs e)
        {
            DungeonMap.Draw(_mapConsole);
            Player.Draw(_mapConsole, DungeonMap);
            // Blit the sub consoles to the root console in the correct locations
            RLConsole.Blit(_mapConsole, 0, 0, _mapWidth, _mapHeight,
              _rootConsole, 0, _inventoryHeight);
            RLConsole.Blit(_statConsole, 0, 0, _statWidth, _statHeight,
              _rootConsole, _mapWidth, 0);
            RLConsole.Blit(_messageConsole, 0, 0, _messageWidth, _messageHeight,
              _rootConsole, 0, _screenHeight - _messageHeight);
            RLConsole.Blit(_inventoryConsole, 0, 0, _inventoryWidth, _inventoryHeight,
              _rootConsole, 0, 0);

            // Tell RLNET to draw the console that we set
            _rootConsole.Draw();
            
        }
    }    
}

