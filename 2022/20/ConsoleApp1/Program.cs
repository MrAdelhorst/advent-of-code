namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines(@"c:\data\aoc\20\aoc20-1.txt");
            var originalOrder = new List<KeyValuePair<int, long>>();

            //Puzzle 1
            //var cycles = 1;
            //var factor = 1;

            //Puzzle 2
            var cycles = 10;
            long factor = 811589153; //decryption key

            for (int idx = 0; idx < lines.Count(); idx++)
                originalOrder.Add(new KeyValuePair<int, long>(idx, long.Parse(lines[idx]) * factor));

            var resultingOrder = new List<KeyValuePair<int, long>>(originalOrder);
            for (int i = 0; i < cycles; i++)
                foreach (var number in originalOrder)
                    resultingOrder.MoveElement(number.Key);

            var zeroIndex = resultingOrder.IndexOf(resultingOrder.Single(x => x.Value == 0));
            var result = resultingOrder.ElementAtIndexWrapped(zeroIndex + 1000) + resultingOrder.ElementAtIndexWrapped(zeroIndex + 2000) + resultingOrder.ElementAtIndexWrapped(zeroIndex + 3000);
            Console.WriteLine($"Coordinate sum: {result}");
            Console.ReadLine();
        }
    }

    internal static class Extensions
    {
        public static void MoveElement(this List<KeyValuePair<int,long>> source, int orgIdx)
        {
            var item = source.Single(x => x.Key == orgIdx);
            var idx = source.IndexOf(item);
            var value = item.Value;
            source.Remove(item);
            var destination = (int)((idx + value) % source.Count());
            var destinationIdx = (destination <= 0) ? source.Count() + destination : destination;
            source.Insert(destinationIdx, new KeyValuePair<int, long>(orgIdx, value));
        }

        public static long ElementAtIndexWrapped(this List<KeyValuePair<int, long>> source, int idx)
        {
            return source.ElementAt((idx > source.Count() -1) ? idx % source.Count() : idx).Value;
        }
    }
}