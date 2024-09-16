var file = File.ReadAllText(@"..\..\..\..\day-09.txt").AsSpan();

long CalculateSize(ReadOnlySpan<char> input, bool decompressInnerMarkers)
{
    long size = 0;
    var state = State.Normal;
    var index = 0;
    int firstNumber = 0, secondNumber = 0;
    while (index < input.Length)
    {
        switch (state)
        {
            case State.Normal:
                if (input[index] == '(')
                    state = State.StartBracket;
                else
                    size++;
                break;
            case State.StartBracket:
                {
                    if (int.TryParse(input.Slice(index, 1), out var digit))
                    {
                        firstNumber = digit;
                        state = State.FirstNumber;
                    }
                    else
                        throw new InvalidOperationException("Digit expected");
                    break;
                }
            case State.FirstNumber:
                {
                    if (int.TryParse(input.Slice(index, 1), out var digit))
                        firstNumber = firstNumber * 10 + digit;
                    else if (input[index] == 'x')
                        state = State.X;
                    else
                        throw new InvalidOperationException("Digit or 'x' expected");
                    break;
                }
            case State.X:
                {
                    if (int.TryParse(input.Slice(index, 1), out var digit))
                    {
                        secondNumber = digit;
                        state = State.SecondNumber;
                    }
                    else
                        throw new InvalidOperationException("Digit expected");
                    break;
                }
            case State.SecondNumber:
                {
                    if (int.TryParse(input.Slice(index, 1), out var digit))
                        secondNumber = secondNumber * 10 + digit;
                    else if (input[index] == ')')
                    {
                        if (decompressInnerMarkers)
                        {
                            var innerSize = CalculateSize(input.Slice(index + 1, firstNumber), true);
                            size += innerSize * secondNumber;
                            index += firstNumber;
                        }
                        else
                        {
                            size += firstNumber * secondNumber;
                            index += firstNumber;
                        }
                        state = State.Normal;
                    }
                    else
                        throw new InvalidOperationException("Digit or ')' expected");
                    break;
                }
        }
        index++;
    }
    return size;
}

Console.WriteLine($"Part 1 - decompressed size: {CalculateSize(file, false)}");
Console.WriteLine($"Part 2 - decompressed size: {CalculateSize(file, true)}");
Console.Read();

enum State
{
    Normal,
    StartBracket,
    FirstNumber,
    X,
    SecondNumber,
}