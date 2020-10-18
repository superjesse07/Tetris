using Microsoft.Xna.Framework;

namespace Tetris2.Pieces
{
    public class LPiece : Tetronimo
    {
        public LPiece(TetrisGrid parentGrid) : base(parentGrid)
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