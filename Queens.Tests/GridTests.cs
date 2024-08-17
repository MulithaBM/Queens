namespace Queens.Tests
{
    public class GridTests
    {
        [Fact]
        public void Constructor_ProperInitialization()
        {
            List<List<int>> inputGrid =
            [
                [1, 2],
                [3, 4]
            ];

            Grid grid = new(inputGrid);

            Assert.Equal(2, grid.Rows);
            Assert.Equal(2, grid.Columns);
            Assert.Equal(4, grid.Groups.Count);
        }
    }
}
