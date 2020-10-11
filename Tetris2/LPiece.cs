namespace Tetris2
{
    public class LPiece : Tetronimo
    {
        private const int color = 7;
        public LPiece()
        {
            shape  = new int[,]
            {
                {0    ,color,0},
                {0    ,color,0},
                {color,color,0}
            };
        }
    }
}