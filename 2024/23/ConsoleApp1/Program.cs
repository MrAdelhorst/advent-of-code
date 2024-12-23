var pairs = File.ReadAllLines(@"..\..\..\..\day-23.txt")
    .Select(line => line.Split('-')).ToArray();

var all = pairs.SelectMany(x => x).Distinct();
var connections = all
    .Select(x => (item: x, connections: pairs
        .Where(p => Array.Exists(p, y => y == x))
        .Select(p => p.Other(x))
        .ToArray()))
    .ToDictionary();

//Part 1
var threes = pairs
    .Where(pair => Array.Exists(pair, x => x.StartsWith('t')))
    .SelectMany(pair =>
    {
        var conn1 = connections[pair[0]];
        var conn2 = connections[pair[1]];
        var shared = conn1.Intersect(conn2).ToList();
        return shared.Select(x => pair.Union([x])).ToList();
    })
    .Select(x => x
        .OrderBy(z => z)
        .ToArray())
    .Select(x => (x[0], x[1], x[2]))
    .ToArray();

//Part 2 - takes ~100secs to complete
var best = string.Empty;
var bestLength = 0;
for (int i = 0; i < pairs.Length; i++)
{
    if (!pairs[i][0].StartsWith('t'))
        continue;

    var sets = FindShared(pairs[i], connections);

    var longest = sets.Any() ? sets.Max(x => x.Length) : 0;
    if (longest > bestLength)
    {
        best = string.Join(',', sets.First(set => set.Length == longest)
            .OrderBy(x => x));
        bestLength = longest;
    }
}

Console.WriteLine($"Part 1 - {threes.Distinct().Count()}");
Console.WriteLine($"Part 2 - {best}");
Console.ReadLine();

IEnumerable<string[]> FindShared(string[] items, Dictionary<string, string[]> connections)
{
    var res = new List<string[]>();
    if (items.Length == 1)
        return [[.. items]];
    for (int i = 0; i < items.Length; i++)
    {
        for (var j = i + 1; j < items.Length; j++)
        {
            if (connections[items[i]].Contains(items[j]))
            {
                string[] currentPair = [items[i], items[j]];
                var shared = connections[items[i]].Intersect(connections[items[j]]).ToArray();
                if (shared.Length > 0)
                {
                    var inner = FindShared(shared, shared
                            .Select(s => (s, connections[s].Except(currentPair)
                                .Where(x => shared.Contains(x))
                            .ToArray()))
                        .ToDictionary());
                    res.AddRange(inner
                        .Select(set => set.Union(currentPair)
                        .ToArray()));
                }
                else
                    res.Add(currentPair);
            }
        }
    }
    return res;
}

internal static class Extensions
{
    public static string Other(this string[] pair, string item) => (pair[0] == item) ? pair[1] : pair[0];
}