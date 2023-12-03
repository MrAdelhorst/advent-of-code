bool IsDigit(char c) => c >= '0' && c <= '9';
(char number, int index) FindTextNumber(string line, bool reverse = false)
{
    var numbers = new[]
    {
        new {text = "one", num = '1'},
        new {text = "two", num = '2'},
        new {text = "three", num = '3'},
        new {text = "four", num = '4'},
        new {text = "five", num = '5'},
        new {text = "six", num = '6'},
        new {text = "seven", num = '7'},
        new {text = "eight", num = '8'},
        new {text = "nine", num = '9'}
    };

    var startIdx = reverse ? line.Length - 1 : 0;
    var finalIdx = reverse ? 0 : line.Length - 1;
    var delta = reverse ? -1 : 1;

    for (int i = startIdx; i != finalIdx; i += delta)
    {
        var val = numbers.FirstOrDefault(num => line.Substring(i).StartsWith(num.text));
        if (val != null)
            return (val.num, i);
    }

    return ('0', -1);
}

var sum1 = 0;
var sum2 = 0;
foreach (var line in File.ReadAllLines(@"..\..\..\..\day-01.txt"))
{
    //First digit
    var firstDigit = line.First(IsDigit);
    var firstDigitIndex = line.IndexOf(firstDigit);
    var firstTextNumber = FindTextNumber(line);
    var first = firstTextNumber.number != '0' ? (firstTextNumber.index < firstDigitIndex ? firstTextNumber.number : firstDigit) : firstDigit;

    //Last digit
    var lastDigit = line.Last(IsDigit);
    var lastDigitIndex = line.LastIndexOf(lastDigit);
    var lastTextNumber = FindTextNumber(line, true);
    var last = lastTextNumber.number != '0' ? (lastTextNumber.index > lastDigitIndex ? lastTextNumber.number : lastDigit) : lastDigit;
    
    sum1 += int.Parse(new string(new[] { firstDigit, lastDigit }));
    sum2 += int.Parse(new string(new[] { first, last }));
}

//Print result
Console.WriteLine($"Part 1 - Calibration sum: {sum1}");
Console.WriteLine($"Part 2 - Calibration sum: {sum2}");
Console.ReadLine();