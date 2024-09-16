var grid = File.ReadLines(@"..\..\..\..\day-18.txt").Select(line => line.Select(c => c == '#').ToArray()).ToArray();

Console.WriteLine($"Part 1 - lights on: {AnimateGrid(grid, _ => { }).Sum(l => l.Count(b => b))}");
Console.WriteLine($"Part 2 - lights on: {AnimateGrid(grid, CornersAlwaysOn).Sum(l => l.Count(b =>b))}");
Console.ReadLine();

bool[][] AnimateGrid(bool[][] grid, Action<bool[][]> cornerBehavior)
{
    var current = grid.Select(x => x.ToArray()).ToArray();
    cornerBehavior(current);
    for (int i = 0; i < 100; i++)
        current = current.Animate(cornerBehavior);

    return current;
}

void CornersAlwaysOn(bool[][] grid)
{
    grid[0][0] = true;
    grid[0][grid.First().Length - 1] = true;
    grid[grid.Length - 1][0] = true;
    grid[grid.Length - 1][grid.First().Length - 1] = true;
}

static class Extensions
{
    public static bool[][] Animate(this bool[][] grid, Action<bool[][]> cornerBehavior)
    {
        var clone = grid.Select(x => x.ToArray()).ToArray();
        for (int x = 0; x < grid.First().Length; x++)
        {
            for(int y = 0; y < grid.Length; y++)
                clone[y][x] = grid.AnimatePixel(x, y);
        }
        cornerBehavior(clone);

        return clone;
    }

    private static bool AnimatePixel(this bool[][] grid, int x, int y)
    {
        (int x, int y)[] neighbours = [(x-1, y-1), (x, y-1), (x+1, y-1), (x-1, y), (x+1, y), (x-1, y+1), (x, y+1), (x+1, y+1)];
        var neighboursOn = neighbours.Count((coord) => coord.x >= 0 && coord.x < grid.First().Length && coord.y >= 0 && coord.y < grid.Length && grid[coord.y][coord.x]);
        return neighboursOn == 3 || grid[y][x] && neighboursOn == 2;
    }
}