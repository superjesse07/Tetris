namespace Tetris2
{
    public class TPiece : Tetronimo
    {
        private const int color = 5;
        public TPiece()
        {
            shape = new int[,]
            {
                {0    ,color,    0},
                {color,color,    0},
                {0    ,color,    0}
            };
        }
    }
}