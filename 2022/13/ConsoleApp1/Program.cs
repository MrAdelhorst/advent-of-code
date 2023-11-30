namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var packets = File.ReadAllLines(@"c:\data\aoc\13\aoc13-1.txt").Where(line => line.StartsWith('['));
            var total = 0;

            //Puzzle 2
            var dividerPacket1 = ProcessPacket("[[2]]");
            var dividerPacket2 = ProcessPacket("[[6]]");
            var orderedPackets = packets.Select(ProcessPacket).Union(new[] { dividerPacket1, dividerPacket2 }).Order().ToList();
            total = (orderedPackets.IndexOf(dividerPacket1) + 1) * (orderedPackets.IndexOf(dividerPacket2) + 1);

            //Puzzle 1
            //var currentSet = 0;
            //while (currentSet * 2 < packets.Count())
            //{
            //    var left = ProcessPacket(packets.ElementAt(currentSet * 2));
            //    var right = ProcessPacket(packets.ElementAt(currentSet * 2 + 1));

            //    var compareResult = left.Compare(right);
            //    if (compareResult == 0) { throw new InvalidOperationException("Packets are equal!"); }
            //    if (compareResult < 0)
            //        total += (currentSet + 1);

            //    currentSet++;
            //}

            Console.WriteLine($"Result: {total}");
            Console.ReadLine();
        }

        private static ItemList ProcessPacket(string input)
        {
            var data = new Queue<char>(input);
            ItemList currentContext = null;
            var outerContexts = new Stack<ItemList>();

            while (data.TryDequeue(out var currentItem))
            {
                switch (currentItem)
                {
                    case '[':
                        var tmp = new ItemList();
                        if (currentContext != null)
                        {
                            outerContexts.Push(currentContext);
                            currentContext.Items.Add(tmp);
                        }
                        currentContext = tmp;
                        break;
                    case ']':
                        if (outerContexts.Any())
                            currentContext = outerContexts.Pop();
                        else
                        {
                            if (data.Any())
                                throw new InvalidOperationException("Not enough opening braces");
                            return currentContext;
                        }
                        break;
                    case (>= '0') and (<= '9'):
                        //Hack to handle the value 10 in the input :/
                        if (currentItem == '1' && data.First() == '0')
                        {
                            data.Dequeue();
                            currentContext.Items.Add(new ItemScalar(10));
                        }
                        else
                            currentContext.Items.Add(new ItemScalar(currentItem));
                        break;
                    case ',':
                        break;
                    default:
                        throw new InvalidOperationException("Invalid character in input");
                }
            }

            throw new InvalidOperationException("Not enough closing braces");
        }
    }

    internal abstract class Item : IComparable
    {
        public abstract int Compare(Item other);

        public int CompareTo(object? obj)
        {
            if (obj == null)
                return 1;

            return Compare(obj as Item);
        }
    }

    internal class ItemList : Item
    {
        public List<Item> Items = new List<Item>();
        public ItemList() { }

        public ItemList (ItemScalar scalarValue)
        {
            Items.Add(scalarValue);
        }

        public override int Compare(Item other)
        {
            if (other is ItemScalar)
            {
                var otherList = new ItemList(other as ItemScalar);
                return this.Compare(otherList);
            }

            //Compare lists
            var otherItems = (other as ItemList).Items;
            var maxSize = Math.Max(Items.Count(), otherItems.Count());
            for (int idx = 0; idx < maxSize; idx++)
            {
                if (idx >= Items.Count())
                    return -1;

                if (idx >= otherItems.Count())
                    return 1;

                var thisItem = Items[idx];
                var otherItem = otherItems[idx];
                var res = thisItem.Compare(otherItem);
                if (res != 0)
                    return res;
            }

            return 0;
        }
    }

    internal class ItemScalar : Item
    {
        public int Value;
        public ItemScalar(char content) { this.Value = content - '0'; }
        public ItemScalar(int content) { this.Value = content; }

        public override int Compare(Item other)
        {
            if (other is ItemList)
            {
                var val = new ItemList(this);
                return val.Compare(other);
            }

            return Value.CompareTo((other as ItemScalar).Value);
        }
    }
}