var map = (await File.ReadAllLinesAsync(@"..\..\..\..\day-04.txt"))
    .Select(line => line.ToCharArray())
    .ToArray();

var iterations = new List<int>();
do
{
    var movable = map.Movable().ToArray();
    iterations.Add(movable.Count());
    foreach (var roll in movable)
        map[roll.Y][roll.X] = '.';
} while (iterations[^1] != 0);

Console.WriteLine($"Part 1 - Paper rolls: {iterations[0]}");
Console.WriteLine($"Part 2 - Paper rolls: {iterations.Sum()}");
Console.ReadLine();

record Point(int X, int Y);

internal static class Extensions
{
    internal static Point[] Neighbours(this Point source, char[][] map)
    {
        List<Point> neighbours = 
            [
                new Point(source.X - 1, source.Y - 1),
                new Point(source.X, source.Y - 1),
                new Point(source.X + 1, source.Y - 1),
                new Point(source.X - 1, source.Y),
                new Point(source.X + 1, source.Y),
                new Point(source.X - 1, source.Y + 1),
                new Point(source.X, source.Y + 1),
                new Point(source.X + 1, source.Y + 1)
            ];

        return neighbours
            .Where(p => p.X >= 0 && p.Y >= 0 && p.Y < map.Length && p.X < map[0].Length)
            .ToArray();
    }

    internal static IEnumerable<Point> Movable(this char[][] map)
    {
        for (int y = 0; y < map.Length; y++)
            for (int x = 0; x < map[0].Length; x++)
            {
                var coord = new Point(x, y);
                var countBlocked = () => coord.Neighbours(map).Count(neighbour => map[neighbour.Y][neighbour.X] == '@');
                if (map[y][x] == '@' && countBlocked() < 4)
                    yield return coord;
            }
    }
}