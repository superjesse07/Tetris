using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tetris2.Pieces;

namespace Tetris2
{
    public class TetrisGrid
    {
        private int[,] _grid;

        private Color _outline = Color.White;
        public static Color[] colors = {new Color(25, 25, 25), Color.Yellow, Color.Aqua, Color.Green, Color.Red, Color.Purple, Color.Blue, Color.Orange};
        private readonly Vector2 _offset;

        private Tetronimo _currentTetronimo, _holdTetronimo, _nextTetronimo;

        private readonly InputHelper _inputHelper;
        private float _gravityTimer = 0.5f;
        private const float GravityTimerReset = 0.5f;
        private const float DownPressGravityMultiplier = 10;
        private const int MaxLevel = 20;
        private const float GravityTimerSubtractPerLevel = 0.02f;
        private float _movementTimer;
        private const float MovementTimerReset = 0.1f;
        private readonly Random _random;
        private bool _hasHeld;

        private readonly Rectangle _holdGrid = new Rectangle(-5, 0, Tetris.SidePanelSizes, Tetris.SidePanelSizes);
        private readonly Rectangle _nextGrid;

        private int _clearedLines;
        private const int LinesPerLevel = 10;


        private int _score;
        public bool lost;

        private readonly Keys _left, _right, _down, _rotate, _place, _hold;

        public TetrisGrid(int width, int height, Vector2 offset, int seed, Keys left, Keys right, Keys down, Keys rotate, Keys place, Keys hold)
        {
            _random = new Random(seed);
            _offset = offset;
            _left = left;
            _right = right;
            _down = down;
            _rotate = rotate;
            _place = place;
            _hold = hold;
            _grid = new int[width, height];
            _inputHelper = new InputHelper();
            _nextGrid = new Rectangle(width + 1, 0, Tetris.SidePanelSizes, Tetris.SidePanelSizes);
            SpawnNewTetronimo();
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //Draw score and level
            spriteBatch.DrawString(Tetris.font, _score.ToString(), _offset + new Vector2(0, _holdGrid.Height) * Tetronimo.BlockSize, Color.White, 0, new Vector2(Tetris.font.MeasureString(_score.ToString()).X, 0), 0.75f, SpriteEffects.None, 0);
            string level = "Level: " + (_clearedLines / LinesPerLevel).ToString();
            spriteBatch.DrawString(Tetris.font, level, _offset + new Vector2(0, _holdGrid.Height + 1) * Tetronimo.BlockSize, Color.White, 0, new Vector2(Tetris.font.MeasureString(level).X, 0), 0.75f, SpriteEffects.None, 0);

            for (int x = -1; x <= _grid.GetLength(0); x++)
            {
                for (int y = -1; y <= _grid.GetLength(1); y++)
                {
                    bool isOutline = x == -1 || y == -1 || x == _grid.GetLength(0) || y == _grid.GetLength(1);
                    spriteBatch.Draw(Tetronimo.block, new Vector2(x + 1, y + 1) * Tetronimo.BlockSize + _offset, isOutline ? _outline : colors[_grid[x, y]]);
                }
            }

            //Draw the hold grid
            DrawRectangle(spriteBatch, _holdGrid);
            DrawRectangle(spriteBatch, _nextGrid);


            DrawTetronimoInRect(spriteBatch, _holdTetronimo, _holdGrid);
            DrawTetronimoInRect(spriteBatch, _nextTetronimo, _nextGrid);

            _currentTetronimo?.Draw(spriteBatch, _offset + Tetronimo.BlockSize * (lost ? new Vector2(1, 0) : new Vector2(1, 1)));

            //Display Controls
            spriteBatch.DrawString(Tetris.font, $"Left: {_left.ToString()}\nRight: {_right.ToString()}\nDown: {_down.ToString()}\nRotate: {_rotate.ToString()}\nPlace: {_place.ToString()}\nHold: {_hold.ToString()}", _offset + new Vector2(_grid.GetLength(0) + 2, _holdGrid.Height) * Tetronimo.BlockSize, Color.White, 0, Vector2.Zero, 0.4f, SpriteEffects.None, 0);
        }

        private void DrawTetronimoInRect(SpriteBatch spriteBatch, Tetronimo tetronimo, Rectangle rect)
        {
            tetronimo?.Draw(spriteBatch, _offset + (new Vector2(rect.X, rect.Y) + (new Vector2(rect.Width, rect.Height) - tetronimo.Size) / 2) * Tetronimo.BlockSize);
        }


        private void DrawRectangle(SpriteBatch spriteBatch, Rectangle rect)
        {
            for (int x = 0; x < rect.Width; x++)
            {
                for (int y = 0; y < rect.Height; y++)
                {
                    bool isOutline = x == 0 || y == 0 || x == rect.Width - 1 || y == rect.Height - 1;
                    if (isOutline)
                    {
                        spriteBatch.Draw(Tetronimo.block, (new Vector2(x, y) + new Vector2(rect.X, rect.Y)) * Tetronimo.BlockSize + _offset, _outline);
                    }
                }
            }
        }
        /// <summary>
        /// handles the logic for the grid
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            if (lost) return;
            float deltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;
            _inputHelper.Update(gameTime);

            if (_movementTimer <= 0)
            {
                if (_inputHelper.KeyDown(_right))
                {
                    if (_currentTetronimo.Move(new Point(1, 0)))
                        _movementTimer = MovementTimerReset;
                }
                else if (_inputHelper.KeyDown(_left))
                {
                    if (_currentTetronimo.Move(new Point(-1, 0)))
                        _movementTimer = MovementTimerReset;
                }
            }
            else
            {
                _movementTimer -= deltaTime;
            }

