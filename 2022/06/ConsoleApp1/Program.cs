using System;
using System.IO;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            int currentIdx = -1;
            string currentString = string.Empty;
            var input = File.ReadAllText(@"c:\data\aoc\6\aoc6-1.txt");
            do
            {
                currentIdx++;
                currentString = input.Substring(currentIdx, 14);
            } while (currentString.Distinct().Count() != 14);

            Console.Write($"Result: {currentIdx + 14}");
            Console.ReadLine();
        }
    }
}
