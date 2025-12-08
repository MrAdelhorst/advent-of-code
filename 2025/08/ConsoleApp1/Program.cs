var boxes = (await File.ReadAllLinesAsync(@"..\..\..\..\day-08.txt"))
    .Select(line => line.Split(',').Select(long.Parse).ToArray())
    .Select(coord => new Point(coord[0], coord[1], coord[2]))
    .ToList();

//Generate all distances
var distances = new HashSet<(double distance, Point a, Point b)>();
for (int i=0; i < boxes.Count; i++)
    for (int j = i + 1; j < boxes.Count; j++)
        distances.Add((EuclideanProduct(boxes[i], boxes[j]), boxes[i], boxes[j]));

//Connect circuits
var orderedDistances = distances.OrderBy(d => d.distance).ToList();
var circuits = new List<List<Point>>();
long part1 = 0, part2 = 0;
for (int i=0;; i++)
{
    var (_, a, b) = orderedDistances[i];
    var existing = circuits.Where(c => c.Contains(a) || c.Contains(b)).ToList() ?? [];
    foreach (var circuit in existing)
        circuits.Remove(circuit);
    circuits.Add([..existing.SelectMany(c => c).Union([a, b]).Distinct()]);

    //Part 1 finished
    if (i == 999)
    {
        part1 = circuits
            .OrderByDescending(c => c.Count).Take(3)
            .Aggregate(1, (acc, x) => acc * x.Count);
    }
    //Part 2 finished
    if (circuits.Count == 1 && circuits[0].Count == boxes.Count)
    {
        part2 = a.X * b.X;
        break;
    }
}
static double EuclideanProduct(Point a, Point b) => Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2) + Math.Pow(a.Z - b.Z, 2));

Console.WriteLine($"Part 1 - Circuit product: {part1}");
Console.WriteLine($"Part 2 - Last connection: {part2}");
Console.ReadLine();

record Point(long X, long Y, long Z);