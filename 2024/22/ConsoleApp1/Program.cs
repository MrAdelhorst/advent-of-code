var numbers = File.ReadAllLines(@"..\..\..\..\day-22.txt")
    .Select(long.Parse);

Console.WriteLine($"Part 1 - {numbers.Sum(number => Enumerable.Range(0, 2000).Aggregate(number, (prev, _) => CalculateSecretNumber(prev)))}");
Console.WriteLine($"Part 2 - {CalculateMostBananas()}");
Console.ReadLine();

long CalculateMostBananas()
{
    var prices = numbers.Select(number =>
    {
        var res = new Dictionary<string, int>();
        var diffPattern = new List<int>();
        var previous = int.MaxValue;
        var current = number;
        for (int i = 0; i < 2000; i++)
        {
            current = CalculateSecretNumber(current);
            var price = (int)(current % 10);
            if (previous != int.MaxValue)
                diffPattern.Add(price - previous);
            var pattern = diffPattern.Count >= 4 ? string.Join("", diffPattern[^4..]) : string.Empty;
            if (!res.ContainsKey(pattern))
                res[pattern] = price;
            previous = price;
        }
        return res;
    }).ToArray();

    var patterns = prices
        .SelectMany(x => x.Keys)
        .Distinct();

    return patterns
        .Where(pattern => pattern != string.Empty)
        .Max(pattern => prices.Sum(p => p.TryGetValue(pattern, out var price) ? price : 0));
}

long CalculateSecretNumber(long number)
{
    var current = number;

    current = MixAndPrune(current, current * 64);
    current = MixAndPrune(current, current / 32);
    current = MixAndPrune(current, current * 2048);

    return current;
}

long MixAndPrune(long number, long seed)
{
    var tmp = number ^ seed;
    return tmp % 16777216;
}