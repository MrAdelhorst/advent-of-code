using System.Collections.Concurrent;
using System.Text.RegularExpressions;

var lines = File.ReadAllLines(@"..\..\..\..\day-19.txt");
var replacements = lines.Where(line => line.Contains("=>")).Select(line => line.Split(" => ").ToTuple());
var replacementMap = replacements.ToLookup(r => r.from, r => r.to);
var molecule = lines.Last();

//Part 1
var allMolecules = new List<string>();
foreach (var atom in replacementMap.Select(x => x.Key))
{
    var indexes = Regex.Matches(molecule, atom).Select(m => m.Index);
    foreach (var index in indexes)
    {
        var removed = molecule.Remove(index, atom.Length);
        foreach (var substitute in replacementMap[atom])
            allMolecules.Add(removed.Insert(index, substitute));
    }
}

//Part2
var reverseReplacements = replacements.OrderBy(r => r.to.Length);
var jobs = new ConcurrentStack<(string molecule, int replacements)>([(molecule, 0)]);
var done = new ConcurrentDictionary<string, byte>();
var results = new ConcurrentBag<int> { int.MaxValue };
//var result = int.MaxValue;
//var lockObj = new object();

await Task.WhenAll(Enumerable.Range(0, 15).Select(id => Task.Run(() =>
{
    Thread.Sleep(id * 1000);

    while (jobs.TryPop(out var job))
    {
        if (job.molecule == "e")
        {
            //lock (lockObj)
            //{
            //    result = Math.Min(result, job.replacements);
            //}
            results.Add(job.replacements);
        }
        else if (!done.ContainsKey(job.molecule) /*&& job.replacements < result*/)
        {
            foreach (var replacement in reverseReplacements)
                jobs.PushRange(job.molecule.ReplaceAtom(replacement.to, replacement.from).Select(m => (m, job.replacements + 1)).ToArray());

            done.TryAdd(job.molecule, 0);
        }
    }
})));

Console.WriteLine($"Part 1 - Distinct molecules: {allMolecules.Distinct().Count()}");
Console.WriteLine($"Part 2 - Fewest steps: {results.Min()}");
Console.ReadLine();

static class Extensions
{
    public static (string from, string to) ToTuple(this string[] parts) => (parts[0], parts[1]);
    
    //public static void PushRange(this Stack<(string, int)> jobs, IEnumerable<string> molecules, int iterations)
    //{
    //    foreach (var molecule in molecules)
    //        jobs.Push((molecule, iterations));
    //}

    public static IEnumerable<string> ReplaceAtom(this string molecule, string atom, string replacement)
    {
        var indexes = Regex.Matches(molecule, atom).Select(m => m.Index);
        foreach (var index in indexes)
            yield return molecule.Remove(index, atom.Length).Insert(index, replacement);
    }
}