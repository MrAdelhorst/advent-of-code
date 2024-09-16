var lines = File.ReadAllLines(@"..\..\..\..\day-02.txt");

//Part 1
var keypadPart1 = new[]
{
    "123",
    "456",
    "789"
};

var startPt1 = new Point(1, 1);
var part1 = NavigateKeypad(keypadPart1, startPt1, lines);

//Part 2
var keypadPart2 = new[]
{
    "  1  ",
    " 234 ",
    "56789",
    " ABC ",
    "  D  "
};

var startPt2 = new Point(0, 2);
var part2 = NavigateKeypad(keypadPart2, startPt2, lines);

Console.WriteLine($"Part 1: {part1}");
Console.WriteLine($"Part 2: {part2}");
Console.ReadLine();

string NavigateKeypad(string[] keypad, Point start, string[] lines)
{
    var current = start;
    string digits = string.Empty;
    foreach (var line in lines)
    {
        foreach (var command in line)
            current = current.Move(command, keypad);

        digits += keypad[current.Y][current.X];
    }
    return digits;
}

record Point(int X, int Y)
{
    public Point Move(char command, string[] keyPad)
    {
        var destination = command switch
        {
            'U' => new Point(X, Y - 1),
            'D' => new Point(X, Y + 1),
            'L' => new Point(X - 1, Y),
            'R' => new Point(X + 1, Y),
            _ => throw new ArgumentException($"Invalid command {command}")
        };

        if (destination.X < 0 || destination.X >= keyPad[0].Length || destination.Y < 0 || destination.Y >= keyPad.Length || keyPad[destination.Y][destination.X] == ' ')
            return new Point(X, Y);
        else
            return destination;
    }
}