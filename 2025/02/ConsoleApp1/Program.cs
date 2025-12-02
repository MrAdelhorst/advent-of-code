var ranges = (await File.ReadAllTextAsync(@"..\..\..\..\day-02.txt"))
    .Split(',')
    .Select(range => range.Split('-'))
    .Select(pair => (from: long.Parse(pair[0]), to: long.Parse(pair[1])));

long totalPart1 = 0;
long totalPart2 = 0;

foreach (var range in ranges)
{
    for(long i = range.from; i <= range.to; i++)
    {
        if (!IsValidPart1(i.ToString()))
            totalPart1 += i;

        if (!IsValidPart2(i.ToString()))
            totalPart2 += i;
    }
}

Console.WriteLine($"Part 1 - Sum: {totalPart1}");
Console.WriteLine($"Part 2 - Sum: {totalPart2}");
Console.ReadLine();

bool IsValidPart1(string id)
{
    if (id.Length % 2 != 0)
        return true;

    var mid = id.Length / 2;
    return id[..mid] != id[mid..];
}

bool IsValidPart2(string id)
{
    for (int i=1; i<=id.Length / 2; i++)
    {
        if (id.Length % i != 0)
            continue;

        var pattern = id[..i];
        int j = 1;
        for (; j < id.Length / i; j++)
        {
            if (id[(i * j)..(i * (j + 1))] != pattern)
                break;
        }
        if (j == id.Length / i)
            return false;
    }
    return true;
}