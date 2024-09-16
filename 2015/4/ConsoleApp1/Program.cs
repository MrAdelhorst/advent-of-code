using System.Security.Cryptography;
using System.Text;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var key = "ckczppom";
            var res = new byte[] { 0x1 };
            var suffix = -1;
            while (!BitConverter.ToString(res).StartsWith("00-00-00"))
            {
                suffix++;
                var input = ASCIIEncoding.ASCII.GetBytes(key + suffix.ToString());
                res = MD5.HashData(input);
            }

            Console.Write($"Suffix: {suffix}  -  Hash:{BitConverter.ToString(res)}");
            Console.ReadLine();
        }
    }
}