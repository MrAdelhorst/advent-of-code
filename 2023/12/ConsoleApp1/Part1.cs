using System.Text.RegularExpressions;

public class Part1
{
    public static long Run()
    {
        //Try to brute-force part 1
        return File.ReadAllLines(@"..\..\..\..\day-12.txt").Select(line =>
        {
            var parts = line.Split(" ");
            return new { springs = parts[0], groups = parts[1].Split(',').Select(int.Parse).ToArray() };
        }).Sum(record =>
            {
                var notAccountedFor = record.groups.Sum() - record.springs.Count(condition => condition == '#');
                if (notAccountedFor == 0)
                    return 1;
                else
                {
                    var unknownIdxs = record.springs.Select((c, idx) => c == '?' ? idx : -1).Where(idx => idx != -1);
                    var allCombinations = unknownIdxs.GenerateCombinations(notAccountedFor);
                    return allCombinations.Count(combination =>
                        record.groups.SequenceEqual(record.springs.Select((c, idx) => combination.Contains(idx) ? '#' : c).ToGroups()));
                }
            });
    }
}

public static partial class Extensions
{
    public static IEnumerable<int[]> GenerateCombinations(this IEnumerable<int> elements, int itemsPerCombination)
    {
        if (itemsPerCombination == 1)
        {
            return elements.Select(e => new int[] { e });
        }
        else
        {
            var allCombinations = new List<int[]>();
            var remainingElements = elements;
            while (remainingElements.Any())
            {
                var current = remainingElements.First();
                remainingElements = remainingElements.Skip(1).ToArray();
                var combinationsOfRemaining = GenerateCombinations(remainingElements, itemsPerCombination - 1);
                allCombinations.AddRange(combinationsOfRemaining.Select(c => c.Union(new int[] { current }).ToArray()));
            }
            return allCombinations.ToArray();
        }
    }

    public static int[] ToGroups(this IEnumerable<char> springs)
    {
        var regex = new Regex(@"(#+)");
        return regex.Matches(new string(springs.ToArray())).Select(g => g.Value.Length).ToArray();
    }
}
