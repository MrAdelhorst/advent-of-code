using System;
using System.IO;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var points = 0;
            foreach (var line in File.ReadAllLines(@"c:\data\aoc\2\aoc2-1.txt"))
            {
                var parts = line.TrimEnd().Split(' ');
                if (parts.Length != 2) throw new ArgumentException("Incorrectly formatted string");
                points += CalculatePoints(parts[0], parts[1]);
            }
            Console.Write($"Result: {points}");
            Console.ReadLine();
        }

        private static int CalculatePoints(string hisMove, string yourMove)
        {
            //Puzzle 1:
            //var combinations = new []
            //    {
            //        new  { his="A", yours="X", points=4},
            //        new  { his="A", yours="Y", points=8},
            //        new  { his="A", yours="Z", points=3},
            //        new  { his="B", yours="X", points=1},
            //        new  { his="B", yours="Y", points=5},
            //        new  { his="B", yours="Z", points=9},
            //        new  { his="C", yours="X", points=7},
            //        new  { his="C", yours="Y", points=2},
            //        new  { his="C", yours="Z", points=6}
            //    };

            //Puzzle 2:
            var combinations = new[]
            {
                    new  { his="A", yours="X", points=3},
                    new  { his="A", yours="Y", points=4},
                    new  { his="A", yours="Z", points=8},
                    new  { his="B", yours="X", points=1},
                    new  { his="B", yours="Y", points=5},
                    new  { his="B", yours="Z", points=9},
                    new  { his="C", yours="X", points=2},
                    new  { his="C", yours="Y", points=6},
                    new  { his="C", yours="Z", points=7}
            };

            return combinations.Single(c => c.his == hisMove && c.yours == yourMove).points;
        }
    }
}
