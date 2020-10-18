using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Configuration;
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
        public static SpriteFont font;
        public static GameState state;
        public static Tetris tetris { private set; get; }

        public static readonly int[] scorePerLine = {40, 100, 300, 1200};
        public TetrisGrid[] players;
        private const int GridWidth=10, GridHeight = 20; //the sizes of the grids
        public const int SidePanelSizes = 6; //This is the same as the 6 in Tetrisgrid for the holdGrid and nextGrid 6 because max size of piece is 4 plus 2 for outline
        private const int Divider = 4; //The amount of blocks between the two player grids
        private Vector2 ScreenSize => new Vector2(Window.ClientBounds.Width,Window.ClientBounds.Height);
        
        private Random _random = new Random();
        private Song song;
        public static SoundEffect placeSFX;
        
        public Tetris()
        {
            tetris = this;
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 800;
            IsMouseVisible = true;
        }
        
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Tetronimo.block = Content.Load<Texture2D>("block");
            font = Content.Load<SpriteFont>("Arial");
            song = Content.Load<Song>("Tetris");
            placeSFX = Content.Load<SoundEffect>("hitsfx");
            SoundEffect.MasterVolume = 0.1f;
            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.1f;
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState keyboard = Keyboard.GetState();
            if (state == GameState.MainMenu)
            {
                if (keyboard.IsKeyDown(Keys.D1))
                { 
                    //change the screen size to fit one window
                    _graphics.PreferredBackBufferHeight = (int) ((GridHeight + 2) * Tetronimo.BlockSize.Y); // 2 is for the outline blocks
                    _graphics.PreferredBackBufferWidth = (int) ((GridWidth + SidePanelSizes*2) * Tetronimo.BlockSize.Y); // the outline blocks are included in the side panels so no +2
                    _graphics.ApplyChanges();
                    int seed = _random.Next();
                    players = new[]
                    {
                        new TetrisGrid(GridWidth, GridHeight, new Vector2(SidePanelSizes-1,0) * Tetronimo.BlockSize, seed, Keys.A, Keys.D, Keys.S, Keys.W, Keys.Space, Keys.C)
                    };
                    state = GameState.Playing;
                }
                if (keyboard.IsKeyDown(Keys.D2))
                {
                    int seed = _random.Next();
                    _graphics.PreferredBackBufferHeight = (int) ((GridHeight + 2) * Tetronimo.BlockSize.Y); // 2 is for the outline blocks
                    _graphics.PreferredBackBufferWidth = (int) (((GridWidth + SidePanelSizes*2)*2 + Divider) * Tetronimo.BlockSize.Y); // the outline blocks are included in the side panels so no +2
                    players = new[]
                    {
                        new TetrisGrid(GridWidth, GridHeight, new Vector2(SidePanelSizes-1,0) * Tetronimo.BlockSize, seed, Keys.A, Keys.D, Keys.S, Keys.W, Keys.Space, Keys.C),
                        new TetrisGrid(GridWidth, GridHeight, new Vector2(SidePanelSizes-1 + SidePanelSizes*2 + GridWidth+Divider,0) * Tetronimo.BlockSize, seed, Keys.Left, Keys.Right, Keys.Down, Keys.Up, Keys.RightControl, Keys.RightShift)
                    };
                    _graphics.ApplyChanges();
                    state = GameState.Playing;
                }
            }
            else if (state == GameState.Playing)
            {
                foreach (var player in players)
                {
                    player.Update(gameTime);
                    if (player.lost)
                    {
                        state = GameState.Finished;
                    }
                }
            }
            else if (state == GameState.Finished)
            {
                if (keyboard.IsKeyDown(Keys.Enter)) state = GameState.MainMenu;
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
            if (state == GameState.MainMenu)
            {
                string mainMenuText = "Press 1 for Single-player\nor 2 for Multi-player";
                _spriteBatch.DrawString(font,mainMenuText,ScreenSize/2,Color.White,0,font.MeasureString(mainMenuText)/2,1,SpriteEffects.None,0);
            }
            if (state == GameState.Playing || state == GameState.Finished)
            {
                foreach (var player in players)
                {
                    player.Draw(_spriteBatch, gameTime);
                }
            }

            if (state == GameState.Finished)
            {
                string spaceText = "Press enter to continue";
                if (players.Length == 1)
                {
                    string finishedText = "You lost";
                    _spriteBatch.DrawString(font, finishedText, ScreenSize / 2, Color.White, 0, font.MeasureString(finishedText) / 2, 1, SpriteEffects.None, 0);
                    _spriteBatch.DrawString(font, spaceText, ScreenSize / 2 + new Vector2(0, font.MeasureString(finishedText).Y / 1.5f), Color.White, 0, font.MeasureString(spaceText) / 2, 0.5f, SpriteEffects.None, 0);
                }
                else
                {
                    string finishedText = $"{(players[0].lost ? "Left" : "Right")} lost";
                    _spriteBatch.DrawString(font, finishedText, ScreenSize / 2, Color.White, 0, font.MeasureString(finishedText) / 2, 1, SpriteEffects.None, 0);
                    _spriteBatch.DrawString(font, spaceText, ScreenSize / 2 + new Vector2(0, font.MeasureString(finishedText).Y / 1.5f), Color.White, 0, font.MeasureString(spaceText) / 2, 0.5f, SpriteEffects.None, 0);
                }
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public void SendGarbageLines(int amount, TetrisGrid sender)
        {
            var reciever = players.FirstOrDefault(x => x != sender);
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
