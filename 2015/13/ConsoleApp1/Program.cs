using System.Text.RegularExpressions;

var regex = new Regex(@"(\w+) would (gain|lose) (\d+) happiness units by sitting next to (\w+).");
var happinessMatrix = File.ReadAllLines(@"..\..\..\..\day-13.txt").Select(line =>
{
    var match = regex.Match(line);
    return (person: match.Groups[1].Value, neighbour: match.Groups[4].Value, happiness: int.Parse(match.Groups[3].Value) * (match.Groups[2].Value == "gain" ? 1 : -1));
}).GroupBy(t => t.person).ToDictionary(g => g.Key, g => g.ToDictionary(v => v.neighbour, v => v.happiness));

var people = happinessMatrix.Select(g => g.Key).ToArray();
var allCombinations = SeatPerson(people).ToArray();

Console.WriteLine($"Part 1 - optimal happiness: {allCombinations.Max(c => c.ToArray().CalculateHappiness(happinessMatrix))}");
Console.WriteLine($"Part 2 - optimal happiness: {allCombinations.Max(c => c.AddMe().Max(c2 => c2.CalculateHappiness(happinessMatrix)))}");
Console.ReadLine();

static IEnumerable<string>[] SeatPerson(IEnumerable<string> people) => people.Count() == 1 ? [ people ] :
    people.SelectMany(p => SeatPerson(people.Where(c => c != p).ToArray()).Select(r => r.Union([p]).ToArray())).ToArray();

static class Extensions
{
    public static int CalculateHappiness(this string[] combination, Dictionary<string, Dictionary<string, int>> happinessMatrix)
    {
        return combination.Select((person, index) =>
        {
            if (person == "Me") 
                return 0;

            var leftNeighbour = combination[index == 0 ? combination.Length - 1 : index - 1];
            var rightNeighbour = combination[index == combination.Length - 1 ? 0 : index + 1];
            var happiness = happinessMatrix[person];
            return (leftNeighbour != "Me" ? happiness[leftNeighbour] : 0) + (rightNeighbour != "Me" ? happiness[rightNeighbour] : 0);
        }).Sum();
    }

    public static IEnumerable<string[]> AddMe(this IEnumerable<string> combination) =>
        Enumerable.Range(0, combination.Count() - 1).Select(i => combination.Take(i).Union([ "Me" ]).Union(combination.Skip(i)).ToArray());
}