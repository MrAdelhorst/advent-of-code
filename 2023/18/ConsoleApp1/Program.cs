using ConsoleApp1;

IEnumerable<(char direction, int distance, string colour)> digOrders = File.ReadAllLines(@"..\..\..\..\day-18.txt").Select(line =>
{
    var parts = line.Split(' ');
    return (parts[0].Single(), int.Parse(parts[1]), parts[2]);
});

var horizontal = digOrders.Where(o => o.direction == 'R' || o.direction == 'L');
int minX = 0, maxX = 0, currentX = 0;
foreach (var (direction, distance, _) in horizontal)
{
    currentX += (direction == 'R') ? distance : -distance;
    maxX = Math.Max(maxX, currentX);
    minX = Math.Min(minX, currentX);
}
var width = maxX - minX + 1;

var vertical = digOrders.Where(o => o.direction == 'U' || o.direction == 'D');
int minY = 0, maxY = 0, currentY = 0;
foreach (var (direction, distance, _) in vertical)
{
    currentY += (direction == 'D') ? distance : -distance;
    maxY = Math.Max(maxY, currentY);
    minY = Math.Min(minY, currentY);
}
var height = maxY - minY + 1;

var start = new Point(0 - minX, 0 - minY);

Console.WriteLine($"Part 1 - Area: {Part1.Run(digOrders, width, height, start)}");
Console.WriteLine($"Part 2 - Area: {Part2.Run(digOrders, width, height, start)}");
Console.ReadLine();