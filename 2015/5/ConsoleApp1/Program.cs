namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Puzzle 1
            //var rules = new Predicate<string>[] { Puzzle1_Rule1, Puzzle1_Rule2, Puzzle1_Rule3 };
            
            //Puzzle 2
            var rules = new Predicate<string>[] { Puzzle2_Rule1, Puzzle2_Rule2 };

            var niceStrings = File.ReadAllLines(@"c:\data\aoc\2015\5\aoc5-1.txt").Count(line => rules.All(rule => rule(line)));

            Console.WriteLine($"Nice strings: {niceStrings}");
            Console.ReadLine();
        }

        private static bool Puzzle2_Rule1(string line)
        {
            for (int i = 0; i < line.Length - 1; i++)
                if (line.Substring(i+2, line.Length - i - 2).Contains(line.Substring(i, 2)))
                    return true;

            return false;
        }
        
        private static bool Puzzle2_Rule2(string line)
        {
            for (int i = 0; i < line.Length - 2; i++)
                if (line[i] == line[i + 2])
                    return true;

            return false;
        }

        private static bool Puzzle1_Rule3(string line)
        {
            var evilStrings = new string[] { "ab", "cd", "pq", "xy" };
            foreach (var evil in evilStrings) 
                if (line.Contains(evil)) return false;

            return true;
        }

        private static bool Puzzle1_Rule2(string line)
        {
            for (int i=0; i<line.Length-1; i++)
                if (line[i] == line[i+1])
                    return true;

            return false;
        }

        private static bool Puzzle1_Rule1(string line)
        {
            var vowels = new char[] { 'a', 'e', 'i', 'o', 'u' };
            return line.Count(vowels.Contains) >= 3;
        }
    }
}