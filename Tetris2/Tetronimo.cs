using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Tetris2
{
    public abstract class Tetronimo
    {
        public static Texture2D block;
        public static Vector2 BlockSize => new Vector2(block.Width, block.Height);
        public Point position;
        public Vector2 Vector2Position => new Vector2(position.X,position.Y);
        public bool[,] shape;
        public int color;
        private TetrisGrid parentGrid;


        protected Tetronimo(Point position, TetrisGrid parentGrid)
        {
            this.position = position;
            this.parentGrid = parentGrid;
        }


        /// <summary>
        /// Rotates the Tetronimo 90 degrees
        /// only works if the shape is a square array
        /// </summary>
        /// <param name="clockWise">if true rotate the tetronimo clockwise otherwise counter clockwise</param>
        public void Rotate(bool clockWise)
        {
            int size = shape.GetLength(0);
            bool[,] rotated = new bool[size, size];

            for (int x = 0; x < size; ++x)
            {
                for (int y = 0; y < size; ++y)
                {
                    if (clockWise)
                        rotated[x, y] = shape[y, size - x - 1]; //swap the x and y and negate the x. (-1 is necessary because otherwise it would be from 1-size instead of 0-(size-1)
                    else
                        rotated[x, y] = shape[size - y - 1, x]; //here we do the same but we negate the y 
                }
            }
            if(parentGrid.ShapeFitsInPos(rotated,position))
                shape = rotated;
        }

        public bool Fits()
        {
            return parentGrid.ShapeFitsInPos(shape, position);
        }

        public bool Move(Point dir)
        {
            bool canMove = parentGrid.ShapeFitsInPos(shape, position + dir);
            if (canMove)
            {
                position += dir;
            }

            return canMove;

        }

        public void Draw(SpriteBatch spriteBatch,Vector2 offset)
        {
            for (int x = 0; x < shape.GetLength(0); x++)
            {
                for (int y = 0; y < shape.GetLength(1); y++)
                {
                    if(shape[x,y])
                        spriteBatch.Draw(block,offset + (Vector2Position+new Vector2(x,y))*BlockSize,TetrisGrid.colors[color]);
                }
            }
        }
    }
}