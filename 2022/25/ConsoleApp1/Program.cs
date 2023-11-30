using System.Numerics;
using System.Runtime.CompilerServices;

namespace ConsoleApp1
{
    internal class Program
    {
        public static Dictionary<char, long> Mapping = new[]
            {
                new { SnafuDigit = '=', Base10Value = (long) -2},
                new { SnafuDigit = '-', Base10Value = (long) -1},
                new { SnafuDigit = '0', Base10Value = (long) 0},
                new { SnafuDigit = '1', Base10Value = (long) 1},
                new { SnafuDigit = '2', Base10Value = (long) 2},
            }.ToDictionary((m => m.SnafuDigit), (m=>m.Base10Value));

        static void Main(string[] args)
        {
            long total = 0;
            foreach (var line in File.ReadAllLines(@"c:\data\aoc\25\aoc25-1.txt"))
            {
                total += line.ToBase10();
            }

            Console.WriteLine($"Total fuel needed - base 10: {total}, SNAFU: {total.ToSnafu()}");
        }
    }

    internal static class Extensions
    {
        public static long ToBase10 (this string snafu)
        {
            long result = 0;
            long baseValue = 5;
            for (int i = snafu.Length - 1; i >= 0; i--)
            {
                result += (long) Math.Pow(baseValue, snafu.Length - 1 - i) * Program.Mapping.Single(m => m.Key == snafu[i]).Value;
            }

            return result;
        }

        public static string ToSnafu (this long base10)
        {
            int noOfDigits = -1;
            while (MaxValue(++noOfDigits) < base10);

            var remaining = base10;
            var result = new char[noOfDigits];
            for (int i = noOfDigits; i > 0; i--)
            {
                var digitValue = (long) Math.Pow(5, i-1);
                if (remaining > digitValue + MaxValue(i-1))
                    result[noOfDigits - i] = '2';
                else if (remaining > MaxValue(i - 1))
                    result[noOfDigits - i] = '1';
                else if (remaining >= -MaxValue (i-1))
                    result[noOfDigits - i] = '0';
                else if (remaining + digitValue + MaxValue(i -1) < 0)
                    result[noOfDigits - i] = '=';
                else
                    result[noOfDigits - i] = '-';

                remaining -= Program.Mapping.Single(m => m.Key == result[noOfDigits - i]).Value * digitValue;
            }

            return string.Concat(result);
        }

        private static long MaxValue(int digits)
        {
            if (digits > 1)
                return (long)Math.Pow(5, digits - 1) * 2 + MaxValue(digits - 1);
            else
                return (long)Math.Pow(5, digits - 1) * 2;
        }
    }
}