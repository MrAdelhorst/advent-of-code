var equations = File.ReadAllLines(@"..\..\..\..\day-07.txt")
    .Select(line => line.Split(": "))
    .Select(parts => (result: long.Parse(parts[0]), operands: parts[1].Split(' ')
        .Select(long.Parse)
        .ToArray()))
    .ToList();

Console.WriteLine($"Part 1 - Calibration result: {equations.Where(x => Valid(x, [Add, Multiply])).Sum(e => e.result)}");
Console.WriteLine($"Part 2 - Calibration result: {equations.Where(x => Valid(x, [Add, Multiply, Concatenate])).Sum(e => e.result)}");
Console.ReadLine();

bool Valid((long result, long[] operands) equation, Func<long, long, long>[] operators)
{
    var operands = equation.operands.Reverse().ToArray();
    return Calculate(operands[1..], operators)
        .Any(res => operators
            .Any(op => op(res, operands[0]) == equation.result));
}

long[] Calculate(long[] operands, Func<long, long, long>[] operators)
{
    return (operands.Length == 1)
        ? operands
        : Calculate(operands[1..], operators)
            .SelectMany(res => operators
                .Select(op => op(res, operands[0])))
            .ToArray();
}

long Add(long a, long b) => a + b;
long Multiply(long a, long b) => a * b;
long Concatenate(long a, long b) => long.Parse($"{a}{b}");