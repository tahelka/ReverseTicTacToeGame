namespace game
{
    internal class Cell
    {
        private const char k_Empty = ' ';
        internal char Symbol { get; set; }
        internal int XDimension { get; set; }
        internal int YDimension { get; set; }

        internal Cell(int i_XDimension, int i_YDimension)
        {
            Symbol = k_Empty;
            XDimension = i_XDimension;
            YDimension = i_YDimension;
        }

        internal bool IsCellEmpty()
        {
            return Symbol == k_Empty;
        }
    }
}