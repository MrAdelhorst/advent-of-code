var hailList = File.ReadAllLines(@"..\..\..\..\day-24.txt").Select(line =>
{
    var coords = line.Split(" @ ").Select(part => new Coordinate(part.Split(", ").Select(long.Parse).ToArray()));
    return (position: coords.First(), speed: coords.Last());
}).ToArray();

//Part 1
((long low, long high) x, (long low, long high) y) testArea = ((200000000000000, 400000000000000), (200000000000000, 400000000000000));
var lines = hailList.Select(hail => hail.ToLinearFunction2D()).ToArray();
var intersections = 0;
for (int i=0; i< lines.Count() - 1; i++)
{
    for (int j = i + 1; j < lines.Count(); j++)
    {
        var point = lines[i].IntersectPoint(lines[j]);
        if (point != default && hailList[i].IsFuture(point) && hailList[j].IsFuture(point) &&
            point.x >= testArea.x.low && point.x <= testArea.x.high && point.y >= testArea.y.low && point.y <= testArea.y.high)
            intersections++;
    }
}

//Part 2: 3 hail are enough to fix the trajectory of the rock
var coordinates = hailList.Take(3).ToArray().CalculateSharedIntersection();

Console.WriteLine($"Part 1 - intersecting lines: {intersections}");
Console.WriteLine($"Part 2 - initial position: {coordinates.x + coordinates.y + coordinates.z}");
Console.ReadLine();

record Coordinate
{
    public long X, Y, Z;
    public Coordinate(long[] axes)
    {
        X = axes[0];
        Y = axes[1];
        Z = axes[2];
    }

    public Coordinate Add(long x, long y, long z) => new Coordinate(new[] { X + x, Y + y, Z + z });
}

static class Extensions
{
    //Part 1
    public static (decimal slope, decimal Y) ToLinearFunction2D(this (Coordinate position, Coordinate speed) hail)
    {
        var (position, speed) = hail;
        var slope = speed.Y / (decimal) speed.X;
        var intercept = position.Y - slope * position.X;
        return (slope, intercept);
    }

    public static (decimal x, decimal y) IntersectPoint(this (decimal slope, decimal intercept) line1, (decimal slope, decimal intercept) line2)
    {
        if (line1.slope == line2.slope)
            return default;

        var x = (line2.intercept - line1.intercept) / (line1.slope - line2.slope);
        var y = line1.slope * x + line1.intercept;
        return (Math.Round(x), Math.Round(y));
    }

    public static bool IsFuture(this (Coordinate position, Coordinate speed) hail, (decimal x, decimal y) point)
    {
        var (position, speed) = hail;
        var xIsFuture = position.X == point.x || (position.X < point.x && speed.X > 0) || (position.X > point.x && speed.X < 0);
        var yIsFuture = position.Y == point.y || (position.Y < point.y && speed.Y > 0) || (position.Y > point.y && speed.Y < 0);
        return xIsFuture && yIsFuture;
    }

    //Part 2
    public static (decimal x, decimal y, decimal z) CalculateSharedIntersection(this (Coordinate position, Coordinate speed)[] hail)
    {
        //Instead of trying to calculate the intersection of multiple moving objects, instead freeze one object, and add it's speed to the other objects
        //Try this for different resonable vectors (-500 to 500 for each of x, y and z)
        for (long x = -500; x < 500; x++)
            for (long y = -500; y < 500; y++)
            {
                var adjusted = hail.Select(hail => (hail.position, speed: hail.speed.Add(x, y, 0)));
                if (adjusted.All(v => v.speed.X != 0 && v.speed.Y != 0))
                {
                    var lines = adjusted.Select(x => x.ToLinearFunction2D()).ToArray();
                    var intersection = lines[0].IntersectPoint(lines[1]);
                    var intersection2 = lines[0].IntersectPoint(lines[2]);
                    if (intersection != default && intersection == intersection2)
                    {
                        //Instead of trying to figure out how to do 3D intersection, just brute force the z axis
                        var time = (intersection.x - hail[1].position.X) / (hail[1].speed.X + x);
                        var time2 = (intersection2.x - hail[2].position.X) / (hail[2].speed.X + x);
                        for (long z = -500; z < 500; z++)
                        {
                            var z1 = hail[1].position.Z + time * (hail[1].speed.Z + z);
                            var z2 = hail[2].position.Z + time2 * (hail[2].speed.Z + z);
                            if (z1 == z2)
                                return (intersection.x, intersection.y, z1);
                        }
                    }
                }
            }

        return default;
    }
}
