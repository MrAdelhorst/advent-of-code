namespace ConsoleApp1
{
    internal class Program
    {
        static List<Coord> coordinates = new List<Coord>();
        static void Main(string[] args)
        {
            foreach (var coordinate in File.ReadAllLines(@"c:\data\aoc\18\aoc18-1.txt"))
            {
                var parts = coordinate.Split(',');
                coordinates.Add(new Coord(int.Parse(parts[0])+1, int.Parse(parts[1])+1, int.Parse(parts[2])+1));
            }

            //Puzzle 2
            var maxX = coordinates.Max(c => c.X);
            var maxY = coordinates.Max(c => c.Y);
            var maxZ = coordinates.Max(c => c.Z);
            var grid = new char[maxX + 2, maxY + 2, maxZ + 2];
            foreach (var coordinate in coordinates)
                grid[coordinate.X, coordinate.Y, coordinate.Z] = 'X';

            var count = 0;
            var toSearch = new Queue<Coord>();
            toSearch.Enqueue(new Coord(0, 0, 0));
            while (toSearch.Any())
            {
                var current = toSearch.Dequeue();
                if (current.X >= 0 && current.X <= maxX+1 &&
                    current.Y >= 0 && current.Y <= maxY+1 &&
                    current.Z >= 0 && current.Z <= maxZ+1)
                {
                    var currentGridEntry = grid[current.X, current.Y, current.Z];
                    if (currentGridEntry == 'X')
                        count++;
                    else if (currentGridEntry == '\0')
                    {
                        grid[current.X, current.Y, current.Z] = '+';
                        toSearch.Enqueue(new Coord(current.X + 1, current.Y, current.Z));
                        toSearch.Enqueue(new Coord(current.X - 1, current.Y, current.Z));
                        toSearch.Enqueue(new Coord(current.X, current.Y + 1, current.Z));
                        toSearch.Enqueue(new Coord(current.X, current.Y - 1, current.Z));
                        toSearch.Enqueue(new Coord(current.X, current.Y, current.Z + 1));
                        toSearch.Enqueue(new Coord(current.X, current.Y, current.Z - 1));
                    }
                }
            }
            Console.WriteLine($"Puzzle 2: surface area: {count}");

            //Puzzle 1
            //var sides = 0;
            //foreach (var coordinate in coordinates)
            //{
            //    if (!coordinates.Any(c => c.X == coordinate.X + 1 && c.Y == coordinate.Y && c.Z == coordinate.Z))
            //        sides++;
            //    if (!coordinates.Any(c => c.X == coordinate.X - 1 && c.Y == coordinate.Y && c.Z == coordinate.Z))
            //        sides++;
            //    if (!coordinates.Any(c => c.X == coordinate.X && c.Y == coordinate.Y + 1 && c.Z == coordinate.Z))
            //        sides++;
            //    if (!coordinates.Any(c => c.X == coordinate.X && c.Y == coordinate.Y - 1 && c.Z == coordinate.Z))
            //        sides++;
            //    if (!coordinates.Any(c => c.X == coordinate.X && c.Y == coordinate.Y && c.Z == coordinate.Z + 1))
            //        sides++;
            //    if (!coordinates.Any(c => c.X == coordinate.X && c.Y == coordinate.Y && c.Z == coordinate.Z - 1))
            //        sides++;
            //}
            //Console.WriteLine($"Puzzle 1: Sides visible: {sides}");

            Console.ReadLine();
        }

        internal struct Coord
        {
            public int X; 
            public int Y;
            public int Z;
            public Coord(int x, int y, int z) { this.X = x; this.Y = y; this.Z = z; }
        }
    }
}