var map = File.ReadAllLines(@"..\..\..\..\day-20.txt")
    .Select(row => row.ToArray())
    .ToArray();

var start = map.Find('S');
var goal = map.Find('E');

var visited = new Dictionary<Coordinate, int>();
var jobs = new PriorityQueue<(Coordinate pos, List<Coordinate> path, int steps), int>([((start, [start], 0), 0)]);
var bestPath = Array.Empty<Coordinate>();
while (jobs.Count > 0)
{
    var (current, path, steps) = jobs.Dequeue();

    if (current == goal)
    {
        bestPath = path.ToArray();
        break;
    }

    if (!visited.TryGetValue(current, out var bestSteps) || steps < bestSteps)
    {
        visited[current] = steps;

        foreach (var candidate in current.Neighbours().Where(map.Valid))
            jobs.Enqueue(
                element: (pos: candidate, path: new List<Coordinate>(path) { candidate }, steps: steps + 1),
                priority: steps + 1 + candidate.Distance(goal));
    }
}

Console.WriteLine($"Part 1 - # of 2ps cheats: {CountCheats(bestPath, 2)}");
Console.WriteLine($"Part 2 - # of 20ps cheats: {CountCheats(bestPath, 20)}");
Console.ReadLine();

int CountCheats(Coordinate[] path, int cheatMaxDuration)
{
    var minStepsSaved = 100;
    return path[..(path.Length - minStepsSaved)]
        .Select((source, sourceIdx) => (source, sourceIdx))
        .Sum(x => path[(x.sourceIdx + 1 + minStepsSaved)..]
            .Count(dest =>
                x.source.Distance(dest) <= cheatMaxDuration &&
                Array.IndexOf(path, dest) - x.sourceIdx - x.source.Distance(dest) >= minStepsSaved));
}

record Coordinate(int X, int Y)
{
    public IEnumerable<Coordinate> Neighbours()
    {
        yield return new Coordinate(X - 1, Y);
        yield return new Coordinate(X + 1, Y);
        yield return new Coordinate(X, Y - 1);
        yield return new Coordinate(X, Y + 1);
    }

    public int Distance(Coordinate goal) => Math.Abs(X - goal.X) + Math.Abs(Y - goal.Y);
}

internal static class Extensions
{
    public static Coordinate Find(this char[][] map, char target)
    {
        for (var y = 0; y < map.Length; y++)
            for (var x = 0; x < map[y].Length; x++)
                if (map[y][x] == target)
                    return new Coordinate(x, y);

        return new Coordinate(-1, -1);
    }

    public static bool Valid(this char[][] map, Coordinate pos)
    {
        return pos.Y >= 0 && pos.Y < map.Length && pos.X >= 0 && pos.X < map[pos.Y].Length && map[pos.Y][pos.X] != '#';
    }
}