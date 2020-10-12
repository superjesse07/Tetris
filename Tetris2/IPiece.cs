namespace Tetris2
{
    public class IPiece : Tetronimo
    {
        private const int color = 2;
        public IPiece()
        {
            shape = new int[,]
            {
                {0    ,color,    0,    0},
                {0    ,color,    0,    0},
                {0    ,color,    0,    0},
                {0    ,color,    0,    0}
            };
        }
    }
}