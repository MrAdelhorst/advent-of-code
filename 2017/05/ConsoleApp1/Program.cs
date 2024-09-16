var instructions = File.ReadAllLines(@"..\..\..\..\day-05.txt").Select(int.Parse);

Console.WriteLine($"Part 1 - steps: {RunInstructions(instructions.ToArray(), x => x + 1)}");
Console.WriteLine($"Part 2 - steps: {RunInstructions(instructions.ToArray(), x => x + (x > 2 ? -1 : 1))}");
Console.ReadLine();

int RunInstructions(int[] instructions, Func<int, int> newInstruction)
{
    var steps = 0;
    var index = 0;
    while (index >= 0 && index < instructions.Length)
    {
        var instruction = instructions[index];
        instructions[index] = newInstruction(instruction);
        index += instruction;
        steps++;
    }

    return steps;
}