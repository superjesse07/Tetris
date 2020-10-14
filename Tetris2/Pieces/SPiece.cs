using Microsoft.Xna.Framework;

namespace Tetris2.Pieces
{
    public class SPiece: Tetronimo
    {
        public SPiece(Point position, TetrisGrid parentGrid): base(position,parentGrid)
        {
            color = 2;
            shape = new[,]
            {
                {false, true, false},
                {true, true, false},
                {true, false, false}
            };
        }
    }
}