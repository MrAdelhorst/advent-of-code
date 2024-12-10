var map = File.ReadAllLines(@"..\..\..\..\day-10.txt")
    .Select(line => line
        .Select(x => int.Parse([x]))
        .ToArray())
    .ToArray();

var scorePart1 = 0;
var scorePart2 = 0;
for (int y = 0; y < map.Length; y++)
{
    for (int x = 0; x < map[y].Length; x++)
    {
        if (map[y][x] == 0)
        {
            var scores = CalculateTrailheadScores(map, new Coordinate(x, y));
            scorePart1 += scores.distinctTops;
            scorePart2 += scores.distinctRoutes;
        }
    }
}

Console.WriteLine($"Part 1 - Score: {scorePart1}");
Console.WriteLine($"Part 2 - Score: {scorePart2}");
Console.ReadLine();

(int distinctTops, int distinctRoutes) CalculateTrailheadScores(int[][] map, Coordinate start)
{
    var jobs = new Queue<(Coordinate coord, int height)>();
    jobs.Enqueue((start, map[start.Y][start.X]));

    var goals = new List<Coordinate>();
    var goalsReached = 0;
    while (jobs.Count > 0)
    {
        var (coord, height) = jobs.Dequeue();
        if (height == 9)
        {
            goals.Add(coord);
            goalsReached++;
        }
        else
            foreach (var neighbor in coord.GetNeighbors()
                .Where(x => x.IsValid(map, height + 1)))
                    jobs.Enqueue((neighbor, height + 1));
    }
    
    return (goals.Distinct().Count(), goalsReached);
}

record Coordinate(int X, int Y)
{
    public bool IsValid(int[][] map, int neededHeight) =>
        X >= 0 && Y >= 0 && Y < map.Length && X < map[Y].Length && map[Y][X] == neededHeight;

    public IEnumerable<Coordinate> GetNeighbors()
    {
        yield return new Coordinate(X - 1, Y);
        yield return new Coordinate(X + 1, Y);
        yield return new Coordinate(X, Y - 1);
        yield return new Coordinate(X, Y + 1);
    }
}