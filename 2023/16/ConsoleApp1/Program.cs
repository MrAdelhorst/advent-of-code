var grid = File.ReadAllLines(@"..\..\..\..\day-16.txt").Select(line => line.ToArray()).ToArray();

Console.WriteLine($"Part 1 - energized squares: {CalculateEnergy((new Point(0, 0), Direction.East))}");
Console.WriteLine($"Part 2 - highest energy: {grid.AllEntryPoints().Max(CalculateEnergy)}");
Console.ReadLine();

int CalculateEnergy((Point position, Direction direction) start)
{
    List<Direction>[,] energy = new List<Direction>[grid.First().Length, grid.Length];
    var beams = new Queue<(Point position, Direction direction)>([start]);
    while (beams.Any())
    {
        var current = beams.Dequeue();
        while (current != default && grid.InBounds(current.position))
        {
            energy.Add(current.position, current.direction);
            var resultingBeams = grid.Move(current.position, current.direction)
                        .Where(b => grid.InBounds(b.newPosition) && energy.IsNew(b));
            if (resultingBeams.Any())
            {
                current = resultingBeams.First();
                foreach (var beam in resultingBeams.Skip(1))
                    beams.Enqueue(beam);
            }
            else
                current = default;
        }
    }
    return energy.Cast<List<Direction>>().Count(c => (c?.Count ?? 0) > 0);
}

enum Direction { North, East, South, West };
record Point(int X, int Y);

static class Extensions
{
    public static (Point newPosition, Direction newDirection)[] Move(this char[][] grid, Point position, Direction direction)
    {
        var field = grid[position.Y][position.X];
        return (direction, field) switch
        {
            { direction: Direction.East, field: '/' } => new[] { position.North() },
            { direction: Direction.East, field: '\\' } => new[] { position.South() },
            { direction: Direction.East, field: '|' } => new[] { position.North(), position.South() },
            { direction: Direction.East, field: '.' } => new[] { position.East() },
            { direction: Direction.East, field: '-' } => new[] { position.East() },
            { direction: Direction.West, field: '/' } => new[] { position.South() },
            { direction: Direction.West, field: '\\' } => new[] { position.North() },
            { direction: Direction.West, field: '|' } => new[] { position.North(), position.South() },
            { direction: Direction.West, field: '.' } => new[] { position.West() },
            { direction: Direction.West, field: '-' } => new[] { position.West() },
            { direction: Direction.North, field: '/' } => new[] { position.East() },
            { direction: Direction.North, field: '\\' } => new[] { position.West() },
            { direction: Direction.North, field: '|' } => new[] { position.North() },
            { direction: Direction.North, field: '.' } => new[] { position.North() },
            { direction: Direction.North, field: '-' } => new[] { position.West(), position.East() },
            { direction: Direction.South, field: '/' } => new[] { position.West() },
            { direction: Direction.South, field: '\\' } => new[] { position.East() },
            { direction: Direction.South, field: '|' } => new[] { position.South() },
            { direction: Direction.South, field: '.' } => new[] { position.South() },
            { direction: Direction.South, field: '-' } => new[] { position.West(), position.East() },
        };
    }

    public static (Point newPosition, Direction newDirection) South(this Point position) => (new Point(position.X, position.Y + 1), Direction.South);
    public static (Point newPosition, Direction newDirection) North(this Point position) => (new Point(position.X, position.Y - 1), Direction.North);
    public static (Point newPosition, Direction newDirection) East(this Point position) => (new Point(position.X + 1, position.Y), Direction.East);
    public static (Point newPosition, Direction newDirection) West(this Point position) => (new Point(position.X - 1, position.Y), Direction.West);

    public static bool InBounds(this char[][] grid, Point position)
    {
        return position.X >= 0 && position.X < grid.First().Length && position.Y >= 0 && position.Y < grid.Length;
    }

    public static void Add (this List<Direction>[,] energy, Point position, Direction direction)
    {
        if (energy[position.X, position.Y] == null)
            energy[position.X, position.Y] = new List<Direction>();
        energy[position.X, position.Y].Add(direction);
    }

    public static bool IsNew(this List<Direction>[,] energy, (Point position, Direction direction) beam)
    {
        var history = energy[beam.position.X, beam.position.Y];
        return !history?.Contains(beam.direction) ?? true;
    }

    public static IEnumerable<(Point position, Direction direction)> AllEntryPoints(this char[][] grid)
    {
        var allEntryPoints = new List<(Point position, Direction direction)>();

        var yCoordinates = Enumerable.Range(0, grid.Length);
        var xCoordinates = Enumerable.Range(0, grid.First().Length);

        allEntryPoints.AddRange(yCoordinates.Select(y => (new Point(0, y), Direction.East)));
        allEntryPoints.AddRange(yCoordinates.Select(y => (new Point(xCoordinates.Last(), y), Direction.West)));
        allEntryPoints.AddRange(xCoordinates.Select(x => (new Point(x, 0), Direction.South)));
        allEntryPoints.AddRange(xCoordinates.Select(x => (new Point(x, yCoordinates.Last()), Direction.North)));

        return allEntryPoints;
    }
}