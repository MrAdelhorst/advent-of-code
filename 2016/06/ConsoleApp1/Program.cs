var input = File.ReadAllLines(@"..\..\..\..\day-06.txt");
var result = new List<char>();
var result2 = new List<char>();
for (int column = 0; column < input[0].Length; column++)
{
    var characterOccurrence = input.Select(line => line[column]).GroupBy(c => c).OrderByDescending(g => g.Count());
    result.Add(characterOccurrence.First().Key);
    result2.Add(characterOccurrence.Last().Key);
}

Console.WriteLine($"Part 1 - Message: {string.Join("", result)}");
Console.WriteLine($"Part 2 - Message: {string.Join("", result2)}");
Console.ReadLine();