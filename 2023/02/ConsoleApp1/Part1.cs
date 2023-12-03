using System.Text.RegularExpressions;

public class Part1
{
    public static int Run()
	{
        var sum = 0;
        var totalCubes = new[]
        {
            new { Colour = "red", Count = 12 },
            new { Colour = "green", Count = 13 },
            new { Colour = "blue", Count = 14 }
        };

        bool Legal(string colour, int count) => totalCubes.Any(c => c.Colour == colour && c.Count >= count);

        foreach (var line in File.ReadAllLines(@"..\..\..\..\day-02.txt"))
        {
            Regex regex = new Regex(@"(\d+)");
            Match match = regex.Match(line);
            var gameId = int.Parse(match.Value);

            var hands = line.Split(": ").Last().Split("; ").SelectMany(round => round.Split(", "));
            if (hands.All(x =>
            {
                var parts = x.Split(" ");
                return Legal(parts[1], int.Parse(parts[0]));
            }))
            {
                sum += gameId;
            }
        }

        return sum;
    }
}