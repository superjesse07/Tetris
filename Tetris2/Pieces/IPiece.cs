using Microsoft.Xna.Framework;

namespace Tetris2.Pieces
{
    public class IPiece : Tetronimo
    {
        public IPiece(TetrisGrid parentGrid) : base(parentGrid)
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