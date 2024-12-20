var test = new[]
{
"##########",
"#..O..O.O#",
"#......O.#",
"#.OO..O.O#",
"#..O@..O.#",
"#O#..O...#",
"#O..O..O.#",
"#.OO.O.OO#",
"#....O...#",
"##########",
"",
"<vv>^<v^>v>^vv^v>v<>v^v<v<^vv<<<^><<><>>v<vvv<>^v^>^<<<><<v<<<v^vv^v>^",
"vvv<<^>^v^^><<>>><>^<<><^vv^^<>vvv<>><^^v>^>vv<>v<<<<v<^v>^<^^>>>^<v<v",
"><>vv>v^v^<>><>>>><^^>vv>v<^^^>>v^v^<^^>v^^>v^<^v>v<>>v^v^<v>v^^<^^vv<",
"<<v<^>>^^^^>>>v^<>vvv^><v<<<>^^^vv^<vvv>^>v<^^^^v<>^>vvvv><>>v^<<^^^^^",
"^><^><>>><>^^<<^^v>>><^<v>^<vv>>v>>>^v><>^v><<<<v>>v<v<v>vvv>^<><<>^><",
"^>><>^v<><^vvv<^^<><v<<<<<><^v<<<><<<^^<v<^^^><^>>^<v^><<<^>>^v<v^v<v^",
">^>>^v>vv>^<<^v<>><<><<v<<v><>v<^vv<<<>^^v^>^^>>><<^v>>v^v><^^>>^<>vv^",
"<><^^>^^^<><vvvvv^v<v<<>^v<v>v<<^><<><<><<<^^<<<^<<>><<><^^^>^^<>^>v<>",
"^^>vv<^v^v<vv>^<><v<^v>^^^>>>^^vvv^>vvv<>>>^<^>>>>>^<<^v>^vvv<>^<><<v>",
"v^^>>><<^^<>>^v^<v^vv<>v^<<>^<^v^v><^<<<><<^<v><v<>vv>>v><v^<vv<>v^<<^",
};

var test1 = new[]
{
"########",
"#..O.O.#",
"##@.O..#",
"#...O..#",
"#.#.O..#",
"#...O..#",
"#......#",
"########",
"",
"<^^>>>vv<v>>v<<",
};

var test2 = new[]
{
"#######",
"#...#.#",
"#.....#",
"#..OO@#",
"#..O..#",
"#.....#",
"#######",
"",
"<vv<<^^<<^^",
};

var lines = File.ReadAllLines(@"..\..\..\..\day-15.txt");

var movements = new List<Direction>();
var mapLines = new List<string>();

bool isMovements = false;
foreach (var line in lines)
{
    if (string.IsNullOrEmpty(line))
        isMovements = true;
    else
    {
        if (isMovements)
        {
            movements.AddRange(line.Select(x => x switch
            {
                '^' => Direction.North,
                '>' => Direction.East,
                'v' => Direction.South,
                '<' => Direction.West,
                _ => throw new InvalidOperationException()
            }));
        }
        else
            mapLines.Add(line);
    }
}

Console.WriteLine($"Part 1 - GPS sum: {Part1.CalculateGpsSum(mapLines, movements)}");
Console.WriteLine($"Part 2 - GPS sum: {Part2.CalculateGpsSum(mapLines, movements)}");
Console.ReadLine();

public enum Direction
{
    North,
    East,
    South,
    West
}

record Coordinate(int X, int Y)
{
    public Coordinate East() => new Coordinate(X + 1, Y);
    public Coordinate West() => new Coordinate(X - 1, Y);
    public Coordinate North() => new Coordinate(X, Y - 1);
    public Coordinate South() => new Coordinate(X, Y + 1);
    public Coordinate Move(Direction direction) => direction switch
    {
        Direction.North => North(),
        Direction.East => East(),
        Direction.South => South(),
        Direction.West => West(),
        _ => throw new InvalidOperationException()
    };

    internal IEnumerable<Coordinate> ToBoxCoordinates(char[][] map)
    {
        return map[Y][X] switch
        {
            '[' => new[] { this, East() },
            ']' => new[] { this, West() },
        };
    }
}

internal static class Extensions
{
    public static Coordinate FindRobot(this char[][] map)
    {
        for (int y = 0; y < map.Length; y++)
            for (int x = 0; x < map[y].Length; x++)
                if (map[y][x] == '@')
                    return new Coordinate(x, y);
        
        throw new InvalidOperationException("Robot not found");
    }

    public static Direction Inverse(this Direction direction) => direction switch
    {
        Direction.North => Direction.South,
        Direction.East => Direction.West,
        Direction.South => Direction.North,
        Direction.West => Direction.East,
    };
}