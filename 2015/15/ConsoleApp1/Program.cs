var ingredients = File.ReadAllLines(@"..\..\..\..\day-15.txt").Select(line =>
{
    var parts = line.Split(": ");
    var properties = parts[1].Split(", ").Select(property => (property[..property.IndexOf(' ')], int.Parse(property[(property.IndexOf(' ') + 1)..]))).ToArray();
    return (parts[0], properties);
}).ToDictionary(x => x.Item1, x => x.properties.ToDictionary(y => y.Item1, y => y.Item2));

//Part 1
var currentRecipee = ingredients.Select(i => (ingredient: i.Key, amount: 25)).ToArray(); //Assuming 4 ingredients
foreach (var ingredient in ingredients.Keys)
{
    while (true)
    {
        var bestRecipee = currentRecipee.MoveIngredient(ingredient).MaxBy(recipee => recipee.Score(ingredients));
        if (bestRecipee.Score(ingredients) > currentRecipee.Score(ingredients))
            currentRecipee = bestRecipee;
        else
            break;
    }
}

//Part 2
var best500 = 0;
for (int ingredient1 = 1; ingredient1 < 98; ingredient1++) //Assuming 4 ingredients
    for (int ingredient2 = 1; ingredient2 < 99 - ingredient1; ingredient2++)
        for (int ingredient3 = 1; ingredient3 < 100 - (ingredient1+ingredient2); ingredient3++)
        {
            var ingredient4 = 100 - (ingredient1+ingredient2+ingredient3);
            var current = ingredients.Keys.Zip([ingredient1, ingredient2, ingredient3, ingredient4]);
            if (current.Calories(ingredients) == 500 && current.Score(ingredients) > best500)
                best500 = current.Score(ingredients);
        }

Console.WriteLine($"Part 1 - score: {currentRecipee.Score(ingredients)}");
Console.WriteLine($"Part 2 - score: {best500}");
Console.ReadLine();

static class Extensions
{
    public static IEnumerable<(string ingredient, int amount)[]> MoveIngredient(this IEnumerable<(string ingredient, int amount)> recipee, string ingredient)
    {
        var baseRecipee = recipee.ToDictionary();
        baseRecipee[ingredient] -= 1;
        foreach (var newIngredient in baseRecipee.Keys.Except([ingredient]))
        {
            var newRecipee = new Dictionary<string, int>(baseRecipee);
            newRecipee[newIngredient] += 1;
            yield return newRecipee.Select(x => (x.Key, x.Value)).ToArray();
        }
    }
    public static int Score(this IEnumerable<(string ingredient, int amount)> recipee, Dictionary<string, Dictionary<string, int>> ingredients)
    {
        var capacity = recipee.Sum(i => i.amount * ingredients[i.ingredient]["capacity"]);
        var durability = recipee.Sum(i => i.amount * ingredients[i.ingredient]["durability"]);
        var flavor = recipee.Sum(i => i.amount * ingredients[i.ingredient]["flavor"]);
        var texture = recipee.Sum(i => i.amount * ingredients[i.ingredient]["texture"]);

        return Math.Max(0, capacity) * Math.Max(0, durability) * Math.Max(0, flavor) * Math.Max(0, texture);
    }

    public static int Calories(this IEnumerable<(string ingredient, int amount)> recipee, Dictionary<string, Dictionary<string, int>> ingredients)
    {
        return recipee.Sum(i => i.amount * ingredients[i.ingredient]["calories"]);
    }
}