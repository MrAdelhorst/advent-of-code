using System.Text.RegularExpressions;

var lines = File.ReadAllLines(@"..\..\..\..\day-08.txt");
var lengths = lines.Select(line => (original: line.Length, unescaped: Regex.Unescape(line).Length - 2, escaped: (line.Length + line.Count(c => c == '\"' || c == '\\') + 2))).ToList();

Console.WriteLine($"Part 1 - unescape savings: {lengths.Sum(l => l.original - l.unescaped)}");
Console.WriteLine($"Part 2 - escape costs: {lengths.Sum(l => l.escaped - l.original)}");
Console.ReadLine();