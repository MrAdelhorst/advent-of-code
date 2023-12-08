using System.ComponentModel.Design;
using System.Reflection;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            {
                //Puzzle1();
                Puzzle2();
            }
        }

        private static void Puzzle2()
        {
            var trees = File.ReadAllLines(@"c:\data\aoc\8\aoc8-1.txt");
            var rows = trees.Length;
            var cols = trees.First().Length;
            var grid = new int[rows, cols];

            for (int col = 0; col < cols; col++)
                for (int row = 0; row < rows; row++)
                    grid[row, col] = NorthView(row, col, trees) * SouthView(row, col, trees, rows) * WestView(row, col, trees) * EastView(row, col, trees, cols);

            var scenicScore = Flatten(grid).Max(e => e);

            Console.Write($"Highest scenic score: {scenicScore}");
            Console.ReadLine();
        }

        private static int EastView(int row, int col, string[] trees, int totalCols)
        {
            int score = 0;
            var height = trees[row][col] - '0';
            var currentCol = col;
            while (++currentCol < totalCols)
            {
                score++;
                if (height <= trees[row][currentCol] - '0')
                    break;
            } 

            return score;
        }

        private static int SouthView(int row, int col, string[] trees, int totalRows)
        {
            int score = 0;
            var height = trees[row][col] - '0';
            var currentRow = row;
            while (++currentRow < totalRows)
            {
                score++;
                if (height <= trees[currentRow][col] - '0')
                    break;
            }

            return score;
        }

        private static int WestView(int row, int col, string[] trees)
        {
            int score = 0;
            var height = trees[row][col] - '0';
            var currentCol = col;
            while (--currentCol >= 0)
            {
                score++;
                if (height <= trees[row][currentCol] - '0')
                    break;
            }

            return score;
        }

        private static int NorthView(int row, int col, string[] trees)
        {
            int score = 0;
            var height = trees[row][col] - '0';
            var currentRow = row;
            while (--currentRow >= 0)
            {
                score++;
                if (height <= trees[currentRow][col] - '0')
                    break;
            }

            return score;
        }

        private static void Puzzle1()
        {
            var trees = File.ReadAllLines(@"c:\data\aoc\8\aoc8-1.txt");
            var rows = trees.Length;
            var cols = trees.First().Length;
            var grid = new bool[rows, cols];

            //From the top
            for (int col = 0; col < cols; col++)
            {
                var height = -1;
                for (int row = 0; row < rows; row++)
                {
                    var current = trees[row][col] - '0';
                    if (current > height)
                    {
                        height = current;
                        grid[row, col] = true;
                    }
                }
            }

            //From the bottom
            for (int col = 0; col < cols; col++)
            {
                var height = -1;
                for (int row = rows - 1; row >= 0; row--)
                {
                    var current = trees[row][col] - '0';
                    if (current > height)
                    {
                        height = current;
                        grid[row, col] = true;
                    }
                }
            }

            //From the left
            for (int row = 0; row < rows; row++)
            {
                var height = -1;
                for (int col = 0; col < cols; col++)
                {
                    var current = trees[row][col] - '0';
                    if (current > height)
                    {
                        height = current;
                        grid[row, col] = true;
                    }
                }
            }

            //From the right
            for (int row = 0; row < rows; row++)
            {
                var height = -1;
                for (int col = cols - 1; col >= 0; col--)
                {
                    var current = trees[row][col] - '0';
                    if (current > height)
                    {
                        height = current;
                        grid[row, col] = true;
                    }
                }
            }

            var visible = Flatten(grid).Count(e => e);

            Console.Write($"Visible: {visible}");
            Console.ReadLine();
        }

        public static IEnumerable<T> Flatten<T>(T[,] map)
        {
            for (int row = 0; row < map.GetLength(0); row++)
            {
                for (int col = 0; col < map.GetLength(1); col++)
                {
                    yield return map[row, col];
                }
            }
        }
    }
}