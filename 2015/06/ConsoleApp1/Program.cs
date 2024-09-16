using System.Text.RegularExpressions;

var lines = File.ReadAllLines(@"..\..\..\..\day-06.txt");
var regex = new Regex(@"^(?:turn on|toggle|turn off) (\d+),(\d+) through (\d+),(\d+)$");

var grid1 = new bool[1000, 1000];
var grid2 = new int[1000, 1000];
foreach (var line in lines)
{
    var match = regex.Match(line);
    var action = line.Substring(0, line.IndexOf(match.Groups[1].Value) - 1);
    var operation = (action == "turn on") ? Extensions.TurnOn :
                    (action == "turn off") ? Extensions.TurnOff :
                    (Action<bool[,], int, int>) Extensions.Toggle;
    var delta = (action == "turn on") ? 1 : (action == "turn off") ? -1 : 2;
    for (int x = int.Parse(match.Groups[1].Value); x <= int.Parse(match.Groups[3].Value); x++)
        for (int y = int.Parse(match.Groups[2].Value); y <= int.Parse(match.Groups[4].Value); y++)
        {
            operation(grid1, x, y);
            grid2.Adjust(x, y, delta);
        }
}

Console.WriteLine($"Part 1 - Lights on: {grid1.Cast<bool>().Count(b => b)}");
Console.WriteLine($"Part 2 - Total brightness: {grid2.Cast<int>().Sum()}");
Console.ReadLine();

static class Extensions
{
    public static void TurnOn(this bool[,] grid, int x, int y) => grid[x, y] = true;
    public static void TurnOff(this bool[,] grid, int x, int y) => grid[x, y] = false;
    public static void Toggle(this bool[,] grid, int x, int y) => grid[x, y] = !grid[x, y];
    public static void Adjust(this int[,] grid, int x, int y, int delta) => grid[x, y] = Math.Max(0, grid[x, y] + delta);
}