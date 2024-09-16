var commands = File.ReadAllText(@"..\..\..\..\day-01.txt").Split(", ").Select(command => (dir: command[0], distance: int.Parse(command[1..]))).ToArray();

var start = new Point(0, 0);
var currentPosition = start;
var currentDirection = Direction.North;
var visited = new List<(Point from, Point to)>();
Point bunnyHq = default;
foreach (var command in commands)
{
    currentDirection = command.dir switch
    {
        'L' => currentDirection.TurnLeft(),
        'R' => currentDirection.TurnRight(),
        _ => throw new InvalidDataException($"Invalid direction in input: {command.dir}")
    };

    var newPosition = currentDirection switch
    {
        Direction.North => new Point(currentPosition.X, currentPosition.Y + command.distance),
        Direction.South => new Point(currentPosition.X, currentPosition.Y - command.distance),
        Direction.East => new Point(currentPosition.X + command.distance, currentPosition.Y),
        Direction.West => new Point(currentPosition.X - command.distance, currentPosition.Y),
    };

    if (bunnyHq == default)
    {
        bunnyHq = visited.FirstOverlapOrDefault((currentPosition, newPosition));
    }

    visited.Add((currentPosition, newPosition));

    currentPosition = newPosition;
}

Console.WriteLine($"Part 1 - distance: {Math.Abs(currentPosition.X) + Math.Abs(currentPosition.Y)}");
Console.WriteLine($"Part 2 - distance: {Math.Abs(bunnyHq.X) + Math.Abs(bunnyHq.Y)}");
Console.ReadLine();

enum Direction { North, East, South, West }
record Point(int X, int Y);

static class Extensions
{
    public static Direction TurnLeft(this Direction direction)
    {
        return direction switch
        {
            Direction.North => Direction.West,
            Direction.East => Direction.North,
            Direction.South => Direction.East,
            Direction.West => Direction.South
        };
    }

    public static Direction TurnRight(this Direction direction)
    {
        return direction switch
        {
            Direction.North => Direction.East,
            Direction.East => Direction.South,
            Direction.South => Direction.West,
            Direction.West => Direction.North
        };
    }

    public static bool Contains(this List<(Point from, Point to)> visited, int x, int y)
    {
        return visited.Any(v => Math.Min(v.from.X, v.to.X) <= x &&
                                Math.Max(v.from.X, v.to.X) >= x &&
                                Math.Min(v.from.Y, v.to.Y) <= y &&
                                Math.Max(v.from.Y, v.to.Y) >= y);
    }

    public static Point FirstOverlapOrDefault(this List<(Point from, Point to)> visited, (Point from, Point to) range)
    {
        if (range.from.X == range.to.X)
        {
            var delta = range.from.Y > range.to.Y ? -1 : 1;
            for (int i = range.from.Y + delta; i != range.to.Y + delta; i += delta)
            {
                if (visited.Contains(range.from.X, i))
                    return new Point(range.from.X, i);
            }
        }
        else
        {
            var delta = range.from.X > range.to.X ? -1 : 1;
            for (int i = range.from.X + delta; i != range.to.X + delta; i += delta)
            {
                if (visited.Contains(i, range.from.Y))
                    return new Point(i, range.from.Y);
            }
        }

        return default;
    }
}