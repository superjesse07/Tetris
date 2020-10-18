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

        public static readonly int[] scorePerLine = {40, 100, 300, 1200}; //Score for clearing lines
        public TetrisGrid[] players; //Seperately stores the games for the different players 
        private const int GridWidth=10, GridHeight = 20; //the sizes of the grids
        public const int SidePanelSizes = 6; //This is the same as the 6 in Tetrisgrid for the holdGrid and nextGrid 6 because max size of piece is 4 plus 2 for outline
        private const int Divider = 4; //The amount of blocks between the two player grids
        private Vector2 ScreenSize => new Vector2(Window.ClientBounds.Width,Window.ClientBounds.Height); //Var for the screensize
        
        private Random _random = new Random();
        private Song song;
        public static SoundEffect placeSFX;
        
        public Tetris() //Initial screen when you launch the game
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

            Tetronimo.block = Content.Load<Texture2D>("block"); //load the block texture (homemade)
            font = Content.Load<SpriteFont>("Arial"); //load the textfont
            song = Content.Load<Song>("Tetris"); //load the song
            placeSFX = Content.Load<SoundEffect>("hitsfx"); //load the sound effects
            SoundEffect.MasterVolume = 0.1f; //To make sure you're not earraped
            MediaPlayer.Play(song); 
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.1f; //To make sure you're not earraped
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) //To make sure you can accidentally close the game
                Exit();

            KeyboardState keyboard = Keyboard.GetState();
            if (state == GameState.MainMenu)
            {
                if (keyboard.IsKeyDown(Keys.D1)) //Press 1 for single player
                { 
                    //change the screen size to fit one window
                    _graphics.PreferredBackBufferHeight = (int) ((GridHeight + 2) * Tetronimo.BlockSize.Y); // 2 is for the outline blocks
                    _graphics.PreferredBackBufferWidth = (int) ((GridWidth + SidePanelSizes*2) * Tetronimo.BlockSize.Y); // the outline blocks are included in the side panels so no +2
                    _graphics.ApplyChanges();
                    int seed = _random.Next();
                    players = new[]
                    {
                        new TetrisGrid(GridWidth, GridHeight, new Vector2(SidePanelSizes-1,0) * Tetronimo.BlockSize, seed, Keys.A, Keys.D, Keys.S, Keys.W, Keys.Space, Keys.C) //Make a new instance of the game
                    };
                    state = GameState.Playing; //Set gamestate
                }
                if (keyboard.IsKeyDown(Keys.D2)) //Press 2 for multiplayer battle
                {
                    int seed = _random.Next();
                    _graphics.PreferredBackBufferHeight = (int) ((GridHeight + 2) * Tetronimo.BlockSize.Y); // 2 is for the outline blocks
                    _graphics.PreferredBackBufferWidth = (int) (((GridWidth + SidePanelSizes*2)*2 + Divider) * Tetronimo.BlockSize.Y); // the outline blocks are included in the side panels so no +2
                    players = new[]
                    {
                        new TetrisGrid(GridWidth, GridHeight, new Vector2(SidePanelSizes-1,0) * Tetronimo.BlockSize, seed, Keys.A, Keys.D, Keys.S, Keys.W, Keys.Space, Keys.C), //Make a new instance of the game
                        new TetrisGrid(GridWidth, GridHeight, new Vector2(SidePanelSizes-1 + SidePanelSizes*2 + GridWidth+Divider,0) * Tetronimo.BlockSize, seed, Keys.Left, Keys.Right, Keys.Down, Keys.Up, Keys.RightControl, Keys.RightShift) //Make a new instance of the game with different controls and a offset
                    };
                    _graphics.ApplyChanges();
                    state = GameState.Playing; //Set gamestate
                }
            }
            else if (state == GameState.Playing)
            {
                foreach (var player in players)
                {
                    player.Update(gameTime); //Update each players game
                    if (player.lost) //Check if a player has lost
                    {
                        state = GameState.Finished; //Update gamestate
                    }
                }
            }
            else if (state == GameState.Finished)
            {
                if (keyboard.IsKeyDown(Keys.Enter)) state = GameState.MainMenu; //Update gamestate
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
                _spriteBatch.DrawString(font,mainMenuText,ScreenSize/2,Color.White,0,font.MeasureString(mainMenuText)/2,1,SpriteEffects.None,0); //Draw menuscreen text
            }
            if (state == GameState.Playing || state == GameState.Finished)
            {
                foreach (var player in players)
                {
                    player.Draw(_spriteBatch, gameTime); //Draw the game(s)
                }
            }

            if (state == GameState.Finished)
            {
                string spaceText = "Press enter to continue";
                if (players.Length == 1) //Check for multiplayer
                {
                    string finishedText = "You lost";
                    _spriteBatch.DrawString(font, finishedText, ScreenSize / 2, Color.White, 0, font.MeasureString(finishedText) / 2, 1, SpriteEffects.None, 0); //Draw loser text
                    _spriteBatch.DrawString(font, spaceText, ScreenSize / 2 + new Vector2(0, font.MeasureString(finishedText).Y / 1.5f), Color.White, 0, font.MeasureString(spaceText) / 2, 0.5f, SpriteEffects.None, 0); //Draw 'Enter' text
                }
                else
                {
                    string finishedText = $"{(players[0].lost ? "Left" : "Right")} lost";
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
            var reciever = players.FirstOrDefault(x => x != sender);
            if (reciever != null) reciever.garbageLines += amount;
        }
    }
    public enum GameState
    {
        MainMenu,
        Playing,
        Finished
    }
}
