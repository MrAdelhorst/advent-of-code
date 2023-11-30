namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var monkeys = new Dictionary<string, MonkeyJob>();
            foreach (var line in File.ReadAllLines(@"c:\data\aoc\21\aoc21-1.txt"))
            {
                var parts = line.Split(": ");
                var job = parts[1].Split(' ');
                if (job.Length == 1)
                    monkeys.Add(parts[0], new NumberJob { Number = long.Parse(job[0]) });
                else
                    monkeys.Add(parts[0], new CalculationJob(job[0], job[2], job[1].Single(), monkeys));
            }

            //Puzzle 2
            var root = monkeys["root"] as CalculationJob;
            root.Operator = '-';
            Console.WriteLine($"Humn value that satisfies equation: {root.FindMatchingHumnValue(0)}");
            Console.ReadLine();

            //Puzzle 1
            //Console.WriteLine($"<root> says: {monkeys["root"].NumberToSay}");
            //Console.ReadLine();
        }
    }

    internal abstract class MonkeyJob
    {
        public abstract long NumberToSay { get; }
    }

    internal class NumberJob : MonkeyJob
    {
        public long Number;

        public override long NumberToSay => Number;
    }

    internal class CalculationJob : MonkeyJob
    {
        public string Monkey1;
        public string Monkey2;
        public char Operator;

        private Dictionary<string, MonkeyJob> monkeys;
        public CalculationJob(string monkey1, string monkey2, char op, Dictionary<string, MonkeyJob> monkeys)
        {
            this.monkeys = monkeys;
            this.Monkey1 = monkey1;
            this.Monkey2 = monkey2;
            this.Operator = op;
        }

        public override long NumberToSay
        { 
            get
            {
                return Operator switch
                {
                    '+' => monkeys[Monkey1].NumberToSay + monkeys[Monkey2].NumberToSay,
                    '-' => monkeys[Monkey1].NumberToSay - monkeys[Monkey2].NumberToSay,
                    '*' => monkeys[Monkey1].NumberToSay * monkeys[Monkey2].NumberToSay,
                    '/' => monkeys[Monkey1].NumberToSay / monkeys[Monkey2].NumberToSay,
                    _ => throw new InvalidOperationException("Not an allowed operator")
                };
            }
        }

        internal bool UsesHumn(string monkey)
        {
            if (monkey == "humn")
                return true;

            var job = monkeys[monkey] as CalculationJob;
            if (job == null)
                return false;
            else
                return UsesHumn(job.Monkey1) || UsesHumn(job.Monkey2);
        }

        public long FindMatchingHumnValue(long value)
        {
            if (UsesHumn(Monkey1))
            {
                var otherValue = monkeys[Monkey2].NumberToSay;
                var newValueToTarget = Operator switch
                {
                    '+' => value - otherValue,
                    '-' => value + otherValue,
                    '*' => value / otherValue,
                    '/' => value * otherValue,
                    _ => throw new InvalidOperationException("Not an allowed operator")
                };
                if (Monkey1 == "humn")
                    return newValueToTarget;
                else
                    return (monkeys[Monkey1] as CalculationJob).FindMatchingHumnValue(newValueToTarget);
            }
            else
            {
                var otherValue = monkeys[Monkey1].NumberToSay;
                var newValueToTarget = Operator switch
                {
                    '+' => value - otherValue,
                    '-' => -(value - otherValue),
                    '*' => value / otherValue,
                    '/' => otherValue / value,
                    _ => throw new InvalidOperationException("Not an allowed operator")
                };
                if (Monkey2 == "humn")
                    return newValueToTarget;
                else
                    return (monkeys[Monkey2] as CalculationJob).FindMatchingHumnValue(newValueToTarget);
            }
        }
    }
}