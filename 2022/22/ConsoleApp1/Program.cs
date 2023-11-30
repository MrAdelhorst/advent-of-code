using System.Text.RegularExpressions;

namespace ConsoleApp1
{
    internal class Program
    {
        internal static char[][] map;
        internal static Coord currentPos;
        internal static Direction direction;
        static void Main(string[] args)
        {
            //Parse input
            var lines = File.ReadAllLines(@"c:\data\aoc\22\aoc22-1.txt");
            var width = lines.Take(lines.Length - 2).Max(l => l.Length);

            var expression = new Regex(@"([LR]|\d+)");
            var moves = expression.Matches(lines.Last()).Select(m => m.Value);

            map = lines.Take(lines.Length - 2).Select(l =>
            {
                var tmp = new char[width];
                Array.Copy(l.ToCharArray(), tmp, l.Length);
                return tmp;
            }).ToArray();

            currentPos = new Coord(0, map[0].ToList().IndexOf('.'));
            direction = Direction.Right;

            foreach (var move in moves)
                switch (move)
                {
                    case "R":
                        if (direction == Direction.Up)
                            direction = Direction.Right;
                        else
                            direction++; 
                        break;
                    case "L": 
                        if (direction == Direction.Right)
                            direction = Direction.Up;
                        else
                            direction--; 
                        break;
                    default:
                        //Puzzle 2
                        Puzzle2.MoveForward(int.Parse(move));

                        //Puzzle 1
                        //Puzzle1.MoveForward(int.Parse(move)); 

                        break;
                }

            Console.WriteLine($"Final password: {1000*(currentPos.Y+1) + 4*(currentPos.X+1) + (int)direction}");
            Console.ReadLine();
        }

        internal class Puzzle2
        {
            public static void MoveForward(int steps)
            {
                for (int j = 0; j < steps; j++)
                {
                    var newPos = direction switch
                    {
                        Direction.Right => currentPos.Right(),
                        Direction.Down => currentPos.Down(),
                        Direction.Left => currentPos.Left(),
                        Direction.Up => currentPos.Up(),
                        _ => throw new InvalidOperationException("Invalid direction")
                    };

                    var newDirection = direction;
                    if (CrossingFaces(newPos))
                    {
                        var quadrantWidth = map.Length / 4;
                        switch ($"{currentPos.ToQuadrant()}->{currentPos.NextQuadrant(direction)}")
                        {
                            case "1->4":
                                newPos.X = 0;
                                newPos.Y = 3*quadrantWidth-1 - currentPos.Y;
                                newDirection = Direction.Right;
                                break;
                            case "1->6":
                                newPos.X = 0;
                                newPos.Y = 2*quadrantWidth + currentPos.X;
                                newDirection = Direction.Right;
                                break;
                            case "2->3":                                
                                newPos.X = 2 * quadrantWidth - 1;
                                newPos.Y = currentPos.X - 50;
                                newDirection = Direction.Left;
                                break;
                            case "2->5":
                                newPos.X = 2 * quadrantWidth - 1;
                                newPos.Y = 3 * quadrantWidth - 1 - currentPos.Y;
                                newDirection = Direction.Left;
                                break;
                            case "2->6":
                                newPos.X = currentPos.X - 2 * quadrantWidth;
                                newPos.Y = 4 * quadrantWidth - 1;
                                newDirection = Direction.Up;
                                break;
                            case "3->2":
                                newPos.X = currentPos.Y + quadrantWidth;
                                newPos.Y = quadrantWidth - 1;
                                newDirection = Direction.Up;
                                break;
                            case "3->4":
                                newPos.X = currentPos.Y - quadrantWidth;
                                newPos.Y = 2 * quadrantWidth;
                                newDirection = Direction.Down;
                                break;
                            case "4->3":
                                newPos.X = quadrantWidth;
                                newPos.Y = quadrantWidth + currentPos.X;
                                newDirection = Direction.Right;
                                break;
                            case "4->1":
                                newPos.X = quadrantWidth;
                                newPos.Y = 3 * quadrantWidth - 1 - currentPos.Y;
                                newDirection= Direction.Right;
                                break;
                            case "5->2":
                                newPos.X = 3 * quadrantWidth - 1;
                                newPos.Y = 3 * quadrantWidth - 1 - currentPos.Y;
                                newDirection = Direction.Left;
                                break;
                            case "5->6":
                                newPos.X = quadrantWidth - 1;
                                newPos.Y = 2 * quadrantWidth + currentPos.X;
                                newDirection = Direction.Left;
                                break;
                            case "6->1":
                                newPos.X = currentPos.Y - 2 * quadrantWidth;
                                newPos.Y = 0;
                                newDirection = Direction.Down;
                                break;
                            case "6->2":
                                newPos.X = currentPos.X + 2 * quadrantWidth;
                                newPos.Y = 0;
                                newDirection = Direction.Down;
                                break;
                            case "6->5":
                                newPos.X = currentPos.Y - 2 * quadrantWidth;
                                newPos.Y = 3 * quadrantWidth - 1;
                                newDirection = Direction.Up;
                                break;

                            case "1->2":
                            case "1->3":
                            case "2->1":
                            case "3->1":
                            case "3->5":
                            case "4->5":
                            case "4->6":
                            case "5->3":
                            case "5->4":
                            case "6->4":
                                //All good :)
                                break;
                            default:
                                throw new InvalidOperationException($"Illegal move {currentPos.ToQuadrant()}->{newPos.ToQuadrant()}");
                        }
                    }

                    if (map[newPos.Y][newPos.X] == '.')
                    {
                        currentPos = newPos;
                        direction = newDirection;
                    }
                }
            }

