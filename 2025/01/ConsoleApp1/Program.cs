var turns = (await File.ReadAllLinesAsync(@"..\..\..\..\day-01.txt"))
    .Select(line => (direction: line[0], range: int.Parse(line[1..])));

var current = 50;
var endZeros = 0;
var totalZeros = 0;

foreach (var (direction, range) in turns)
{
    if (direction == 'R')
    {
        current += range;
        totalZeros += current / 100;
        current %= 100;
    }
    else
    {
        if (current == 0)
            totalZeros--;

        current -= range;
        while (current < 0)
        {
            current += 100;
            totalZeros++;
        }

        if (current == 0)
            totalZeros++;
    }

    if (current == 0)
        endZeros++;
}

Console.WriteLine($"Part 1 - password: {endZeros}");
Console.WriteLine($"Part 2 - password: {totalZeros}");
Console.ReadLine();

//More readable version with poorer performance
//foreach (var (direction, range) in turns)
//{
//    foreach (var tick in Enumerable.Range(0, range))
//    {
//        current = direction == 'R' ? current + 1 : current - 1;

//        if (current == 100)
//            current = 0;

//        if (current == -1)
//            current = 99;

//        if (current == 0)
//            totalZeros++;
//    }

//    if (current == 0)
//        endZeros++;
//}