using System.Text;

var input = "1321131112";
Console.WriteLine($"Part 1 - look-and-say length (40): {input.Iterate(40).Length}");
Console.WriteLine($"Part 2 - look-and-say length (50): {input.Iterate(50).Length}");
Console.ReadLine();

static class Extensions
{
    public static string Iterate(this string input, int iterations)
    {
        if (iterations == 0) return input;

        var builder = new StringBuilder();
        var currentIdx = 0;
        var currentNumber = input[currentIdx];
        var currentStreak = 1;
        while (currentIdx < input.Length)
        {
            if (currentIdx == input.Length - 1)
                builder.Append($"{currentStreak}{currentNumber}");
            else if (input[currentIdx + 1] != currentNumber)
            {
                builder.Append($"{currentStreak}{currentNumber}");
                currentNumber = input[currentIdx + 1];
                currentStreak = 1;
            }
            else
                currentStreak++;

            currentIdx++;
        }

        return Iterate(builder.ToString(), iterations - 1);
    }
}