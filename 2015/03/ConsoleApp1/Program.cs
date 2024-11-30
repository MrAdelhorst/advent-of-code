using Microsoft.Win32.SafeHandles;
using System.IO;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var posSanta = new Coord(0, 0);
            var posRoboSanta = new Coord(0, 0);
            bool santa = true;
            var uniquePositions = new HashSet<Coord>();
            uniquePositions.Add(posSanta);
            foreach (var move in File.ReadAllText(@"c:\data\aoc\2015\3\aoc3-1.txt"))
            {
                ref Coord pos = ref posSanta;
                if (!santa)
                    pos = ref posRoboSanta;
                santa = !santa;

                pos.x += move switch
                {
                    '<' => -1,
                    '>' => 1,
                    _ => 0
                };
                pos.y += move switch
                {
                    'v' => -1,
                    '^' => 1,
                    _ => 0
                };
                uniquePositions.Add(pos);
            }

            Console.WriteLine($"Houses visited: {uniquePositions.Count()}");
            Console.ReadLine();
        }

        internal struct Coord
        {
            public int x;
            public int y;

            public Coord(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public void AddToX(int delta) => x += delta;
            public void AddToY(int delta) => y += delta;
        }

    }
}