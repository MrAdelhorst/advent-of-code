using System.Text.RegularExpressions;

//Parse input
var regex = new Regex(@"(AND|OR|LSHIFT|RSHIFT|NOT)");
var transformations = File.ReadAllLines(@"..\..\..\..\day-07.txt").Select(line =>
{
    var parts = line.Split(" -> ");
    var operation = regex.Match(parts[0]).Value;
    var operands = operation switch
    {
        "AND" => parts[0].Split(" AND "),
        "OR" => parts[0].Split(" OR "),
        "LSHIFT" => parts[0].Split(" LSHIFT "),
        "RSHIFT" => parts[0].Split(" RSHIFT "),
        "NOT" => [parts[0].Replace("NOT ", "")],
        _ => [parts[0]]
    };

    return new KeyValuePair<string, (string operation, string[] operands)>(parts[1], (operation, operands));
}).ToDictionary();

//Part 1
var signalOnA = transformations.GetSignal("a", new Dictionary<string, uint>());
Console.WriteLine($"Part 1 - Signal on wire A: {signalOnA}");

//Part 2
transformations["b"] = ("", new[] { signalOnA.ToString() });
Console.WriteLine($"Part 2 - Signal on wire A: {transformations.GetSignal("a", new Dictionary<string, uint>())}");
Console.ReadLine();

static class Extensions
{
    public static uint GetSignal(this Dictionary<string, (string operation, string[] operands)> transformations, string signal, Dictionary<string, uint> cache)
    {
        if (cache.TryGetValue(signal, out var value))
            return value;

        var operands = transformations[signal].operands.Select(operand =>
                (uint.TryParse(operand, out var value)) ? value : transformations.GetSignal(operand, cache)).ToArray();

        cache[signal] = transformations[signal].operation switch
        {
            "AND" => operands[0] & operands[1],
            "OR" => operands[0] | operands[1],
            "LSHIFT" => operands[0] << (int)operands[1],
            "RSHIFT" => operands[0] >> (int)operands[1],
            "NOT" => ~operands[0],
            _ => operands[0]
        };

        return cache[signal];
    }
}