var input = File.ReadAllText(@"..\..\..\..\day-01.txt").Select(c => c - '0').ToArray();

Console.WriteLine($"Part 1 - Captcha: {input.CalculateCaptcha(1)}");
Console.WriteLine($"Part 2 - Captcha: {input.CalculateCaptcha(input.Length / 2)}");
Console.ReadLine();

static class Extensions
{
    public static int CalculateCaptcha(this int[] input, int indexToCompareWith)
    {
        var sum = 0;
        for (var index = 0; index < input.Length; index++)
        {
            var secondIndex = (index + indexToCompareWith) % input.Length;
            if (input[index] == input[secondIndex])
                sum += input[index];
        }
        return sum;
    }
}