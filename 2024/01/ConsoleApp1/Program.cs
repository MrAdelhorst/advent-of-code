var pairs = File.ReadAllLines(@"..\..\..\..\day-01.txt")
    .Select(x => System.Text.RegularExpressions.Regex.Split(x, @"\s+"));
var list1 = pairs.Select(x => int.Parse(x[0])).Order().ToList();
var list2 = pairs.Select(x => int.Parse(x[1])).Order().ToList();

var sum = 0;
var similarityScore = 0;

for (int i=0; i<list1.Count; i++)
{
    sum += Math.Abs(list1[i] - list2[i]);
    similarityScore += list1[i] * list2.Count(x => x == list1[i]);
}

Console.WriteLine($"Part 1 - Sum: {sum}"); 
Console.WriteLine($"Part 2 - Score: {similarityScore}");
Console.ReadLine();