using System.Text.RegularExpressions;

var hypernetRegex = new Regex(@"\[([a-z]+)\]");
var ipAddresses = File.ReadLines(@"..\..\..\..\day-07.txt").Select(line =>
{
    var hypernets = hypernetRegex.Matches(line).ToArray();
    var ipText = line;
    foreach (var hypernet in hypernets)
         ipText = ipText.Replace(hypernet.Groups[1].Value, "");
    return (ips: ipText.Split("[]").ToArray(), hypernets);
});

var supportsTls = ipAddresses.Where(ipAddress =>
                        ipAddress.ips.Any(str => str.IsAbba()) &&
                        !ipAddress.hypernets.Any(str => str.Value.IsAbba()));

var supportsSsl = ipAddresses.Where(ipAddress =>
{
    var abas = ipAddress.ips.SelectMany(str => str.XYXs()).ToArray();
    var babs = ipAddress.hypernets.SelectMany(str => str.Value.XYXs()).ToArray();
    return abas.Any(aba => babs.Any(bab => aba.a == bab.b && aba.b == bab.a));
});

Console.WriteLine($"Part 1 - IPs supporting TLS: {supportsTls.Count()}");
Console.WriteLine($"Part 2 - IPs supporting SSL: {supportsSsl.Count()}");
Console.ReadLine();

static class Extenstions
{
    public static bool IsAbba(this string str)
    {
        Regex regex = new Regex(@"(.)((?:(?!\1).))\2\1");
        var match = regex.Match(str);
        return match.Success;
    }

    public static IEnumerable<(char a, char b)> XYXs(this string str)
    {
        Regex regex = new Regex(@"(.)((?:(?!\1).))\1");
        var match = regex.Match(str);
        while (match.Success)
        {
            yield return (match.Value[0], match.Value[1]);
            match = regex.Match(str, match.Index + 1);
        }
    }
}