            private static bool CrossingFaces(Coord newPos)
            {
                var quadrantWidth = map.Length / 4;
                if ((newPos.X % quadrantWidth == 0 && currentPos.X % quadrantWidth == quadrantWidth - 1) ||
                    (newPos.X % quadrantWidth == quadrantWidth - 1 && currentPos.X % quadrantWidth == 0))
                    return true;

                if ((newPos.Y % quadrantWidth == 0 && currentPos.Y % quadrantWidth == quadrantWidth - 1) ||
                    (newPos.Y % quadrantWidth == quadrantWidth - 1 && currentPos.Y % quadrantWidth == 0))
                    return true;

                if (newPos.X < 0 || newPos.Y < 0)
                    return true;

                return false;
            }
        }

        internal class Puzzle1
        {
            public static void MoveForward(int steps)
            {
                for (int j = 0; j < steps; j++)
                {
                    var newPos = direction switch
                    {
                        Direction.Right => currentPos.Right(),
                        Direction.Down => currentPos.Down(),
                        Direction.Left => currentPos.Left(),
                        Direction.Up => currentPos.Up(),
                        _ => throw new InvalidOperationException("Invalid direction")
                    };

                    if (OutOfBounds(newPos) || map[newPos.Y][newPos.X] == ' ' || map[newPos.Y][newPos.X] == '\0')
                    {
                        switch (direction)
                        {
                            case Direction.Right:
                                for (int i = 0; i < map.First().Length; i++)
                                    if (map[newPos.Y][i] == '.' || map[newPos.Y][i] == '#')
                                    {
                                        newPos.X = i;
                                        break;
                                    }
                                break;

                            case Direction.Down:
                                for (int i = 0; i < map.Length; i++)
                                    if (map[i][newPos.X] == '.' || map[i][newPos.X] == '#')
                                    {
                                        newPos.Y = i;
                                        break;
                                    }
                                break;

                            case Direction.Left:
                                for (int i = map.First().Length - 1; i >= 0; i--)
                                    if (map[newPos.Y][i] == '.' || map[newPos.Y][i] == '#')
                                    {
                                        newPos.X = i;
                                        break;
                                    }
                                break;

                            case Direction.Up:
                                for (int i = map.Length - 1; i >= 0; i--)
                                    if (map[i][newPos.X] == '.' || map[i][newPos.X] == '#')
                                    {
                                        newPos.Y = i;
                                        break;
                                    }
                                break;
                        }
                    }

                    if (map[newPos.Y][newPos.X] == '.')
                        currentPos = newPos;
                }
            }

            private static bool OutOfBounds(Coord position)
            {
                if (position.X < 0 || position.X >= map.First().Length ||
                    position.Y < 0 || position.Y >= map.Length)
                    return true;

                return false;
            }
        }

        internal enum Direction { Right = 0, Down = 1, Left = 2, Up = 3 }

        internal struct Coord
        {
            public int X;
            public int Y;
            public Coord(int y, int x) { this.X = x; this.Y = y; }
            public Coord Up() => new Coord(Y - 1, X);
            public Coord Down() => new Coord(Y + 1, X);
            public Coord Left() => new Coord(Y, X - 1);
            public Coord Right() => new Coord(Y, X + 1);

            //Quadrants:
            // . | 1 | 2
            // . | 3 | .
            // 4 | 5 | .
            // 6 | . | .
            public int ToQuadrant()
            {
                if (X < 0 || X >= map.First().Length || Y < 0 || Y >= map.Length ||
                    map[Y][X] == ' ' || map[Y][X] == '\0')
                    throw new InvalidOperationException("This method only applies to coordinates inside the cube");

                var quadrantWidth = map.Length / 4;
                if (X < quadrantWidth)
                    if (Y < 3 * quadrantWidth)
                        return 4;
                    else
                        return 6;
                else if (X >= 2 * quadrantWidth)
                    return 2;
                else if (Y < quadrantWidth)
                    return 1;
                else if (Y >= 2 * quadrantWidth)
                    return 5;
                else
                    return 3;
            }
            public int NextQuadrant(Direction direction)
            {
                switch (this.ToQuadrant())
                {
                    case 1:
                        switch (direction)
                        {
                            case Direction.Left: return 4;
                            case Direction.Right: return 2;
                            case Direction.Up: return 6;
                            case Direction.Down: return 3;
                        }
                        break;
                    case 2:
                        switch (direction)
                        {
                            case Direction.Left: return 1;
                            case Direction.Right: return 5;
                            case Direction.Up: return 6;
                            case Direction.Down: return 3;
                        }
                        break;
                    case 3:
                        switch (direction)
                        {
                            case Direction.Left: return 4;
                            case Direction.Right: return 2;
                            case Direction.Up: return 1;
                            case Direction.Down: return 5;
                        }
                        break;
                    case 4:
                        switch (direction)
                        {
                            case Direction.Left: return 1;
                            case Direction.Right: return 5;
                            case Direction.Up: return 3;
                            case Direction.Down: return 6;
                        }
                        break;
                    case 5:
                        switch (direction)
                        {
                            case Direction.Left: return 4;
                            case Direction.Right: return 2;
                            case Direction.Up: return 3;
                            case Direction.Down: return 6;
                        }
                        break;
                    case 6:
                        switch (direction)
                        {
                            case Direction.Left: return 1;
                            case Direction.Right: return 5;
                            case Direction.Up: return 4;
                            case Direction.Down: return 2;
                        }
                        break;
                }

                return -1;
            }
        }

    }
}