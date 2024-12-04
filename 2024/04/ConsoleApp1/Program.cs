using System.Collections;

var matrix = File.ReadAllLines(@"..\..\..\..\day-04.txt")
    .Select(line => line.ToArray()).ToArray();

Console.WriteLine($"Part 1 - Xmas count: {matrix.Select((row, x) => row.Select((_, y) => matrix.CountXmas(x, y)).Sum()).Sum()}");
Console.WriteLine($"Part 2 - X-MAS count: {matrix.Select((row, x) => row.Select((_, y) => matrix.IsX_mas(x, y) ? 1 : 0).Sum()).Sum()}");
Console.ReadLine();

internal static class Extensions
{
    public static int CountXmas(this char[][] matrix, int x, int y)
    {
        var count = 0;

        //right
        if (x < matrix.Length - 3 && new[] { matrix[x][y], matrix[x + 1][y], matrix[x + 2][y], matrix[x + 3][y] }.IsXmas())
            count++;

        //down
        if (y < matrix[x].Length - 3 && new[] { matrix[x][y], matrix[x][y + 1], matrix[x][y + 2], matrix[x][y + 3] }.IsXmas())
            count++;

        //right & down
        if (x < matrix.Length - 3 && y < matrix[x].Length - 3 && new[] { matrix[x][y], matrix[x + 1][y + 1], matrix[x + 2][y + 2], matrix[x + 3][y + 3] }.IsXmas())
            count++;

        //right & up
        if (x < matrix.Length - 3 && y > 2 && new[] { matrix[x][y], matrix[x + 1][y - 1], matrix[x + 2][y - 2], matrix[x + 3][y - 3] }.IsXmas())
            count++;

        return count;
    }

    public static bool IsX_mas(this char[][] matrix, int x, int y)
    {
        var count = 0;

        if (x == matrix.Length - 1 || x == 0 || y == matrix[x].Length - 1 || y == 0)
            return false;

        //right & down
        if (new[] { matrix[x - 1][y - 1], matrix[x][y], matrix[x + 1][y + 1] }.IsMas())
            count++;

        //right & up
        if (new[] { matrix[x - 1][y + 1], matrix[x][y], matrix[x + 1][y - 1] }.IsMas())
            count++;

        return count == 2;
    }

    static readonly char[] xmas = new[] { 'X', 'M', 'A', 'S' };
    static readonly char[] samx = new[] { 'S', 'A', 'M', 'X' };
    public static bool IsXmas(this char[] input)
    {
        return AreEqual(input, xmas) || AreEqual(input, samx);
    }

    static readonly char[] mas = new[] { 'M', 'A', 'S' };
    static readonly char[] sam = new[] { 'S', 'A', 'M' };
    public static bool IsMas(this char[] input)
    {
        return AreEqual(input, mas) || AreEqual(input, sam);
    }

    public static bool AreEqual<T>(this T a, T b)
    where T : IStructuralEquatable
    {
        return a.Equals(b, StructuralComparisons.StructuralEqualityComparer);
    }
}