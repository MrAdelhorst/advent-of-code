var grid = File.ReadAllLines(@"..\..\..\..\day-17.txt").Select(line => line.Select(v => v - '0').ToArray()).ToArray();

Console.WriteLine($"Part 1 - Lowest heatloss: {RunSimulation(grid, Extensions.ValidMovesPart1)}");
Console.WriteLine($"Part 2 - Lowest heatloss: {RunSimulation(grid, Extensions.ValidMovesPart2)}");
Console.ReadLine();

int RunSimulation(int[][] grid, Func<int[][], (Point position, Direction direction, int streak), (Point position, Direction direction, int streak)[]>  validMovesFunction)
{
    var goal = grid.EndPosition();
    var queue = new PriorityQueue<(Point position, Direction direction, int streak), int>([((new Point(0, 0), Direction.None, 0), 0)]);
    var cost_so_far = new Dictionary<(Point, Direction, int), int> { [(new Point(0, 0), Direction.None, 0)] = 0 };

    while (queue.Count > 0)
    {
        var current = queue.Dequeue();
        if (current.position == goal && current.streak >= 4)
            break;

        foreach (var next in validMovesFunction(grid, current))
        {
            var new_cost = cost_so_far[current] + grid[next.position.Y][next.position.X];
            if (!cost_so_far.ContainsKey(next) || cost_so_far[next] > new_cost)
            {
                cost_so_far[next] = new_cost;
                var priority = new_cost + next.position.Distance(goal);
                queue.Enqueue(next, priority);
            }
        }
    }
    return cost_so_far.Where(c => c.Key.Item1 == goal).Min(c => c.Value);
}

enum Direction { None, East, South, West, North }

record Point(int X, int Y);

static class Extensions
{
    public static Point Move(this Point current, Direction direction) => direction switch
    {
        Direction.East => new Point(current.X + 1, current.Y),
        Direction.South => new Point(current.X, current.Y + 1),
        Direction.West => new Point(current.X - 1, current.Y),
        Direction.North => new Point(current.X, current.Y - 1),
        _ => throw new ArgumentException(nameof(direction))
    };

    public static Direction[] AllDirections() => new[] { Direction.East, Direction.South, Direction.West, Direction.North };

    public static Direction Opposite(this Direction direction) => direction switch
    {
        Direction.East => Direction.West,
        Direction.South => Direction.North,
        Direction.West => Direction.East,
        Direction.North => Direction.South,
        Direction.None => Direction.None,
        _ => throw new ArgumentException(nameof(direction))
    };

    public static bool InBounds(this int[][] grid, Point point) => point.X >= 0 && point.Y >= 0 && point.X < grid[0].Length && point.Y < grid.Length;

    public static Point EndPosition(this int[][] grid) => new Point(grid[0].Length - 1, grid.Length - 1);

    public static int Distance(this Point first, Point second) => Math.Abs(first.X - second.X) + Math.Abs(first.Y - second.Y);

    public static (Point position, Direction direction, int streak)[] ValidMovesPart1(int[][] grid, (Point position, Direction direction, int streak) current)
    {
        var (position, direction, streak) = current;
        var legalDirections = AllDirections().Where(d => d != direction.Opposite());
        return legalDirections.Select(d => (position: position.Move(d), direction: d, streak: (d == direction) ? streak + 1 : 1)).Where(d => grid.InBounds(d.position) && d.streak <= 3).ToArray();
    }

    public static (Point position, Direction direction, int streak)[] ValidMovesPart2(int[][] grid, (Point position, Direction direction, int streak) current)
    {
        var (position, direction, streak) = current;
        var possibleDirections = AllDirections().Where(d => d != direction.Opposite());
        var legalDirections = (streak < 4 && direction != Direction.None) ? [direction] : (streak < 10) ? possibleDirections : possibleDirections.Where(d => d != direction);

        return legalDirections.Select(d => (position: position.Move(d), direction: d, streak: (d == direction) ? streak + 1 : 1)).Where(d => grid.InBounds(d.position)).ToArray();
    }
}