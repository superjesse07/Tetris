using Microsoft.Xna.Framework;

namespace Tetris2.Pieces
{
    public class ZPiece : Tetronimo
    {
        public ZPiece(Point position, TetrisGrid parentGrid): base(position,parentGrid)
        {
            color = 2;
            shape = new[,]
            {
                {true, false, false},
                {true, true, false},
                {false, true, false}
            };
        }
    }
}