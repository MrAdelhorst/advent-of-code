using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {
        internal const int OK = 100;
        internal static DirEntry current;
        internal static DirEntry root = new DirEntry("<root>", null);
        internal static Dictionary<string, int> Sizes = new Dictionary<string, int>();

        static void Main(string[] args)
        {
            foreach (var line in File.ReadAllLines(@"c:\data\aoc\7\aoc7-1.txt"))
            {
                var x = line.Split(' ') switch
                {
                    ["$", "ls"] => OK,
                    ["$", "cd", var arg] => ChangeDir(arg),
                    ["dir", var name] => AddDir(name),
                    [var size, var filename] => AddFile(filename, size),
                    _ => throw new ArgumentException($"command not covered: {line}"),
                };
            }

            var rootSize = RecordDirSizes(root);

            //Puzzle 1
            //var points = Sizes.Where(s => s.Value < 100000).Sum(e => e.Value);
            //Console.WriteLine($"Result: {points}");

            //Puzzle 2
            const int fullSpace = 70000000;
            const int requiredSpace = 30000000;

            var freeSpace = fullSpace - rootSize;
            var missingSpace = requiredSpace - freeSpace;

            var sizeToDelete = Sizes.OrderBy(e => e.Value).First(e => e.Value > missingSpace).Value;

            Console.WriteLine($"Missing space: {missingSpace}, Space being cleaned up: {sizeToDelete}");

            Console.ReadLine();
        }

        private static int RecordDirSizes(DirEntry dir)
        {
            var fileSizes = dir.Entries.Where(e => e is FileEntry).Sum(e => (e as FileEntry).Size);
            var dirSizes = dir.Entries.Where(e => e is DirEntry).Sum(e => RecordDirSizes(e as DirEntry));
            var total = fileSizes + dirSizes;

            var fullName = dir.GetQualifiedName();

            Sizes.Add(fullName, total);
            return total;
        }

        private static int AddFile(string filename, string size)
        {
            current.Entries.Add(new FileEntry(filename, int.Parse(size)));
            return OK;
        }

        private static int AddDir(string name)
        {
            current.Entries.Add(new DirEntry(name, current));
            return OK;
        }

        private static int ChangeDir(string dir)
        {
            switch (dir)
            {
                case "/":
                {
                    current = root;
                    break;
                }
                case "..":
                {
                    current = current.Parent;
                    break;
                }
                default:
                {
                    current = current.Entries.Single(e => e.Name == dir) as DirEntry;
                    break;
                }
            }
            return OK;
        }

        internal abstract class Entry
        {
            public string Name;
        }

        internal class FileEntry : Entry
        {
            public int Size;

            public FileEntry(string name, int size) { this.Name = name; this.Size = size; }
        }

        internal class DirEntry : Entry
        {
            public List<Entry> Entries = new List<Entry>();
            public DirEntry Parent;

            public DirEntry(string name, DirEntry parent) { this.Name = name; this.Parent = parent; }

            public string GetQualifiedName()
            {
                return (Parent != null) ? $"{Parent.GetQualifiedName()}\\{this.Name}" : this.Name;
            }
        }
    }
}
