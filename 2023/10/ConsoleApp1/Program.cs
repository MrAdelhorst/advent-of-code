//Parse input
var originalMap = File.ReadAllLines(@"..\..\..\..\day-10.txt").Select(line => line.ToArray()).ToArray();

//Part 1
var map1 = originalMap.DeepClone();
//Find start
Coordinate start = default;
for (int y = 0; y < map1.Length; y++)
    for (int x = 0; x < map1.First().Length; x++)
        if (map1[y][x] == 'S')
            start = new Coordinate(x, y);

//Traverse
var validDirections = new[]{ Direction.East, Direction.West, Direction.North, Direction.South }.Where(c => map1.IsValidMove(start, c)).Select(d => start.MoveTo(d)).ToArray();
var current = validDirections.First();
var distance = 1;
while (current != validDirections.Last())
{
    var nextDirection = current.MoveTo(new[] { Direction.East, Direction.West, Direction.North, Direction.South }.Single(c => map1.IsValidMove(current, c)));
    map1[current.Y][current.X] = '*';
    distance++;
    current = nextDirection;
}
map1[current.Y][current.X] = '*';
map1[start.Y][start.X] = '*';

//Part 2
//Initialize everything that's not the main loop to '.'
var map2 = map1.DeepClone();
for (int y = 0; y < map2.Length; y++)
    for (int x = 0; x < map2.First().Length; x++)
        if (map1[y][x] != '*')
            map2[y][x] = '.';

//'Zoom' the map to make the below fill operation able to pass between pipes
var zoomedMap = new char[map2.Length * 2 - 1][];
for (int y = 0; y < map2.Length * 2 - 1; y++)
{
    zoomedMap[y] = new char[map2.First().Length * 2 - 1];
    for (int x = 0; x < map2.First().Length * 2 - 1; x++)
    {
        if (y % 2 == 0)
        {
            if (x % 2 == 0)
                zoomedMap[y][x] = map2[y / 2][x / 2];
            else
                zoomedMap[y][x] = (originalMap.IsConnectedHorizontally(new Coordinate((x-1) / 2, y / 2), new Coordinate((x-1) / 2 + 1, y / 2))) ? '*' : '´';
        }
        else
        {
            if (x % 2 == 0)
                zoomedMap[y][x] = (originalMap.IsConnectedVertically(new Coordinate(x / 2, (y-1) / 2), new Coordinate(x / 2, (y-1) / 2 + 1))) ? '*' : '´';
            else
                zoomedMap[y][x] = '´';
        }
    }
}

//Add surrounding border of '.'s to compensate for pipes edging to the border
var lastX = zoomedMap.First().Length - 1;
var lastY = zoomedMap.Length - 1;
var emptyLine = new string('.', zoomedMap.First().Length + 2);
zoomedMap = zoomedMap.SelectMany((line, idx) =>
{
    var newLine = line.SelectMany((sym, ix) =>
        (ix == 0) ? new[] { '.', sym } : (ix == lastX) ? new[] { sym, '.' } : new[] { sym }).ToArray();
    return (idx == 0) ? new[] { emptyLine.ToArray(), newLine } : (idx == lastY) ? new[] { newLine, emptyLine.ToArray() } : new[] { newLine };
}).ToArray();

//Fill all connected spaces with ' '
var queue = new Queue<Coordinate>([new Coordinate(0, 0)]);
while (queue.Any())
{
    var location = queue.Dequeue();
    zoomedMap[location.Y][location.X] = ' ';
    foreach (var adjacent in new[] { location.East, location.West, location.North, location.South }.Where(a => zoomedMap.CanMoveTo(a) && !queue.Contains(a)))
        queue.Enqueue(adjacent);
}

//Print results
Console.WriteLine($"Part 1 - Furthest distance: {distance / 2 + 1}");
Console.WriteLine($"Part 2 - Enclosed spaces: {zoomedMap.Sum(line => line.Count(c => c == '.'))}");
Console.ReadLine();

enum Direction { North, South, East, West }

record Coordinate(int X, int Y)
{
    public Coordinate East => new Coordinate(X + 1, Y);
    public Coordinate West => new Coordinate(X - 1, Y);
    public Coordinate North => new Coordinate(X, Y - 1);
    public Coordinate South => new Coordinate(X, Y + 1);
    public Coordinate MoveTo (Direction to)
    {
        return to switch
        {
            Direction.East => East,
            Direction.West => West,
            Direction.North => North,
            Direction.South => South,
            _ => throw new InvalidOperationException("Illegal direction")
        };
    }
}

static class Extensions
{
    public static Direction Opposite(this Direction direction)
    {
        return direction switch
        {
            Direction.East => Direction.West,
            Direction.West => Direction.East,
            Direction.North => Direction.South,
            Direction.South => Direction.North,
            _ => throw new InvalidOperationException("Illegal direction")
        };
    }

    public static bool IsValidMove(this char[][] map, Coordinate from, Direction to)
    {
        var newLocation = from.MoveTo(to);
        if (newLocation.X < 0 || newLocation.X >= map.First().Length || newLocation.Y < 0 || newLocation.Y >= map.Length)
            return false;

        var fromSymbol = map[from.Y][from.X];
        var toSymbol = map[newLocation.Y][newLocation.X];
        var validConnections = new[]
        {
        new { direction = Direction.North, validDestinations = new[] { '|', '7', 'F' } },
        new { direction = Direction.South, validDestinations = new[] { '|', 'L', 'J' } },
        new { direction = Direction.East, validDestinations = new[] { '-', '7', 'J' } },
        new { direction = Direction.West, validDestinations = new[] { '-', 'L', 'F' } },
    };

        if (fromSymbol == 'S')
        {
            return validConnections.Single(c => c.direction == to).validDestinations.Contains(toSymbol);
        }
        else
        {
            return validConnections.Single(c => c.direction == to).validDestinations.Contains(toSymbol) &&
                    validConnections.Single(c => c.direction == to.Opposite()).validDestinations.Contains(fromSymbol);
        }
    }

    public static char[][] DeepClone(this char[][] map)
    {
        return map.Select(line => line.Clone() as char[]).ToArray();
    }

    public static bool CanMoveTo(this char[][] map, Coordinate newLocation)
    {
        if (newLocation.X < 0 || newLocation.X >= map.First().Length || newLocation.Y < 0 || newLocation.Y >= map.Length)
            return false;

        var content = map[newLocation.Y][newLocation.X];
        return  content == '.' || content == '´';
    }

    static public bool IsConnectedHorizontally(this char[][] map, Coordinate coordinateLeft, Coordinate coordinateRight)
    {
        if (coordinateRight.X >= map.First().Length)
            return false;

        var symbolLeft = map[coordinateLeft.Y][coordinateLeft.X];
        var SymbolRight = map[coordinateRight.Y][coordinateRight.X];

        return new[]{ 'S', '-', 'L', 'F' }.Contains(symbolLeft) && new[]{ 'S', '7', 'J', '-' }.Contains(SymbolRight);
    }
    static public bool IsConnectedVertically(this char[][] map, Coordinate coordinateTop, Coordinate coordinateBottom)
    {
        if (coordinateBottom.Y >= map.Length)
            return false;

        var symbolTop = map[coordinateTop.Y][coordinateTop.X];
        var SymbolBottom = map[coordinateBottom.Y][coordinateBottom.X];

        return new[] { 'S', '|', '7', 'F' }.Contains(symbolTop) && new[] { 'S', '|', 'J', 'L' }.Contains(SymbolBottom);
    }
}