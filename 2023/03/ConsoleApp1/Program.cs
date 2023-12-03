using System.Text.RegularExpressions;

var numbers = new List<(int number, Coord startCoord, int length)>();
var symbols = new List<(string symbol, Coord coord)>();

//Parse input
var lines = File.ReadAllLines(@"..\..\..\..\day-03.txt");
for (int i=0; i<lines.Length; i++)
{
    Regex regNum = new Regex(@"(\d+)");
    var matchesNum = regNum.Matches(lines[i]);
    var lastMatch = 0;
    foreach (var match in matchesNum)
    {
        var txt = match.ToString() ?? throw new ArgumentException("Match is empty");
        var idx = lines[i].Substring(lastMatch).IndexOf(txt);
        numbers.Add((int.Parse(txt), new Coord(lastMatch + idx, i), txt.Length));
        lastMatch += idx + txt.Length;
    }
    
    Regex regSym = new Regex(@"[^0-9.]");
    var matchesSym = regSym.Matches(lines[i]);
    lastMatch = 0;
    foreach (var match in matchesSym)
    {
        var txt = match.ToString() ?? throw new ArgumentException("Match is empty"); ;
        var idx = lines[i].Substring(lastMatch).IndexOf(txt);
        symbols.Add((txt, new Coord(lastMatch + idx, i)));
        lastMatch += idx + txt.Length;
    }
}

//Part 1
var sumParts = 0;
foreach (var number in numbers)
{
    if (symbols.Any(s => IsAdjacent(number.startCoord, number.length, s.coord)))
    {
        sumParts += number.number;
    }
}

//Part 2
var sumRatios = 0;
var gears = symbols.Where(s => s.symbol == "*");
foreach (var gear in gears)
{
    var adjacentParts = numbers.Where(number => IsAdjacent(number.startCoord, number.length, gear.coord));
    if (adjacentParts.Count() == 2)
    {
        sumRatios += adjacentParts.First().number * adjacentParts.Last().number;
    }
}

Console.WriteLine($"Sum of part numbers: {sumParts}");
Console.WriteLine($"Sum of gear ratios: {sumRatios}");
Console.ReadLine();

//Helpers
bool IsAdjacent(Coord numberCoord, int numberLength, Coord symbolCoord)
{
    for (int i = 0; i < numberLength; i++)
    {
        if (Math.Abs(numberCoord.X + i - symbolCoord.X) <= 1 &&
            Math.Abs(numberCoord.Y - symbolCoord.Y) <= 1)
        {
            return true;
        }
    }
    return false;
}

record Coord(int X, int Y);
