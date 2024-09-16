using System.IO;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllText(@"c:\data\aoc\2015\1\aoc1-1.txt");

            //Puzzle 1
            //Console.WriteLine($"Floor: {input.Count(c => c == '(') - input.Count(c => c == ')')}");

            //Puzzle 2
            int pos = 1;
            while (!MostDown(pos, input.Substring(0, pos)))
                pos++;
            Console.WriteLine($"Character: {pos}");
            Console.ReadLine();
        }

        private static bool MostDown(int pos, string input) => input.Count(c => c == '(') - input.Count(c => c == ')') < 0;
    }
}