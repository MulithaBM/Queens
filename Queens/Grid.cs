namespace Queens
{
    internal class Grid
    {
        private readonly int _rows;
        private readonly int _columns;
        private readonly List<List<Cell?>> _grid = [];
        private readonly Dictionary<int, List<Cell?>> _groups = [];
        private readonly HashSet<(int, int)> _crowns = [];

        public Grid(List<List<int>> grid)
        {
            _rows = grid.Count;
            _columns = grid[0].Count;

            for (int row = 0; row < _rows; row++)
            {
                List<Cell?> newRow = [];

                for (int column = 0; column < _columns; column++)
                {
                    Cell cell = new(grid[row][column], row, column);
                    newRow.Add(cell);

                    if (!_groups.TryAdd(cell.Color, [cell]))
                    {
                        _groups[cell.Color].Add(cell);
                    }
                }

                _grid.Add(newRow);
            }

            _groups = _groups
                .OrderBy(kvp => kvp.Value.Count)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public void Run()
        {
            Dictionary<int, List<Cell?>> groups;
            GetCopy(_groups, out groups);

            Execute(groups);

            PrintCrowns();
        }

        private void Execute(Dictionary<int, List<Cell?>> groups)
        {
            List<Cell?> cells = groups.First().Value;
            groups.Remove(groups.First().Key);

            foreach (Cell? cell in cells)
            {
                if (cell != null)
                {
                    int row = cell.Row;
                    int column = cell.Column;

                    _crowns.Add((row, column));
                    //if (_crowns.Count == _groups.Count) return;

                    GetCopy(groups, out Dictionary<int, List<Cell?>> newGroups);

                    RemoveCells(newGroups, row, column);

                    if (newGroups.Count > 0 && !EmptyGroups(newGroups))
                    {
                        Execute(newGroups);
                    }

                    if (_crowns.Count == _groups.Count) return;
                    else _crowns.Remove((row, column));
                }
            }
        }

        private void GetCopy(Dictionary<int, List<Cell?>> original, out Dictionary<int, List<Cell?>> copy)
        {
            copy = original.ToDictionary(
                kvp => kvp.Key,
                kvp => new List<Cell?>(kvp.Value)
            );
        }

        private void RemoveCells(Dictionary<int, List<Cell?>> groups, int row, int column)
        {
            HashSet<Cell?> removables = [];

            GetNeighbors(removables, row, column);
            GetRowCells(removables, row);
            GetColumnCells(removables, column);

            foreach (Cell? cell in removables)
            {
                if (cell != null)
                {
                    groups.TryGetValue(cell.Color, out List<Cell?>? group);

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

        private void GetNeighbors(HashSet<Cell?> removables, int row, int column)
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
                    removables.Add(_grid[neighbor_row][neighbor_column]);
                }
            }
        }

        private void GetRowCells(HashSet<Cell?> removables, int row)
        {
            foreach (Cell? cell in _grid[row])
            {
                removables.Add(cell);
            }
        }

        private void GetColumnCells(HashSet<Cell?> removables, int column)
        {
            int row = 0;

            while (row < _rows)
            {
                removables.Add(_grid[row][column]);
                row++;
            }
        }

        private bool IsValidCell(int row, int column)
        {
            return (row >= 0 && row < _rows && column >= 0 && column < _columns);
        }

        private bool EmptyGroups(Dictionary<int, List<Cell?>> groups)
        {
            foreach (KeyValuePair<int, List<Cell?>> kvp in groups)
            {
                if (kvp.Value.Count == 0) return true;
            }

            return false;
        }

        private void PrintCrowns()
        {
            foreach ((int, int) crown in _crowns.OrderBy(t => t.Item1))
            {
                Console.WriteLine($"[ {crown.Item1}, {crown.Item2} ]");
            }
        }

        // Helper methods
        public void PrintGrid()
        {
            Console.WriteLine(" <<< Grid >>>");

            foreach (List<Cell?> row in _grid)
            {
                Console.WriteLine($"[ {string.Join(", ", row.Select(c => c == null ? 0 : c.Color))} ]");
            }

            Console.WriteLine();
            Console.WriteLine();
        }

        public void PrintGroups()
        {
            Console.WriteLine(" <<< Groups >>>");


            foreach (KeyValuePair<int, List<Cell?>> kvp in _groups)
            {
                Console.WriteLine($"{kvp.Key}: [ {string.Join(", ", kvp.Value.Select(c => c == null ? 0 : c.Color).ToList())} ]");
            }

            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
