var map = File.ReadAllLines(@"..\..\..\..\day-12.txt")
    .Select(row => row.ToArray())
    .ToArray();

var regions = new List<(Coordinate[] region, char crop)>();
for (int y = 0; y < map.Length; y++)
    for (int x = 0; x < map[y].Length; x++)
        if (map[y][x] != '.')
            regions.Add(FindRegion(map, new Coordinate(x, y)));

Console.WriteLine($"Part 1 - Price: {regions.Sum(r => r.region.Perimeter() * r.region.Area())}");
Console.WriteLine($"Part 2 - Price: {regions.Sum(r => r.region.Sides() * r.region.Area())}");
Console.ReadLine();

(Coordinate[] region, char crop) FindRegion(char[][] map, Coordinate position)
{
    var crop = map[position.Y][position.X];
    var region = new List<Coordinate>();
    var jobs = new Queue<Coordinate>([position]);
    while (jobs.Count > 0)
    {
        var current = jobs.Dequeue();
        if (map[current.Y][current.X] == crop)
        {
            map[current.Y][current.X] = '.';
            region.Add(current);
            foreach (var neighbour in current.Neighbours().Where(x => x.InBounds(map)))
                jobs.Enqueue(neighbour);
        }
    }

    return (region.ToArray(), crop);
}

internal static class Extensions
{
    public static int Perimeter(this Coordinate[] region)
    {
        return region.Sum(plot => plot.Neighbours().Count(x => !region.Contains(x)));
    }

    public static int Area(this Coordinate[] region)
    {
        return region.Length;
    }

    public static int Sides(this Coordinate[] region)
    {
        //Process outer borders
        var start = region.First(x => !region.Contains(x.North()) && !region.Contains(x.West()));
        var (sides, borders) = region.TraverseEdge(start, Direction.East, innerBorder: false);

        //Process inner borders
        foreach (var crop in region)
        {
            var innerCrops = crop.Neighbours().Where(n => !region.Contains(n) && !borders.Contains(n));
            foreach (var inner in innerCrops)
            {
                if (!borders.Contains(inner)) //Could have been added when processing one of the other inners
                {
                    var direction = region.Contains(inner.East()) 
                        ? Direction.South 
                        : region.Contains(inner.South())
                            ? Direction.West
                            : region.Contains(inner.West())
                                ? Direction.North
                                : Direction.East;

                    var (innerSides, innerBorders) = region.TraverseEdge(inner, direction, innerBorder: true);
                    sides += innerSides;
                    borders.AddRange(innerBorders);
                }
            }
        }

        return sides;
    }

    private static (int sides, List<Coordinate> borders) TraverseEdge(this Coordinate[] region, Coordinate startPosition, Direction startDirection, bool innerBorder)
    {
        var borders = new List<Coordinate>();
        var direction = startDirection;
        var position = startPosition;
        var sides = 0;
        do
        {
            borders.Add(innerBorder ? position : position.Left(direction));
            var nextPosition = position.Next(direction);
            if (region.Contains(nextPosition) == innerBorder)
            {
                direction = direction.TurnRight();
                sides++;
            }
            else if (region.Contains(nextPosition.Left(direction)) != innerBorder)
            {
                position = nextPosition.Left(direction);
                direction = direction.TurnLeft();
                sides++;
            }
            else
            {
                position = nextPosition;
            }
        } while (position != startPosition || direction != startDirection);

        return (sides, borders);
    }

    public static Direction TurnLeft(this Direction direction)
    {
        return direction switch
        {
            Direction.East => Direction.North,
            Direction.South => Direction.East,
            Direction.West => Direction.South,
            Direction.North => Direction.West,
            _ => throw new NotImplementedException()
        };
    }

    public static Direction TurnRight(this Direction direction)
    {
        return direction switch
        {
            Direction.East => Direction.South,
            Direction.South => Direction.West,
            Direction.West => Direction.North,
            Direction.North => Direction.East,
            _ => throw new NotImplementedException()
        };
    }
}

record Coordinate(int X, int Y)
{
    internal IEnumerable<Coordinate> Neighbours()
    {
        yield return North();
        yield return South();
        yield return West();
        yield return East();
    }

    internal Coordinate North() => new Coordinate(X, Y - 1);
    internal Coordinate South() => new Coordinate(X, Y + 1);
    internal Coordinate West() => new Coordinate(X - 1, Y);
    internal Coordinate East() => new Coordinate(X + 1, Y);

    internal bool InBounds(char[][] map)
    {
        return X >= 0 && X < map[0].Length && Y >= 0 && Y < map.Length;
    }

    internal Coordinate Next(Direction direction)
    {
        return direction switch
        {
            Direction.East => East(),
            Direction.South => South(),
            Direction.West => West(),
            Direction.North => North(),
            _ => throw new NotImplementedException()
        };
    }

    internal Coordinate Left(Direction direction)
    {
        return direction switch
        {
            Direction.East => North(),
            Direction.South => East(),
            Direction.West => South(),
            Direction.North => West(),
            _ => throw new NotImplementedException()
        };
    }
}

enum Direction
{
    East,
    South,
    West,
    North
}