using static System.Math;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Puzzle1.Run();
            Puzzle2.Run();
        }

        internal static class Puzzle2
        {
            internal class Coord
            {
                public int x;
                public int y;

                public Coord(int x, int y)
                {
                    this.x = x;
                    this.y = y;
                }

                public void AddToX(int delta) => x += delta;
                public void AddToY(int delta) => y += delta;
            }

            internal static void Run()
            {
                var rope = new List<Coord>();
                for (int i = 0; i < 10; i++)
                    rope.Add(new Coord(0, 0));

                var uniquePositions = new HashSet<Tuple<int, int>>();
                uniquePositions.Add(new Tuple<int,int>(0, 0));

                foreach (var line in File.ReadAllLines(@"c:\data\aoc\9\aoc9-1.txt"))
                {
                    var parts = line.Split(' ');
                    var direction = parts[0];
                    var steps = int.Parse(parts[1]);

                    for (int i = 0; i < steps; i++)
                    {
                        //Move head
                        rope.First().AddToX(direction switch
                        {
                            "R" => 1,
                            "L" => -1,
                            _ => 0
                        });
                        rope.First().AddToY(direction switch
                        {
                            "U" => 1,
                            "D" => -1,
                            _ => 0
                        });

                        //Should other knots move?
                        for (int pos = 1; pos < rope.Count(); pos++)
                        {
                            var xDist = rope[pos - 1].x - rope[pos].x;
                            var yDist = rope[pos - 1].y - rope[pos].y;
                            if (Max(Abs(xDist), Abs(yDist)) > 1)
                            {
                                rope[pos].AddToX(xDist switch
                                {
                                    > 0 => 1,
                                    0 => 0,
                                    < 0 => -1
                                });
                                rope[pos].AddToY(yDist switch
                                {
                                    > 0 => 1,
                                    0 => 0,
                                    < 0 => -1
                                });
                            }
                            else
                                break;
                        }
                        uniquePositions.Add(new Tuple<int, int>(rope.Last().x, rope.Last().y));
                    }
                }

                Console.Write($"Result: {uniquePositions.Count()}");
                Console.ReadLine();
            }
        }

        internal static class Puzzle1
        {
            internal struct Coord
            {
                public int x;
                public int y;

                public Coord(int x, int y)
                {
                    this.x = x;
                    this.y = y;
                }

                public void AddToX(int delta) => x += delta;
                public void AddToY(int delta) => y += delta;
            }

            internal static void Run()
            {
                var headPos = new Coord(0, 0);
                var tailPos = new Coord(0, 0);
                var uniquePositions = new HashSet<Coord>();
                uniquePositions.Add(new Coord(0, 0));

                foreach (var line in File.ReadAllLines(@"c:\data\aoc\9\aoc9-1.txt"))
                {
                    var parts = line.Split(' ');
                    var direction = parts[0];
                    var steps = int.Parse(parts[1]);

                    for (int i = 0; i < steps; i++)
                    {
                        //Move head
                        headPos.x += direction switch
                        {
                            "R" => 1,
                            "L" => -1,
                            _ => 0
                        };
                        headPos.y += direction switch
                        {
                            "U" => 1,
                            "D" => -1,
                            _ => 0
                        };

                        //Should tail move?
                        var xDist = headPos.x - tailPos.x;
                        var yDist = headPos.y - tailPos.y;
                        if (Max(Abs(xDist), Abs(yDist)) > 1)
                        {
                            tailPos.x += xDist switch
                            {
                                > 0 => 1,
                                0 => 0,
                                < 0 => -1
                            };
                            tailPos.y += yDist switch
                            {
                                > 0 => 1,
                                0 => 0,
                                < 0 => -1
                            };
                            uniquePositions.Add(tailPos);
                        }
                    }
                }

                Console.Write($"Result: {uniquePositions.Count()}");
                Console.ReadLine();
            }
        }
    }
}