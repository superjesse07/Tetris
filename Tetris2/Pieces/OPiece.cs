using Microsoft.Xna.Framework;

namespace Tetris2.Pieces
{
    public class OPiece: Tetronimo
    {
        public OPiece(TetrisGrid parentGrid): base(parentGrid)
        {
            color = 1;
            shape = new[,]
            {
                {true, true},
                {true, true}
            };
        }
    }
}