using Microsoft.Xna.Framework;

namespace Tetris2.Pieces
{
    public class SquarePiece: Tetronimo
    {
        public SquarePiece(Point position, TetrisGrid parentGrid): base(position,parentGrid)
        {
            color = 7;
            shape = new[,]
            {
                {true, true},
                {true, true}
            };
        }
    }
}