using System.Collections.Immutable;
using System.Text.RegularExpressions;

var lines = File.ReadAllLines(@"..\..\..\..\day-05.txt");
var updates = lines.Select(line => new Regex(@"^\d+(,\d+)*$").Match(line))
    .Where(m => m.Success)
    .Select(x => x.Value.Split(',')
        .Select(int.Parse)
        .ToList())
    .ToList();

var ordering = lines.Select(line => new Regex(@"\d+\|\d+").Match(line))
    .Where(m => m.Success)
    .Select(x => x.Value.Split('|'))
    .Select(x => (key: int.Parse(x[0]), value: int.Parse(x[1])))
    .ToImmutableHashSet();

var comparer = new PageComparer(ordering);
var sortedUpdates = updates
    .Select(update => (original: update, ordered: update.Order(comparer).ToList()))
    .Select(x => (update: x.ordered, equal: x.original.SequenceEqual(x.ordered)))
    .ToList();

Console.WriteLine($"Part 1 - Sum: {sortedUpdates.Where(x => x.equal).Sum(x => x.update[x.update.Count / 2])}");
Console.WriteLine($"Part 2 - Sum: {sortedUpdates.Where(x => !x.equal).Sum(x => x.update[x.update.Count / 2])}");
Console.ReadLine();

internal class PageComparer(ImmutableHashSet<(int key, int value)> ordering) : IComparer<int>
{
    public int Compare(int x, int y)
    {
        if (ordering.Contains((x,y)))
            return -1;

        if (ordering.Contains((y, x)))
            return 1;

        return 0;
    }
}