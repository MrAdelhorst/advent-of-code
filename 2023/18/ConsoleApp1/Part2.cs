namespace ConsoleApp1
{
    internal static class Part2
    {
        public static long Run(IEnumerable<(char direction, int distance, string colour)> orders, int width, int height, Point start)
        {
            //Convert based on the new instructions
            orders = orders.Select(order =>
            {
                var direction = order.colour[7] switch
                {
                    '0' => 'R',
                    '1' => 'D',
                    '2' => 'L',
                    '3' => 'U',
                    _ => throw new InvalidOperationException("Unknown direction")
                };
                return (direction, Convert.ToInt32(new string(order.colour.Skip(2).Take(5).ToArray()), 16), string.Empty);
            });

            var current = start;
            var allVectors = new List<(Point from, Point to, Direction direction)> { };
            foreach (var order in orders)
            {
                var vector = order.direction switch
                {
                    'R' => (current, new Point(current.X + order.distance, current.Y), Direction.Horizontal),
                    'L' => (current, new Point(current.X - order.distance, current.Y), Direction.Horizontal),
                    'D' => (current, new Point(current.X, current.Y + order.distance), Direction.Vertical),
                    'U' => (current, new Point(current.X, current.Y - order.distance), Direction.Vertical),
                    _ => throw new InvalidOperationException("Unknown direction")
                };

                current = vector.Item2;
                allVectors.Add(vector);
            }
            long area = 0;

            //extend the horizontal lines to cover the full width and then map to vertical lines to identify squares
            var allHorizontalLines = allVectors.Where(v => v.direction == Direction.Horizontal).Select(v => v.from.Y).Distinct().Order().ToArray();
            var allVerticalLines = allVectors.Where(v => v.direction == Direction.Vertical).ToArray();

            var previousXValues = Enumerable.Empty<int>();
            for (int i=0; i<allHorizontalLines.Count()-1; i++)
            {
                var xValues = allVerticalLines.Where(v =>
                {
                    var min = Math.Min(v.from.Y, v.to.Y);
                    var max = Math.Max(v.from.Y, v.to.Y);
                    return min <= allHorizontalLines[i] && max >= allHorizontalLines[i + 1];
                }).Select(v => v.from.X).Order().AsEnumerable();
                long yDistance = allHorizontalLines[i + 1] - allHorizontalLines[i] + 1;

                var tmp = xValues;
                while (tmp.Any())
                {
                    var twoX = tmp.Take(2);
                    long xDistance = twoX.Last() - twoX.First() + 1;
                    tmp = tmp.Skip(2);
                    area += xDistance * yDistance;

                    //Remove the overlapping parts at the top edge where they overlap with the previous line of squares
                    area -= previousXValues.FindOverlap(twoX.First(), twoX.Last());
                }

                previousXValues = xValues.ToArray();
            }

            return area;
        }
    }

    enum Direction { Horizontal, Vertical }
}

static class ExtensionsPart2
{
    public static int FindOverlap(this IEnumerable<int> xValues, int start, int end)
    {
        int overlap = 0;
        while (xValues.Any())
        {
            var twoX = xValues.Take(2);
            if (twoX.First() < end && twoX.Last() > start)
                overlap += Math.Min(twoX.Last(), end) - Math.Max(twoX.First(), start) + 1;

            xValues = xValues.Skip(2);
        }
        return overlap;
    }
}