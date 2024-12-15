public static class Part2
{
    public static int CalculateGpsSum(List<string> mapLines, List<Direction> movements)
    {
        var map = mapLines.Select(row => row.SelectMany(x => x switch
        {
            '#' => new[] { '#', '#' },
            'O' => new[] { '[', ']' },
            '.' => new[] { '.', '.' },
            '@' => new[] { '@', '.' },
            _ => throw new InvalidOperationException()
        }).ToArray())
        .ToArray();

        var robot = map.FindRobot();
        foreach (var direction in movements)
        {
            if (TryMove([robot], direction, map))
                robot = robot.Move(direction);
        }

        return map
            .Select((row, y) => row
                .Select((item, x) => item == '[' ? y * 100 + x : 0)
                .Sum())
            .Sum();
    }

    private static bool TryMove(Coordinate[] sources, Direction direction, char[][] map)
    {
        var destinations = sources.Select(s => s.Move(direction));
        var blockedDestinations = destinations.Where(destination => !Vacant(map[destination.Y][destination.X], direction));
        
        if (blockedDestinations.Any(destination => map[destination.Y][destination.X] == '#'))
            return false;

        var toMove = blockedDestinations
            .SelectMany(d => d.ToBoxCoordinates(map))
            .Distinct()
            .ToArray();

        if (!blockedDestinations.Any() || TryMove(toMove, direction, map))
        {
            //Success - start moving from the end of the recursion
            foreach (var destination in direction == Direction.West ? destinations.OrderBy(d => d.X) : destinations.OrderByDescending(d => d.X))
            {
                var source = destination.Move(direction.Inverse());
                map[destination.Y][destination.X] = map[source.Y][source.X];
                map[source.Y][source.X] = '.';
            }
            return true;
        }
        else
            return false;
    }

    private static bool Vacant(char content, Direction direction)
    {
        return 
            content == '.' ||
            content == ']' && direction == Direction.East ||
            content == '[' && direction == Direction.West;
    }
}