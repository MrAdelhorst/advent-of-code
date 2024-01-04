//Process input
var lines = File.ReadAllLines(@"..\..\..\..\day-11.txt");
var emptyRows = Enumerable.Range(0, lines.Length).Where(row => lines[row].All(c => c == '.')).ToArray(); 
var emptyCols = Enumerable.Range(0, lines[0].Length).Where(col => lines.All(line => line[col] == '.')).ToArray();

//Find galaxies
var galaxies = lines.SelectMany((line, y) => line.Select((sym, x) => lines[y][x] == '#' ? new Coordinate(x, y) : default)).Where(c => c != default).ToArray();

//Create combinations
var combinations = new List<(Coordinate, Coordinate)>();
for (int i=0; i < galaxies.Count() - 1; i++)
{
    for (int j = i + 1; j < galaxies.Count(); j++)
    {
        combinations.Add((galaxies[i], galaxies[j]));
    }
}

//Print results
Console.WriteLine($"Part 1 - sum of distances: {combinations.Sum(c => CalculateDistance(c.Item1, c.Item2, 1))}");
Console.WriteLine($"Part 2 - sum of distances: {combinations.Sum(c => CalculateDistance(c.Item1, c.Item2, 999999))}");
Console.ReadLine();

decimal CalculateDistance(Coordinate galaxy1, Coordinate galaxy2, int expansionFactor)
{
    return Math.Abs(galaxy1.X - galaxy2.X) + expansionFactor * emptyCols.Count(c => c > Math.Min(galaxy1.X, galaxy2.X) && c < Math.Max(galaxy1.X, galaxy2.X))
        + Math.Abs(galaxy1.Y - galaxy2.Y) + expansionFactor * emptyRows.Count(r => r > Math.Min(galaxy1.Y, galaxy2.Y) && r < Math.Max(galaxy1.Y, galaxy2.Y));
}

record Coordinate(int X, int Y);