using System.Text.RegularExpressions;

var lines = File.ReadAllLines(@"..\..\..\..\day-13.txt");
var coordRegex = new Regex(@"X\+(\d+), Y\+(\d+)");
var prizeRegex = new Regex(@"X=(\d+), Y=(\d+)");

var machines = new List<(Coordinate buttonA, Coordinate buttonB, Coordinate prize)>();

int lineIndex = 0;
while (lineIndex < lines.Length)
{
    var buttonAText = coordRegex.Match(lines[lineIndex]);
    var buttonBText = coordRegex.Match(lines[lineIndex + 1]);
    var prizeText = prizeRegex.Match(lines[lineIndex + 2]);
    machines.Add(
        (new Coordinate(long.Parse(buttonAText.Groups[1].Value), long.Parse(buttonAText.Groups[2].Value)), 
        new Coordinate(long.Parse(buttonBText.Groups[1].Value), long.Parse(buttonBText.Groups[2].Value)), 
        new Coordinate(long.Parse(prizeText.Groups[1].Value), long.Parse(prizeText.Groups[2].Value))));
    lineIndex += 4;
}

Console.WriteLine($"Part 1 - Tokens spent: {Calculate(machines, false)}");
Console.WriteLine($"Part 2 - Tokens spent: {Calculate(machines, true)}");
Console.ReadLine();

decimal Calculate(List<(Coordinate buttonA, Coordinate buttonB, Coordinate prize)> machines, bool part2)
{
    var offset = part2 ? 10000000000000m : 0;

    return machines.Sum(m =>
    {
        var line1 = new Line(0 - (decimal)m.buttonA.X / m.buttonB.X, (m.prize.X + offset) / m.buttonB.X);
        var line2 = new Line(0 - (decimal)m.buttonA.Y / m.buttonB.Y, (m.prize.Y + offset) / m.buttonB.Y);
        var intersection = line1.Intersection(line2);
        return intersection != default && (part2 || intersection.InBounds()) 
            ? intersection.X * 3 + intersection.Y 
            : 0;
    });
}

public record Coordinate(long X, long Y)
{
    public bool InBounds() => X >= 0 && Y >= 0 && X <= 100 && Y <= 100;
}

public record Line(decimal Slope, decimal Intercept);

public static class Extensions
{
    public static Coordinate Intersection(this Line line1, Line line2)
    {
        if (line1.Slope == line2.Slope)
            return default;

        var x = (line2.Intercept - line1.Intercept) / (line1.Slope - line2.Slope);
        var y = line1.Slope * x + line1.Intercept;

        //Must be an integer - discard rounding errors
        if (Math.Abs(x - Math.Round(x)) >= 0.0000001m || Math.Abs(y - Math.Round(y)) >= 0.0000001m)
            return default;

        return new Coordinate((long)Math.Round(x), (long)Math.Round(y));
    }
}