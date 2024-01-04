namespace ConsoleApp1
{
    internal static class Part1
    {
        public static long Run(IEnumerable<(char direction, int distance, string colour)> orders, int width, int height, Point start)
        {
            var grid = Enumerable.Range(0, height).Select(_ => new string('.', width).ToArray()).ToArray();
            var current = start;
            grid[current.Y][current.X] = '#';

            foreach (var order in orders)
            {
                var trench = order.direction switch
                {
                    'R' => Enumerable.Range(current.X + 1, order.distance).Select(x => new Point(x, current.Y)),
                    'L' => Enumerable.Range(current.X - (order.distance), order.distance).Reverse().Select(x => new Point(x, current.Y)),
                    'D' => Enumerable.Range(current.Y + 1, order.distance).Select(y => new Point(current.X, y)),
                    'U' => Enumerable.Range(current.Y - (order.distance), order.distance).Reverse().Select(y => new Point(current.X, y)),
                    _ => throw new InvalidOperationException("Unknown direction")
                };

                current = trench.Last();
                foreach (var point in trench)
                    grid[point.Y][point.X] = '#';
            }

            //Add surrounding border of '.'s
            var lastX = grid.First().Length - 1;
            var lastY = grid.Length - 1;
            var emptyLine = new string('.', grid.First().Length + 2);
            grid = grid.SelectMany((line, idx) =>
            {
                var newLine = line.SelectMany((c, ix) =>
                    (ix == 0) ? new[] { '.', c } : (ix == lastX) ? new[] { c, '.' } : new[] { c }).ToArray();
                return (idx == 0) ? new[] { emptyLine.ToArray(), newLine } : (idx == lastY) ? new[] { newLine, emptyLine.ToArray() } : new[] { newLine };
            }).ToArray();

            //Fill all connected spaces with ' '
            var queue = new Queue<Point>([new Point(0, 0)]);
            while (queue.Any())
            {
                var location = queue.Dequeue();
                grid[location.Y][location.X] = '*';
                foreach (var adjacent in new[] { location.East(), location.West(), location.North(), location.South() }.Where(a => grid.CanMoveTo(a) && !queue.Contains(a)))
                    queue.Enqueue(adjacent);
            }

            return grid.Sum(line => line.Count(c => c == '#' || c == '.'));
        }
    }

    record Point(int X, int Y);

    static class Extensions
    {
        public static Point East(this Point point) => new Point(point.X + 1, point.Y);
        public static Point West(this Point point) => new Point(point.X - 1, point.Y);
        public static Point North(this Point point) => new Point(point.X, point.Y - 1);
        public static Point South(this Point point) => new Point(point.X, point.Y + 1);

        public static bool CanMoveTo(this char[][] map, Point point)
        {
            return (point.X >= 0 && point.X < map.First().Length && point.Y >= 0 && point.Y < map.Length && map[point.Y][point.X] == '.');
        }
    }
}
