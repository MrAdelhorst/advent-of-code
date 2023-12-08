using System.Text.RegularExpressions;

//Parse input
var lines = File.ReadAllLines(@"..\..\..\..\day-08.txt");
var directions = new Directions(string.Join("", lines.TakeWhile(line => line != "")));
Regex regex = new Regex(@"\b[A-Z]{3}\b");
var map = lines.Where(line => line.Contains('=')).Select(line => regex.Matches(line)).Select(matches => new KeyValuePair<string, (string left, string right)>(matches[0].Value, (matches[1].Value, matches[2].Value))).ToDictionary();

//Part 1 - Traverse map
var totalStepsPart1 = 0;
var currentPos = "AAA";
while (currentPos != "ZZZ")
{
    totalStepsPart1++;
    currentPos = (directions.Next == 'L') ? map[currentPos].left : map[currentPos].right;
}

//Part 2 - Traverse map
var currentPositions = map.Where(x => x.Key.EndsWith('A')).Select(x => x.Key).ToArray();
var possibleTrips = currentPositions.Select(pos =>
{
    long totalSteps = 0;
    var cp = pos;
    var endPositions = new Dictionary<string, HashSet<long>>();
    while (true)
    {
        totalSteps++;
        cp = (directions.Next == 'L') ? map[cp].left : map[cp].right;
        if (cp.EndsWith('Z'))
        {
            var relativeIndex = (int)totalSteps % directions.Length;
            if (!endPositions.ContainsKey(cp))
                endPositions.Add(cp, new HashSet<long>([totalSteps]));
            else
            {
                if (endPositions[cp].Any(idx => idx % directions.Length == relativeIndex))
                    break;
                else
                    endPositions[cp].Add(totalSteps);
            }
        }
    }
    return endPositions;
}).ToArray();

//Inspecting the data shows that each trip yields only one possible destination and repeats at a cadence - proof:
if (possibleTrips.Any(trip => trip.Count > 1 || trip.Single().Value.Count > 1))
    throw new NotSupportedException("The code below assumes cyclic trips with a single possible destination");

var stepsPerRoundTrip = possibleTrips.Select(trip => trip.Single().Value.Single());

//This was my initial solution - it's unfortunately very slow - ~3½ min in debug mode - it was only needed due to my lack of math skills ;)
var maxSteps = (double)stepsPerRoundTrip.Max();
var remaining = stepsPerRoundTrip.Where(s => s != maxSteps);
var current = maxSteps;
while (true)
{
    current += maxSteps;
    if (remaining.All(s => current % s == 0))
        break;
}

Console.WriteLine($"Part 1 - Total steps: {totalStepsPart1}");
Console.WriteLine($"Part 2 - Total steps: {current}");

//Bonus: turns out it can be done a lot faster using math that I didn't know :/ (LCM - Least Common Multiple)
var lcm = stepsPerRoundTrip.Select(s => Extensions.Factorize(s).ToArray()).CalculateLeastCommonMultiple();
Console.WriteLine($"Part 2 - optimized implementation: {lcm}");
Console.ReadLine();

class Directions(string directions)
{
    private int index = -1;
    public int Length => directions.Length;
    public char Next => (index < directions.Length - 1) ? directions[++index] : directions[index = 0];
    public void Reset() => index = -1;
}

public static class Extensions
{
    public static IEnumerable<long> Factorize(this long number)
    {
        for (int divisor = 2; divisor < number; divisor++)
        {
            while (number % divisor == 0)
            {
                number /= divisor;
                yield return divisor;
            }
        }
        //Also return the remainder
        yield return number;
    }

    public static double CalculateLeastCommonMultiple(this IEnumerable<IEnumerable<long>> factors) =>
        factors.SelectMany(f => f.Select(n => n)).Distinct().Select(factor =>
        {
            var maxCount = factors.Max(f => f.Count(x => x == factor));
            return (double)Math.Pow(factor, maxCount);
        }).Aggregate((x, y) => x * y);
}