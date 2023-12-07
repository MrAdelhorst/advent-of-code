var lines = File.ReadAllLines(@"..\..\..\..\day-07.txt");
var hands = lines.Select(line => line.Split(' '));

Console.WriteLine($"Part 1 - Total winnings: {Part1.Part1.Run(hands)}");
Console.WriteLine($"Part 2 - Total winnings: {Part2.Part2.Run(hands)}");
Console.ReadLine();