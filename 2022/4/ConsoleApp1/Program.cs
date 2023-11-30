using System;
using System.IO;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var result = Puzzle2();

            Console.WriteLine($"Total matches: {result}");
            Console.ReadLine();
        }

        private static int Puzzle2()
        {
            int total = 0;

            foreach (var line in File.ReadAllLines(@"c:\data\aoc\4\aoc4-1.txt"))
            {
                var pair = line.Trim().Split(',');
                var pairOneMin = Math.Min(pair[0].Lower(), pair[0].Upper());
                var pairOneMax = Math.Max(pair[0].Lower(), pair[0].Upper());
                var pairTwoMin = Math.Min(pair[1].Lower(), pair[1].Upper());
                var pairTwoMax = Math.Max(pair[1].Lower(), pair[1].Upper());

                if (!(pairOneMin > pairTwoMax || pairOneMax < pairTwoMin))
                    total++;
            }

            return total;
        }
        private static int Puzzle1()
        {
            int total = 0;

            foreach (var line in File.ReadAllLines(@"c:\data\aoc\4\aoc4-1.txt"))
            {
                var pair = line.Trim().Split(',');
                var lowerDiff = pair[0].Lower() - pair[1].Lower();
                var upperDiff = pair[0].Upper() - pair[1].Upper();

                if (lowerDiff <= 0 && upperDiff >= 0 ||
                    lowerDiff >= 0 && upperDiff <= 0)
                    total++;
            }

            return total;
        }
    }

    static class RangeExtensions
    {
        public static int Lower(this string coord)
        {
            return int.Parse(coord.Split('-')[0]);
        }
        public static int Upper(this string coord)
        {
            return int.Parse(coord.Split('-')[1]);
        }
    }
}
