using System.Text;

//Parse input
var input = File.ReadAllText(@"..\..\..\..\day-15.txt").Split(',');

//Part 1
var sum = input.Sum(GetHash);

//Part 2
var boxes = new Dictionary<int, List<string>>();
foreach (var command in input)
{
    if (command.Last() == '-')
    {
        var label = command.TrimEnd('-');
        var box = GetHash(label);
        if (boxes.TryGetValue(box, out var lenses))
        {
            var item = lenses.FirstOrDefault(l => l.StartsWith(label));
            if (item != default)
                lenses.Remove(item);
        }
    }
    else
    {
        var parts = command.Split('=');
        var box = GetHash(parts[0]);
        if (boxes.TryGetValue(box, out var lenses))
        {
            var item = lenses.FirstOrDefault(l => l.StartsWith(parts[0]));
            if (item != default)
            {
                var idx = lenses.IndexOf(item);
                lenses.RemoveAt(idx);
                lenses.Insert(idx, command);
            }
            else
            {
                lenses.Add(command);
            }
        }
        else
        {
            boxes.Add(box, new List<string>([command]));
        }
    }
}
var sum2 = boxes.Sum(box => box.Value.Select((lens, idx) => (box.Key + 1) * (idx + 1) * int.Parse([lens.Last()])).Sum());

Console.WriteLine($"Part 1 - Sum: {sum}");
Console.WriteLine($"Part 2 - Sum: {sum2}");
Console.ReadLine();

int GetHash(string input)
{
    var currentValue = 0;
    foreach (var character in Encoding.ASCII.GetBytes(input))
    {
        currentValue = ((currentValue + character) * 17) % 256;
    }
    return currentValue;
}