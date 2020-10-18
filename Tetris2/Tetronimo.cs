using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Tetris2
{
    public abstract class Tetronimo
    {
        public static Texture2D block; //block texture
        public static Vector2 BlockSize => new Vector2(block.Width, block.Height); //Block Size dependent on the texture
        public Point position; //position of the tetronimo. Upper left corner
        private Vector2 Vector2Position => new Vector2(position.X,position.Y); //A vector2 of the position
        public bool[,] shape { get; protected set; } //type of tetronimo
        public Vector2 Size => new Vector2(shape.GetLength(0), shape.GetLength(1)); //GridSize of the tetronimo x and y
        public int color; //color(colour) of the tetronimo piece
        private TetrisGrid parentGrid; //Copy of the current grid 


        protected Tetronimo(TetrisGrid parentGrid) //getting access to the TetrisGrid
        {
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
            if(parentGrid.ShapeFitsInPos(rotated,position)) //check if the shape fits when it is rotated
                shape = rotated;
            else if (parentGrid.ShapeFitsInPos(rotated, position + new Point(1, 0))) //check if the shape fits when rotated in the space to the right next to the rotated tetronimo
            {
                shape = rotated;
                position+=new Point(1,0); //update tetronimo pos (this is called a kick)
            }
            else if (parentGrid.ShapeFitsInPos(rotated, position + new Point(-1, 0))) //check if the shape fits when rotated in the space to the left next to the rotated tetronimo
            {
                shape = rotated;
                position-=new Point(1,0); //update tetronimo pos (this is called a kick)
            }
        }

        public bool Fits() // return true if the shape fits in the position
        {
            return parentGrid.ShapeFitsInPos(shape, position);
        }

        public bool Move(Point dir) //return true if the move can be done
        {
            bool canMove = parentGrid.ShapeFitsInPos(shape, position + dir);
            if (canMove)
            {
                position += dir;
            }

            return canMove;

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="offset">Provides a offset for the grid.</param>
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