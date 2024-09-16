using System.Text.RegularExpressions;

var lines = File.ReadAllLines(@"..\..\..\..\day-10.txt");
var bots = new Dictionary<string, (List<int> values, string low, string high)>();

//Add bots
var botRegex = new Regex(@"(bot \d+) gives low to (\w+ \d+) and high to (\w+ \d+)");
foreach (var bot in lines.Where(line => line.StartsWith("bot")))
{
    var match = botRegex.Match(bot);
    bots.Add(match.Groups[1].Value, (new List<int>(), match.Groups[2].Value, match.Groups[3].Value));
}

//Add values
var valueRegex = new Regex(@"value (\d+) goes to (bot \d+)");
foreach (var value in lines.Where(line => line.StartsWith("value")))
{
    var match = valueRegex.Match(value);
    bots[match.Groups[2].Value].values.Add(int.Parse(match.Groups[1].Value));
}

//Process bots
var doneBots = new List<string>();
do
{
    var readyBots = bots.Where(bot => bot.Ready(doneBots)).ToArray();
    foreach (var bot in readyBots)
    {
        bots.GetOrAdd(bot.Value.low).values.Add(bot.Value.values.Min());
        bots.GetOrAdd(bot.Value.high).values.Add(bot.Value.values.Max());
        doneBots.Add(bot.Key);
    }
} while (bots.Any(bot => bot.Ready(doneBots)));

Console.WriteLine($"Part 1 - bot comparing 17 & 61: {bots.Single(bot => bot.Value.values.Contains(17) && bot.Value.values.Contains(61)).Key}");
Console.WriteLine($"Part 2 - product of outputs 0, 1 & 2: {bots["output 0"].values[0] * bots["output 1"].values[0] * bots["output 2"].values[0]}");
Console.ReadLine();

static class Extensions
{
    public static (List<int> values, string low, string high) GetOrAdd(this IDictionary<string, (List<int> values, string low, string high)> dict, string key)
    {
        if (!dict.ContainsKey(key))
            dict.Add(key, (new List<int>(), string.Empty, string.Empty));

        return dict[key];
    }

    public static bool Ready(this KeyValuePair<string, (List<int> values, string low, string high)> bot, List<string> doneBots)
    {
        return bot.Value.values.Count == 2 && !doneBots.Contains(bot.Key);
    }
}