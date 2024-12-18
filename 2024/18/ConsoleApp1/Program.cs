var bytes = File.ReadAllLines(@"..\..\..\..\day-18.txt")
    .Select(line => line.Split(','))
    .Select(line => new Coordinate(int.Parse(line[0]), int.Parse(line[1])))
    .ToArray();

var map = Enumerable.Range(0, 71)
    .Select(x =>
    {
        var tmp = new char[71];
        Array.Fill(tmp, '.');
        return tmp;
    }).ToArray();

var goal = new Coordinate(map[0].Length - 1, map.Length - 1);

var dropped = 0;
var part1 = 0;
var part2 = new Coordinate(-1, -1);
foreach (var b in bytes)
{
    map[b.Y][b.X] = '#';
    dropped++;

    if (dropped >= 1024)
    {
        var steps = FindPath(map, new Coordinate(0, 0), goal);
        if (dropped == 1024)
            part1 = steps;

        if (steps == -1)
        {
            part2 = b;
            break;
        }
    }
}

Console.WriteLine($"Part 1 - Steps: {part1}");
Console.WriteLine($"Part 2 - Blocking coordinate: {part2.X},{part2.Y}");
Console.ReadLine();

int FindPath(char[][] map, Coordinate start, Coordinate goal)
{
    var visited = new Dictionary<Coordinate, int>();
    var jobs = new PriorityQueue<(Coordinate coord, int steps), int>([((new Coordinate(0, 0), 0), 0)]);
    var res = 0;
    while (jobs.Count > 0)
    {
        var current = jobs.Dequeue();
        if (!visited.TryGetValue(current.coord, out var steps) || current.steps < steps)
        {
            if (current.coord == goal)
            {
                return current.steps;
            }

            visited[current.coord] = current.steps;
            foreach (var candidate in current.coord.Neighbours().Where(x => x.Valid(map)))
            {
                jobs.Enqueue((candidate, current.steps + 1), current.steps + candidate.Distance(map, goal));
            }
        }
    }
    return -1;
}

record Coordinate(int X, int Y)
{
    internal IEnumerable<Coordinate> Neighbours()
    {
        yield return new Coordinate(X + 1, Y);
        yield return new Coordinate(X - 1, Y);
        yield return new Coordinate(X, Y + 1);
        yield return new Coordinate(X, Y - 1);
    }

    internal int Distance(char[][] map, Coordinate goal)
    {
        return Math.Abs(X - goal.X) + Math.Abs(Y - goal.Y);
    }

    internal bool Valid(char[][] map)
    {
        return X >= 0 && Y >= 0 && X < map[0].Length && Y < map.Length && map[Y][X] != '#';
    }
}