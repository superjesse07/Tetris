using Microsoft.Xna.Framework;

namespace Tetris2.Pieces
{
    public class JPiece: Tetronimo
    {
        public JPiece(Point position, TetrisGrid parentGrid): base(position,parentGrid)
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