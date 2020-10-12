namespace Tetris2
{
    public class ZPiece : Tetronimo
    {
        private const int color = 4;
        public ZPiece()
        {
            shape = new int[,]
            {
                {color,    0,    0},
                {color,color,    0},
                {0    ,color,    0}
            };
        }
    }
}