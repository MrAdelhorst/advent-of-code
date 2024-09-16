//Part 1
var regex = new System.Text.RegularExpressions.Regex(@"\d+");
var triangles = File.ReadAllLines(@"..\..\..\..\day-03.txt").Select(line => (regex.Matches(line).Select(num => int.Parse(num.Value)).OrderDescending().ToArray()));
var totalLegalPt1 = triangles.Count(triangle => triangle[0] < triangle[1..].Sum());

//Part 2
var input = File.ReadAllLines(@"..\..\..\..\day-03.txt").Select(line => regex.Matches(line).Select(num => int.Parse(num.Value)).ToArray()).ToArray();
var totalLegalPt2 = 0;
for (var column = 0; column < 3; column++)
{
    for (var row = 0; row < input.Length; row += 3)
    {
        var numbers = input.Skip(row).Take(3).Select(line => line[column]).OrderDescending().ToArray();
        if (numbers[0] < numbers[1] + numbers[2])
            totalLegalPt2++;
    }
}

Console.WriteLine($"Part 1 - possible triangles: {totalLegalPt1}");
Console.WriteLine($"Part 2 - possible triangles: {totalLegalPt2}");
Console.ReadLine();