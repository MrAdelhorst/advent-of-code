var containers = File.ReadAllLines(@"..\..\..\..\day-17.txt").Select(line => int.Parse(line)).OrderDescending().ToArray();//.AsSpan();

IEnumerable<int[]> CountCombinations(int startIndex, int remainingVolume, Span<int> containers, int[] found)
{
    var res = new List<int[]>();
    for (int idx = startIndex; idx < containers.Length; idx++)
    {
        var newVolume = remainingVolume - containers[idx];
        if (newVolume == 0)
        {
            res.Add(found.Concat([idx]).ToArray());
        }
        else if (newVolume > 0)
            res.AddRange(CountCombinations(idx + 1, newVolume, containers, found.Concat([idx]).ToArray()));
    }
    return res;
}

var combinations = CountCombinations(0, 150, containers, []);
var minContainers = combinations.MinBy(c => c.Length).Length;

Console.WriteLine($"Part 1 - Combinations: {combinations.Count()}");
Console.WriteLine($"Part 2 - Combinations: {combinations.Where(c => c.Length == minContainers).Count()}");
Console.ReadLine();