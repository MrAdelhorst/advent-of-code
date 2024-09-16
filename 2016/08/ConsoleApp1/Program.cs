var grid = new bool[50, 6];
var commands = File.ReadAllLines(@"..\..\..\..\day-08.txt").Select<string, (Action<bool[,], int, int> func, int[] arguments)>(line => line.Split(' ') switch
{
    ["rect", var rect] => (Rect, rect.Split('x').Select(int.Parse).ToArray()),
    ["rotate", "row", var row, _, var number] => (RotateRow, [int.Parse(row.Split('=').Last()), int.Parse(number)]),
    ["rotate", "column", var column, _, var number] => (RotateColumn, [int.Parse(column.Split('=').Last()), int.Parse(number)])
});

foreach (var (func, arguments) in commands)
{
    func(grid, arguments[0], arguments[1]);
}

Console.WriteLine($"Part 1 - # of lit pixels: {grid.Cast<bool>().Count(b => b)}");

//Part 2
for (int y = 0; y < grid.GetLength(1); y++)
{
    Console.WriteLine();
    for (int x = 0; x < grid.GetLength(0); x++)
    {
        var space = (x % 5 == 0) ? " " : "";
        Console.Write(space + (grid[x, y] ? '#' : ' '));
    }
}

Console.ReadLine();

static void Rect(bool[,] grid, int x, int y)
{
    for (int i = 0; i < x; i++)
        for (int j = 0; j < y; j++)
            grid[i, j] = true;
}

static void RotateRow(bool[,] grid, int row, int number)
{
    var copy = grid.Clone() as bool[,];
    var width = grid.GetLength(0);
    var rotateBy = number % width;
    for (int x = 0; x < width; x++)
        grid[x, row] = copy[(x - rotateBy < 0) ? width + (x - rotateBy) : x - rotateBy, row];    
}

static void RotateColumn(bool[,] grid, int column, int number)
{
    var copy = grid.Clone() as bool[,];
    var height = grid.GetLength(1);
    var rotateBy = number % height;
    for (int y = 0; y < height; y++)
        grid[column, y] = copy[column, (y - rotateBy < 0) ? height + (y - rotateBy) : y - rotateBy];
}