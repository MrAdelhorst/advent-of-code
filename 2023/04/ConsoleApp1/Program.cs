using System.Text.RegularExpressions;

//Parse input
var games = new Dictionary<int, int>();
var lines = File.ReadAllLines(@"..\..\..\..\day-04.txt");
for (int gameNo = 0; gameNo < lines.Count(); gameNo++)
{
    Regex regex = new Regex(@"(\d+)");
    var pairs = lines[gameNo].Split(": ").Last().Split(" | ");
    var winning = regex.Matches(pairs.First()).Select(n => int.Parse(n.Value));
    var mine = regex.Matches(pairs.Last()).Select(n => int.Parse(n.Value));
    games.Add(gameNo, winning.Count(w => mine.Contains(w)));
}

//Calculate
Console.WriteLine($"Part 1 - Points: {games.Sum(g => g.Value == 0 ? 0 : (int)Math.Pow(2, g.Value - 1))}");
Console.WriteLine($"Part 2 - Total cards: {games.Sum(g => CountTotalCards(g.Key))}");
Console.ReadLine();

//Helpers
int CountTotalCards(int gameNo) => 1 + Enumerable.Range(1, games[gameNo]).Sum(n => CountTotalCards(gameNo + n));