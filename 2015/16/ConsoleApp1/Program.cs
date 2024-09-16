using System.Text.RegularExpressions;

var clues = new[]
{
    ("children", 3),
    ("cats", 7),
    ("samoyeds", 2),
    ("pomeranians", 3),
    ("akitas", 0),
    ("vizslas", 0),
    ("goldfish", 5),
    ("trees", 3),
    ("cars", 2),
    ("perfumes", 1)
}.ToDictionary();

var regex = new Regex(@"(\w+: \d)+");
var aunts = File.ReadAllLines(@"..\..\..\..\day-16.txt").Select((line, idx) => (aunt: idx + 1, regex.Matches(line).Select(e => e.ToKeyValuePair()))).ToDictionary(x => x.aunt, x => x.Item2.ToDictionary());

Console.WriteLine($"Part 1 - Aunt: {aunts.Single(aunt => aunt.Value.All(x => clues.Contains(x))).Key}");
Console.WriteLine($"Part 2 - Aunt: {aunts.Single(aunt => aunt.Value.Matches(clues)).Key}");
Console.ReadLine();

static class Extensions
{
    public static KeyValuePair<string, int> ToKeyValuePair(this Match match)
    {
        var parts = match.Value.Split(": ");
        return new KeyValuePair<string, int>(parts[0], int.Parse(parts[1]));
    }

    public static bool Matches(this Dictionary<string, int> aunt, Dictionary<string, int> clues)
    {
        foreach (var fact in aunt) 
        {
            switch (fact.Key)
            {
                case "cats":
                case "trees":
                    if (fact.Value <= clues[fact.Key])
                        return false;
                    break;
                case "pomeranians":
                case "goldfish":
                    if (fact.Value >= clues[fact.Key])
                        return false;
                    break;
                default:
                    if (fact.Value != clues[fact.Key])
                        return false;
                    break;
            }
        }

        return true;
    }
}