using System.Security.Cryptography;
using static ConsoleApp1.Program;

namespace ConsoleApp1
{
    internal class Program
    {
        static char[,] map;
        static int xMin, xMax, yMax;
        static void Main(string[] args)
        {
            var rockVectors = new List<List<Coord>>();
            foreach (var line in File.ReadAllLines(@"c:\data\aoc\14\aoc14-1.txt"))
            {
                var currentVector = new List<Coord>();
                rockVectors.Add(currentVector);
                foreach (var coordinate in line.Split(" -> "))
                {
                    var parts = coordinate.Split(',');
                    currentVector.Add(new Coord(int.Parse(parts[0]), int.Parse(parts[1])));   
                };
            }
            var numberOfGrains = 0;

            //Puzzle 2
            xMin = rockVectors.Min(line => line.Min(coord => coord.X));
            xMax = rockVectors.Max(line => line.Max(coord => coord.X));
            yMax = rockVectors.Max(line => line.Max(coord => coord.Y));
            //Add infinite floor
            rockVectors.Add(new List<Coord>(new[] { new Coord(xMin - 1000, yMax + 2), new Coord(xMax + 1000, yMax + 2) }));
            xMin -= 1000;
            xMax += 1000;
            yMax += 2;
            map = new char[xMax - xMin + 1, yMax + 1];

            //Puzzle 1
            //xMin = rockVectors.Min(line => line.Min(coord => coord.X));
            //xMax = rockVectors.Max(line => line.Max(coord => coord.X));
            //yMax = rockVectors.Max(line => line.Max(coord => coord.Y));
            //map = new char[xMax - xMin + 1, yMax + 1];

            foreach (var vector in rockVectors)
            {
                Coord previous = vector.First();
                foreach (var coordinate in vector)
                {
                    DrawRock(previous, coordinate);
                    previous = coordinate;
                }
            }

            var startPosition = new Coord(500, 0);
            while (TryDropSand(startPosition))
                numberOfGrains++;

            Console.WriteLine($"Grains of sand dropped: {numberOfGrains}");
            Console.ReadLine();
        }

        private static bool TryDropSand(Coord startPosition)
        {
            if (!LegalPosition(startPosition))
                return false;

            var position = startPosition;
            var previousPosition = position;
            map[position.X - xMin, position.Y] = 'O';
            do
            {
                previousPosition = position;
                //PrintMap();
                var directionPriorities = new[] { position.Down(), position.Left(), position.Right() };
                foreach (var direction in directionPriorities)
                {
                    if (LegalPosition(direction))
                    {
                        position = direction;
                        MoveSand(previousPosition, position);
                        break;
                    }
                }
            } while (!position.Equals(previousPosition) && position.X >= xMin && position.X <= xMax);

            if (position.X < xMin || position.X > xMax)
                return false;
            else 
                return true;
        }

        private static void PrintMap()
        {
            for (int j = 0; j < yMax + 1; j++)
            {
                for (int i = 0; i <= xMax - xMin; i++)
                    Console.Write((map[i, j] == '\0') ? '.' : map[i,j]);

                Console.Write('\n');
            }
        }

        private static bool LegalPosition(Coord coord)
        {
            if (coord.X < xMin || coord.X > xMax) 
                return true;

            return map[coord.X - xMin, coord.Y] == '\0';
        }

        private static void DrawRock(Coord from, Coord to)
        {
            if (from.X == to.X)
                for (int i = Math.Min(from.Y, to.Y); i <= Math.Max(from.Y, to.Y); i++)
                    map[from.X - xMin, i] = '#';
            else
            {
                if (from.Y != to.Y)
                    throw new InvalidOperationException("Only straight lines supported!");

                for (int i = Math.Min(from.X, to.X); i <= Math.Max(from.X, to.X); i++)
                    map[i - xMin, from.Y] = '#';
            }
        }

        private static void MoveSand(Coord previous, Coord current)
        {
            if (current.X >= xMin && current.X <= xMax)
            {
                map[previous.X - xMin, previous.Y] = '\0';
                map[current.X - xMin, current.Y] = 'O';
            }
        }

        internal struct Coord
        {
            public int X;
            public int Y;
            public Coord(int x, int y) { this.X = x; this.Y = y; }
            public Coord Down() => new Coord(X, Y + 1);
            public Coord Left() => new Coord(X - 1, Y + 1);
            public Coord Right() => new Coord(X + 1, Y + 1);
            public bool Equals(Coord other) => this.X == other.X && this.Y == other.Y;
        }
    }
}