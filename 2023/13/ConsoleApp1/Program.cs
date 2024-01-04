using System.Text;

//Parse input
var patterns = new List<string[]>();
var current = new List<string>();
foreach (var line in File.ReadAllLines(@"..\..\..\..\day-13.txt"))
{
    if (line != "")
    {
        current.Add(line);
    }
    else
    {
        patterns.Add(current.ToArray());
        current.Clear();
    }
}
patterns.Add(current.ToArray());

var diffFunctions = new[]
{
    //Part 1
    Extensions.IsMirroredAt,
    //Part 2
    Extensions.IsMirroredExceptOneAt
};

var part = 1;
foreach (var diffFunction in diffFunctions)
{
    var sum = 0;
    foreach (var pattern in patterns)
    {
        bool foundMatch = false;
        //Horizontal match
        for (int i = 0; i < pattern.Length - 1; i++)
            if (diffFunction.Invoke(pattern, i))
            {
                sum += 100 * (i + 1);
                if (foundMatch) throw new InvalidOperationException("Multiple matches");
            }
        //Vertical match
        var transposedPattern = pattern.Transpose();
        for (int i = 0; i < transposedPattern.Length - 1; i++)
            if (diffFunction.Invoke(transposedPattern, i))
            {
                sum += i + 1;
                if (foundMatch) throw new InvalidOperationException("Multiple matches");
            }
    }
    Console.WriteLine($"Part {part++} - Sum of notes: {sum}");
}

Console.ReadLine();

static class Extensions
{
    public static string[] Transpose(this string[] original)
    {
        var rows = original.First().Length;
        var transposed = new string[rows];
        for (int i=0; i <rows; i++)
        {
            var sb = new StringBuilder();
            foreach (var line in original)
                sb.Append(line[i]);
            transposed[i] = sb.ToString();
        }
        return transposed;
    }

    public static bool IsMirroredAt(this string[] pattern, int index)
    {
        foreach (var idx in Enumerable.Range(0, index + 1).Reverse())
        {
            var idxToCompare = index + 1 + (index - idx);
            if (idxToCompare > pattern.Length - 1)
                return true;
            if (pattern[idx] != pattern[idxToCompare]) 
                return false;
        }
        return true;
    }

    public static bool IsMirroredExceptOneAt(this string[] pattern, int index)
    {
        var totalDiffs = 0;
        foreach (var idx in Enumerable.Range(0, index + 1).Reverse())
        {
            var idxToCompare = index + 1 + (index - idx);
            if (idxToCompare > pattern.Length - 1)
                return totalDiffs == 1;
            var diffs = pattern[idx].CountDiffs(pattern[idxToCompare]);
            if (diffs > 0)
                if ((totalDiffs += diffs) > 1)
                    return false;
        }
        return totalDiffs == 1;
    }

    public static int CountDiffs(this string first, string second)
    {
        var diffs = 0;
        for (int i = 0; i < first.Length; i++)
            if (first[i] != second[i])
                diffs++;
        return diffs;
    }
}