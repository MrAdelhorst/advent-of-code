var connections = File.ReadAllLines(@"..\..\..\..\day-25.txt").Select(line =>
{
    var parts = line.Split(": ");
    return new KeyValuePair<string, string[]>(parts[0], parts[1].Split(" ").ToArray());
}).ToDictionary();

var allNodes = connections.Keys.Union(connections.Values.SelectMany(v => v.Select(w => w))).Distinct().ToArray();
var allConnections = allNodes.Select(i => (i, (connections.TryGetValue(i, out string[]? value) ? value : new string[0]).Union(connections.Where(c => c.Value.Contains(i)).Select(c => c.Key)).ToArray())).ToDictionary();
var individualConnections = connections.SelectMany(i => i.Value.Select(j => (new[] { i.Key, j }))).Distinct(new StringArrayComparer()).ToArray();

//Traverse random trips across the mesh to identify the 3 "bridges" that connect the two main areas
//The bridges will be the connections with the most traffic
var visited = individualConnections.Select(i => (connection:i, visited:0)).ToDictionary(new StringArrayComparer());
var randomTrips = Enumerable.Range(0, 1000).Select(_ => (allConnections.RandomNode(), allConnections.RandomNode())).ToArray();
foreach (var trip in randomTrips)
{
    var path = allConnections.FindShortestPath(trip.Item1, trip.Item2);
    for (int i = 1; i < path.Length; i++)
    {
        var connection = new[] { path[i - 1], path[i] };
        visited[connection]++;
    }
}

var top3 = visited.OrderByDescending(v => v.Value).Take(3);
foreach (var connection in top3)
{
    allConnections[connection.Key.First()] = allConnections[connection.Key.First()].Except([connection.Key.Last()]).ToArray();
    allConnections[connection.Key.Last()] = allConnections[connection.Key.Last()].Except([connection.Key.First()]).ToArray();
}

var group1 = allConnections.FindAllConnectedTo(top3.First().Key.First());
var group2 = allConnections.FindAllConnectedTo(top3.First().Key.Last());

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
    public static string RandomNode(this Dictionary<string, string[]> connections)
    {
        return connections.Keys.ElementAt(Random.Shared.Next(connections.Count));
    }

    public static string[] FindShortestPath(this Dictionary<string, string[]> connections, string start, string end)
    {
        var queue = new Queue<string>([start]);
        var came_from = new Dictionary<string, string>([new KeyValuePair<string, string>(start, string.Empty)]);
        while (queue.Any())
        {
            var current = queue.Dequeue();
            if (current == end)
                break;

            foreach (var next in connections[current])
            {
                if (!came_from.ContainsKey(next))
                {
                    queue.Enqueue(next);
                    came_from[next] = current;
                }
            }
        }

        var trip = new List<string>();
        var node = end;
        while (node != string.Empty)
        {
            trip.Add(node);
            node = came_from[node];
        }

        return trip.ToArray();
    }

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
}