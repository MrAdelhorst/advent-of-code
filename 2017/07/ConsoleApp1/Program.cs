using System.Text.RegularExpressions;

var programs = File.ReadAllLines(@"..\..\..\..\day-07.txt").Select(line =>
{
    var parts = line.Split("->");
    var regex = new Regex(@"(\w+) \((\d+)\)");
    var program = regex.Match(parts[0]);

    return new KeyValuePair<string, (int weight, string[] subPrograms)>(program.Groups[1].Value, (weight: int.Parse(program.Groups[2].Value), subPrograms: (parts.Length > 1) ? parts[1].Trim().Split(", ").ToArray() : Array.Empty<string>()));
}).ToDictionary();

//Part 1
var current = programs.First().Key;
var bottom = string.Empty;
while (!string.IsNullOrEmpty(current))
{
    bottom = current;
    current = programs.SingleOrDefault(p => Array.Exists(p.Value.subPrograms, sp => sp == current)).Key;
}

//Part 2
var currentProgram = programs[bottom];
var candidateWeight = 0;
while (true)
{
    var weightGroups = currentProgram.subPrograms.Select(subProgram => (name: subProgram, treeWeight: CalculateWeight(subProgram))).GroupBy(p => p.treeWeight);
    if (weightGroups.Any(g => g.Count() == 1))
    {
        var single = weightGroups.Single(g => g.Count() == 1).Single();
        var multiple = weightGroups.Single(g => g.Count() > 1);
        currentProgram = programs[single.name];
        candidateWeight = currentProgram.weight + (multiple.First().treeWeight - single.treeWeight);
    }
    else
        break;
}

int CalculateWeight(string subProgram)
{
    var program = programs[subProgram];
    return program.weight + program.subPrograms.Sum(sp => CalculateWeight(sp));
}

Console.WriteLine($"Part 1 - Bottom program: {bottom}");
Console.WriteLine($"Part 1 - Fixed weigth: {candidateWeight}");
Console.ReadLine();