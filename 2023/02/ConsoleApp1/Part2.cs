using System.Text.RegularExpressions;

public class Part2
{
    public static int Run()
	{
        var sum = 0;
        foreach (var line in File.ReadAllLines(@"..\..\..\..\day-02.txt"))
        {
            var sets = line.Split(": ").Last().Split("; ").SelectMany(round => round.Split(", ").Select(hand =>
            {
                var parts = hand.Split(' ');
                return new { Colour = parts[1], Count = int.Parse(parts[0]) };
            }));

            var reds = sets.Where(x => x.Colour == "red").Max(x => x.Count);
            var greens = sets.Where(x => x.Colour == "green").Max(x => x.Count);
            var blues = sets.Where(x => x.Colour == "blue").Max(x => x.Count);

            sum += reds * greens * blues;
        }

        return sum;
    }
}