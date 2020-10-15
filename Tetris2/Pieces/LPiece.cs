using Microsoft.Xna.Framework;

namespace Tetris2.Pieces
{
    public class LPiece : Tetronimo
    {
        public LPiece(Point position, TetrisGrid parentGrid) : base(position,parentGrid)
        {
            color = 7;
            shape = new[,]
            {
                {false, true, false},
                {false, true, false},
                {true, true, false}
            };
        }
    }
}