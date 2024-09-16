using System.Text.Json;
using System.Text.RegularExpressions;

//Part 1
var input = File.ReadAllText(@"..\..\..\..\day-12.txt");
var regex = new Regex(@"(-?\d+)");
var sum = regex.Matches(input).Select(m => int.Parse(m.Value)).Sum();

//Part 2
var json = JsonDocument.Parse(input);
var sum2 = ProcessJsonElement(json.RootElement);

Console.WriteLine($"Part 1 - sum of all numbers: {sum}");
Console.WriteLine($"Part 2 - sum of all remaining numbers: {sum2}");
Console.ReadLine();

int ProcessJsonElement(JsonElement json)
{
    if (json.ValueKind == JsonValueKind.Object)
    {
        var children = json.EnumerateObject();
        if (!children.Any(p => p.Value.ValueKind == JsonValueKind.String && p.Value.ToString() == "red"))
            return children.Select(p => ProcessJsonElement(p.Value)).Sum();
    }
    else if (json.ValueKind == JsonValueKind.Array)
        return json.EnumerateArray().Select(ProcessJsonElement).Sum();
    else if (json.ValueKind == JsonValueKind.Number && json.TryGetInt32(out int value))
        return value;
    
    return 0;
}