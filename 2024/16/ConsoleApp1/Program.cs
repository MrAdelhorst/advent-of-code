var map = File.ReadAllLines(@"..\..\..\..\day-16.txt")
    .Select(row => row.ToArray()).ToArray();

var start = new Coordinate(1, map.Length - 2);
var goal = new Coordinate(map[1].Length - 2, 1);
var direction = Direction.East;

var visited = new Dictionary<(Coordinate pos, Direction direction), int>();
var jobs = new PriorityQueue<(Coordinate pos, Direction direction, Coordinate[] steps, int points), int>();

jobs.Enqueue((start, direction, [start], 0), 0);
var paths = new List<Coordinate[]>();
var score = int.MaxValue;
while (jobs.Count > 0)
{
    var current = jobs.Dequeue();
    if (current.points > score)
    {
        break;
    }

    if (current.pos == goal)
    {
        score = current.points;
        paths.Add(current.steps);
    }

    if (!visited.TryGetValue((current.pos, current.direction), out var points) || current.points <= points)
    {
        visited[(current.pos, current.direction)] = current.points;
        foreach (var candidate in (current.pos, current.direction, current.points).Candidates(map))
        {
            jobs.Enqueue((candidate.pos, candidate.direction, current.steps.Union([candidate.pos]).ToArray(), candidate.points), candidate.Distance(goal));
        }
    }
}

Console.WriteLine($"Part 1 - Best score: {score}");
Console.WriteLine($"Part 2 - Best route tiles: {paths.SelectMany(p => p).Distinct().Count()}");
Console.ReadLine();

record Coordinate(int X, int Y)
{
    public Coordinate Move(Direction direction) => direction switch
    {
        Direction.North => new(X, Y - 1),
        Direction.East => new(X + 1, Y),
        Direction.South => new(X, Y + 1),
        Direction.West => new(X - 1, Y)
    };
}

enum Direction
{
    North,
    East,
    South,
    West
}

internal static class Extensions
{
    public static IEnumerable<(Coordinate pos, Direction direction, int points)> Candidates(this (Coordinate pos, Direction direction, int points) current, char[][] map)
    {
        var next = current.pos.Move(current.direction);
        var candidates = new List<(Coordinate pos, Direction direction, int points)>
        {
            (current.pos, current.direction.TurnRight(), current.points + 1000),
            (current.pos, current.direction.TurnLeft(), current.points + 1000)
        };
        if (next.Valid(map))
        {
            candidates.Add((next, current.direction, current.points + 1));
        }

        return candidates;
    }

    public static bool Valid(this Coordinate pos, char[][] map) => pos.X >= 0 && pos.X < map[0].Length && pos.Y >= 0 && pos.Y < map.Length && map[pos.Y][pos.X] != '#';

    public static int Distance(this (Coordinate pos, Direction direction, int points) current, Coordinate goal)
    {
        var (pos, _, points) = current;
        return Math.Abs(pos.X - goal.X) + Math.Abs(pos.Y - goal.Y) + points;
    }

    public static Direction TurnLeft(this Direction direction) => direction switch
    {
        Direction.North => Direction.West,
        Direction.East => Direction.North,
        Direction.South => Direction.East,
        Direction.West => Direction.South,
    };

    public static Direction TurnRight(this Direction direction) => direction switch
    {
        Direction.North => Direction.East,
        Direction.East => Direction.South,
        Direction.South => Direction.West,
        Direction.West => Direction.North,
    };
}