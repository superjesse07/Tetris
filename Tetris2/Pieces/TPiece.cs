using Microsoft.Xna.Framework;

namespace Tetris2.Pieces
{
    public class TPiece: Tetronimo
    {
        public TPiece(TetrisGrid parentGrid): base(parentGrid)
        {
            color = 5;
            shape = new[,]
            {
                {false, true, false},
                {true, true, false},
                {false, true, false}
            };
        }
    }
}