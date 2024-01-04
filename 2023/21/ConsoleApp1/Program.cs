var input = File.ReadAllLines(@"..\..\..\..\day-21.txt");
Point start = input.FindStart();
var map = input.Select(line => line.Select(c => (c == '#') ? -1 : int.MaxValue).ToArray()).ToArray();

//Part 1
var nextRound = new HashSet<Point>([start]);
var stepsToTravel = 64;
for (int iteration=0; iteration<stepsToTravel; iteration++)
{ 
    var currentRound = nextRound;
    nextRound = new HashSet<Point>();

    foreach (var position in currentRound.Distinct())
        foreach (var newPosition in position.Adjacent().Where(p => map[p.Y][p.X] > 0))
            nextRound.Add(newPosition);
}
var part1totalSteps = nextRound.Distinct().Count();

//Part 2 - the matrix is 131x131, which causes a pattern to emerge per 131 rounds, so travel a multiple of 131 steps (+remainder)
nextRound = new HashSet<Point>([start]);
var totalStepsToTravel = 26501365;
var stepsPerCycle = 131;
var remainder = totalStepsToTravel % stepsPerCycle;
stepsToTravel = stepsPerCycle * 5 + remainder; //Add remainder in order to make the last iteration align with the total steps
var positionsReachedPerRound = new Dictionary<int, int>();
for (int iteration = 0; iteration < stepsToTravel; iteration++)
{
    var currentRound = nextRound;
    nextRound = new HashSet<Point>();

    foreach (var position in currentRound)
    {
        foreach (var nextPosition in position.Adjacent().Where(p =>
        {
            var tmp = p.ToRelativePosition(map);
            return map[tmp.Y][tmp.X] > 0 && !nextRound.Contains(p);
        }))
        {
            nextRound.Add(nextPosition);
        }
    };

    positionsReachedPerRound.Add(iteration + 1, nextRound.Count());
}

//Calculate the total number of steps based on the amount that it grows by
var growthFactor = positionsReachedPerRound.CalculateGrowthFactor(stepsPerCycle);
var positionsThisRound = positionsReachedPerRound[stepsToTravel];
var positionsPreviousCycle = positionsReachedPerRound[stepsToTravel - stepsPerCycle];
long currentDelta = positionsThisRound - positionsPreviousCycle;
long currentPositions = positionsThisRound;
var missingCycles = (totalStepsToTravel - stepsToTravel) / stepsPerCycle;
for (int cycle = 0; cycle < missingCycles; cycle++)
{
    currentDelta += growthFactor;
    currentPositions += currentDelta;
}

Console.WriteLine($"Part 1 - Number of positions reached: {part1totalSteps}");
Console.WriteLine($"Part 2 - Number of positions reached: {currentPositions}");
Console.ReadLine();

public record Point(int X, int Y);

static class Extensions
{
    public static Point FindStart(this string[] map)
    {
        for (int i = 0; i < map.Length; i++)
        {
            var s = map[i].IndexOf('S');
            if (s != -1)
                return new Point(s, i);
        }

        throw new InvalidOperationException("Unable to find start element!");
    }

    public static Point East(this Point point) => new Point(point.X + 1, point.Y);
    public static Point West(this Point point) => new Point(point.X - 1, point.Y);
    public static Point North(this Point point) => new Point(point.X, point.Y - 1);
    public static Point South(this Point point) => new Point(point.X, point.Y + 1);
    public static Point[] Adjacent(this Point point) => new Point[] { point.East(), point.West(), point.North(), point.South() };
    public static Point ToRelativePosition(this Point point, int[][] map)
    {
        var xOffset = point.X % map.First().Length;
        var yOffset = point.Y % map.Length;
        var x = xOffset < 0 ? map.First().Length + xOffset : xOffset;
        var y = yOffset < 0 ? map.Length + yOffset : yOffset;
        return new Point(x, y);
    }

    public static int CalculateGrowthFactor(this Dictionary<int, int> positionsReachedPerRound, int stepsPerCycle)
    {
        var diffsPerCycle = new List<int[]>();
        //Skip the first two cycles to make sure the pattern has emerged
        for (int cycle = 3; cycle <= 5; cycle++)
        {
            var diffs = new int[5];
            //Examine the last 5 elements to make sure we have a pattern
            for (int i = 0; i < 5; i++)
                diffs[i] = positionsReachedPerRound[cycle * stepsPerCycle - i] - positionsReachedPerRound[(cycle - 1) * stepsPerCycle - i];
            diffsPerCycle.Add(diffs);
        }

        //Now examine how much the diff grows by per round - this should be constant
        var diffGains = new List<int>();
        for (int cycle = 1; cycle < diffsPerCycle.Count; cycle++)
            for (int j = 0; j < 5; j++)
                diffGains.Add(diffsPerCycle[cycle][j] - diffsPerCycle[cycle - 1][j]);

        //Make sure the growth factor is constant
        if (diffGains.Distinct().Count() != 1)
            throw new InvalidOperationException("Error: the growth factor isn't constant...");

        return diffGains.First();
    }
}