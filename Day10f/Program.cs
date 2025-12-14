var inputFile = "test1.txt";
inputFile = "input.txt";
var input = File.ReadAllLines(inputFile)
    .Select(line => line.Split(' '))
    .Select(line => (diagrams: line[0][1..^1], wiring: line[1..^1].Select(x => x[1..^1].Split(',').Select(x => int.Parse(x)).ToArray()).ToArray(), joltage: line[^1][1..^1].Split(',').Select(x => int.Parse(x)).ToArray()))
    .ToArray();

var count = 0;
foreach (var (_, wiring, joltage) in input)
{
    Console.WriteLine(string.Join(',', joltage));
    var range = Enumerable.Range(0, joltage.Length);

    var stack = new Stack<int[]>();
    var combi = new int[wiring.Length]; // combination of vectors / quantity for each vectors
    stack.Push(combi);
    var backtrack = new Stack<int>();
    backtrack.Push(-1);
    var found = false;
    var vectors = wiring.Select(w => range.Select(i => w.Contains(i) ? 1 : 0).ToArray()).ToArray();
    foreach (var v in vectors)
        Console.WriteLine(string.Join('-', v));
    var pressed = int.MaxValue;
    while (stack.Count > 0)
    {
        var t = stack.Pop();
        var current = backtrack.Pop();
        var currentPressed = t.Sum();
        if (currentPressed >= pressed)
            continue;
        var rese = range.Select(i => t.Select((v, j) => vectors[j][i] * v).Sum());
        var res = rese.ToArray();
        if (range.All(a => res[a] == joltage[a]))
        {
            found = true;
            pressed = currentPressed;
            Console.WriteLine($"best = {pressed}");
            continue;
        }
        var candidates = Enumerable.Range(0, vectors.Length)
            .Where(i => i == current || !backtrack.Contains(i))
            .Where(i => range.All(j => joltage[j] - res[j] - vectors[i][j] >= 0))
            .OrderByDescending(i => range.Sum(j => joltage[j] - res[j] - vectors[i][j]))
            .ToArray();
        //if (candidates.Length == 0)
        //    backtrack.Pop();
        foreach (var v in candidates)
        //foreach (var v in Enumerable.Range(0, vectors.Length)
        //    .Where(i => range.All(j => joltage[j] - res[j] - vectors[i][j] >= 0))
        //    .OrderByDescending(i => vectors[i].Sum()))
        {
            var nt = t.ToArray();
            nt[v]++;
            stack.Push(nt);
            backtrack.Push(v);
        }
    }
    if (!found)
        throw new Exception("No solution");

    Console.WriteLine($"pressed = {pressed}");
    count += pressed;
}

Console.WriteLine(count);