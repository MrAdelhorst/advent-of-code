namespace ConsoleApp1
{
    internal class Program
    {
        static List<Tuple<int, int>> timeline = new List<Tuple<int, int>>();

        static void Main(string[] args)
        {
            int currentCycle = 0;
            int currentValue = 1;
            timeline.Add(new Tuple<int, int>(currentCycle, currentValue));
            foreach (var line in File.ReadAllLines(@"c:\data\aoc\10\aoc10-1.txt"))
            {
                var command = line.Split(' ') switch
                {
                    ["noop"] => new Tuple<int, int>(1, 0),
                    ["addx", var arg] => new Tuple<int, int>(2, int.Parse(arg)),
                    _ => throw new ArgumentException($"command not covered: {line}"),
                };
                timeline.Add(new Tuple<int, int>(currentCycle = currentCycle + command.Item1, currentValue = currentValue + command.Item2));
            }

            //Puzzle2
            for (int line = 0; line < 6; line++)
            {
                for (int pixel = 0; pixel < 40; pixel++)
                {
                    var cycle = line * 40 + pixel + 1;
                    Console.Write((Math.Abs(GetRegisterDuringCycle(cycle) - pixel) > 1) ? "." : "#");
                }
                Console.Write("\n");
            }

            Console.ReadLine();

            //Puzzle1
            //var signalStrength = new[]{ 20, 60, 100, 140, 180, 220 }.Sum(CalculateValueDuringCycle);
            //Console.Write($"SignalStrength: {signalStrength}");
            //Console.ReadLine();
        }

        private static int GetRegisterDuringCycle(int cycle)
        {
            return timeline.Last(e => e.Item1 < cycle).Item2;
        }
        private static int CalculateValueDuringCycle(int cycle)
        {
            return GetRegisterDuringCycle(cycle) * cycle;
        }
    }
}