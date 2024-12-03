var patternPart1 = @"mul\((\d+),(\d+)\)";
var patternPart2 = patternPart1 + @"|do\(\)|don't\(\)";

var input = File.ReadAllText(@"..\..\..\..\day-03.txt");

Console.WriteLine($"Part 1 - Result: {Calculate(patternPart1)}");
Console.WriteLine($"Part 2 - Result: {Calculate(patternPart2)}");
Console.ReadLine();

int Calculate(string pattern)
{
    bool add = true;

    return new System.Text.RegularExpressions.Regex(pattern)
        .Matches(input)
        .Select(m =>
        {
            return m switch
            {
                { Value: "do()" } => EnableAdd(true),
                { Value: "don't()" } => EnableAdd(false),
                _ => Multiply(m.Groups[1].Value, m.Groups[2].Value)
            };
        }).Sum();

    int EnableAdd(bool enable)
    {
        add = enable;
        return 0;
    }

    int Multiply(string x, string y) => add ? int.Parse(x) * int.Parse(y) : 0;
}