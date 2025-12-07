var map = (await File.ReadAllLinesAsync(@"..\..\..\..\day-07.txt"))
    .Select(line => line
        .Select(loc => loc switch
        {
            'S' => 1L,
            '^' => -1L,
            _ => 0L,
        }).ToArray())
    .ToArray();

var splits = 0;
for (var y = 0; y < map.Length - 1; y++)
    for (var x=0; x < map[y].Length; x++)
        if (map[y][x] > 0)
            switch (map[y + 1][x])
            {
                case -1:
                    map[y + 1][x - 1] += map[y][x];
                    map[y + 1][x + 1] += map[y][x];
                    splits++;
                    break;
                default:
                    map[y + 1][x] += map[y][x];
                    break;
            }

Console.WriteLine($"Part 1 - Splits: {splits}");
Console.WriteLine($"Part 2 - Timelines: {map[^1].Sum()}");
Console.ReadLine();