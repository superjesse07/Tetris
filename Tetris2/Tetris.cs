using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Tetris2
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Tetris : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Vector3 _screenOffset;
        private Color _rainbowcolor;
        private const float ColorChangeSpeed = 0.05f;
        private TetrisGrid _grid;
        public InputHelper inputHelper = new InputHelper();
        

        public Tetris()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 800;
            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
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
            _grid = new TetrisGrid(10,20,Vector2.Zero);


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
            
            inputHelper.Update(gameTime);
            
            if(inputHelper.KeyPressed(Keys.E)) _grid.t.Rotate(true);
            if(inputHelper.KeyPressed(Keys.Q)) _grid.t.Rotate(false);
            
            if(inputHelper.KeyPressed(Keys.D))_grid.t.position+= new Point(1,0);
            if(inputHelper.KeyPressed(Keys.A))_grid.t.position-= new Point(1,0);
            if(inputHelper.KeyPressed(Keys.W))_grid.t.position-= new Point(0,1);
            if(inputHelper.KeyPressed(Keys.S))_grid.t.position+= new Point(0,1);
            if(inputHelper.KeyPressed(Keys.Space))_grid.PlaceInGrid();

            _rainbowcolor = GetRainbowColor(gameTime.TotalGameTime.TotalSeconds, ColorChangeSpeed); // setting the background color (or colour for educated people)
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(_rainbowcolor);

            _spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Matrix.CreateTranslation(_screenOffset));
            _grid.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
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
