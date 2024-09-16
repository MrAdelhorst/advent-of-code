var passPhrases = File.ReadAllLines(@"..\..\..\..\day-04.txt");

Console.WriteLine($"Part 1 - valid passphrases: {passPhrases.Count(p => p.IsValid())}");
Console.WriteLine($"Part 2 - valid passphrases: {passPhrases.Count(p => p.IsValidPart2())}");
Console.ReadLine();

static class Extensions
{
    public static bool IsValid(this string passPhrase)
    {
        var words = passPhrase.Split(' ');
        return words.Distinct().Count() == words.Length;
    }

    public static bool IsValidPart2(this string passPhrase)
    {
        var words = passPhrase.Split(' ');
        return words.Select(word => new string(word.Order().ToArray())).Distinct().Count() == words.Length;
    }
}