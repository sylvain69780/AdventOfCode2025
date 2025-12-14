using System.Diagnostics;

var inputFile = "test1.txt";
inputFile = "input.txt";
var input = File.ReadAllLines(inputFile)
    .Select(line => line.Split(' '))
    .Select(line => (diagrams: line[0][1..^1], wiring: line[1..^1].Select(x => x[1..^1].Split(',').Select(x => int.Parse(x)).ToArray()).ToArray(), joltage: line[^1][1..^1].Split(',').Select(x => int.Parse(x)).ToArray()))
    .ToArray();

var count = 0;
foreach (var (_, wiring, joltage) in input)
{
    int pressed = Solve(wiring, joltage);
    count += pressed;
}

Console.WriteLine(count);

static int Solve(int[][] wiring, int[] joltage)
{
    var watch = Stopwatch.StartNew();
    Console.WriteLine(string.Join(',', joltage));
    var range = Enumerable.Range(0, joltage.Length);

    var backtrack = new Stack<int>();
    backtrack.Push(0);
    var found = false;
    var vectors = wiring.Select(w => range.Select(i => w.Contains(i) ? 1 : 0).ToArray()).ToArray();
    foreach (var v in vectors)
        Console.WriteLine(string.Join('-', v));
    var stack = new Stack<int[]>();
    stack.Push(joltage);
    var pressed = int.MaxValue;
    while (stack.Count > 0)
    {
        var errors = stack.Pop();
        var currentPressed = backtrack.Pop();
        if (currentPressed >= pressed)
            continue;
        if (errors.All(a => a == 0))
        {
            found = true;
            pressed = currentPressed;
            Console.WriteLine($"best = {pressed}");
            continue;
        }
        var column = errors
            .Select((e, i) => i)
            .Where(i => errors[i] != 0)
            .OrderBy(i => errors[i]).First();

        var candidates = vectors
            .OrderBy(v => v.Length)
            .Select((v, i) => i)
            .Where(i => vectors[i][column] == 1 && !vectors[i].Where((v,col) => vectors[i][col] == 1 && errors[col] == 0).Any());
        
        foreach(var v in candidates)
        {
            stack.Push(errors.Zip(vectors[v], (e, v) => e -v).ToArray());
            backtrack.Push(currentPressed + 1);
        }

    }
    if (!found)
        throw new Exception("No solution");
    watch.Stop();
    Console.WriteLine($"pressed = {pressed} elapsed {watch.ElapsedMilliseconds}");
    return pressed;
}