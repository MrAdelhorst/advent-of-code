using System.Text.RegularExpressions;

var test = new[]
{
"Button A: X+94, Y+34",
"Button B: X+22, Y+67",
"Prize: X=8400, Y=5400",
"",
"Button A: X+26, Y+66",
"Button B: X+67, Y+21",
"Prize: X=12748, Y=12176",
"",
"Button A: X+17, Y+86",
"Button B: X+84, Y+37",
"Prize: X=7870, Y=6450",
"",
"Button A: X+69, Y+23",
"Button B: X+27, Y+71",
"Prize: X=18641, Y=10279",
};

var lines = File.ReadAllLines(@"..\..\..\..\day-13.txt");
var coordRegex = new Regex(@"X\+(\d+), Y\+(\d+)");
var prizeRegex = new Regex(@"X=(\d+), Y=(\d+)");

var machines = new List<(Coordinate buttonA, Coordinate buttonB, Coordinate prize)>();

int lineIndex = 0;
while (lineIndex < lines.Length)
{
    var buttonAText = coordRegex.Match(lines[lineIndex]);
    var buttonA = new Coordinate(int.Parse(buttonAText.Groups[1].Value), int.Parse(buttonAText.Groups[2].Value));
    var buttonBText = coordRegex.Match(lines[lineIndex + 1]);
    var buttonB = new Coordinate(int.Parse(buttonBText.Groups[1].Value), int.Parse(buttonBText.Groups[2].Value));
    var prizeText = prizeRegex.Match(lines[lineIndex + 2]);
    var prize = new Coordinate(int.Parse(prizeText.Groups[1].Value), int.Parse(prizeText.Groups[2].Value));
    machines.Add((buttonA, buttonB, prize));
    lineIndex += 4;
}

var tokens = 0;
foreach (var machine in machines)
{
    var (won, tokensUsed) = Calculate(machine.buttonA, machine.buttonB, machine.prize);
    if (won)
        tokens += tokensUsed;
}

Console.WriteLine($"Part 1 - {tokens}");
Console.WriteLine($"Part 2 - ");
Console.ReadLine();

(bool won, int tokensUsed) Calculate(Coordinate buttonA, Coordinate buttonB, Coordinate prize)
{
    if (100 * (buttonA.X + buttonB.X) < prize.X || 100 * (buttonA.Y + buttonB.Y) < prize.Y)
        return (false, 0);

    var bPresses = Math.Min(100, MaxNeeded(buttonB, prize));
    var aPresses = MaxNeeded(buttonA, new Coordinate(prize.X - bPresses * buttonB.X, prize.Y - bPresses * buttonB.Y));

    while (bPresses >= 0 
        && (aPresses * buttonA.X + bPresses * buttonB.X != prize.X
            || aPresses * buttonA.Y + bPresses * buttonB.Y != prize.Y))
    {
        bPresses--;
        aPresses = Math.Min(100, MaxNeeded(buttonA, new Coordinate(prize.X - bPresses * buttonB.X, prize.Y - bPresses * buttonB.Y)));
    }

    if(aPresses * buttonA.X + bPresses * buttonB.X == prize.X
            && aPresses * buttonA.Y + bPresses * buttonB.Y == prize.Y)
        return (true, aPresses * 3 + bPresses);
    else
        return (false, 0);
}

int MaxNeeded(Coordinate button, Coordinate deltaNeeded)
{
    return Math.Min(deltaNeeded.X / button.X, deltaNeeded.Y / button.Y);
}

record Coordinate(int X, int Y);