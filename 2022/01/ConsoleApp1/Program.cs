using System;
using System.IO;

namespace ConsoleApp1
{
    class Program
    {
        internal static int[] highest = new int[3];
        static void Main(string[] args)
        {
            var lines = File.ReadLines(@"c:\data\aoc\1\aoc1-1.txt");
            var current = 0;
            foreach (var line in lines)
            {
                if (int.TryParse(line, out var cal))
                    current += cal;
                else
                {
                    StoreIfHigher(current);
                    current = 0;
                }
            }
            StoreIfHigher(current);
            Console.WriteLine($"Highest calorie counts: {highest[0]}, {highest[1]}, {highest[2]}, total: {highest[0]+ highest[1]+ highest[2]}");
            Console.ReadLine();
        }

        internal static void StoreIfHigher(int current)
        {
            int temp = 0;
            if (current > highest[0])
            {
                temp = highest[0];
                highest[0] = current;
                current = temp;
            }
            if (current > highest[1])
            {
                temp = highest[1];
                highest[1] = current;
                current = temp;
            }
            highest[2] = Math.Max(highest[2], current);
        }
    }
}
