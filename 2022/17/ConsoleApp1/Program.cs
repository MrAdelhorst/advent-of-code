using System.Data;

namespace ConsoleApp1
{
    internal class Program
    {
        public static Dictionary<long, HashSet<Context>> topRowsPerCycle = new Dictionary<long, HashSet<Context>>();
        static void Main(string[] args)
        {
            var rocks = new List<char[][]>()
            {
                new char[][]{
                    "####".ToArray()},
                new char[][]{
                    ".#.".ToArray(),
                    "###".ToArray(),
                    ".#.".ToArray()},
                new char[][]{
                    "..#".ToArray(),
                    "..#".ToArray(),
                    "###".ToArray()},
                new char[][]{
                    "#".ToArray(),
                    "#".ToArray(),
                    "#".ToArray(),
                    "#".ToArray()},
                new char[][]{
                    "##".ToArray(),
                    "##".ToArray()}
            };
            var currentRock = 0;

            var chamber = new Chamber();

            var wind = File.ReadAllText(@"c:\data\aoc\17\aoc17-1.txt");
            var windIndex = -1;

            
            long currentX = 2, currentY = 4;
            long totalRocksDropped = 0;

            //Puzzle 2
            bool shouldDetectCycles = true;
            long totalRocksToDrop = long.MaxValue;

            //Puzzle 1
            //bool shouldDetectCycles = false;
            //long totalRocksToDrop = 2022;
            do
            {
                //Blow wind
                if (++windIndex >= wind.Length)
                    windIndex = 0;
                var delta = wind[windIndex] switch
                {
                    '<' => -1,
                    '>' => 1,
                    _ => throw new InvalidOperationException("Illegal wind direction")
                };
                if (chamber.CanMoveRockTo(rocks[currentRock], currentX + delta, currentY))
                    currentX += delta;

                //Drop rock
                if (chamber.CanMoveRockTo(rocks[currentRock], currentX, currentY - 1))
                    currentY--;
                else
                {
                    chamber.InsertRockAt(rocks[currentRock], currentX, currentY);
                    totalRocksDropped++;

                    //Detect repeating cycle
                    if (shouldDetectCycles)
                    {
                        var firstCycleStart = FindRepeatedCycle(chamber, totalRocksDropped, windIndex);
                        if (firstCycleStart != null)
                        {
                            //Fast forward 
                            long totalRocksToCalculate = 1000000000000;
                            var rocksInCycle = totalRocksDropped - firstCycleStart.RocksDropped;
                            var numberOfFullCycles = (totalRocksToCalculate - firstCycleStart.RocksDropped) / rocksInCycle;
                            
                            //Update target
                            totalRocksToDrop = totalRocksToCalculate;
                            totalRocksDropped = firstCycleStart.RocksDropped + numberOfFullCycles * rocksInCycle;
                            chamber.Height = firstCycleStart.Height + numberOfFullCycles * (chamber.Height - firstCycleStart.Height);

                            shouldDetectCycles = false;
                        }
                    }

                    //Prepare next rock
                    currentX = 2;
                    currentY = chamber.Height + 4;
                    if (++currentRock == rocks.Count)
                        currentRock = 0;
                }
                //chamber.Print(rocks[currentRock], currentX, currentY);
                //Console.ReadLine();
            } while (totalRocksDropped < totalRocksToDrop);

            Console.WriteLine($"Total height: {chamber.Height}");
            Console.ReadLine();
        }

        private static Context FindRepeatedCycle(Chamber chamber, long rocksDropped, int idx)
        {
            var top30rows = string.Concat(chamber.rows.Take(30).Select(r => string.Concat(r)));
            if (topRowsPerCycle.TryGetValue(idx, out var set))
            {
                if (set.Any(s => s.Rows == top30rows))
                {
                    return set.Single(s => s.Rows == top30rows);
                }
                else
                    set.Add(new Context { Rows = top30rows, RocksDropped = rocksDropped, Height = chamber.Height });
            }
            else
                topRowsPerCycle.Add(idx, new HashSet<Context>(new[] { new Context { Rows = top30rows, RocksDropped = rocksDropped, Height = chamber.Height } }));

            return null;
        }

        internal class Chamber
        {
            public FixedSizeList<char[]> rows = new FixedSizeList<char[]>(101);
            private long height = 0;
            public long Height
            {
                get { return height; }
                set { height = value; }
            }

            public Chamber() 
            {
                //Add floor
                rows.Add("#######".ToArray());
            }

            public bool CanMoveRockTo(char[][] rock, long x, long y)
            {
                if (x < 0 || x + rock.First().Length - 1 > 6)
                    return false;

                if (y > Height)
                    return true;
                else
                {
                    var rowIndex = Height > 100 ? y - Height + 100 : y;
                    for (long j = 0; j < rock.Length; j++)
                    {
                        if (j <= rows.Count() - 1 - rowIndex)
                        {
                            var targetRow = rows.ElementAt((int) (j + rowIndex));
                            var rockRow = rock[rock.Length - 1 - j];
                            for (long i = 0; i < rockRow.Length; i++)
                            {
                                if (targetRow[x+i] == '#' && rockRow[i] == '#')
                                    return false;
                            }
                        }
                    }

                    return true;
                }
            }

            public void InsertRockAt(char[][] rock, long x, long y)
            {
                while (Height < y + rock.Length - 1)
                {
                    rows.Add(".......".ToArray());
                    height++;
                }

                var rowIndex = Height > 100 ? y - Height + 100 : y;
                for (int j=0; j < rock.Length; j++)
                {
                    var rockRow = rock[rock.Length - 1 - j];
                    var targetRow = rows.ElementAt((int)(rowIndex + j));
                    for (long i = 0; i < rockRow.Length; i++)
                        if (rockRow[i] == '#')
                            targetRow[x+i] = '#';
                }
            }

            public void Print(char[][] rock, long x, long y)
            {
                Console.Clear();

                var rowIndex = Height > 100 ? y - Height + 100 : y;

                for (long j = rowIndex + rock.Length - 1; j > rows.Count() - 1; j--)
                {
                    var row = ".......".ToCharArray();
                    if (j >= rowIndex)
                        for (long k = x; k < x + rock.First().Length; k++)
                            row[k] = (rock[rowIndex + rock.Length - 1 - j][k - x] == '#') ? '@' : '.';
                    Console.WriteLine(string.Concat(row));
                }

                for (int i=rows.Count()-1; i>=0 && i>rows.Count()-30; i--)
                {
                    var row = rows[i].ToArray();
                    if (i >= rowIndex && i < rowIndex + rock.Length)
                        for (long k = x; k < x + rock.First().Length; k++)
                            if (rock[rock.Length - 1 - (i - rowIndex)][k - x] == '#')
                                row[k] = '@';
                    Console.WriteLine(row);
                }

                Console.WriteLine($"Height: {Height}");
            }
        }
    }

    public class Context
    {
        public string Rows;
        public long RocksDropped;
        public long Height;
    }

    public class FixedSizeList<T> : List<T>
    {
        private readonly int maxSize;

        public FixedSizeList(int maxSize)
        {
            this.maxSize = maxSize;
        }

        public new void Add(T item) //Nasty, but everyting's allowed, right? ;)
        {
            base.Add(item);
            if (Count > maxSize)
                RemoveAt(0);
        }
    }
}