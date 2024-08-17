namespace Queens
{
    public class Grid
    {
        public int Rows { get; private set; }

        public int Columns { get; private set; }

        public List<List<Cell>> Cells { get; private set; } = [];

        public Dictionary<int, List<Cell>> Groups { get; private set; } = [];

        public HashSet<(int, int)> Crowns { get; private set; } = [];

        public Grid(List<List<int>> grid)
        {
            Rows = grid.Count;
            Columns = grid[0].Count;

            for (int row = 0; row < Rows; row++)
            {
                List<Cell> newRow = [];

                for (int column = 0; column < Columns; column++)
                {
                    Cell cell = new(grid[row][column], row, column);
                    newRow.Add(cell);

                    if (!Groups.TryAdd(cell.Color, [cell]))
                    {
                        Groups[cell.Color].Add(cell);
                    }
                }

                Cells.Add(newRow);
            }

            Groups = Groups
                .OrderBy(kvp => kvp.Value.Count)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public void Run()
        {
            GetCopy(Groups, out Dictionary<int, List<Cell>> groups);

            FindCrowns(groups);

            PrintCrowns();
        }

        private void FindCrowns(Dictionary<int, List<Cell>> groups)
        {
            List<Cell> cells = groups.First().Value; // get the cells in the current color group
            groups.Remove(groups.First().Key); // remove current color group from groups

            foreach (Cell? cell in cells)
            {
                if (cell != null)
                {
                    int row = cell.Row;
                    int column = cell.Column;

                    Crowns.Add((row, column));

                    GetCopy(groups, out Dictionary<int, List<Cell>> newGroups);

                    RemoveCells(newGroups, row, column);

                    if (newGroups.Count > 0 && !HaveEmptyGroups(newGroups))
                    {
                        FindCrowns(newGroups);
                    }

                    if (Crowns.Count == Groups.Count) return;
                    else Crowns.Remove((row, column));
                }
            }
        }

        private static void GetCopy(Dictionary<int, List<Cell>> original, out Dictionary<int, List<Cell>> copy)
        {
            copy = original.ToDictionary(
                kvp => kvp.Key,
                kvp => new List<Cell>(kvp.Value)
            );
        }

        private void RemoveCells(Dictionary<int, List<Cell>> groups, int row, int column)
        {
            HashSet<Cell> removables = [];

            GetNeighbors(removables, row, column);
            GetRowCells(removables, row);
            GetColumnCells(removables, column);

            foreach (Cell cell in removables)
            {
                if (cell != null)
                {
                    groups.TryGetValue(cell.Color, out List<Cell>? group);

                    if (group != null)
                    {
                        group.Remove(cell);

                        if (group.Count == 0)
                        {
                            groups.Remove(cell.Color);
                        }
                    }
                }
            }
        }

        private void GetNeighbors(HashSet<Cell> removables, int row, int column)
        {
            List<(int, int)> deltas =
            [
                (-1, 0), // N
                (-1, 1), // NE
                (0, 1), // E
                (1, 1), // SE
                (1, 0), // S
                (1, -1), // SW
                (0, -1), // W
                (-1, -1) // NW
            ];

            foreach ((int rowDelta, int columnDelta) in deltas)
            {
                int neighbor_row = row + rowDelta;
                int neighbor_column = column + columnDelta;

                if (IsValidCell(neighbor_row, neighbor_column))
                {
                    removables.Add(Cells[neighbor_row][neighbor_column]);
                }
            }
        }

        private void GetRowCells(HashSet<Cell> removables, int row)
        {
            foreach (Cell? cell in Cells[row])
            {
                removables.Add(cell);
            }
        }

        private void GetColumnCells(HashSet<Cell> removables, int column)
        {
            int row = 0;

            while (row < Rows)
            {
                removables.Add(Cells[row][column]);
                row++;
            }
        }

        private bool IsValidCell(int row, int column)
        {
            return (row >= 0 && row < Rows && column >= 0 && column < Columns);
        }

        private static bool HaveEmptyGroups(Dictionary<int, List<Cell>> groups)
        {
            foreach (KeyValuePair<int, List<Cell>> kvp in groups)
            {
                if (kvp.Value.Count == 0) return true;
            }

            return false;
        }

        private void PrintCrowns()
        {
            foreach ((int, int) crown in Crowns.OrderBy(t => t.Item1))
            {
                Console.WriteLine($"[ {crown.Item1}, {crown.Item2} ]");
            }
        }
    }
}
