namespace ConsoleApp1
{
    internal class Program
    {
        public static int maxX, maxY, uniqueBlizzards;
        public static Coord startPos, endPos;
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines(@"c:\data\aoc\24\aoc24-1.txt");
            var map = new Dictionary<Coord, List<char>>();
            var validBlizzards = new[] { '<', '>', 'v', '^' };
            for (int y = 1; y < lines.Length - 1; y++)
            {
                var x = 0;
                foreach (var slot in lines[y])
                {
                    if (validBlizzards.Contains(slot))
                        map.Add(new Coord(x-1, y-1), new List<char>(new[] { slot }));
                    x++;
                }
            }
            maxX = lines.First().Length - 3;
            maxY = lines.Length - 3;
            uniqueBlizzards = (maxX + 1) * (maxY + 1);

            startPos = new Coord(0, -1);
            endPos = new Coord(maxX, maxY + 1);
            var bestJobs = new Job[3];

            foreach (var round in new[] { 0, 1, 2 })
            {
                var bestConfigs = new Dictionary<string, int>();
                var activeJobs = new Stack<Job>();
                activeJobs.Push(new Job { CurrentPos = startPos, CurrentMap = map.Clone() });
                while (activeJobs.Any())
                {
                    var currentJob = activeJobs.Pop();
                    while (currentJob.IsAlive)
                    {
                        var currentPos = currentJob.CurrentPos;
                        if (currentJob.Time >= (bestJobs[round]?.Time ?? int.MaxValue))
                            currentJob.Kill();

                        var xDist = endPos.X - currentPos.X;
                        var yDist = endPos.Y - currentPos.Y;

                        var directionsToExplore = new List<Coord>();
                        var yDir = yDist > 0 ? currentPos.Down() : currentPos.Up();
                        var xDir = xDist > 0 ? currentPos.Right() : currentPos.Left();
                        if (Math.Abs(xDist) > Math.Abs(yDist))
                            directionsToExplore.AddRange(new[] { xDir, yDir });
                        else
                            directionsToExplore.AddRange(new[] { yDir, xDir });
                        directionsToExplore.Add(currentPos); //Wait for next round
                        yDir = yDist > 0 ? currentPos.Up() : currentPos.Down();
                        xDir = xDist > 0 ? currentPos.Left() : currentPos.Right();
                        if (Math.Abs(xDist) > Math.Abs(yDist))
                            directionsToExplore.AddRange(new[] { yDir, xDir });
                        else
                            directionsToExplore.AddRange(new[] { xDir, yDir });

                        //Move blizzards
                        currentJob.CurrentMap = currentJob.CurrentMap.AdvanceRound();
                        currentJob.Time++;


                        //Check valid destinations
                        var validDirections = new List<Coord>();
                        var blizzardConfig = currentJob.Time % uniqueBlizzards;
                        foreach (var direction in directionsToExplore)
                        {
                            if (direction.Equals(endPos))
                            {
                                if (currentJob.Time < (bestJobs[round]?.Time ?? int.MaxValue))
                                {
                                    currentJob.CurrentPos = direction;
                                    bestJobs[round] = currentJob;
                                    Console.WriteLine($"New best: {bestJobs[0].Time}, {bestJobs[1]?.Time ?? -1}, {bestJobs[2]?.Time ?? -1}");
                                }

                                currentJob.Kill();
                                break;
                            }
                            if (ValidPosition(direction) && !currentJob.CurrentMap.ContainsKey(direction))
                            {
                                var newConfig = $"{blizzardConfig}.{direction.X}.{direction.Y}";
                                int best = int.MaxValue;
                                if (!bestConfigs.TryGetValue(newConfig, out best) || currentJob.Time < best)
                                {
                                    validDirections.Add(direction);
                                    bestConfigs[newConfig] = currentJob.Time;
                                }
                            }
                        }

                        if (currentJob.IsAlive && validDirections.Any())
                        {
                            currentJob.CurrentPos = validDirections.First();
                            var remainingDirections = validDirections.Skip(1);
                            foreach (var direction in remainingDirections)
                                activeJobs.Push(new Job { CurrentPos = direction, CurrentMap = currentJob.CurrentMap.Clone(), Time = currentJob.Time });
                        }
                        else
                            currentJob.Kill();
                    }
                }

                //Get ready for next round
                endPos = startPos;
                startPos = bestJobs[round].CurrentPos;
                map = bestJobs[round].CurrentMap;
            }

