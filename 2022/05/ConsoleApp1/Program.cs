using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {
        private static Dictionary<int, Stack<string>> crates = new Dictionary<int, Stack<string>>();
        static void Main(string[] args)
        {
            var lines = new Queue<string>(File.ReadAllLines(@"c:\data\aoc\5\aoc5-1.txt"));
            var crateLines = new Stack<string>();
            
            //Separate crastes from moves
            var line = lines.Dequeue();
            while (line != string.Empty)
            {
                crateLines.Push(line);
                line = lines.Dequeue();
            }

            //Initialize crates collection
            var columns = crateLines.Pop().Split("   ");
            foreach (var column in columns)
                crates.Add(int.Parse(column.Trim()), new Stack<string>());

            //Populate crates collection
            foreach (var crateLine in crateLines)
            {
                for (int i = 0; i < (crateLine.Length + 1) / 4; i++)
                {
                    var crate = crateLine.Substring(4 * i, 3).Trim();
                    if (crate != string.Empty)
                        crates[i + 1].Push(crate);
                }
            }

            //Execute moves
            foreach (var move in lines)
            {
                var parts = move.Split(' ');
                MoveCrates(int.Parse(parts[1]), int.Parse(parts[3]), int.Parse(parts[5]));
            }

            //Generate message
            string res = string.Empty;
            foreach (var crate in crates)
                res += crate.Value.Pop().Trim('[', ']');

            Console.WriteLine($"Top crates: {res}");
            Console.ReadLine();
        }

        private static void MoveCrates(int amount, int from, int to)
        {
            var temp = new List<string>();
            for (int i = 0; i < amount; i++)
                temp.Add(crates[from].Pop());

            temp.Reverse();
            foreach (var crate in temp)
                crates[to].Push(crate);
        }
    }
}
