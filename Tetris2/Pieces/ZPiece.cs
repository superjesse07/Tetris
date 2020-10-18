using Microsoft.Xna.Framework;

namespace Tetris2.Pieces
{
    public class ZPiece : Tetronimo
    {
        public ZPiece(TetrisGrid parentGrid): base(parentGrid)
        {
            color = 4;
            shape = new[,]
            {
                {true, false, false},
                {true, true, false},
                {false, true, false}
            };
        }
    }
}