namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Puzzle 1
            //var total = File.ReadAllLines(@"c:\data\aoc\2015\2\aoc2-1.txt").Sum(line =>
            //{
            //    var sides = line.Split('x').Select(int.Parse).OrderBy(e => e).ToList();
            //    return sides[0] * sides[1] * 3 + (sides[1] * sides[2] + sides[0] * sides[2]) * 2;
            //});
            //Console.WriteLine($"Total wrapping paper: {total}");

            //Puzzle 2
            var total = File.ReadAllLines(@"c:\data\aoc\2015\2\aoc2-1.txt").Sum(line =>
            {
                var sides = line.Split('x').Select(int.Parse).OrderBy(e => e).ToList();
                return sides[0] * 2 + sides[1] * 2 + sides[0] * sides[1] * sides[2];
            });

            Console.WriteLine($"Total ribbon: {total}");
            Console.ReadLine();
        }
    }
}