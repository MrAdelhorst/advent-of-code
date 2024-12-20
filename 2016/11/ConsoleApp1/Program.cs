using System.Text.RegularExpressions;
using Floor = System.Collections.Generic.List<(ComponentType type, string material)>;

var chipRegex = new Regex(@"a (\w+)-compatible microchip");
var generatorRegex = new Regex(@"a (\w+) generator");

var test = new[]
{
    new Floor { (ComponentType.Microchip, "hydrogen"), (ComponentType.Microchip, "lithium") },
    new Floor { (ComponentType.Generator, "hydrogen") },
    new Floor { (ComponentType.Generator, "lithium") },
    new Floor()
};

var floors = File.ReadAllLines(@"..\..\..\..\day-11.txt")
    .Select(line => chipRegex.Matches(line)
        .Select(x => (type: ComponentType.Microchip, material: x.Groups[1].Value))
        .Concat(generatorRegex.Matches(line)
            .Select(x => (type: ComponentType.Generator, material: x.Groups[1].Value)))
        .ToList())
    .ToArray();

floors[0].Add(Extensions.elevator);
//floors[0].AddRange(new[]
//{
//    (ComponentType.Microchip, "elerium"), (ComponentType.Generator, "elerium"),
//    (ComponentType.Microchip, "dilithium"), (ComponentType.Generator, "dilithium"),
//});

Console.WriteLine($"Part 1 - Minimum steps: {FindFastest(floors)}");
Console.WriteLine($"Part 2 - Minimum steps: ");
Console.ReadLine();

int FindFastest(Floor[] floors)
{
    var history = new Dictionary<Floor[], int>(new FloorArrayComparer());

    var jobs = new PriorityQueue<(Floor[] state, int steps), int>([((floors, 0), 0)]);
    while (jobs.Count > 0)
    {
        var current = jobs.Dequeue();

        if (current.state.IsGoal())
            return current.steps;

        //var hash = current.state.Hash();
        if (!history.TryGetValue(current.state, out var steps) || current.steps < steps)
        {
            history[current.state] = current.steps;

            foreach (var candidate in current.state.Candidates().Where(Valid))
                jobs.Enqueue((candidate, current.steps + 1), candidate.Distance());
        }
    }

    return -1;
}

bool Valid(Floor[] floors)
{
    foreach (var floor in floors)
    {
        var chips = floor.Where(x => x.type == ComponentType.Microchip);
        var generators = floor.Where(x => x.type == ComponentType.Generator);
        foreach (var chip in chips)
        {
            if (generators.Any() && !generators.Any(x => x.material == chip.material))
                return false;
        }
        //if (chips.Any(x => floor.Any(y => y.type == ComponentType.Generator && y.material != x.material) 
        //    && !floor.Any(y => y.type == ComponentType.Generator && y.material == x.material)))
        //    return false;
    }

    return true;
}

public enum ComponentType { Generator, Microchip, Elevator }

internal static class Extensions
{
    public static readonly (ComponentType type, string material) elevator = (ComponentType.Elevator, string.Empty);

    public static int Weight(this ComponentType type) =>
        type switch
        {
            ComponentType.Microchip => 3,
            ComponentType.Generator => 1,
            ComponentType.Elevator => 0,
            _ => throw new InvalidOperationException()
        };

    public static int Distance(this Floor[] floors)
    {
        var sum = 0;
        for (int i = 0; i < floors.Length; i++)
        {
            sum += floors[i].Select(x => (int) Math.Pow(floors.Length - i, 2) * x.type.Weight()).Sum();
        }

        return sum;
    }

    public static bool IsGoal(this Floor[] floors)
    {
        return floors[0..^1].All(floor => floor.Count == 0);
    }

    public static int Hash(this Floor[] floors)
    {
        int hash = 11;
        foreach (var floor in floors)
            hash *= 23 ^ floor.Order().Hash();

        return hash;
    }

    public static IEnumerable<Floor[]> Candidates(this Floor[] floors)
    {
        var elevatorFloor = floors.ElevatorFloor();
        var components = floors[elevatorFloor].Where(c => c != elevator).ToArray();
        var combinations = components
            .SelectMany((component, i) => components.Skip(i + 1)
                .Select(otherComponent => new[] { component, otherComponent }))
            .Union(components.Select(x => new[] { x }))
            .ToList();

        var candidates = new List<Floor[]>();
        if (elevatorFloor > 0)
        {
            candidates.AddRange(combinations.Select(c =>
            {
                var tmp = floors.CloneFloor();
                tmp[elevatorFloor] = tmp[elevatorFloor].Remove(c.Union([elevator]));
                tmp[elevatorFloor - 1] = tmp[elevatorFloor - 1].Add(c.Union([elevator]));
                return tmp;
            }));
        }

        if (elevatorFloor < floors.Length - 1)
        {
            candidates.AddRange(combinations.Select(c =>
            {
                var tmp = floors.CloneFloor();
                tmp[elevatorFloor] = tmp[elevatorFloor].Remove(c.Union([elevator]));
                tmp[elevatorFloor + 1] = tmp[elevatorFloor + 1].Add(c.Union([elevator]));
                return tmp;
            }));
        }

        return candidates;
    }

    private static int ElevatorFloor(this Floor[] floors)
    {
        return Array.IndexOf(floors, floors.Single(floor => floor.Any(x => x.type == ComponentType.Elevator)));
    }

    private static Floor[] CloneFloor(this Floor[] floors)
    {
        return floors.Select(x => x.ToList()).ToArray();
    }

    private static Floor Add(this Floor floor, IEnumerable<(ComponentType type, string material)> components) =>
        floor.Union(components).ToList();

    private static Floor Remove(this Floor floor, IEnumerable<(ComponentType type, string material)> components) =>
        floor.Except(components).ToList();

    public static int Hash(this IOrderedEnumerable<(ComponentType type, string material)> floor)
    {
        return floor.ToList().Hash();
    }

    public static int Hash(this List<(ComponentType type, string material)> floor)
    {
        int hash = 11;
        foreach (var component in floor)
            hash = (hash * 23 + (int)component.type) * 31 + component.material.GetHashCode();

        return hash;
    }
}

public class FloorArrayComparer : IEqualityComparer<Floor[]>
{
    public bool Equals(Floor[] x, Floor[] y)
    {
        return x.SequenceEqual(y, new FloorComparer());
    }

    public int GetHashCode(Floor[] obj)
    {
        return obj.Hash(); // Select(x => x.Hash()).Aggregate(17, (current, hash) => current * 23 + hash.GetHashCode());
    }
}

public class FloorComparer : IEqualityComparer<Floor>
{
    public bool Equals(Floor x, Floor y)
    {
        return x.Order().SequenceEqual(y.Order());
    }

    public int GetHashCode(Floor obj)
    {
        return obj.Hash();
    }
}