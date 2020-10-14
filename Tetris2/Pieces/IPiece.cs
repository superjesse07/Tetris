using Microsoft.Xna.Framework;

namespace Tetris2.Pieces
{
    public class IPiece : Tetronimo
    {
        public IPiece(Point position, TetrisGrid parentGrid) : base(position,parentGrid)
        {
            color = 2;
            shape = new[,]
            {
                {false, true, false, false},
                {false, true, false, false},
                {false, true, false, false},
                {false, true, false, false}
            };
        }
    }
}