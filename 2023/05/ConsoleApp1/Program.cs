//Parse seeds from input
var lines = File.ReadAllLines(@"..\..\..\..\day-05.txt");
var seedPoints = lines[0].Split(": ").Last().Split(" ").Select(long.Parse).ToArray();
var seedVectors = new List<(long position, long length)>();
for (int i = 0; i < seedPoints.Count(); i += 2)
    seedVectors.Add((seedPoints[i], seedPoints[i + 1]));

//Parse maps from input
var maps = new Dictionary<int, List<(long destinationPosition, long sourcePosition, long length)>>();
var nextMapNo = 0;
List<(long destinationPosition, long sourcePosition, long length)> currentMap = [];
for (int i=1; i < lines.Length; i++)
{
    if (lines[i] == "")
    {
        maps.Add(nextMapNo++, currentMap = []);
        i++;
    }
    else
    {
        var parts = lines[i].Split(" ").Select(long.Parse).ToArray();
        currentMap.Add(new (parts[0], parts[1], parts[2]));
    }
}

//Execute the mappings and find the closest location
Console.WriteLine($"Part 1 - closest location:{seedPoints.Min(seed => ApplyMapToPoint(seed, 0))}");
Console.WriteLine($"Part 2 - closest location:{ApplyMapToVectors(seedVectors, 0).Min(v => v.position)}");

long ApplyMapToPoint(long point, int mapNo)
{
    //The last map has been processed - return the result
    if (!maps.ContainsKey(mapNo))
        return point;

    //Map the point to destination coordinates
    var map = maps[mapNo];
    var match = map.SingleOrDefault(e => e.sourcePosition <= point && point <= e.sourcePosition + e.length - 1);

    //Execute the next map
    return (match != default) ? ApplyMapToPoint(match.destinationPosition + (point - match.sourcePosition), mapNo + 1) : ApplyMapToPoint(point, mapNo + 1);
}

IEnumerable<(long position, long length)> ApplyMapToVectors(IEnumerable<(long position, long length)> vectors, int mapNo)
{
    //The last map has been processed - return the result
    if (!maps.ContainsKey(mapNo))
        return vectors;

    //Map each vector to destination coordinates
    var mappedResult = new List<(long position, long length)>();
    var map = maps[mapNo];
    foreach (var vector in vectors)
    {
        var current = vector;
        while (current != default)
        {
            //Find match for the start of the vector
            var match = map.SingleOrDefault(e => e.sourcePosition <= current.position && current.position <= e.sourcePosition + e.length - 1);
            if (match == default)
            {
                //No transformation for this point, so find out how long the 'null vector' is
                match = map.FindNullVector(current.position);
            }
            var offset = (current.position - match.sourcePosition);
            if (current.length <= match.length - offset)
            {
                //The current vector is completely covered
                mappedResult.Add((match.destinationPosition + offset, current.length));
                current = default;
            }
            else
            {
                //Store the part that's covered and create a new vector for the remainder
                mappedResult.Add((match.destinationPosition + offset, match.length - offset));
                current = (current.position + match.length - offset, current.length - (match.length - offset));
            }
        }
    }

    //Execute the next map
    return ApplyMapToVectors(mappedResult, mapNo +1);
}

static class Extensions
{
    internal static (long destinationPosition, long sourcePosition, long length) FindNullVector(this List<(long destinationPosition, long sourcePosition, long length)> map, long position)
    {
        var vectorsBelow = map.Where(e => e.sourcePosition + e.length - 1 < position);
        var highestValueBelow = vectorsBelow.Any() ? vectorsBelow.Max(e => e.sourcePosition + e.length - 1) : -1;
        var vectorsAbove = map.Where(e => e.sourcePosition > position);
        var lowestValueAbove = vectorsAbove.Any() ? vectorsAbove.Min(e => e.Item2) : long.MaxValue - 1;
        return (highestValueBelow + 1, highestValueBelow + 1, lowestValueAbove - highestValueBelow - 1);
    }
}