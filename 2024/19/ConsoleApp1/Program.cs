var lines = File.ReadAllLines(@"..\..\..\..\day-19.txt");

var towels = lines[0].Split(", ").ToArray();
var designs = lines.Skip(2).ToArray();

Console.WriteLine($"Part 1 - {designs.Count(d => d.Valid(towels, []))}");
Console.WriteLine($"Part 2 - {designs.Sum(x => x.CountCombinations(towels, []))}");
Console.ReadLine();

internal static class Extensions
{
    public static bool Valid(this string fragment, string[] towels, Dictionary<string, bool> cache)
    {
        if (cache.TryGetValue(fragment, out var result))
            return result;

        cache[fragment] = Array.Exists(towels, towel =>
            fragment.StartsWith(towel)
            && (fragment.Length == towel.Length
                || fragment.Substring(towel.Length).Valid(towels, cache)));

        return cache[fragment];
    }

    public static long CountCombinations(this string fragment, string[] towels, Dictionary<string, long> cache)
    {
        if (string.IsNullOrEmpty(fragment))
            return 1;

        if (cache.TryGetValue(fragment, out var combinations))
            return combinations;
        else
        {
            var subFragments = towels
                .Where(fragment.StartsWith)
                .Select(towel => fragment.Substring(towel.Length))
                .Select(x => (remainder: x, combinations: x.CountCombinations(towels, cache)));

            foreach (var subFragment in subFragments)
                cache[subFragment.remainder] = subFragment.combinations;

            return subFragments.Sum(x => x.combinations);
        }
    }
}