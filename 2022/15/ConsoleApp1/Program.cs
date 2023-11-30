namespace ConsoleApp1
{
    internal class Program
    {
        internal static List<Vector> commLinks = new List<Vector>();
        static void Main(string[] args)
        {
            foreach (var line in File.ReadAllLines(@"c:\data\aoc\15\aoc15-1.txt")) 
            {
                var coordinates = line.Substring(10).Split(": closest beacon is at ");
                commLinks.Add(new Vector(new Coord(coordinates[0]), new Coord(coordinates[1])));
            }

            //Puzzle 2
            const int minIndex = 0;
            const int maxIndex = 4000000;
            for(int row = minIndex; row <= maxIndex; row++)
            {
                var covered = FindPositionsCoveredOnRow(row);
                var vectorContainingStartIndex = covered.First(c => c.From <= minIndex && c.To >= minIndex);
                if (vectorContainingStartIndex.To < maxIndex)
                    Console.WriteLine($"Found beacon: X:{vectorContainingStartIndex.To + 1}, Y:{row}, Frequency: {(ulong)row+((ulong)vectorContainingStartIndex.To + 1)*4000000UL}");
            }

            // Puzzle 1
            //const int rowToInvestigate = 2000000;
            //var notOverlappingVectors = FindPositionsCoveredOnRow(rowToInvestigate);
            //var coveredPositions = notOverlappingVectors.Sum( v => v.To - v.From + 1);
            //var beaconsOnRow = commLinks.Where(cl => cl.Beacon.Y == rowToInvestigate).Select(cl => cl.Beacon.X).Distinct().Count();
            //Console.WriteLine($"Covered positions: {coveredPositions - beaconsOnRow}");

            Console.ReadLine();
        }

        private static IEnumerable<Vector1d> FindPositionsCoveredOnRow(int rowToInvestigate)
        {
            var closeEnoughSensors = commLinks.Where(cl => Math.Abs(rowToInvestigate - cl.Sensor.Y) <= cl.ManhattanDistance);
            var coveredCoordinates = closeEnoughSensors.Select(cl =>
            {
                var distanceToRow = new Vector(cl.Sensor, new Coord(cl.Sensor.X, rowToInvestigate)).ManhattanDistance;
                var surplusDistance = cl.ManhattanDistance - distanceToRow;
                return new Vector1d(cl.Sensor.X - surplusDistance, cl.Sensor.X + surplusDistance);
            });

            return MergeVectors(coveredCoordinates.OrderBy(v => v.From));
        }
        private static IEnumerable<Vector1d> MergeVectors(IOrderedEnumerable<Vector1d> allVectors)
        {
            Vector1d? current = null;
            foreach (var v in allVectors)
            {
                if (current == null)
                    current = v;
                else
                {
                    if (v.From <= current.Value.To)
                        current = new Vector1d(current.Value.From, Math.Max(v.To, current.Value.To));
                    else
                    {
                        yield return current.Value;
                        current = v;
                    }
                }
            }
            yield return current.Value;
        }

        internal struct Coord
        {
            public int X;
            public int Y;

            public Coord(int x, int y) { this.X = x; this.Y = y; }
            public Coord(string coordString)
            {
                var parts = coordString.Split(", ");
                X = int.Parse(parts[0].Split('=')[1]);
                Y = int.Parse(parts[1].Split('=')[1]);
            }
        }

        internal struct Vector
        {
            public Coord Sensor;
            public Coord Beacon;

            public Vector(Coord sensor, Coord beacon) { Sensor = sensor; Beacon = beacon; }
            public int ManhattanDistance => Math.Abs(Sensor.X - Beacon.X) + Math.Abs(Sensor.Y - Beacon.Y);
        }

        internal struct Vector1d
        {
            public int From;
            public int To;
            public Vector1d(int from, int to) { this.From = from; this.To = to; }
        }
    }
}