using System.Text.RegularExpressions;

var regex = new Regex(@"(\w+) can fly (\d+) km/s for (\d+) seconds, but then must rest for (\d+) seconds.");
var reindeers = File.ReadAllLines(@"..\..\..\..\day-14.txt").Select(line =>
{
    var match = regex.Match(line);
    return (name: match.Groups[1].Value, speed: int.Parse(match.Groups[2].Value), fly: int.Parse(match.Groups[3].Value), rest: int.Parse(match.Groups[4].Value));
}).ToArray();

var status = reindeers.Select(r => (r.name, distance: 0, points: 0)).ToArray();
for (var iteration = 0; iteration < 2503; iteration++)
{
    foreach (var (reindeer, idx) in reindeers.Select((r, i) => (r, i)))
        status[idx].distance += (iteration % (reindeer.fly + reindeer.rest) < reindeer.fly) ? reindeer.speed : 0;

    var maxDistance = status.Max(s => s.distance);
    foreach (var idx in Enumerable.Range(0, status.Length))
        if (status[idx].distance == maxDistance)
            status[idx].points++;
}

Console.WriteLine($"Part 1 - winning distance: {status.Max(s => s.distance)}");
Console.WriteLine($"Part 2 - winning points: {status.Max(s => s.points)}");
Console.ReadLine();