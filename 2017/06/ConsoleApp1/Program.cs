var memoryBanks = File.ReadAllText(@"..\..\..\..\day-06.txt").Split('\t').Select(int.Parse).ToArray();

var history = new List<int[]>();
var current = memoryBanks;
for (var cycles = 0; !history.Contains(current, new IntArrayComparer()); cycles++)
{
    history.Add(current);
    current = Reallocate(current);
}
var loopStartIndex = history.IndexOf(history.Single(x => x.SequenceEqual(current)));

Console.WriteLine($"Part 1 - cycles: {history.Count()}");
Console.WriteLine($"Part 2 - cycles: {history.Count() - loopStartIndex}");
Console.ReadLine();

int[] Reallocate(int[] banks)
{
    var tmp = banks.Clone() as int[];
    var blocks = tmp.Max();
    var index = Array.IndexOf(tmp, blocks);
    tmp[index] = 0;
    for (int i = 0; i < blocks; i++)
    {
        index++;
        if (index >= tmp.Length)
            index = 0;
        tmp[index]++;
    }

    return tmp;
}

class IntArrayComparer : IEqualityComparer<int[]>
{
    public bool Equals(int[]? x, int[]? y) => x.SequenceEqual(y);
    public int GetHashCode(int[] obj) => obj.FirstOrDefault();
}