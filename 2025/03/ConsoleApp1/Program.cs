var banks = await File.ReadAllLinesAsync(@"..\..\..\..\day-03.txt");

Console.WriteLine($"Part 1 - Total Joltage: {banks.Aggregate(0L, (acc, bank) => acc + bank.CalculateMaxJoltage(2))}");
Console.WriteLine($"Part 2 - Total Joltage: {banks.Aggregate(0L, (acc, bank) => acc + bank.CalculateMaxJoltage(12))}");
Console.ReadLine();

public static class Extensions
{
    public static long CalculateMaxJoltage(this string bank, int numDigits)
    {
        var result = new List<int>();
        char[] digits = ['9', '8', '7', '6', '5', '4', '3', '2', '1'];
        for (int i = 0; i < numDigits; i++)
        {
            foreach (var digit in digits)
            {
                var start = i == 0 ? 0 : result[i - 1] + 1;
                var end = numDigits - 1 - i;
                var idx = bank[start..^end].IndexOf(digit);
                if (idx != -1)
                {
                    result.Add(start + idx);
                    break;
                }
            }
        }
        return long.Parse(string.Concat(result.Select(i => bank[i])));
    }
}