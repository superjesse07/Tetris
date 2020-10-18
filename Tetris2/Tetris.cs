using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Tetris2
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Tetris : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public static SpriteFont font; //Text font
        private static GameState _state;
        public static Tetris tetris { private set; get; }

        public static readonly int[] ScorePerLine = {40, 100, 300, 1200}; //Amount of points for different amount of lines cleared
        private TetrisGrid[] _players; //Stores the different games for the players
        private const int GridWidth=10, GridHeight = 20; //the sizes of the grids
        public const int SidePanelSizes = 6; //This is the same as the 6 in Tetrisgrid for the holdGrid and nextGrid 6 because max size of piece is 4 plus 2 for outline
        private const int Divider = 4; //The amount of blocks between the two player grids
        private Vector2 ScreenSize => new Vector2(Window.ClientBounds.Width,Window.ClientBounds.Height); //vector2 for screensize
        
        private readonly Random _random = new Random();
        private Song _song;
        public static SoundEffect placeSfx;
        
        /// <summary>
        /// Initial window for the game
        /// </summary>
        public Tetris()
        {
            tetris = this;
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 800;
        }
        
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Tetronimo.block = Content.Load<Texture2D>("block"); //Load block texture
            font = Content.Load<SpriteFont>("Arial"); //Load Text font
            _song = Content.Load<Song>("Tetris"); //Load song
            placeSfx = Content.Load<SoundEffect>("hitsfx"); //Load sound effects
            SoundEffect.MasterVolume = 0.1f; //So you don't get earraped
            MediaPlayer.Play(_song);
            MediaPlayer.IsRepeating = true; //Make the music nonstop!
            MediaPlayer.Volume = 0.1f; //So you don't get earraped
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) //So you can accidentally quit the game
                Exit();

            KeyboardState keyboard = Keyboard.GetState();
            if (_state == GameState.MainMenu)
            {
                if (keyboard.IsKeyDown(Keys.D1)) //Press 1 for single player
                { 
                    //change the screen size to fit one window
                    _graphics.PreferredBackBufferHeight = (int) ((GridHeight + 2) * Tetronimo.BlockSize.Y); // 2 is for the outline blocks
                    _graphics.PreferredBackBufferWidth = (int) ((GridWidth + SidePanelSizes*2) * Tetronimo.BlockSize.Y); // the outline blocks are included in the side panels so no +2
                    _graphics.ApplyChanges();
                    int seed = _random.Next();
                    _players = new[]
                    {
                        new TetrisGrid(GridWidth, GridHeight, new Vector2(SidePanelSizes-1,0) * Tetronimo.BlockSize, seed, Keys.A, Keys.D, Keys.S, Keys.W, Keys.Space, Keys.C) //Create new player
                    };
                    _state = GameState.Playing;
                }
                if (keyboard.IsKeyDown(Keys.D2)) //Press 2 for Multiplayer
                {
                    int seed = _random.Next();
                    _graphics.PreferredBackBufferHeight = (int) ((GridHeight + 2) * Tetronimo.BlockSize.Y); // 2 is for the outline blocks
                    _graphics.PreferredBackBufferWidth = (int) (((GridWidth + SidePanelSizes*2)*2 + Divider) * Tetronimo.BlockSize.Y); // the outline blocks are included in the side panels so no +2
                    _players = new[]
                    {
                        new TetrisGrid(GridWidth, GridHeight, new Vector2(SidePanelSizes-1,0) * Tetronimo.BlockSize, seed, Keys.A, Keys.D, Keys.S, Keys.W, Keys.Space, Keys.C), //Create new player
                        new TetrisGrid(GridWidth, GridHeight, new Vector2(SidePanelSizes-1 + SidePanelSizes*2 + GridWidth+Divider,0) * Tetronimo.BlockSize, seed, Keys.Left, Keys.Right, Keys.Down, Keys.Up, Keys.RightControl, Keys.RightShift) //Create new player with diffent inputs and an offset
                    };
                    _graphics.ApplyChanges();
                    _state = GameState.Playing;
                }
            }
            else if (_state == GameState.Playing)
            {
                foreach (var player in _players)
                {
                    player.Update(gameTime); //Update each players game
                    if (player.lost) //Check if a player has lost
                    {
                        _state = GameState.Finished; //Update gamestate
                    }
                }
            }
            else if (_state == GameState.Finished)
            {
                if (keyboard.IsKeyDown(Keys.Enter)) _state = GameState.MainMenu; //Update gamestate
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            if (_state == GameState.MainMenu)
            {
                string mainMenuText = "Press 1 for Single-player\nor 2 for Multi-player";
                _spriteBatch.DrawString(font,mainMenuText,ScreenSize/2,Color.White,0,font.MeasureString(mainMenuText)/2,1,SpriteEffects.None,0);
            }
            if (_state == GameState.Playing || _state == GameState.Finished)
            {
                foreach (var player in _players)
                {
                    player.Draw(_spriteBatch, gameTime); //Draw the game(s)
                }
            }

            if (_state == GameState.Finished)
            {
                string spaceText = "Press enter to continue";
                if (_players.Length == 1)
                {
                    string finishedText = "You lost";
                    _spriteBatch.DrawString(font, finishedText, ScreenSize / 2, Color.White, 0, font.MeasureString(finishedText) / 2, 1, SpriteEffects.None, 0); //Draw loser text
                    _spriteBatch.DrawString(font, spaceText, ScreenSize / 2 + new Vector2(0, font.MeasureString(finishedText).Y / 1.5f), Color.White, 0, font.MeasureString(spaceText) / 2, 0.5f, SpriteEffects.None, 0); //Draw 'Enter' text
                }
                else
                {
                    string finishedText = $"{(_players[0].lost ? "Left" : "Right")} lost";
                    _spriteBatch.DrawString(font, finishedText, ScreenSize / 2, Color.White, 0, font.MeasureString(finishedText) / 2, 1, SpriteEffects.None, 0); //Draw loser text
                    _spriteBatch.DrawString(font, spaceText, ScreenSize / 2 + new Vector2(0, font.MeasureString(finishedText).Y / 1.5f), Color.White, 0, font.MeasureString(spaceText) / 2, 0.5f, SpriteEffects.None, 0); //Draw 'Enter' text
                }
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Sends a 'garbage line' to the other player
        /// </summary>
        /// <param name="amount">The amound of garbage line to be added to the other player</param>
        /// <param name="sender">The player that sends the garbage lines</param>
        public void SendGarbageLines(int amount, TetrisGrid sender)
        {
            var reciever = _players.FirstOrDefault(x => x != sender);
            if (reciever!= null) reciever.garbageLines += amount;
        }
        
        /// <summary>
        /// Gets a color that shifts over time
        /// </summary>
        /// <param name="time">time value in seconds</param>
        /// <param name="speed">the speed with which the color changes</param>
        /// <returns></returns>
        public static Color GetRainbowColor(double time, double speed)
        {

            double value = time * speed; // the value that is used for the sine wave
            double TAU = Math.PI * 2; // helper variable because radians go from 0 to 2PI
            // the 3 color components of the color are all calculated by a sine wave that is offset by 1/3 PI for every next value
            float redComponent = (float)Math.Abs(Math.Sin(value * TAU));
            float greenComponent = (float)Math.Abs(Math.Sin(value * TAU + TAU / 3));
            float blueComponent = (float)Math.Abs(Math.Sin(value * TAU + TAU * 2 / 3));
            return new Color(redComponent, greenComponent, blueComponent);
        }
    }
    public enum GameState
    {
        MainMenu,
        Playing,
        Finished
    }
}
