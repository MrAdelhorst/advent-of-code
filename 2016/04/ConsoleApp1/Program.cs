using System.Text.RegularExpressions;

var regex = new Regex(@"([a-z-]+)-(\d+)\[(\w+)\]");
var rooms = File.ReadAllLines(@"..\..\..\..\day-04.txt").Select<string, (string name, int sectorId, string checksum)>(line =>
{
    var parts = regex.Match(line);
    return (parts.Groups[1].Value, int.Parse(parts.Groups[2].Value), parts.Groups[3].Value);
});

var validRooms = rooms.Where(room => room.name.CalculateChecksum() == room.checksum);
var decryptedRooms = validRooms.Select(room => (name: string.Join("", room.name.Select(c => c == '-' ? ' ' : c.Shift(room.sectorId))), room.sectorId));
var northPoleRoom = decryptedRooms.Single(r => r.name.Contains("north") && r.name.Contains("pole"));

Console.WriteLine($"Part 1 - Sum of sector IDs: {validRooms.Sum(r => r.sectorId)}");
Console.WriteLine($"Part 2 - North Pole room sector ID: {northPoleRoom.sectorId}");
Console.ReadLine();

static class Extensions
{
    public static string CalculateChecksum(this string input)
    {
        var sortedLetters = input.Where(c => c != '-').GroupBy(c => c).OrderByDescending(g => g.Count()).ThenBy(g => g.Key);
        return string.Join("", sortedLetters.Take(5).Select(g => g.Key));
    }

    //Shifts any lowercase letter 'right', wraps around from 'z' to 'a'
    public static char Shift(this char c, int offset) => (char) ('a' + (c - 'a' + offset) % 26);
}