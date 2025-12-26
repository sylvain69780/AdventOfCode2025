/*
 * Marche en avant pour trouver un combinaison qui fonctionne.
 * On utilise les plus grands vecteurs d'abord
 */

var inputFile = "test1.txt";
//inputFile = "input.txt";
var input = File.ReadAllLines(inputFile)
    .Select(line => line.Split(' '))
    .Select(line => (diagrams: line[0][1..^1], wiring: line[1..^1].Select(x => x[1..^1].Split(',').Select(x => int.Parse(x)).ToArray()).ToArray(), joltage: line[^1][1..^1].Split(',').Select(x => int.Parse(x)).ToArray()))
    .ToArray();

var count = 0;
foreach (var (_, wiring, joltage) in input)
{
    Console.WriteLine(string.Join(',', joltage));
    foreach (var w in wiring)
        Console.WriteLine(string.Join(',', w));
    int pressed = Solve(wiring, joltage);
    count += pressed;
}

Console.WriteLine(count);

static int Solve(int[][] wiring, int[] joltage)
{
    wiring = wiring.OrderBy(w => w.Length).ToArray();

    var stack = new Stack<int[]>();
    stack.Push(new int[wiring.Length]);
    while (stack.Count > 0)
    {
        var combinaison1 = stack.Pop();
        var errors = joltage.Select((v, col) => v - wiring.Select((w, c) => combinaison1[c] * (w.Contains(col) ? 1 : 0)).Sum()).ToArray();
        if (errors.Any(e => e < 0))
            continue;
        if (errors.All(e => e == 0))
        {
            Console.WriteLine(new string('-', 30));
            Console.WriteLine(string.Join('-', combinaison1));
            return combinaison1.Sum();
        }
        var i = 0;
        foreach (var w in wiring)
        {
            var combinaison = combinaison1.ToArray();
            combinaison[i++]++;
            stack.Push(combinaison);
        }
    }
    throw new Exception("Not found");
}

