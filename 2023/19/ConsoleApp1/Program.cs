var rootWorkflows = new Dictionary<string, Workflow>();
var parts = new List<(int x, int m, int a, int s)>();

bool isWorkflow = true;
foreach (var line in File.ReadAllLines(@"..\..\..\..\day-19.txt"))
{
    if (isWorkflow = string.IsNullOrWhiteSpace(line) ? false : isWorkflow)
    {
        var workflowParts = line.Split('{');
        rootWorkflows.Add(workflowParts[0], new Workflow(workflowParts[1].TrimEnd('}'), rootWorkflows));
    }
    else
    {
        if (!string.IsNullOrWhiteSpace(line))
        {
            var parameters = line.Trim('{', '}').Split(',').Select(p => int.Parse(p.Split('=').Last())).ToArray();
            parts.Add((parameters[0], parameters[1], parameters[2], parameters[3]));
        }
    }
}

//Part 1
var sum = parts.Where(part => new WorkflowAction(rootWorkflows["in"]).Execute(part))
                .Sum(part => part.x + part.m + part.a + part.s);

//Part 2
var tree = new WorkflowAction(rootWorkflows["in"]).CreateNode(null, 
            [new Range('x', 1, 4000), new Range('m', 1, 4000), new Range('a', 1, 4000), new Range('s', 1, 4000)]);
var leafNodes = tree.FindLeafNodes();
var combinations = leafNodes.Where(node => node.LeafResult == "A").Sum(node => node.Ranges.Select(r => r.Max - r.Min + 1).Aggregate((a, b) => a * b));

Console.WriteLine($"Part 1 - Sum of all valid parts: {sum}");
Console.WriteLine($"Part 2 - Number of valid combinations: {combinations}");
Console.ReadLine();

class Workflow
{
    public (char operand, char operation, int value) Condition;
    public IAction ifTrue;
    public IAction ifFalse;

    public Workflow(string description, Dictionary<string, Workflow> namedWorkflows)
    {
        var parts = description.Split(':', 2);
        string conditionString = parts[0];
        Condition = (operand: conditionString[0], operation: conditionString[1], value: int.Parse(conditionString.Substring(2)));
        var actions = parts[1].Split(',', 2).Select<string, IAction>(part => part.Contains(':') ? new WorkflowAction(new Workflow(part, namedWorkflows)) : new FixedAction(part, namedWorkflows));
        ifTrue = actions.First();
        ifFalse = actions.Last();
    }
}

interface IAction
{
    public bool Execute((int x, int m, int a, int s) part);
    public Node CreateNode(Node parent, Range[] ranges);
}

class WorkflowAction(Workflow workflow) : IAction
{
    public bool Execute((int x, int m, int a, int s) part)
    {
        var operand = workflow.Condition.operand switch
        {
            'x' => part.x,
            'm' => part.m,
            'a' => part.a,
            's' => part.s,
            _ => throw new InvalidOperationException("Unknown operand")
        };

        var result = workflow.Condition.operation switch
        {
            '<' => operand < workflow.Condition.value,
            '>' => operand > workflow.Condition.value,
            _ => throw new InvalidOperationException("Unknown operation")
        };

        return result ? workflow.ifTrue.Execute(part) : workflow.ifFalse.Execute(part);
    }

    public Node CreateNode(Node parent, Range[] ranges)
    {
        var (operand, operation, value) = workflow.Condition;

        var node = new Node(parent, ranges);
        var impactedRange = ranges.Single(r => r.name == operand);
      
        bool valueAffectsRange = value >= impactedRange.Min && value <= impactedRange.Max;
        if (valueAffectsRange)
        {
            var rangesToKeep = ranges.Except([impactedRange]);

            node.Right = workflow.ifTrue.CreateNode(node, rangesToKeep.Union([new Range(operand, (operation == '<') ? impactedRange.Min : value + 1, (operation == '<') ? value - 1 : impactedRange.Max)]).ToArray());
            node.Left = workflow.ifFalse.CreateNode(node, rangesToKeep.Union([new Range(operand, (operation == '<') ? value : impactedRange.Min, (operation == '<') ? impactedRange.Max : value )]).ToArray());
        }
        else
        {
            node.Right = workflow.ifTrue.CreateNode(node, ranges);
            node.Left = workflow.ifFalse.CreateNode(node, ranges);
        }

        return node;
    }
}

class FixedAction(string action, Dictionary<string, Workflow> namedWorkflows) : IAction
{
    public bool Execute((int x, int m, int a, int s) part)
    {
        return action switch
        {
            "A" => true,
            "R" => false,
            _ => new WorkflowAction(namedWorkflows[action]).Execute(part)
        };
    }

    public Node CreateNode(Node parent, Range[] ranges)
    {
        if (action == "A" || action == "R")
            return new Node(parent, ranges){ LeafResult = action };
        else
            return new WorkflowAction(namedWorkflows[action]).CreateNode(parent, ranges);
    }
}

record Range(char name, long Min, long Max);

record Node(Node Parent, Range[] Ranges)
{
    public Node Left;
    public Node Right;
    public string LeafResult;
}

static class Extensions
{
    public static IEnumerable<Node> FindLeafNodes(this Node node)
    {
        var allNodes = new List<Node>();
        var currentNodes = new Queue<Node>([node]);
        while (currentNodes.Any())
        {
            var current = currentNodes.Dequeue();
            allNodes.Add(current);
            if (current.Left != null)
                currentNodes.Enqueue(current.Left);
            if (current.Right != null)
                currentNodes.Enqueue(current.Right);
        }
        return allNodes;
    }
}