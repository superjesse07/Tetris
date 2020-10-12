namespace Tetris2
{
    public class JPiece : Tetronimo
    {
        private const int color = 6;
        public JPiece()
        {
            shape = new int[,]
            {
                {color,color,    0},
                {0    ,color,    0},
                {0    ,color,    0}
            };
        }
    }
}