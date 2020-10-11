using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tetris2
{
    class TetrisGrid
    {
        public int[,] grid;
        
        private Color _outline = Color.White;
        public static Color[] colors = new[] {Color.Black,Color.Yellow, Color.Aqua, Color.Green, Color.Red, Color.Purple, Color.Blue, Color.Orange};
        private Vector2 offset;

        public Tetronimo t;

        public TetrisGrid(int width, int height, Vector2 offset)
        {
            this.offset = offset;
            grid = new int[width,height];
            t = new LPiece();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int x = -1; x <= grid.GetLength(0); x++)
            {
                for (int y = -1; y <= grid.GetLength(1); y++)
                {

                    bool isOutline = x == -1 || y == -1 || x == grid.GetLength(0) || y == grid.GetLength(1);
                    spriteBatch.Draw(Tetronimo.block, new Vector2(x+1,y+1)*Tetronimo.blockSize + offset,isOutline ? _outline : colors[grid[x,y]]);
                }
            }
            t.Draw(spriteBatch,offset + Tetronimo.blockSize);
        }


        public void PlaceInGrid()
        {
            for (int x = 0; x < t.shape.GetLength(0); x++)
            {
                for (int y = 0; y < t.shape.GetLength(1); y++)
                {
                    if(t.shape[x,y] != 0)
                        grid[x + t.position.X, y + t.position.Y] = t.shape[x, y];
                }
            }
        }


    }
}
