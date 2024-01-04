var map = File.ReadAllLines(@"..\..\..\..\day-23.txt").Select(line => line.ToArray()).ToArray();
var start = new Point(1, 0);
var goal = new Point(map[0].Length - 2, map.Length - 1);
var edges = map.GenerateEdges(start, goal);

Console.WriteLine($"Part 1 - Max cost: {RunSimulation((current) => map.LegalDirections(current, false))}");
Console.WriteLine($"Part 2 - Max cost: {RunSimulation(edges.Destinations)}");
Console.ReadLine();

//This takes a couple of minutes for part 2, but it could be easily optimized by parallelization
int RunSimulation(Func<Point, IEnumerable<(Point destination, int cost)>> NextDestination)
{
    var tripId = Extensions.NextTripId();
    var jobs = new Stack<(Point pos, int trip)>(new[] { (start, tripId) });
    var cost_so_far = new Dictionary<int, Dictionary<Point, int>>([new KeyValuePair<int, Dictionary<Point, int>>(tripId, new Dictionary<Point, int>([new KeyValuePair<Point, int>(start, 0)]))]);
    var longest = 0;
    while (jobs.Count > 0)
    {
        var (current, trip) = jobs.Pop();
        if (current == goal)
            longest = Math.Max(longest, cost_so_far[trip][current]);

        var currentCosts = new Dictionary<Point, int>(cost_so_far[trip]);
        foreach (var (next, cost, nextTrip) in NextDestination(current).Where(d => !cost_so_far[trip].ContainsKey(d.destination)).Select((dir, idx) => (dir.destination, dir.cost, (idx == 0) ? trip : Extensions.NextTripId())))
        {
            var new_cost = currentCosts[current] + cost;
            //Clone current trip history if creating a new one
            if (!cost_so_far.ContainsKey(nextTrip))
                cost_so_far[nextTrip] = new Dictionary<Point, int>(currentCosts);
            //If this is position hasn't been travelled yet for this trip, go ahead and enqueue it
            if (!cost_so_far[nextTrip].ContainsKey(next))
            {
                cost_so_far[nextTrip][next] = new_cost;
                jobs.Push((next, nextTrip));
            }
        }
    }
    return longest;
}


record Point(int X, int Y);

static class Extensions
{
    static int nextTripId = 0;
    static public int NextTripId() => nextTripId++;
    public static readonly Point Empty = new Point(-1, -1);
    public static bool allowUphill = false;
    public static IEnumerable<(Point destination, int cost)> Destinations(this (Point[] connection, int cost)[] edges, Point position) =>
        edges.Where(v => v.connection.Any(c => c == position)).Select(v => (v.connection.Single(c => c != position), v.cost));

    public static IEnumerable<(Point destination, int cost)> LegalDirections(this char[][] map, Point position, bool allowUphill) =>
        new[] { (map.NorthIfLegal(position, allowUphill), 1), 
                (map.EastIfLegal(position, allowUphill), 1), 
                (map.SouthIfLegal(position, allowUphill), 1), 
                (map.WestIfLegal(position, allowUphill), 1)}.Where(d => d.Item1 != Empty);

    public static Point NorthIfLegal(this char[][] map, Point position, bool allowUphill)
    {
        var illegalMoves = new[] { '#', allowUphill ? '_' : 'v' };
        return position switch
        {
            var p when p.Y == 0 => Empty,
            var p when illegalMoves.Contains(map[p.Y - 1][p.X]) => Empty,
            _ => new Point(position.X, position.Y - 1)
        };
    }

    public static Point EastIfLegal(this char[][] map, Point position, bool allowUphill)
    {
        var illegalMoves = new[] { '#', allowUphill ? '_' : '<' };
        return position switch
        {
            var p when p.X == map[0].Length - 1 => Empty,
            var p when illegalMoves.Contains(map[p.Y][p.X + 1]) => Empty,
            _ => new Point(position.X + 1, position.Y)
        };
    }

    public static Point SouthIfLegal(this char[][] map, Point position, bool allowUphill)
    {
        var illegalMoves = new[] { '#', allowUphill ? '_' : '^' };
        return position switch
        {
            var p when p.Y == map.Length - 1 => Empty,
            var p when illegalMoves.Contains(map[p.Y + 1][p.X]) => Empty,
            _ => new Point(position.X, position.Y + 1)
        };
    }

    public static Point WestIfLegal(this char[][] map, Point position, bool allowUphill)
    {
        var illegalMoves = new[] { '#', allowUphill ? '_' : '>' };
        return position switch
        {
            var p when p.X == 0 => Empty,
            var p when illegalMoves.Contains(map[p.Y][p.X - 1]) => Empty,
            _ => new Point(position.X - 1, position.Y)
        };
    }

    public static (Point[] connection, int cost)[] GenerateEdges(this char[][] map, Point start, Point end)
    {
        var shortcuts = new List<(Point[] connection, int cost)>();

        var queue = new Queue<(Point origin, Point current, int distance)>([(start, start, 0)]);
        while (queue.Count > 0)
        {
            var startPos = queue.Dequeue();
            var current = startPos.current;
            var previous = startPos.origin;
            var distance = startPos.distance;
            IEnumerable<Point> directions = Enumerable.Empty<Point>();
            while ((directions = map.LegalDirections(current, true).Select(d => d.destination).Except([previous])).Count() == 1)
            {
                previous = current;
                current = directions.Single();
                distance++;
                if (current == end)
                {
                    shortcuts.Add((new[] { startPos.origin, current }, distance));
                    break;
                }
            }

            if (current != end && directions.Any() && !shortcuts.Any(s => s.connection.Contains(current) && s.connection.Contains(startPos.origin)))
            {
                shortcuts.Add((new[] { startPos.origin, current }, distance));

                foreach (var direction in directions.Where(d => d != current))
                {
                    queue.Enqueue((current, direction, 1));
                }
            }
        }

        return shortcuts.ToArray();
    }
}