var stones = File.ReadAllText(@"..\..\..\..\day-11.txt")
    .Split(' ')
    .Select(long.Parse)
    .ToList();

var uniqueStones = new Dictionary<long, long[]>();
List<long> stonesToCalculate = stones;
while (stonesToCalculate.Count != 0)
{
    var calculatedStones = stonesToCalculate.Select(x => (original: x, result: MutateStone(x)));
    stonesToCalculate = calculatedStones.SelectMany(x => x.result).Distinct().Where(x => !uniqueStones.ContainsKey(x)).ToList();
    foreach (var stone in calculatedStones)
        uniqueStones[stone.original] = stone.result;
}

Console.WriteLine($"Part 1 - Number of stones: {MutateStones(stones, 25)}");
Console.WriteLine($"Part 2 - Number of stones: {MutateStones(stones, 75)}");
Console.ReadLine();

long MutateStones(List<long> stones, int iterations)
{
    //Iteration 1
    var currentCounts = uniqueStones.ToDictionary(x => x.Key, x => (long)x.Value.Length);

    //Remaining iterations
    for (int i = 1; i < iterations; i++)
    {
        currentCounts = uniqueStones.Keys
            .ToDictionary(stone => stone, stone => uniqueStones[stone].Sum(x => currentCounts[x]));
    }

    return stones.Sum(x => currentCounts[x]);
}

long[] MutateStone(long stone)
{
    if (stone == 0)
        return [1];
    else
    {
        var tmp = stone.ToString().AsSpan();
        if (tmp.Length % 2 == 0)
        {
            var half = tmp.Length / 2;
            return [long.Parse(tmp[..half]), long.Parse(tmp[half..])];
        }
        else
            return [stone * 2024];
    }
}