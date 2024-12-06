var map = File.ReadAllLines(@"..\..\..\..\day-06.txt")
    .Select(line => line.ToArray()).ToArray();

//Find the starting position
var start = new Coordinate(0, 0);
for (int y = 0; y < map.Length; y++)
    for (int x = 0; x < map[0].Length; x++)
        if (map[y][x] == '^')
            start = new Coordinate(x, y);

//Iterate all possible obstacle positions
var infiniteLoops = 0;
for (int y = 0; y < map.Length; y++)
    for (int x = 0; x < map[0].Length; x++)
    {
        var newMap = map.CloneMap();
        newMap[y][x] = '#';
        
        if (NavigateMap(start, newMap).isInfiniteLoop)
            infiniteLoops++;
    }

Console.WriteLine($"Part 1 - {NavigateMap(start, map).map.CountSteps()}");
Console.WriteLine($"Part 2 - {infiniteLoops}");
Console.ReadLine();

static (char[][] map, bool isInfiniteLoop) NavigateMap(Coordinate start, char[][] map)
{
    var maxIterations = map.Length * map[0].Length;
    var iterations = 0;
    var current = start;
    var direction = new Coordinate(0, -1);
    do
    {
        map[current.Y][current.X] = 'X';
        var newPos = current.Move(direction);
        if (newPos.InsideMap(map) && map[newPos.Y][newPos.X] == '#')
        {
            direction = direction.Turn();
        }
        else
        {
            current = newPos;
            iterations++;
        }
    } while (current.InsideMap(map) && iterations < maxIterations);
    return (map, iterations == maxIterations);
}

record Coordinate(int x, int y)
{
    public int X => x;
    public int Y => y;

    public Coordinate Turn()
    {
        return x switch
        {
            0 => y switch
            {
                -1 => new Coordinate(1, 0),
                _ => new Coordinate(-1, 0)
            },
            1 => new Coordinate(0, 1),
            _ => new Coordinate(0, -1)
        };
    }

    public Coordinate Move(Coordinate direction) => new Coordinate(x + direction.x, y + direction.y);
    public bool InsideMap(char[][] map) => x >= 0 && x < map[0].Length && y >= 0 && y < map.Length;
}

public static class Extensions
{
    public static char[][] CloneMap(this char[][] map)
    {
        return map.Select(x => x.ToArray()).ToArray();
    }

    public static int CountSteps(this char[][] map) => map.Sum(column => column.Count(x => x == 'X'));
}