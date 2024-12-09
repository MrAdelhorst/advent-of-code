public class Part1
{
	public static long Calculate()
	{
        var fileSystem = File.ReadAllText(@"..\..\..\..\day-09.txt")
            .SelectMany((x, idx) => Enumerable.Repeat<long>(idx % 2 == 0 ? idx / 2 : -1, int.Parse([x])))
            .ToList();

        var currentIdx = 0;
        var reverseIdx = fileSystem.Count - 1;
        while (reverseIdx > currentIdx)
        {
            while (fileSystem[currentIdx] != -1)
                currentIdx++;

            while (fileSystem[reverseIdx] == -1)
                reverseIdx--;

            if (reverseIdx > currentIdx)
            {
                var current = fileSystem[currentIdx];
                fileSystem[currentIdx] = fileSystem[reverseIdx];
                fileSystem[reverseIdx] = current;
            }
        }
        return fileSystem
            .Select((x, i) => x != -1 ? x * i : 0)
            .Sum();
    }
}