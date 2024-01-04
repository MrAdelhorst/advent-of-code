var grid = File.ReadAllLines(@"..\..\..\..\day-14.txt").Select(line => line.ToArray()).ToArray();

//Part 1
var part1Grid = grid.DeepClone();
grid.RollNorth();
var weight = grid.CalculateWeight();

//Part 2
long finalWeight = 0;
var history = new HashSet<Grid>(new GridComparer());
for (long iteration = 1; iteration <= 1000000000; iteration++)
{
    grid.RollNorth();
    grid.RollWest();
    grid.RollSouth();
    grid.RollEast();
    var current = new Grid(grid.DeepClone(), iteration);
    if (history.TryGetValue(current, out var match))
    {
        var cycleLength = iteration - match.Iteration;
        var iterationOfInterest = ((1000000000 - match.Iteration) % cycleLength) + match.Iteration;
        var gridOfInterest = history.First(g => g.Iteration == iterationOfInterest).AsArray();
        finalWeight = gridOfInterest.CalculateWeight();
        break;
    }
    else
        history.Add(current);
}


Console.WriteLine($"Part 1 - Weight: {weight}");
Console.WriteLine($"Part 2 - Final weight: {finalWeight}");
Console.ReadLine();

static class Extensions
{
    public static void RollNorth(this char[][] grid)
    {
        for (int x = 0; x < grid.First().Length; x++)
        {
            for (int y = 1; y < grid.Length; y++)
            {
                if (grid[y][x] == 'O')
                {
                    var yIdx = y;
                    while (--yIdx >= 0 && grid[yIdx][x] == '.');
                    if (y - yIdx > 1)
                    {
                        grid[y][x] = '.';
                        grid[yIdx + 1][x] = 'O';
                    }
                }
            }
        }
    }
    public static void RollWest(this char[][] grid)
    {
        for (int y = 0; y < grid.Length; y++)
        {
            for (int x = 1; x < grid.First().Length; x++)
            {
                if (grid[y][x] == 'O')
                {
                    var xIdx = x;
                    while (--xIdx >= 0 && grid[y][xIdx] == '.');
                    if (x - xIdx > 1)
                    {
                        grid[y][x] = '.';
                        grid[y][xIdx + 1] = 'O';
                    }
                }
            }
        }
    }
    public static void RollSouth(this char[][] grid)
    {
        for (int x = 0; x < grid.First().Length; x++)
        {
            for (int y = grid.Length - 2; y >= 0; y--)
            {
                if (grid[y][x] == 'O')
                {
                    var yIdx = y;
                    while (++yIdx < grid.Length && grid[yIdx][x] == '.');
                    if (yIdx - y > 1)
                    {
                        grid[y][x] = '.';
                        grid[yIdx - 1][x] = 'O';
                    }
                }
            }
        }
    }
    public static void RollEast(this char[][] grid)
    {
        for (int y = 0; y < grid.Length; y++)
        {
            for (int x = grid.First().Length - 2; x >= 0; x--)
            {
                if (grid[y][x] == 'O')
                {
                    var xIdx = x;
                    while (++xIdx < grid.First().Length && grid[y][xIdx] == '.');
                    if (xIdx - x > 1)
                    {
                        grid[y][x] = '.';
                        grid[y][xIdx - 1] = 'O';
                    }
                }
            }
        }
    }

    public static long CalculateWeight(this char[][] grid) => grid.Select((line, idx) => (grid.Length - idx) * line.Count(c => c == 'O')).Sum();

    public static char[][] DeepClone(this char[][] grid)
    {
        var clone = new char[grid.Length][];
        for (int i = 0; i < grid.Length; i++)
        {
            clone[i] = new char[grid[i].Length];
            Array.Copy(grid[i], clone[i], grid[i].Length);
        }
        return clone;
    }
}

public class Grid(char[][] internalValue, long iteration)
{
    public char[][] AsArray() => internalValue;
    public long Iteration => iteration;
    public override bool Equals(object obj)
    {
        return obj is Grid grid &&
               EqualityComparer<char[][]>.Default.Equals(internalValue, grid.AsArray());
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(internalValue);
    }
}

public class GridComparer : IEqualityComparer<Grid>
{
    public bool Equals(Grid x, Grid y)
    {
        if (x.AsArray().Length != y.AsArray().Length)
        {
            return false;
        }
        for (int i = 0; i < x.AsArray().Length; i++)
        {
            if (x.AsArray()[i].Length != y.AsArray()[i].Length)
            {
                return false;
            }
            for (int j = 0; j < x.AsArray()[i].Length; j++)
            {
                if (x.AsArray()[i][j] != y.AsArray()[i][j])
                {
                    return false;
                }
            }
        }
        return true;
    }

    public int GetHashCode(Grid obj)
    {
        return obj.GetHashCode();
    }
}