            Console.WriteLine($"Final best: : {bestJobs[0].Time}, {bestJobs[1]?.Time ?? -1}, {bestJobs[2]?.Time ?? -1}, grand total: {bestJobs.Sum(j => j.Time)}");
            Console.ReadLine();
        }

        private static bool ValidPosition(Coord pos)
        {
            //Allow going back to start (and end)
            if (startPos.Equals(pos) || endPos.Equals(pos))
                return true;

            if (pos.X < 0 || pos.X > maxX) 
                return false;
            if (pos.Y < 0 || pos.Y > maxY)
                return false;
            return true;
        }
    }

    internal struct Coord
    {
        public int X;
        public int Y;
        public Coord(int x, int y) { this.X = x; this.Y = y; }
        public Coord Up() => new Coord(X, Y - 1);
        public Coord Down() => new Coord(X, Y + 1);
        public Coord Left() => new Coord(X - 1, Y);
        public Coord Right() => new Coord(X + 1, Y);
    }

    internal class Job
    {
        public Coord CurrentPos;
        public Dictionary<Coord, List<char>> CurrentMap;
        public int Time = 0;
        public bool isAlive = true;
        public bool IsAlive => isAlive;
        public void Kill()
        {
            this.isAlive = false;
        }

        public void PrintMap()
        {
            for (int i = 0; i <= Program.maxY; i++)
            {
                for (int j = 0; j <= Program.maxX; j++)
                    if (CurrentMap.TryGetValue(new Coord(j, i), out var z))
                    {
                        if (z.Count() > 1)
                            Console.Write(z.Count());
                        else
                            Console.Write(z.Single());
                    }
                    else
                    {
                        if (CurrentPos.Equals(new Coord(j, i)))
                            Console.Write('E');
                        else
                            Console.Write('.');
                    }
                Console.Write('\n');
            }
            Console.WriteLine($"Current time: {Time}");
        }
    }

    internal static class Extensions
    {
        public static Dictionary<Coord, List<char>> Clone(this Dictionary<Coord, List<char>> source)
        {
            var tmp = new Dictionary<Coord, List<char>>();
            foreach (var kvp in source)
                tmp.Add(kvp.Key, new List<char>(kvp.Value));

            return tmp;
        }

        public static Dictionary<Coord, List<char>> AdvanceRound(this Dictionary<Coord, List<char>> source)
        {
            var tmp = new Dictionary<Coord, List<char>>();
            foreach (var kvp in source)
            {
                foreach (var blizzard in kvp.Value)
                {
                    var newPos = blizzard switch
                    {
                        '>' => kvp.Key.X == Program.maxX ? new Coord(0, kvp.Key.Y) : kvp.Key.Right(),
                        '<' => kvp.Key.X == 0 ? new Coord(Program.maxX, kvp.Key.Y) : kvp.Key.Left(),
                        '^' => kvp.Key.Y == 0 ? new Coord(kvp.Key.X, Program.maxY) : kvp.Key.Up(),
                        'v' => kvp.Key.Y == Program.maxY ? new Coord(kvp.Key.X, 0) : kvp.Key.Down(),
                        _ => throw new InvalidOperationException("Invalid direction!")
                    };

                    if (tmp.TryGetValue(newPos, out var list))
                        list.Add(blizzard);
                    else
                        tmp.Add(newPos, new List<char>(new[] { blizzard }));
                }
            }

            return tmp;
        }
    }
}