            if (_gravityTimer <= 0)
            {
                if (!_currentTetronimo.Move(new Point(0, 1)))
                {
                    PlaceInGrid();
                }

                _gravityTimer = GravityTimerReset - GravityTimerSubtractPerLevel * Math.Min(MaxLevel, _clearedLines / LinesPerLevel);
            }
            else
            {
                _gravityTimer -= deltaTime * (_inputHelper.KeyDown(_down) ? DownPressGravityMultiplier : 1);
            }

            if (_inputHelper.KeyPressed(_rotate)) _currentTetronimo.Rotate(true);
            if (_inputHelper.KeyPressed(_place))
            {
                while (_currentTetronimo.Move(new Point(0, 1))) ;

                    PlaceInGrid();
            }

            if (_inputHelper.KeyPressed(_hold) && !_hasHeld)
            {
                var temp = _holdTetronimo;
                _holdTetronimo = _currentTetronimo;
                _holdTetronimo.position = Point.Zero;
                if (temp == null)
                    SpawnNewTetronimo();
                else
                {
                    _currentTetronimo = temp;
                    PlaceCurrentTetronimo();
                }

                _hasHeld = true;
            }
        }

        /// <summary>
        /// Checks for filled lines and removes them, then it moves down all the lines above it
        /// </summary>
        private void ClearLines()
        {
            int fullLines = 0;
            for (int y = 0; y < _grid.GetLength(1); y++)
            {
                bool isFull = true;
                for (int x = 0; x < _grid.GetLength(0); x++)
                {
                    if (_grid[x, y] == 0)
                    {
                        isFull = false;
                        break;
                    }
                }

                if (isFull)
                {
                    fullLines++;
                    for (int oldY = y; oldY > 0; oldY--)
                    {
                        for (int x = 0; x < _grid.GetLength(0); x++)
                        {
                            _grid[x, oldY] = _grid[x, oldY - 1];
                        }
                    }
                }
            }

            if (fullLines > 0)
            {
                _score += Tetris.scorePerLine[Math.Min(fullLines - 1, Tetris.scorePerLine.Length)]; //add the defined scores per line to the score variable ( The math.min is only there for absolute certainty even though you can't clear more than 4 at once)
                _clearedLines += fullLines;
            }
        }

        /// <summary>
        /// sets current tetronimo to next and picks a new one for the next tetronimo
        /// also checks if the current one fits and if not ends the game
        /// </summary>
        private void SpawnNewTetronimo()
        {
            _currentTetronimo = _currentTetronimo == null ? GetRandomTetronimo() : _nextTetronimo;
            _nextTetronimo = GetRandomTetronimo();

            PlaceCurrentTetronimo();

            if (!_currentTetronimo.Fits())
            {
                lost = true;
            }
        }


        /// <summary>
        /// Pick a random tetronimo out of the 7 possible
        /// </summary>
        /// <returns>a random tetronimo</returns>
        private Tetronimo GetRandomTetronimo()
        {
            int nextPiece = _random.Next(7); // apparently maxvalue is not really max value so to have it pick 6 it has to be 7 ¯\_(ツ)_/¯
            switch (nextPiece)
            {
                case 0:
                    return new IPiece(this);
                case 1:
                    return new JPiece(this);
                case 2:
                    return new LPiece(this);
                case 3:
                    return new SPiece(this);
                case 4:
                    return new OPiece(this);
                case 5:
                    return new TPiece(this);
                default:
                    return new ZPiece(this);
            }
        }

        /// <summary>
        /// Positions the current tetronimo at the spawn position
        /// </summary>
        private void PlaceCurrentTetronimo()
        {
            Point spawnPos = new Point((int) (_grid.GetLength(0) - _currentTetronimo.Size.X) / 2, 0);
            for (int y = 0; y < _currentTetronimo.shape.GetLength(1); y++)
            {
                bool hasBlockInRow = false;
                for (int x = 0; x < _currentTetronimo.shape.GetLength(0); x++)
                {
                    if (_currentTetronimo.shape[x, y])
                    {
                        hasBlockInRow = true;
                        break;
                    }
                }

                if (hasBlockInRow)
                {
                    spawnPos -= new Point(0, y);
                    break;
                }
            }

            _currentTetronimo.position = spawnPos;
        }

        /// <summary>
        /// Checks if the supplied shape with the supplied position fits in the grid
        /// </summary>
        /// <param name="shape">the shape to check</param>
        /// <param name="position">the position of the shape</param>
        /// <returns>if the shape fits</returns>
        public bool ShapeFitsInPos(bool[,] shape, Point position)
        {
            for (int x = 0; x < shape.GetLength(0); x++)
            {
                for (int y = 0; y < shape.GetLength(1); y++)
                {
                    if (shape[x, y] == false) continue;
                    Point blockPosition = new Point(x, y) + position;
                    if (blockPosition.X < 0 || blockPosition.Y < 0 || blockPosition.X >= _grid.GetLength(0) || blockPosition.Y >= _grid.GetLength(1)) return false;
                    if (_grid[blockPosition.X, blockPosition.Y] != 0) return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Places the current tetronimo in the grid at it's current location
        /// </summary>
        private void PlaceInGrid()
        {
            for (int x = 0; x < _currentTetronimo.shape.GetLength(0); x++)
            {
                for (int y = 0; y < _currentTetronimo.shape.GetLength(1); y++)
                {
                    if (_currentTetronimo.shape[x, y])
                        _grid[x + _currentTetronimo.position.X, y + _currentTetronimo.position.Y] = _currentTetronimo.color;
                }
            }

            _hasHeld = false;
            ClearLines();
            SpawnNewTetronimo();
        }
    }
}