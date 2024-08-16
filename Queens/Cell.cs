namespace Queens
{
    public class Cell
    {
        public int Color { get; }
        public int Row { get; }
        public int Column { get; }

        public Cell(int color, int row, int column)
        {
            Color = color;
            Row = row;
            Column = column;
        }
    }
}
