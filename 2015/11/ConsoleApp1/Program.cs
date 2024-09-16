var input = "cqjxjnds";
var password1 = input.NextValidPassword();
var password2 = password1.NextValidPassword();

Console.WriteLine($"Part 1 - new password: {password1}");
Console.WriteLine($"Part 1 - new password: {password2}");
Console.ReadLine();

static class Extensions
{
    public static string NextValidPassword(this string password)
    {
        var newPassword = password.Next();
        while (!newPassword.IsValid())
            newPassword = newPassword.Next();
        return newPassword;
    }

    public static string Next(this string password)
    {
        var currentIdx = password.Length - 1;
        var newPassword = password.ToCharArray();
        while (true)
        {
            if (newPassword[currentIdx] == 'z')
            {
                newPassword[currentIdx] = 'a';
                currentIdx--;
            }
            else
            {
                if (new[] { 'i', 'o', 'l' }.Contains(newPassword[currentIdx])) //skip illegal characters
                    newPassword[currentIdx]++;
                newPassword[currentIdx]++;
                return new string(newPassword);
            }
        }
    }

    public static bool IsValid(this string password) => password.ContainsSequence() && password.ContainsTwoPairs();

    public static bool ContainsSequence(this string password)
    {
        var current = password.First();
        var streak = 1;
        foreach (var c in password.Skip(1))
        {
            if (c == ++current)
                streak++;
            else
            {
                current = c;
                streak = 1;
            }
            if (streak == 3)
                return true;
        }
        return false;
    }

    public static bool ContainsTwoPairs(this string password)
    {
        var pairs = new List<char>();
        var current = password.First();
        foreach (var c in password.Skip(1))
        {
            if (c == current && !pairs.Contains(c))
                pairs.Add(c);
            else
                current = c;
        }
        return pairs.Count >= 2;
    }
}