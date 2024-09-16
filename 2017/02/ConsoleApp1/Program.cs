var lines = File.ReadAllLines(@"..\..\..\..\day-02.txt").Select(line => line.Split('\t').Select(int.Parse).ToArray());

Console.WriteLine($"Part 1 - checksum: {lines.Sum(line => line.Max() - line.Min())}");
Console.WriteLine($"Part 2 - checksum: {lines.Sum(line => line.DivideSingleDivisiblePair())}");
Console.ReadLine();

static class Extensions
{
    public static int DivideSingleDivisiblePair(this int[] numbers)
    {
        foreach (var number in numbers)
        {
            var num = numbers.Except([number]).SingleOrDefault(n => Math.Max(n, number) % Math.Min(n, number) == 0);
            if (num != default)
                return Math.Max(num, number) / Math.Min(num, number);
        }

        throw new InvalidOperationException("No divisible numbers found");
    }
}