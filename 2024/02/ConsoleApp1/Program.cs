var reports = File.ReadAllLines(@"..\..\..\..\day-02.txt")
    .Select(x => x.Split(' ').Select(int.Parse).ToList());

static bool Valid(List<int> report)
{
    bool ascending = report[1] > report[0];
    return report.GetRange(1, report.Count - 1)
        .Aggregate(report[0], (prev, current) =>
        {
            var diff = current - prev;
            return Math.Abs(diff) > 0 
                && Math.Abs(diff) < 4 
                && diff > 0 == ascending
                    ? current
                    : -1000;
        }) != -1000;
}

static bool Valid2(List<int> report)
{
    return Enumerable.Range(0, report.Count)
        .Select(idx =>
        {
            var tmp = new List<int>(report);
            tmp.RemoveAt(idx);
            return tmp;
        })
        .Any(Valid);
}

Console.WriteLine($"Part 1 - Valid reports: {reports.Count(Valid)}");
Console.WriteLine($"Part 2 - Valid reports: {reports.Count(Valid2)}");
Console.ReadLine();