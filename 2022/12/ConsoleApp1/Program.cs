namespace ConsoleApp1
{
    internal class Program
    {
        internal static string[] map = File.ReadAllLines(@"c:\data\aoc\12\aoc12-1.txt");
        internal static Coord startPos;
        internal static Coord endPos;
        internal static int[,] distances = new int[map.First().Length, map.Length];
        static void Main(string[] args)
        {
            //Puzzle1.Run();
            Puzzle2.Run();
        }

        internal class Puzzle2
        {
            internal static void Run()
            {
                //Find start and end positions
                for (int row = 0; row < map.Length; row++)
                {
                    var idxE = map[row].IndexOf('E');
                    if (idxE != -1)
                        startPos = new Coord(idxE, row);
                }

                //Initialize distances structure and capture 'a's
                var lowPoints = new List<Coord>();
                for (int i = 0; i < map.First().Length; i++)
                    for (int j = 0; j < map.Length; j++)
                    {
                        distances[i, j] = int.MaxValue;
                        if (map[j][i] == 'a')
                            lowPoints.Add(new Coord(i, j));
                    }

                //Start traversing map
                var candidates = new Queue<Coord>();
                candidates.Enqueue(startPos);
                distances[startPos.X, startPos.Y] = 0;
                while (candidates.Any())
                {
                    var current = candidates.Dequeue();
                    var directions = new[] { current.Right(), current.Down(), current.Up(), current.Left() };
                    foreach (var direction in directions)
                    {
                        var currentDist = distances[current.X, current.Y];
                        if (IsValidPosition(direction, current) && currentDist < distances[direction.X, direction.Y] - 1)
                        {
                            distances[direction.X, direction.Y] = currentDist + 1;
                            candidates.Enqueue(direction);
                        }
                    }
                }

                var fewest = lowPoints.Select(p => distances[p.X, p.Y]).Min();

                Console.WriteLine($"Fewest steps: {fewest}");
                Console.ReadLine();
            }

            internal static bool IsValidPosition(Coord newPos, Coord previousPos)
            {
                if (newPos.X < 0 || newPos.Y < 0 || newPos.X >= map.First().Length || newPos.Y >= map.Length)
                    return false;

                var currentElevation = map[previousPos.Y][previousPos.X] switch
                {
                    'S' => 'a',
                    'E' => 'z',
                    _ => map[previousPos.Y][previousPos.X]
                };
                var newElevation = map[newPos.Y][newPos.X] switch
                {
                    'S' => 'a',
                    'E' => 'z',
                    _ => map[newPos.Y][newPos.X]
                };

                if (newElevation - currentElevation > -2)
                    return true;
                else
                    return false;
            }
        }

        internal class Puzzle1
        {
            internal static void Run()
            {
                //Find start and end positions
                for (int row = 0; row < map.Length; row++)
                {
                    var idxS = map[row].IndexOf('S');
                    if (idxS != -1)
                        startPos = new Coord(idxS, row);
                    var idxE = map[row].IndexOf('E');
                    if (idxE != -1)
                        endPos = new Coord(idxE, row);
                }

                //Initialize distances structure
                for (int i = 0; i < map.First().Length; i++)
                    for (int j = 0; j < map.Length; j++)
                        distances[i, j] = int.MaxValue;

                //Start traversing map
                var candidates = new Queue<Coord>();
                candidates.Enqueue(startPos);
                distances[startPos.X, startPos.Y] = 0;
                while (candidates.Any())
                {
                    var current = candidates.Dequeue();
                    var directions = new[] { current.Right(), current.Down(), current.Up(), current.Left() };
                    foreach (var direction in directions)
                    {
                        var currentDist = distances[current.X, current.Y];
                        if (IsValidPosition(direction, current) && currentDist < distances[direction.X, direction.Y] - 1)
                        {
                            distances[direction.X, direction.Y] = currentDist + 1;
                            candidates.Enqueue(direction);
                        }
                    }
                }

                Console.WriteLine($"Fewest steps: {distances[endPos.X, endPos.Y]}");
                Console.ReadLine();
            }

            internal static bool IsValidPosition(Coord newPos, Coord previousPos)
            {
                if (newPos.X < 0 || newPos.Y < 0 || newPos.X >= map.First().Length || newPos.Y >= map.Length)
                    return false;

                var currentElevation = map[previousPos.Y][previousPos.X] switch
                {
                    'S' => 'a',
                    'E' => 'z',
                    _ => map[previousPos.Y][previousPos.X]
                };
                var newElevation = map[newPos.Y][newPos.X] switch
                {
                    'S' => 'a',
                    'E' => 'z',
                    _ => map[newPos.Y][newPos.X]
                };

                if (newElevation - currentElevation < 2)
                    return true;
                else
                    return false;
            }
        }

        internal struct Coord
        {
            public int X;
            public int Y;
            public Coord(int x, int y) { this.X = x; this.Y = y; }
            public Coord Up() => new Coord(X, Y - 1);
            public Coord Down() => new Coord(X, Y + 1);
            public Coord Left() => new Coord(X-1, Y);
            public Coord Right() => new Coord(X+1, Y);
        }
    }
}