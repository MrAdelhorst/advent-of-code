var connections = File.ReadAllLines(@"..\..\..\..\day-25.txt").Select(line =>
{
    var parts = line.Split(": ");
    return new KeyValuePair<string, string[]>(parts[0], parts[1].Split(" ").ToArray());
}).ToDictionary();

var allNodes = connections.Keys.Union(connections.Values.SelectMany(v => v.Select(w => w))).Distinct().ToArray();
var allConnections = allNodes.Select(i => (i, (connections.TryGetValue(i, out string[]? value) ? value : new string[0]).Union(connections.Where(c => c.Value.Contains(i)).Select(c => c.Key)).ToArray())).ToDictionary();
var individualConnections = connections.SelectMany(i => i.Value.Select(j => (new[] { i.Key, j }))).Distinct(new StringArrayComparer()).ToArray();

//Iterate all nodes and calculate the coherence between nearby nodes
var connectionCount = allConnections.Select(conn => (Root: conn.Key, NodesInGroup: allConnections.ConnectionCount(conn.Key, depth: 4))).ToList();
//Corellate cohesion for each individual connection and find the "weakest links"
var allStrengths = individualConnections.SelectMany(v => new[] { (x1: v.First(), x2: v.Last()), (x1: v.Last(), x2: v.First()) }.Select(c =>
                    connectionCount.Select(x =>
                    {
                        var node = x.NodesInGroup.SingleOrDefault(y => y.node == c.x1);
                        if (node != default)
                        {
                            var conn = node.Item2.SingleOrDefault(y => y.connection == c.x2);
                            if (conn != default)
                                return (node.node, conn.connection, conn.coherence);
                        }
                        return default;
                    }).Where(x => x != default).ToArray())).ToArray()
                    .GroupBy(x => new[] { x.First().node, x.First().connection }, new StringArrayComparer()).ToArray().Select(x =>
                    {
                        var flattened = x.SelectMany(z => z).ToArray();
                        return (flattened.First().node, flattened.First().connection, AvgCoherence: flattened.Average(y => y.coherence));
                    }).OrderBy(x => x.AvgCoherence).ToArray();

var top3 = allStrengths.Take(3).ToArray();

foreach (var connection in top3)
{
    allConnections[connection.node] = allConnections[connection.node].Except([connection.connection]).ToArray();
    allConnections[connection.connection] = allConnections[connection.connection].Except([connection.node]).ToArray();
}

var group1 = allConnections.FindAllConnectedTo(top3.First().node);
var group2 = allConnections.FindAllConnectedTo(top3.First().connection);

Console.WriteLine($"Product of group sizes: {group1.Length * group2.Length}");
Console.ReadLine();

class StringArrayComparer : IEqualityComparer<string[]>
{
    public bool Equals(string[]? x, string[]? y)
    {
        if (x == null && y == null)
            return true;
        if (x == null || y == null)
            return false;
        return x.OrderBy(i => i).SequenceEqual(y.OrderBy(i => i));
    }

    public int GetHashCode(string[] obj)
    {
        return obj.OrderBy(i => i).Aggregate(0, (a, b) => a ^ b.GetHashCode());
    }
}

static class Extensions
{
    public static string[] FindAllConnectedTo(this Dictionary<string, string[]> connections, string node)
    {
        var result = new List<string>();

        var queue = new Queue<string>([node]);
        while (queue.Any())
        {
            var current = queue.Dequeue();
            if (!result.Contains(current))
            {
                result.Add(current);
                foreach (var next in connections[current])
                {
                    queue.Enqueue(next);
                }
            }
        }

        return result.ToArray();
    }

    public static (string node, (string connection, int coherence)[])[] ConnectionCount(this Dictionary<string, string[]> connections, string startNode, int depth)
    {
        var group = new[] { startNode };
        for (int i = 0; i < depth; i++)
        {
            group = group.Union(group.SelectMany(item => connections[item])).Distinct().ToArray();
        }

        return group.Skip(1).Select(member => (member, connections[member].Select(connection =>
                (connection, group.Count(g => g != member && connections[connection].Contains(g)))).ToArray())).ToArray();
    }
}