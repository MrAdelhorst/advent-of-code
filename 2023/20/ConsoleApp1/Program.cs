//Construct modules
var input = File.ReadAllLines(@"..\..\..\..\day-20.txt");
var modules = input.Select(line =>
{
    var parts = line.Split(" -> ");
    return new KeyValuePair<string, Module>(parts[0].ToName(), parts[0].ToModule(parts[1].Split(", ").ToArray()));
}).ToDictionary();

//Add untyped modules
var untypedModules = input.SelectMany(line => line.Split(" -> ")[1].Split(", ")).Distinct().Where(m => !modules.ContainsKey(m));
foreach (var untypedModule in untypedModules)
    modules.Add(untypedModule, null);

//Conjunction modules need to know their inputs
foreach (var module in modules.Where(module => module.Value is Conjunction))
{
    var inputs = modules.Where(other => other.Value?.HasDestination(module.Key) ?? false).Select(other => other.Key).ToArray();
    (module.Value as Conjunction).AssignInputs(inputs);
}

//Run sequence
long highSignalCount = 0;
long lowSignalCount = 0;
var queue = new Queue<(Signal signal, string origin, string destination)>();
var highInputsForRxAncestor = new List<(string module, long iteration)>();
for (long i = 1; i <= 10000; i++)
{
    if (i == 1)
    {
        //Assign listener to rx's ancestor for Part 2
        (modules.Ancestors(["rx"]).Single().module as Conjunction).AssignListener((signal, origin) =>
        {
            if (signal == Signal.High)
            {
                highInputsForRxAncestor.Add((origin, i));
            }
        });
    }
    queue.Enqueue((Signal.Low, "button", "broadcaster"));
    while (queue.Any())
    {
        var current = queue.Dequeue();

        //Count signals for Part 1
        if (i <= 1000)
        {
            if (current.signal == Signal.High)
                highSignalCount++;
            else
                lowSignalCount++;
        }

        modules[current.destination]?.Receive(current.signal, current.origin, modules, queue);
    }
}

//Part 2
var numberOfInputCircuits = (modules.Ancestors(["rx"]).Single().module as Conjunction).memory.Count;
var buttonPresses = highInputsForRxAncestor.Take(numberOfInputCircuits).Select(i => i.iteration.Factorize()).CalculateLeastCommonMultiple();

Console.WriteLine($"Part 1 - Product of signals transmitted: {lowSignalCount * highSignalCount}");
Console.WriteLine($"Part 2 - Buttons presses: {buttonPresses}");
Console.ReadLine();

//Types and helpers
enum ModuleType { Broadcast, FlipFlop, Conjunction }
enum Signal { Low, High }

abstract class Module(string name, string[] destinations)
{
    public abstract void Receive(Signal signal, string origin, Dictionary<string, Module> allModules, Queue<(Signal signal, string origin, string destination)> signalQueue);

    public bool HasDestination(string destination) => destinations.Contains(destination);
}

class FlipFlop(string name, string[] destinations) : Module(name, destinations)
{
    public bool state = false;
    public override void Receive(Signal signal, string origin, Dictionary<string, Module> allModules, Queue<(Signal signal, string origin, string destination)> signalQueue)
    {
        if (signal == Signal.Low)
        {
            state = !state;
            var signalToSend = state ? Signal.High : Signal.Low;

            foreach (var destination in destinations)
                signalQueue.Enqueue((signalToSend, name, destination));
        }
    }
}

class Conjunction(string name, string[] destinations) : Module(name, destinations)
{
    public Dictionary<string, Signal> memory;
    public override void Receive(Signal signal, string origin, Dictionary<string, Module> allModules, Queue<(Signal signal, string origin, string destination)> signalQueue)
    {
        //Inform listeners
        this.listener(signal, origin);

        //Process signal
        memory[origin] = signal;
        var signalToSend = memory.All(memory => memory.Value == Signal.High) ? Signal.Low : Signal.High;

        foreach (var destination in destinations)
            signalQueue.Enqueue((signalToSend, name, destination));
    }

    public void AssignInputs(string[] inputs)
    {
        memory = inputs.ToDictionary(input => input, input => Signal.Low);
    }

    Action<Signal, string> listener = (_, __) => { };
    public void AssignListener(Action<Signal, string> signalListener)
    {
        listener = signalListener;
    }
}

class Broadcaster(string name, string[] destinations) : Module(name, destinations)
{
    public override void Receive(Signal signal, string origin, Dictionary<string, Module> allModules, Queue<(Signal signal, string origin, string destination)> signalQueue)
    {
        foreach (var destination in destinations)
            signalQueue.Enqueue((signal, name, destination));
    }
}

static class Extensions
{
    public static string ToName(this string name)
    {
        if (name == "broadcaster")
            return name;
        else
            return name[1..];
    }

    public static Module ToModule(this string name, string[] destinations)
    {
        var mappings = new[]
        {
            new { prefix = "%", type = ModuleType.FlipFlop },
            new { prefix = "&", type = ModuleType.Conjunction },
            new { prefix = "broadcaster", type = ModuleType.Broadcast }
        };

        foreach (var mapping in mappings)
            if (name.StartsWith(mapping.prefix))
                return mapping.type switch
                {
                    ModuleType.FlipFlop => new FlipFlop(name[1..], destinations),
                    ModuleType.Conjunction => new Conjunction(name[1..], destinations),
                    ModuleType.Broadcast => new Broadcaster("broadcaster", destinations),
                };

        throw new InvalidOperationException("Unknown module type");
    }

    public static (string name, Module module)[] Ancestors(this Dictionary<string, Module> allModules, IEnumerable<string> names) =>
        names.SelectMany(name => allModules.Where(m => m.Value?.HasDestination(name) ?? false)).Select(kvp => (kvp.Key, kvp.Value)).ToArray();

    public static IEnumerable<long> Factorize(this long number)
    {
        for (int divisor = 2; divisor < number; divisor++)
        {
            while (number % divisor == 0)
            {
                number /= divisor;
                yield return divisor;
            }
        }
        //Also return the remainder
        yield return number;
    }

    public static long CalculateLeastCommonMultiple(this IEnumerable<IEnumerable<long>> factors) =>
        factors.SelectMany(f => f.Select(n => n)).Distinct().Select(factor =>
        {
            var maxCount = factors.Max(f => f.Count(x => x == factor));
            return (long)Math.Pow(factor, maxCount);
        }).Aggregate((x, y) => x * y);
}