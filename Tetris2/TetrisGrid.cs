﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
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
        private int[,] grid;
        
        private Color _outline = Color.White;
        public static Color[] colors = {Color.Black,Color.Yellow, Color.Aqua, Color.Green, Color.Red, Color.Purple, Color.Blue, Color.Orange};
        private Vector2 offset;

        private Tetronimo t;

        private InputHelper _inputHelper;
        private float _gravityTimer = 0.5f;
        private const float GravityTimerReset = 0.5f;
        private const float GravityMultiplier = 10;
        private float _movementTimer;
        private const float MovementTimerReset = 0.05f;

        public TetrisGrid(int width, int height, Vector2 offset)
        {
            this.offset = offset;
            grid = new int[width,height];
            t = new ZPiece(new Point(width/2-2,0), this);
            _inputHelper = new InputHelper();
        }

        public void Draw(SpriteBatch spriteBatch,GameTime gameTime)
        {
            for (int x = -1; x <= grid.GetLength(0); x++)
            {
                for (int y = -1; y <= grid.GetLength(1); y++)
                {

                    bool isOutline = x == -1 || y == -1 || x == grid.GetLength(0) || y == grid.GetLength(1);
                    spriteBatch.Draw(Tetronimo.block, new Vector2(x+1,y+1)*Tetronimo.BlockSize + offset,isOutline ? _outline : colors[grid[x,y]]);
                }
            }
            t.Draw(spriteBatch,offset + Tetronimo.BlockSize);
        }

        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _inputHelper.Update(gameTime);

            if (_movementTimer <= 0)
            {
                if (_inputHelper.KeyDown(Keys.D))
                {
                    if (t.Move(new Point(1, 0)))
                        _movementTimer = MovementTimerReset;
                }
                else if (_inputHelper.KeyDown(Keys.A))
                {
                    if (t.Move(new Point(-1, 0)))
                        _movementTimer = MovementTimerReset;
                }
            }
            else
            {
                _movementTimer -= deltaTime;
            }

            if (_gravityTimer <= 0)
            {
                t.Move(new Point(0, 1));
                _gravityTimer = GravityTimerReset;
            }
            else
            {
                _gravityTimer -= deltaTime * (_inputHelper.KeyDown(Keys.S) ? GravityMultiplier : 1);
            }
            
        }

        public bool ShapeFitsInPos(bool[,] shape, Point position)
        {
            for (int x = 0; x < shape.GetLength(0); x++)
            {
                for (int y = 0; y < shape.GetLength(1); y++)
                {
                    if (shape[x, y] == false) continue;
                    Point blockPosition = new Point(x,y) + position;
                    if (blockPosition.X < 0 || blockPosition.Y < 0 || blockPosition.X >= grid.GetLength(0) || blockPosition.Y >= grid.GetLength(1)) return false;
                    if (grid[blockPosition.X, blockPosition.Y] != 0) return false;
                }
            }

            return true;
        }


        public void PlaceInGrid()
        {
            for (int x = 0; x < t.shape.GetLength(0); x++)
            {
                for (int y = 0; y < t.shape.GetLength(1); y++)
                {
                    if(t.shape[x,y])
                        grid[x + t.position.X, y + t.position.Y] = t.color;
                }
            }
        }


    }
}
