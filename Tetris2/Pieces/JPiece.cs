using Microsoft.Xna.Framework;

namespace Tetris2.Pieces
{
    public class JPiece: Tetronimo
    {
        public JPiece(TetrisGrid parentGrid): base(parentGrid)
        {
            color = 6;
            shape = new[,]
            {
                {true, true, false},
                {false, true, false},
                {false, true, false}
            };
        }
    }
}