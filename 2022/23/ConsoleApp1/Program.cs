namespace ConsoleApp1
{
    internal class Program
    {
        static List<Elf> elves = new List<Elf>();
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines(@"c:\data\aoc\23\aoc23-1.txt");
            for (int y = 0; y < lines.Length; y++)
                for (int x = 0; x < lines[y].Length; x++)
                    if (lines[y][x] == '#')
                        elves.Add(new Elf { CurrentPos = new Coord(x, y) });

            var directions = new List<Direction>(new[] { Direction.North, Direction.South, Direction.West, Direction.East });

            //Puzzle 1
            //for (int i = 0; i < 10; i++)

            //Puzzle 2
            var round = 0;
            do
            {
                //Propose moving elves
                foreach (var elf in elves)
                    if (elf.FreeLoS())
                        elf.Stand();
                    else
                        foreach (var direction in directions)
                        {
                            if (elf.CanMove(direction))
                            {
                                elf.ProposeMove(direction);
                                break;
                            }
                            elf.Stand();
                        }

                //Actually move elves
                foreach (var elf in elves)
                    elf.Move();

                //Update direction priorities
                var tmp = directions[0];
                directions.Remove(tmp);
                directions.Add(tmp);

                round++;
                Console.WriteLine($"After round #{round}, moving elves are: {elves.Count(e => !e.ProposedPos.Equals(Coord.Empty))}");
            } while (!elves.All(e => e.ProposedPos.Equals(Coord.Empty)));

            //Puzzle 1
            //var minX = elves.Min(e => e.CurrentPos.X);
            //var maxX = elves.Max(e => e.CurrentPos.X);
            //var minY = elves.Min(e => e.CurrentPos.Y);
            //var maxY = elves.Max(e => e.CurrentPos.Y);

            //var emptySlots = (maxX - minX + 1) * (maxY - minY + 1) - elves.Count();

            //Console.WriteLine($"Empty ground slots: {emptySlots}");
            //Console.ReadLine();

            //Puzzle 2
            Console.WriteLine($"Number of rounds: {round}");
            Console.ReadLine();
        }

        internal class Elf
        {
            public Coord CurrentPos;
            public Coord ProposedPos;

            internal bool CanMove(Direction direction)
            {
                switch (direction)
                {
                    case Direction.North:
                        if (AreAvailablePositions(new[] { CurrentPos.NorhWest(), CurrentPos.North(), CurrentPos.NorthEast() }))
                            return true;
                        break;
                    case Direction.South:
                        if (AreAvailablePositions(new[] { CurrentPos.SouthWest(), CurrentPos.South(), CurrentPos.SouthEast() }))
                            return true;
                        break;
                    case Direction.West:
                        if (AreAvailablePositions(new[] { CurrentPos.NorhWest(), CurrentPos.West(), CurrentPos.SouthWest() }))
                            return true;
                        break;
                    case Direction.East:
                        if (AreAvailablePositions(new[] { CurrentPos.NorthEast(), CurrentPos.East(), CurrentPos.SouthEast() }))
                            return true;
                        break;
                    default:
                        throw new InvalidOperationException("Illegal direction");
                }

                return false;
            }

            private bool AreAvailablePositions(IEnumerable<Coord> coords)
            {
                if (elves.Any(e => coords.Any(c => e.CurrentPos.Equals(c))))
                    return false;
                else
                    return true;
            }

            internal void ProposeMove(Direction direction)
            {
                var newX = CurrentPos.X + direction switch
                {
                    Direction.West => -1,
                    Direction.East => 1,
                    _ => 0
                };
                var newY = CurrentPos.Y + direction switch
                {
                    Direction.North => -1,
                    Direction.South => 1,
                    _ => 0,
                };
                ProposedPos = new Coord(newX, newY);
            }

            internal void Stand()
            {
                ProposedPos = Coord.Empty;
            }

            internal void Move()
            {
                if (!ProposedPos.Equals(Coord.Empty) && !elves.Except(new[] { this }).Any(e => e.ProposedPos.Equals(ProposedPos)))
                    CurrentPos = ProposedPos;
            }

            internal bool FreeLoS()
            {
                if (AreAvailablePositions(new[] 
                {   CurrentPos.NorhWest(), CurrentPos.North(), CurrentPos.NorthEast(), CurrentPos.East(),
                    CurrentPos.SouthEast(), CurrentPos.South(), CurrentPos.SouthWest(), CurrentPos.West() }))
                    return true;
                else
                    return false;
            }
        }

        internal enum Direction { East = 0, South = 1, West = 2, North = 3 }

        internal struct Coord
        {
            public int X;
            public int Y;
            public static Coord Empty => new Coord(int.MaxValue, int.MaxValue);
            public bool Equals(Coord other) => X == other.X && Y == other.Y;
            public Coord(int x, int y) { this.X = x; this.Y = y; }
            public Coord North() => new Coord(X, Y - 1);
            public Coord NorhWest() => new Coord(X - 1, Y - 1);
            public Coord NorthEast() => new Coord(X + 1, Y - 1);
            public Coord South() => new Coord(X, Y + 1);
            public Coord SouthWest() => new Coord(X - 1, Y + 1);
            public Coord SouthEast() => new Coord(X + 1, Y + 1);
            public Coord West() => new Coord(X - 1, Y);
            public Coord East() => new Coord(X + 1, Y);
        }
    }
}