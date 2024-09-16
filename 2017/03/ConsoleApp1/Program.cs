var input = 277678;

//Part 1
var northEast = 3;
var northWest = 5;
var southWest = 7;
var southEast = 9;

var growthFactor = 8;
var corners = new List<(int ring, int value)>();
var currentRing = 2;
var delta = growthFactor;
do
{
    corners.Add((ring: currentRing, value: currentRing * northWest - (currentRing - 1) + delta));
    corners.Add((ring: currentRing, value: currentRing * southWest - (currentRing - 1) + delta));
    corners.Add((ring: currentRing, value: currentRing * southEast - (currentRing - 1) + delta));
    corners.Add((ring: currentRing, value: currentRing * northEast - (currentRing - 1) + delta));
    delta += currentRing * growthFactor;
    currentRing++;
} while (corners.Last().value < input);
var last = corners[^1];
var extra = last.value - input;
var distance = Math.Abs(last.ring) * 2 - extra % Math.Abs(last.ring);

//Part 2
var currentValue = 1;
var currentSquare = new Square(new Coordinate(0, 0));
var squares = new Dictionary<Coordinate, int>([new KeyValuePair<Coordinate, int>(currentSquare.CurrentCoord, currentValue)]);
while (currentValue < input)
{
    currentSquare.Next();
    currentValue = squares.Adjacent(currentSquare.CurrentCoord).Sum();
    squares.Add(currentSquare.CurrentCoord, currentValue);
}

Console.WriteLine($"Part 1 - steps: {distance}");
Console.WriteLine($"Part 2 - next value: {currentValue}");
Console.ReadLine();

enum Direction { North, West, South, East, Undefined }

record Coordinate(int X, int Y);

class Square(Coordinate coord)
{
    public Direction nextDirection = Direction.Undefined;
    public int X => coord.X;
    public int Y => coord.Y;
    public Coordinate CurrentCoord => coord;
    public void Next()
    {
        if (nextDirection == Direction.Undefined)
        {
            coord = new Coordinate(X + 1, Y);
            nextDirection = Direction.North;
        }
        else if (nextDirection == Direction.East)
        {
            coord = new Coordinate(X + 1, Y);
            if (X > Y)
                nextDirection = Direction.North;
        }
        else
        {
            if (nextDirection == Direction.North)
                coord = new Coordinate(X, Y - 1);
            if (nextDirection == Direction.South)
                coord = new Coordinate(X, Y + 1);
            if (nextDirection == Direction.West)
                coord = new Coordinate(X - 1, Y);

            if (Math.Abs(X) == Math.Abs(Y))
                nextDirection++;
        }
    }   
}

static class Extensions
{
    public static IEnumerable<int> Adjacent(this Dictionary<Coordinate, int> squares, Coordinate coord)
    {
        var adjacentCoords = new List<Coordinate>
        {
            new(coord.X - 1, coord.Y - 1),
            new(coord.X - 1, coord.Y    ),
            new(coord.X - 1, coord.Y + 1),
            new(coord.X,     coord.Y - 1),
            new(coord.X,     coord.Y + 1),
            new(coord.X + 1, coord.Y - 1),
            new(coord.X + 1, coord.Y    ),
            new(coord.X + 1, coord.Y + 1)
        };
        return adjacentCoords.Select(c => squares.GetValueOrDefault(c, 0));
    }
}