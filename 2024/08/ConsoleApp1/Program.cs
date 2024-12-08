var lines = File.ReadAllLines(@"..\..\..\..\day-08.txt");
var bounds = new Coordinate(lines[0].Length, lines.Length);
var frequencies = lines
    .SelectMany((row, y) => row
        .Select((square, x) => (coord: new Coordinate(x, y), frequency: square)))
    .Where(a => a.frequency != '.')
    .GroupBy(a => a.frequency)
    .Select(a => a.Select(b => b.coord)
        .ToArray())
    .ToList();

Console.WriteLine($"Part 1 - Unique AntiNodes: {CountAntiNodes(false)}");
Console.WriteLine($"Part 2 - Unique AntiNodes: {CountAntiNodes(true)}");
Console.ReadLine();

int CountAntiNodes(bool extendedRules)
{
    return frequencies
        .SelectMany(antennas => antennas
            .SelectMany((antenna, i) => antennas.Skip(i + 1)
                .SelectMany(otherAntenna => CalculateAntiNodes(antenna, otherAntenna, extendedRules))))
        .Distinct()
        .Count();
}

IEnumerable<Coordinate> CalculateAntiNodes(Coordinate antenna1, Coordinate antenna2, bool extendedRules)
{
    var result = new List<Coordinate>(extendedRules ? [antenna1, antenna2] : []);
    
    var distance = antenna1.Minus(antenna2);
    int factor = 1;
    int nodes;
    do
    {
        nodes = result.Count;
        var delta = distance.Multiply(factor);
        Coordinate[] antiNodes = [antenna1.Plus(delta), antenna2.Minus(delta)];
        result.AddRange(antiNodes.Where(a => a.InBounds(bounds)));
        factor++;
    } while (nodes != result.Count && extendedRules);

    return result;
}

record Coordinate(int X, int Y)
{
    public Coordinate Minus(Coordinate other) => new Coordinate(X - other.X, Y - other.Y);
    public Coordinate Plus(Coordinate other) => new Coordinate(X + other.X, Y + other.Y);
    public Coordinate Multiply(int factor) => new Coordinate(X * factor, Y * factor);
    public bool InBounds(Coordinate bounds) => X >= 0 && X < bounds.X && Y >= 0 && Y < bounds.Y;
}