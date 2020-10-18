using Microsoft.Xna.Framework;

namespace Tetris2.Pieces
{
    public class SPiece: Tetronimo
    {
        public SPiece(TetrisGrid parentGrid): base(parentGrid)
        {
            color = 3;
            shape = new[,]
            {
                {false, true, false},
                {true, true, false},
                {true, false, false}
            };
        }
    }
}