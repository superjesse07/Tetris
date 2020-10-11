using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Tetris2
{
    public abstract class Tetronimo
    {
        public static Texture2D block;
        public static Vector2 blockSize => new Vector2(block.Width, block.Height);
        public Point position;
        public Vector2 vector2Position => new Vector2(position.X,position.Y);
        public int[,] shape;


        /// <summary>
        /// Rotates the Tetronimo 90 degrees
        /// only works if the shape is a square array
        /// </summary>
        /// <param name="clockWise">if true rotate the tetronimo clockwise otherwise counter clockwise</param>
        public void Rotate(bool clockWise)
        {
            int size = shape.GetLength(0);
            int[,] rotated = new int[size, size];

            for (int x = 0; x < size; ++x)
            {
                for (int y = 0; y < size; ++y)
                {
                    if (clockWise)
                        rotated[x, y] = shape[y, size - x - 1];
                    else
                        rotated[x, y] = shape[size - y - 1, x];
                }
            }

            shape = rotated;
        }

        public void Draw(SpriteBatch spriteBatch,Vector2 offset)
        {
            for (int x = 0; x < shape.GetLength(0); x++)
            {
                for (int y = 0; y < shape.GetLength(1); y++)
                {
                    if(shape[x,y] != 0)
                        spriteBatch.Draw(block,offset + (vector2Position+new Vector2(x,y))*blockSize,TetrisGrid.colors[shape[x,y]]);
                }
            }
        }
    }
}