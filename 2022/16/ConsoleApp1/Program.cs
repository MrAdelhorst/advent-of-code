namespace ConsoleApp1
{
    internal class Program
    {
        internal static List<Valve> valves = new List<Valve>();
        static void Main(string[] args)
        {
            foreach (var line in File.ReadAllLines(@"c:\data\aoc\16\aoc16-1.txt"))
            {
                var valveName = line.Substring(6, 2);
                var parts = line.Substring(23).Split(';');
                var pressurePerMinute = int.Parse(parts[0]);
                var connectedValves = parts[1].Contains(',') ? parts[1].Substring(24).Split(", ") : parts[1].Substring(23).Split(", ");
                valves.Add(new Valve { Name = valveName, Open = false, PressurePerMinute = pressurePerMinute, Connections = connectedValves.ToList() });
            }

            //Map out all distances
            foreach (var valve in valves)
                valve.BuildDistanceMap();

            //Clean out broken valves - except for the starting point
            valves = valves.Where(v => v.Name == "AA" || v.PressurePerMinute > 0).ToList();

            //Puzzle 2
            var activeJobs = new Queue<Job>();
            var startValve = valves.Single(v => v.Name == "AA");
            activeJobs.Enqueue(new Job { CurrentValve = startValve });
            var completedJobs = new List<Job>();
            while (activeJobs.Any())
            {
                var currentJob = activeJobs.Dequeue();
                var currentValve = currentJob.CurrentValve;
                bool childJob = false;
                foreach (var destination in valves.Except(currentJob.Moves.Select(m => m.ValveOpened).Union(new[] { startValve })))
                {
                    var newTimeSpent = currentJob.TimeSpent + currentJob.CurrentValve.Distances[destination.Name] + 1;
                    if (newTimeSpent < 26)
                    {
                        activeJobs.Enqueue(new Job
                        {
                            CurrentValve = destination,
                            TimeSpent = newTimeSpent,
                            Moves = currentJob.Moves.Union(new[] { new Move { ValveOpened = destination, TimeIndex = newTimeSpent } })
                        });
                        childJob = true;
                    }
                }
                if (!childJob)
                    completedJobs.Add(currentJob);
            }

            var orderedJobs = completedJobs.Select(job => new Tuple<Job, int>(job, job.Moves.Sum(m => m.ValveOpened.PressurePerMinute * (26 - m.TimeIndex)))).OrderByDescending(j => j.Item2).ToList();

            var bestPressure = 0;
            foreach (var job in orderedJobs)
            {
                var currentKey = job.Item1.MovesKey;
                var pressure = orderedJobs.Where(j => !Overlaps(j.Item1.MovesKey, currentKey)).OrderByDescending(j => j.Item2).First().Item2 + job.Item2;
                if (pressure > bestPressure)
                    bestPressure = pressure;
            }

            Console.WriteLine($"Pressure released: {bestPressure}");
            Console.ReadLine();

            //Puzzle 1
            //var activeJobs = new Queue<Job>();
            //var startValve = valves.Single(v => v.Name == "AA");
            //activeJobs.Enqueue(new Job { CurrentValve = startValve });
            //var completedJobs = new List<Job>();
            //while (activeJobs.Any())
            //{
            //    var currentJob = activeJobs.Dequeue();
            //    var currentValve = currentJob.CurrentValve;
            //    bool childJob = false;
            //    foreach (var destination in valves.Except(currentJob.Moves.Select(m => m.ValveOpened).Union(new[] { startValve })))
            //    {
            //        var newTimeSpent = currentJob.TimeSpent + currentJob.CurrentValve.Distances[destination.Name] + 1;
            //        if (newTimeSpent < 30)
            //        {
            //            activeJobs.Enqueue(new Job
            //            {
            //                CurrentValve = destination,
            //                TimeSpent = newTimeSpent,
            //                Moves = currentJob.Moves.Union(new[] { new Move { ValveOpened = destination, TimeIndex = newTimeSpent } })
            //            });
            //            childJob = true;
            //        }
            //    }
            //    if (!childJob)
            //        completedJobs.Add(currentJob);
            //}

            //var orderedJobs = completedJobs.Select(job => new Tuple<Job, int>(job, job.Moves.Sum(m => m.ValveOpened.PressurePerMinute * (30 - m.TimeIndex)))).OrderByDescending(j => j.Item2).ToList();
            //var pressureReleased = orderedJobs.First().Item2;
            //Console.WriteLine($"Pressure released: {pressureReleased}");
            //Console.ReadLine();
        }

        private static bool Overlaps(byte[] movesKey, byte[] currentKey)
        {
            for (int i=0; i<movesKey.Length; i++)
                if ((movesKey[i] & currentKey[i]) != 0)
                    return true;
            return false;
        }

        internal class Valve
        {
            public string Name;
            public bool Open;
            public int PressurePerMinute;
            public IEnumerable<string> Connections = new List<string>();
            public Dictionary <string, int> Distances= new Dictionary <string, int>();

            public IEnumerable<Valve> GetConnectedValves()
            {
                foreach (var valve in Connections)
                    yield return valves.Single(v => v.Name == valve);
            }

            internal void BuildDistanceMap()
            {
                var candidates = new Queue<Valve>();
                candidates.Enqueue(this);
                Distances[this.Name] = 0;
                while (candidates.Any())
                {
                    var current = candidates.Dequeue();
                    var currentDist = Distances[current.Name];
                    foreach (var direction in current.GetConnectedValves())
                    {
                        var destinationDist = Distances.TryGetValue(direction.Name, out var val) ? val : int.MaxValue;
                        if (currentDist <  destinationDist - 1)
                        {
                            Distances[direction.Name] = currentDist + 1;
                            candidates.Enqueue(direction);
                        }
                    }
                }
            }
        }

        internal class Move
        {
            public Valve ValveOpened;
            public int TimeIndex;
        }

        internal class Job
        {
            public IEnumerable<Move> Moves = new List<Move>();
            public int TimeSpent = 0;
            public Valve CurrentValve;

            private byte[] movesKey = null;
            public byte[] MovesKey => (movesKey == null) ? movesKey = GenerateMovesKey() : movesKey;

            private byte[] GenerateMovesKey()
            {
                var tmp = new byte[valves.Count()/8+1];
                foreach (var m in Moves)
                {
                    var bitNo = valves.IndexOf(m.ValveOpened);
                    var bitMask = (byte)  Math.Pow(2, bitNo % 8);
                    tmp[bitNo / 8] |= bitMask;
                }
                return tmp;
            }
        }
    }
}