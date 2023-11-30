namespace ConsoleApp1
{
    internal class Program
    {
        internal static List<Monkey> monkeys = new List<Monkey>();
        static void Main(string[] args)
        {
            monkeys.Add(new Monkey
            {
                Items = new Queue<ulong>(new[] { 93UL, 98UL }),
                Operation = value => value * 17,
                Condition = value => value % 19 == 0,
                IfTrue = () => 5,
                IfFalse = () => 3
            });
            monkeys.Add(new Monkey
            {
                Items = new Queue<ulong>(new[] { 95UL, 72UL, 98UL, 82UL, 86UL }),
                Operation = value => value + 5,
                Condition = value => value % 13 == 0,
                IfTrue = () => 7,
                IfFalse = () => 6
            });
            monkeys.Add(new Monkey
            {
                Items = new Queue<ulong>(new[] { 85UL, 62UL, 82UL, 86UL, 70UL, 65UL, 83UL, 76UL }),
                Operation = value => value + 8,
                Condition = value => value % 5 == 0,
                IfTrue = () => 3,
                IfFalse = () => 0
            });
            monkeys.Add(new Monkey
            {
                Items = new Queue<ulong>(new[] { 86UL, 70UL, 71UL, 56UL }),
                Operation = value => value + 1,
                Condition = value => value % 7 == 0,
                IfTrue = () => 4,
                IfFalse = () => 5
            });
            monkeys.Add(new Monkey
            {
                Items = new Queue<ulong>(new[] { 77UL, 71UL, 86UL, 52UL, 81UL, 67UL }),
                Operation = value => value + 4,
                Condition = value => value % 17 == 0,
                IfTrue = () => 1,
                IfFalse = () => 6
            });
            monkeys.Add(new Monkey
            {
                Items = new Queue<ulong>(new[] { 89UL, 87UL, 60UL, 78UL, 54UL, 77UL, 98UL }),
                Operation = value => value * 7,
                Condition = value => value % 2 == 0,
                IfTrue = () => 1,
                IfFalse = () => 4
            });
            monkeys.Add(new Monkey
            {
                Items = new Queue<ulong>(new[] { 69UL, 65UL, 63UL }),
                Operation = value => value + 6,
                Condition = value => value % 3 == 0,
                IfTrue = () => 7,
                IfFalse = () => 2
            });
            monkeys.Add(new Monkey
            {
                Items = new Queue<ulong>(new[] { 89UL }),
                Operation = value => value * value,
                Condition = value => value % 11 == 0,
                IfTrue = () => 0,
                IfFalse = () => 2
            });

            for (int i = 0; i < 10000; i++)
            {
                foreach (var monkey in monkeys)
                    monkey.InspectAllItems();
            }

            var activeMonkeys = monkeys.OrderByDescending(m => m.itemsInspected);
            var monkeyBusiness = activeMonkeys.ElementAt(0).itemsInspected * activeMonkeys.ElementAt(1).itemsInspected;

            Console.WriteLine($"Monkey business: {monkeyBusiness}");
            Console.ReadLine();
        }

        internal class Monkey
        {
            public Queue<ulong> Items;
            public Func<ulong, ulong> Operation;
            public Predicate<ulong> Condition;
            public Func<int> IfTrue;
            public Func<int> IfFalse;
            public ulong itemsInspected = 0;

            internal readonly ulong masterModulus = 2 * 3 * 5 * 7 * 11 * 13 * 17 * 19;

            public void InspectAllItems ()
            {
                while (Items.Any())
                {
                    var newWorryLevel = Operation(Items.Dequeue()) % masterModulus;
                    if (Condition (newWorryLevel))
                        monkeys[IfTrue()].Items.Enqueue(newWorryLevel);
                    else
                        monkeys[IfFalse()].Items.Enqueue(newWorryLevel);
                    itemsInspected++;
                }
            }
        }
    }
}