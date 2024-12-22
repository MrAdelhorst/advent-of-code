using MoreLinq;
var codes = File.ReadAllLines(@"..\..\..\..\day-21.txt");

var numericKeypad = new char[4][]
{
    ['7', '8', '9'],
    ['4', '5', '6'],
    ['1', '2', '3'],
    ['-', '0', 'A']
};

var directionalKeypad = new char[2][]
{
    ['-', 'U', 'A'],
    ['L', 'D', 'R'],
};

var validKeys = directionalKeypad.SelectMany(x => x).Where(x => x != '-');
var directionalKeypadMap = validKeys.SelectMany(fromKey => validKeys.Select(toKey => (fromKey, toKey)))
    .ToDictionary(keys => keys, keys =>
    {
        var moves = directionalKeypad.Move(keys.fromKey.Position(directionalKeypad), keys.toKey.Position(directionalKeypad)).Select(x => x + "A");
        return (moves: moves, length: moves.First().Length);
    });

Console.WriteLine($"Part 1 - Sum of code complexities: {CalculateCodeComplexity(2)}");
Console.WriteLine($"Part 2 - Sum of code complexities: {CalculateCodeComplexity(25)}");
Console.ReadLine();

long CalculateCodeComplexity(int robotLayers)
{
    //Iterate each keypad once to find all possible sequences
    var firstLevel = codes.Select(numericKeypad.FindNumericPadSequences);
    var secondLevel = firstLevel.Select(directionalKeypad.FindDirectionalPadSequences);

    //Populate cache with all possible directionalKeyPad sequences
    var dirPadCache = new Dictionary<string, long>();
    foreach (var dirPadSequences in secondLevel)
        foreach (var sequence in dirPadSequences)
            dirPadCache.PopulateCache(sequence, directionalKeypadMap);

    //Assign initial cache values
    foreach (var entry in dirPadCache.Keys)
    {
        var current = 'A';
        int size = 0;
        foreach (var move in entry)
        {
            size += directionalKeypadMap[(current, move)].length;
            current = move;
        }
        dirPadCache[entry] = size;
    }

    //Iterate the remaining robot layers
    //Layer 1 is already embedded in the cache and the last layer will be added when calculating the sum
    for (int i = 0; i < robotLayers - 2; i++)
    {
        var prevCache = new Dictionary<string, long>(dirPadCache);
        foreach (var segment in dirPadCache.Keys)
        {
            dirPadCache[segment] = segment.Expand(directionalKeypadMap).Select(x => x.Segments().Sum(s => prevCache[s])).Min();
        }
    }

    //Calculate the sum of the code complexities
    long sum = 0;
    for (int i = 0; i < codes.Length; i++)
    {
        var code = int.Parse(codes[i].TrimEnd('A'));
        var lengths = secondLevel.ElementAt(i).Select(sequence => sequence.Segments().Sum(s => dirPadCache[s]));
        sum += code * lengths.Min();
    }

    return sum;
}

record Coordinate(int X, int Y);

internal static class Extensions
{
    public static Coordinate Position(this char key, char[][] keypad)
    {
        for (var y = 0; y < keypad.Length; y++)
            for (var x = 0; x < keypad[y].Length; x++)
                if (keypad[y][x] == key)
                    return new Coordinate(x, y);

        return new Coordinate(-1, -1);
    }

    public static IEnumerable<string> FindNumericPadSequences(this char[][] keypad, string code)
    {
        var sequences = new List<string>();

        var currentPosition = 'A'.Position(keypad);
        foreach (var key in code)
        {
            var desiredPosition = key.Position(keypad);
            if (sequences.Count == 0)
                sequences.AddRange(keypad.Move(currentPosition, desiredPosition));
            else
            {
                sequences = sequences.FullJoin(keypad.Move(currentPosition, desiredPosition),
                    _ => 1,
                    (x) => throw new NotImplementedException(),
                    (y) => throw new NotImplementedException(),
                    (x, y) => x + y
                    ).ToList();
            }
            currentPosition = desiredPosition;
            sequences = sequences.Select(x => x + "A").ToList();
        }

        return sequences;
    }

    public static IEnumerable<string> FindDirectionalPadSequences(this char[][] keypad, IEnumerable<string> downStreamSequences)
    {
        var result = downStreamSequences.SelectMany(keypad.FindNumericPadSequences);

        return result.Where(x => x.Length == result.Min(x => x.Length)).ToList();
    }

    public static IEnumerable<string> Move(this char[][] keypad, Coordinate from, Coordinate to)
    {
        var yKey = from.Y > to.Y ? 'U' : 'D';
        var yCount = Math.Abs(from.Y - to.Y);
        var yKeys = Enumerable.Repeat(yKey, yCount).Select((x, i) => (x, i)).ToArray();

        var xKey = from.X > to.X ? 'L' : 'R';
        var xCount = Math.Abs(from.X - to.X);
        var xKeys = Enumerable.Repeat(xKey, xCount).Select((x, i) => (x, i)).ToArray();

        //Find combinations
        var combinations = xKeys.Union(yKeys)
            .Permutations()
            .Select(x => new string(x.Select(y => y.x).ToArray()))
            .Distinct();

        //Skip sequences with invalid key
        var invalid = '-'.Position(keypad);
        if (to.X == invalid.X || to.Y == invalid.Y)
        {
            combinations = combinations.Where(x => !keypad.Move(from, x).Any(coord => coord == invalid));
        }

        return combinations
            .Select(x => new string(x)).ToList();
    }

    public static IEnumerable<Coordinate> Move(this char[][] keypad, Coordinate from, string moves)
    {
        var current = from;
        foreach (var move in moves)
        {
            current = move switch
            {
                'U' => new Coordinate(current.X, current.Y - 1),
                'D' => new Coordinate(current.X, current.Y + 1),
                'L' => new Coordinate(current.X - 1, current.Y),
                'R' => new Coordinate(current.X + 1, current.Y),
                _ => throw new NotImplementedException()
            };
            yield return current;
        }
    }

    public static void PopulateCache(this Dictionary<string, long> cache, string sequence, Dictionary<(char fromKey, char toKey), (IEnumerable<string> moves, int length)> keyPadMap)
    {
        var cacheMisses = sequence.Segments().Where(x => !cache.ContainsKey(x)).ToList();
        foreach (var segment in cacheMisses.Distinct())
            cache.Add(segment, 0);

        if (!cacheMisses.Any())
            return;

        foreach (var segment in cacheMisses)
        {
            foreach (var innerSequence in segment.Expand(keyPadMap))
            {
                cache.PopulateCache(innerSequence, keyPadMap);
            }
        }
    }

    public static IEnumerable<string> Segments(this string sequence)
    {
        return sequence.Segment((_, x, _) => x == 'A').Select(x => new string(x.ToArray()));
    }

    public static IEnumerable<string> Expand(this string segment, Dictionary<(char fromKey, char toKey), (IEnumerable<string> moves, int length)> keyPadMap)
    {
        var sequences = new List<string>();
        var current = 'A';
        foreach (var move in segment)
        {
            if (sequences.Count == 0)
                sequences.AddRange(keyPadMap[(current, move)].moves);
            else
            {
                sequences = sequences.FullJoin(keyPadMap[(current, move)].moves,
                    _ => 1,
                    (x) => throw new NotImplementedException(),
                    (y) => throw new NotImplementedException(),
                    (x, y) => x + y
                ).ToList();
            }
            current = move;
        }
        return sequences;
    }
}