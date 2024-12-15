public static class Part1
{
    public static int CalculateGpsSum(List<string> mapLines, List<Direction> movements)
    {
        var map = mapLines.Select(row => row.ToArray()).ToArray();
        var robot = map.FindRobot();

        foreach (var direction in movements)
        {
            if (TryMove(robot, direction, map))
                robot = robot.Move(direction);
        }

        return map
            .Select((row, y) => row
                .Select((item, x) => item == 'O' ? y * 100 + x : 0)
                .Sum())
            .Sum();
    }
     
    private static bool TryMove(Coordinate source, Direction direction, char[][] map)
    {
        var destination = source.Move(direction);
        if (map[destination.Y][destination.X] == '.' || (map[destination.Y][destination.X] == 'O' && TryMove(destination, direction, map)))
        {
            map[destination.Y][destination.X] = map[source.Y][source.X];
            map[source.Y][source.X] = '.';

            return true;
        }

        return false;
    }
}
