using Microsoft.Xna.Framework;

namespace Tetris2.Pieces
{
    public class TPiece: Tetronimo
    {
        public TPiece(Point position, TetrisGrid parentGrid): base(position,parentGrid)
        {
            color = 7;
            shape = new[,]
            {
                {false, true, false},
                {true, true, false},
                {false, true, false}
            };
        }
    }
}