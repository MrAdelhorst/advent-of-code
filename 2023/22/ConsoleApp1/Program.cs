//Parse input and order bricks by their Y coordinate
var bricks = File.ReadAllLines(@"..\..\..\..\day-22.txt").Select(line =>
{
    var coords = line.Split('~').Select(part => part.Split(',').Select(int.Parse).ToArray()).OrderBy(c => c[2]);
    return (from: new Coordinate(coords.First()[0], coords.First()[1], coords.First()[2]), to: new Coordinate(coords.Last()[0], coords.Last()[1], coords.Last()[2]));
}).OrderBy(coords => coords.from.Z);

//Drop them one by one
var tower = new List<(Coordinate from, Coordinate to)>();
var highestZ = 0;
foreach (var brick in bricks)
{
    var currentZ = brick.from.Z;
    while (currentZ > 1 && !tower.Blocking(brick, currentZ - 1))
        currentZ--;

    var newBrick = brick.ToRelativeHeight(currentZ);
    highestZ = newBrick.to.Z;
    tower.Add(newBrick);
}

//Part 1
var canBeRemoved = 0;
for (int i = highestZ+1; i > 1; i--)
{
    var bricksOnLevel = tower.Where(b => b.from.Z == i);
    var potentialSupportingBricks = tower.Where(b => b.to.Z == i - 1);

    var blockedBricks = new List<(Coordinate from, Coordinate to)>();
    foreach (var brick in bricksOnLevel)
    {
        var supportingBricks = potentialSupportingBricks.Where(b => (new List<(Coordinate, Coordinate)>([b])).Blocking(brick, i - 1));
        if (supportingBricks.Count() == 1)
            blockedBricks.Add(supportingBricks.Single());
    }

    canBeRemoved += potentialSupportingBricks.Where(b => !blockedBricks.Contains(b)).Count();
}

//Part 2
var totalFallingBricks = 0;
for (int i = 1; i <= highestZ; i++)
{
    var bricksOnLevel = tower.Where(b => b.from.Z == i);
    foreach (var brick in bricksOnLevel)
    {
        var towerClone = new List<(Coordinate from, Coordinate to)>(tower);
        towerClone.Remove(brick);
        var currentLevel = i+1;
        var highestAffectedLevel = brick.to.Z + 1;
        while (currentLevel <= highestAffectedLevel)
        {
            var bricksOnSupportedLevel = towerClone.Where(b => b.from.Z == currentLevel);
            var fallingBricks = new List<(Coordinate from, Coordinate to)>();
            foreach (var brickOnSupportedLevel in bricksOnSupportedLevel)
            {
                if (!towerClone.Blocking(brickOnSupportedLevel, currentLevel - 1))
                    fallingBricks.Add(brickOnSupportedLevel);
            }
            if (fallingBricks.Any())
            {
                totalFallingBricks += fallingBricks.Count;
                foreach (var fallingBrick in fallingBricks)
                    towerClone.Remove(fallingBrick);
                highestAffectedLevel = Math.Max(highestAffectedLevel, fallingBricks.Max(b => b.to.Z) + 1);
            }
            currentLevel++;
        }
    }
}


Console.WriteLine($"Part 1 - Safe to remove bricks: {canBeRemoved}");
Console.WriteLine($"Part 2 - Sum of falling bricks: {totalFallingBricks}");
Console.ReadLine();

record Coordinate(int X, int Y, int Z);

static class Extensions
{
    public static bool Blocking(this List<(Coordinate from, Coordinate to)> tower, (Coordinate from, Coordinate to) originalBrick, int desiredZ)
    {
        var brick = originalBrick.ToRelativeHeight(desiredZ);
        var verticals = tower.Where(b => b.IsVertical() && b.to.Z == desiredZ);
        var horizontals = tower.Where(b => !b.IsVertical() && b.from.Z == desiredZ);

        var verticalCollisions = brick.IsVertical()
                                        ? verticals.Any(v => v.from.X == brick.from.X && v.from.Y == brick.from.Y)
                                        : verticals.Any(v => v.from.X >= brick.from.X && v.from.X <= brick.to.X && v.from.Y >= brick.from.Y && v.from.Y <= brick.to.Y);
        var horizontalCollisions = brick.IsVertical()
                                        ? horizontals.Any(h => brick.from.X >= h.from.X && brick.from.X <= h.to.X && brick.from.Y >= h.from.Y && brick.from.Y <= h.to.Y)
                                        : horizontals.Any(h => h.IsXOriented()
                                                                ? brick.IsXOriented() ? h.from.Y == brick.from.Y && h.XOverlap(brick) : h.XYOverlap(brick)
                                                                : brick.IsXOriented() ? brick.XYOverlap(h) : h.from.X == brick.from.X && h.YOverlap(brick));

        return verticalCollisions || horizontalCollisions;
    }

    public static int Height(this (Coordinate from, Coordinate to) brick) => brick.to.Z - brick.from.Z + 1;
    public static (Coordinate from, Coordinate to) ToRelativeHeight(this (Coordinate from, Coordinate to) brick, int newZ) 
        => (from: new Coordinate(brick.from.X, brick.from.Y, newZ), to: new Coordinate(brick.to.X, brick.to.Y, newZ + brick.Height() - 1));
    public static bool IsVertical(this (Coordinate from, Coordinate to) brick) => brick.from.Z != brick.to.Z;
    public static bool IsXOriented(this (Coordinate from, Coordinate to) brick) => brick.from.Y == brick.to.Y;
    public static bool XOverlap(this (Coordinate from, Coordinate to) brick, (Coordinate from, Coordinate to) other) 
        => !(brick.from.X < other.from.X && brick.to.X < other.from.X ||
             brick.from.X > other.to.X && brick.to.X > other.to.X);
    public static bool YOverlap(this (Coordinate from, Coordinate to) brick, (Coordinate from, Coordinate to) other)
        => !(brick.from.Y < other.from.Y && brick.to.Y < other.from.Y ||
             brick.from.Y > other.to.Y && brick.to.Y > other.to.Y);
    public static bool XYOverlap(this (Coordinate from, Coordinate to) xBrick, (Coordinate from, Coordinate to) yBrick)
        => xBrick.from.Y >= yBrick.from.Y && xBrick.from.Y <= yBrick.to.Y && yBrick.from.X >= xBrick.from.X && yBrick.from.X <= xBrick.to.X;
}