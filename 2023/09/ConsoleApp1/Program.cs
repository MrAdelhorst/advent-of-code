var distances = File.ReadAllLines(@"..\..\..\..\day-09.txt").Select(line =>
{
    var numbers = line.Split(" ").Select(int.Parse).ToArray();
    var allSets = new List<int[]>([numbers]);

    //Find the all zeros row
    while (allSets.Last().Any(number => number != 0))
    {
        var current = allSets.Last();
        var nextSet = new List<int>();
        for (int i = 1; i < current.Count(); i++)
            nextSet.Add(current[i] - current[i - 1]);
        allSets.Add(nextSet.ToArray());
    }

    //Extrapolate
    allSets.Reverse();
    var lastDistance = 0;
    var firstDistance = 0;
    foreach (var set in allSets.Skip(1))
    {
        lastDistance += set.Last();
        firstDistance = set.First() - firstDistance;
    }

    return (lastDistance, firstDistance);
});

Console.WriteLine($"Part 1 - Sum of extrapolated values: {distances.Sum(distance => distance.lastDistance)}");
Console.WriteLine($"Part 2 - Sum of extrapolated values: {distances.Sum(distance => distance.firstDistance)}");
Console.ReadLine();