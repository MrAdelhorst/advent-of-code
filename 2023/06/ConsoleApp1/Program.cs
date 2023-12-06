using System.Text.RegularExpressions;

//Parse input
var lines = File.ReadAllLines(@"..\..\..\..\day-06.txt");
Regex regex = new Regex(@"(\d+)");
var times = regex.Matches(lines[0]).Select(t => int.Parse(t.Value));
var distances = regex.Matches(lines[1]).Select(d => int.Parse(d.Value));
var time = int.Parse(string.Join("", times));
var distance = long.Parse(string.Join("", distances));

var waysOfWinning = times.Zip(distances).Select(race => 
    Enumerable.Range(1, race.First).Count(t => DistanceAchieved(t, race.First) > race.Second));

Console.WriteLine("Part 1 - Combined ways of winning: " + waysOfWinning.Aggregate((x, y) => x * y));
Console.WriteLine("Part 2 - Ways of winning: " + Enumerable.Range(1, time).Count(t => DistanceAchieved(t, time) > distance));
Console.ReadLine();

long DistanceAchieved(int timePressed, int timeMax) => ((long)timeMax - timePressed) * timePressed;