namespace Tetris2
{
    public class OPiece : Tetronimo
    {
        private const int color = 1;
        public OPiece()
        {
            shape = new int[,]
            {
                {color,color},
                {color,color}              
            };
        }
    }
}