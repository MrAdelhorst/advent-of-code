using Range = (long from, long to);

var file = await File.ReadAllLinesAsync(@"..\..\..\..\day-05.txt");
var dividerIdx = file.IndexOf(file.Single(string.IsNullOrWhiteSpace));
var ranges = file[..dividerIdx]
    .Select(line => line.Split('-'))
    .Select(parts => (from: long.Parse(parts[0]), to: long.Parse(parts[1])));
var ingredients = file[(dividerIdx + 1)..]
    .Select(long.Parse);

var freshIngredients = ingredients.Where(ingredient => ranges.Any(range => ingredient >= range.from && ingredient <= range.to));
var combinedRanges = ranges.Aggregate(new List<Range>(), (acc, range) => acc.AddRange(range));

Console.WriteLine($"Part 1 - Fresh ingredients: {freshIngredients.Count()}");
Console.WriteLine($"Part 2 - Fresh ingredients: {combinedRanges.Aggregate(0L, (acc, range) => acc + range.to - range.from + 1)}");
Console.ReadLine();

internal static class Extensions
{
    internal static List<Range> AddRange(this List<Range> ranges, Range newRange)
    {
        var intersections = ranges.Where(range => range.Intersect(newRange));
        var combined = ranges.Except(intersections).ToList();
        combined.Add(intersections.Union([newRange]).Merge());
        return combined;
    }

    internal static bool Intersect(this Range range1, Range range2) =>
        range1.from.Within(range2) || range1.to.Within(range2) || range2.from.Within(range1) || range2.to.Within(range1);

    internal static bool Within(this long value, Range range) => 
        value >= range.from && value <= range.to;

    internal static Range Merge(this IEnumerable<Range> source) =>
        (source.Min(x => x.from), source.Max(x => x.to));
}