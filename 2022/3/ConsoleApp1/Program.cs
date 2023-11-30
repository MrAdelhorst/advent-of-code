using System;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var result = Puzzle2();

            Console.WriteLine($"Result: {result}");
            Console.ReadLine();
        }

        private static int Puzzle2()
        {
            int total = 0;

            var lines = File.ReadAllLines(@"c:\data\aoc\3\aoc3-1.txt");
            for(int i=0; i<lines.Length/3; i++)
            {
                var item = lines.ElementAt(i*3).Intersect(lines.ElementAt(i * 3 + 1)).Intersect(lines.ElementAt(i * 3 + 2)).Distinct().Single();

                total += CalculateValue(item);
            }

            return total;
        }

        private static int Puzzle1()
        {
            int total = 0;

            foreach (var line in File.ReadAllLines(@"c:\data\aoc\3\aoc3-1.txt"))
            {
                var room1 = line.Substring(0, line.Length / 2);
                var room2 = line.Substring((line.Length / 2), line.Length / 2);

                var item = room1.Intersect(room2).Distinct().Single();

                total += CalculateValue(item);
            }

            return total;
        }

        private static int CalculateValue(char item)
        {
            return Char.IsLower(item) ? item - 'a' + 1 : item - 'A' + 27;
        }
    }
}
