namespace Tetris2
{
    public class SPiece : Tetronimo
    {
        private const int color = 3;
        public SPiece()
        {
            shape = new int[,]
            {
                {0    ,color,    0},
                {color,color,    0},
                {color,    0,    0}
            };
        }
    }
}