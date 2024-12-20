var lines = File.ReadAllLines(@"..\..\..\..\day-17.txt");
var originalRegisters = lines[0..3].Select(line => long.Parse(line.Split(": ")[1])).ToArray();
var originalProgram = lines[^1].Split(": ")[1].Split(',').Select(int.Parse).ToArray();

Console.WriteLine($"Part 1 - Program output: {string.Join(',', RunProgram(originalRegisters, originalProgram, true))}");
Console.WriteLine($"Part 2 - Register 'A' value: {ResolveRegisterA(originalProgram, 0)}");
Console.ReadLine();

long ResolveRegisterA(int[] program, long restriction)
{
    var offset = (restriction << 3);

    long a = offset;
    while (a < offset + 8)
    {
        int res = RunProgram(originalRegisters.WithA(a), originalProgram, false).Single();
        if (res == program[^1])
        {
            if (program.Length == 1)
                return a;
            else
            {
                var inner = ResolveRegisterA(program[..^1], a);
                if (inner != -1)
                    return inner;
            }
        }
            
        a++;
    }
    
    return -1;
}

int[] RunProgram(long[] registers, int[] program, bool runFullProgram)
{
    var result = new List<int>();
    
    var ip = 0;
    while (true)
    {
        var instruction = program[ip];
        var operand = program[ip + 1];
        ip += 2;

        switch (instruction)
        {
            case 0:
                registers[0] = registers[0] / (int)Math.Pow(2, ComboOperand(registers, operand));
                break;
            case 1:
                registers[1] = registers[1] ^ operand;
                break;
            case 2:
                registers[1] = ComboOperand(registers, operand) % 8;
                break;
            case 3:
                if (registers[0] != 0)
                    ip = operand;
                break;
            case 4:
                registers[1] = registers[1] ^ registers[2];
                break;
            case 5:
                var output = (int)(ComboOperand(registers, operand) % 8);
                if (runFullProgram)
                    result.Add(output);
                else
                    return [output];
                break;
            case 6:
                registers[1] = registers[0] / (int)Math.Pow(2, ComboOperand(registers, operand));
                break;
            case 7:
                registers[2] = registers[0] / (int)Math.Pow(2, ComboOperand(registers, operand));
                break;
        }

        if (ip >= program.Length)
        {
            break;
        }
    }

    return result.ToArray();
}

long ComboOperand(long[] registers, int operand)
{
    return operand switch
    {
        < 4 => operand,
        4 => registers[0],
        5 => registers[1],
        6 => registers[2],
        7 => throw new ArgumentException("Invalid operand")
    };
}

internal static class Extensions
{
    public static long[] WithA(this long[] registers, long a)
    {
        var clone = registers.ToArray();
        clone[0] = a;
        return clone;
    }
}