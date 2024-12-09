public class Part2
{
	public static long Calculate()
	{
        var fileSystem = File.ReadAllText(@"..\..\..\..\day-09.txt")
            .Select((x, idx) => (fileId: idx % 2 == 0 ? idx / 2 : -1, size: int.Parse([x])))
            .ToList();

        foreach (var fileId in Enumerable.Range(0, fileSystem.Count / 2 + 1).Reverse())
            TryMoveFile(fileSystem, fileId);

        long block = 0;
        long sum = 0;
        foreach (var (fileId, size) in fileSystem)
        {
            if (fileId != -1)
                sum += (block * size + Enumerable.Range(0, size).Sum()) * fileId;
            block += size;
        }

        return sum;
    }

    private static void TryMoveFile(List<(int fileId, int size)> fileSystem, int fileId)
    {
        var reverseIdx = fileSystem.Count - 1;
        while (fileSystem[reverseIdx].fileId != fileId)
            reverseIdx--;

        var currentIdx = 0;
        while ((fileSystem[currentIdx].fileId != -1 || fileSystem[currentIdx].size < fileSystem[reverseIdx].size)
            && currentIdx < reverseIdx)
            currentIdx++;

        if (currentIdx < reverseIdx)
        {
            var current = fileSystem[currentIdx];
            var reverse = fileSystem[reverseIdx];
            fileSystem[currentIdx] = reverse;
            fileSystem[reverseIdx] = (-1, reverse.size);
            if (current.size != reverse.size)
                fileSystem.Insert(currentIdx + 1, (-1, current.size - reverse.size));
        }
    }
}