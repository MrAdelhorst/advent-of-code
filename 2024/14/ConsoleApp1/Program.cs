using System.Text;
using System.Text.RegularExpressions;

var regex = new Regex(@"p=(\d+),(\d+) v=(\-?\d+),(\-?\d+)");
var robots = File.ReadAllLines(@"..\..\..\..\day-14.txt")
    .Select(line =>
    {
        var matches = regex.Match(line);
        return new Robot
        (
            new Coordinate(int.Parse(matches.Groups[1].Value), int.Parse(matches.Groups[2].Value)),
            new Coordinate(int.Parse(matches.Groups[3].Value), int.Parse(matches.Groups[4].Value))
        );
    }).ToList();

var sizeX = 101;
var sizeY = 103;

var safetyFactor = 0;
var iterations = 0;
while (true)
{
    foreach (var robot in robots)
    {
        var x = (robot.Position.X + robot.Velocity.X + 1) % sizeX - 1;
        var y = (robot.Position.Y + robot.Velocity.Y + 1) % sizeY - 1;
        robot.SetPosition(new Coordinate(x < 0 ? sizeX + x : x, y < 0 ? sizeY + y : y));
    }
    iterations++;

    //Part 1
    if (iterations == 100)
        safetyFactor = CalculateSafetyFactor();

    //Part 2
    if (robots.IsTree(sizeX, sizeY))
        break;
}

//Show the tree
robots.Print(sizeX, sizeY);
Console.WriteLine($"Iteration: {iterations}");
Console.WriteLine();
Console.WriteLine($"Part 1 - Safety factor: {safetyFactor}");
Console.WriteLine($"Part 2 - Xmas tree interation: {iterations}");
Console.ReadLine();

int CalculateSafetyFactor()
{
    return
        robots.Count(robot => robot.Position.X < sizeX / 2 && robot.Position.Y < sizeY / 2) *
        robots.Count(robot => robot.Position.X > sizeX / 2 && robot.Position.Y < sizeY / 2) *
        robots.Count(robot => robot.Position.X < sizeX / 2 && robot.Position.Y > sizeY / 2) *
        robots.Count(robot => robot.Position.X > sizeX / 2 && robot.Position.Y > sizeY / 2);
}

record Coordinate(int X, int Y);

record Robot(Coordinate position, Coordinate velocity)
{
    public Coordinate Position { get; private set; } = position;
    public Coordinate Velocity { get; } = velocity;
    public void SetPosition(Coordinate position) => Position = position;
}

static class Extensions
{
    public static void Print(this List<Robot> robots, int sizeX, int sizeY)
    {
        for (int y = 0; y <= sizeY; y++)
        {
            for (int x = 0; x <= sizeX; x++)
            {
                Console.Write(robots.Count(robot => robot.Position.X == x && robot.Position.Y == y) > 0 ? '#' : '.');
            }
            Console.WriteLine();
        }
    }

    public static bool IsTree(this List<Robot> robots, int sizeX, int sizeY)
    {
        //Simply search for 20 adjacent robots in a row
        var pattern = "####################";
        for (int y = 0; y <= sizeY; y++)
        {
            var line = robots
                .Where(robot => robot.Position.Y == y)
                .OrderBy(robot => robot.Position.X)
                .ToArray();

            if (line.Length >= pattern.Length)
            {
                var sb = new StringBuilder();
                for (int i = line[0].Position.X; i < line[^1].Position.X; i++)
                {
                    sb.Append(line.Any(robot => robot.Position.X == i) ? '#' : '.');
                }
                var str = sb.ToString();
                if (str.IndexOf(pattern) != -1)
                {
                    return true;
                }
            }
        }

        return false;
    }
}