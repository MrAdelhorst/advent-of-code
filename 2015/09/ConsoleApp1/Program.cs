using System.Text.RegularExpressions;

var regex = new Regex(@"(\w+) to (\w+) = (\d+)");
var distances = File.ReadLines(@"..\..\..\..\day-09.txt").Select(line =>
{
    var match = regex.Match(line);
    return (from: match.Groups[1].Value, to: match.Groups[2].Value, distance: int.Parse(match.Groups[3].Value));
}).GroupBy(x => x.from).ToDictionary(x => x.Key, y => y.ToDictionary(z => z.to, z => z.distance));

var allCities = distances.First().Value.Keys.Union([distances.First().Key]);
var allRoutes = allCities.SelectMany(city => CalculateAllRoutes(city, allCities.Except([city]), distances)).ToArray();

Console.WriteLine($"Part 1 - shortest route: {allRoutes.Min()}");
Console.WriteLine($"Part 2 - longest route: {allRoutes.Max()}");
Console.ReadLine();

IEnumerable<int> CalculateAllRoutes(string city, IEnumerable<string> destinations, Dictionary<string, Dictionary<string, int>> distances)
{
    if (destinations.Count() == 1)
        return [ city.DistanceTo(destinations.Single(), distances) ];
    return destinations.SelectMany(destination => CalculateAllRoutes(destination, destinations.Except([destination]).ToArray(), distances).Select(r => (r + city.DistanceTo(destination, distances)))).ToArray();
}

static class Extensions
{
    public static int DistanceTo(this string from, string to, Dictionary<string, Dictionary<string, int>> distances)
    {
        return distances.TryGetValue(from, out var destinations) && destinations.TryGetValue(to, out var distance) ? distance : distances[to][from];
    }
}