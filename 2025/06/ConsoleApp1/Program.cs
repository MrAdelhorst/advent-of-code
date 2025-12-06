var file = (await File.ReadAllLinesAsync(@"..\..\..\..\day-06.txt")).ToArray();
var operators = file[^1];

long DoMath(string[] file, Func<string[], int, int, IEnumerable<int>> operandSelector)
{
    var currentIndex = 0;
    long total = 0;
    do
    {
        var nextIndex = operators.IndexOfAny(['+', '*'], currentIndex + 1);
        var operands = operandSelector(file, currentIndex, nextIndex == -1 ? operators.Length : nextIndex - 1);
        total += file[^1][currentIndex] == '+' 
            ? operands.Aggregate(0L, (acc, operand) => acc + operand) 
            : operands.Aggregate(1L, (acc, operand) => acc * operand);
        currentIndex = nextIndex;
    } while (currentIndex != -1);
    return total;
}

Console.WriteLine($"Part 1 - Result: {DoMath(file, (file, startIdx, endIdx) =>
{
    return Enumerable.Range(0, file.Length - 1)
    .Select(row => int.Parse(file[row][startIdx..endIdx].Trim()));
})}");
Console.WriteLine($"Part 2 - Result: {DoMath(file, (file, startIdx, endIdx) =>
{
    return Enumerable.Range(startIdx, endIdx - startIdx)
    .Select(idx => int.Parse(new string([.. file[0..^1].Select(row => row[idx])]).Trim()));
})}");
Console.ReadLine();