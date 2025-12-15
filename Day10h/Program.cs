using System.Diagnostics;

var inputFile = "test1.txt";
//inputFile = "input.txt";
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
    var range = Enumerable.Range(0, joltage.Length);

    var pressed = int.MaxValue;
    var cardinality = range.Select(i => wiring.Sum(w => w.Contains(i) ? 1 : 0)).ToArray();
    var remaping = joltage.Select((a, i) => (a, i)).OrderBy(x => x.a).Select((x, j) => (j, x.i)).ToDictionary(x => x.i, x => x.j);
    Array.Sort(joltage);
    wiring = wiring.Select(v => v.Select(i => remaping[i]).OrderBy(i => i).ToArray())
        .OrderBy(v => v[0])
        .ThenByDescending(v => v.Length)
        .ToArray();
    var vectors = wiring.Select(w => range.Select(i => w.Contains(i) ? 1 : 0).ToArray()).ToArray();
    Console.WriteLine(string.Join(',', joltage));
    foreach (var w in wiring)
        Console.WriteLine(string.Join(',', w));
    foreach (var v in vectors)
        Console.WriteLine(string.Join('-', v));

    var stack = new Stack<(int pressed, int joltIndex, int vectorIndex, int[] errors)>();
    stack.Push((0, 0, 0, joltage));
    var found = false;
    while (!found)
    {
        var (currentPressed, joltIndex, vectorIndex, errors) = stack.Pop();
        if (currentPressed >= pressed || errors.Any(x => x < 0))
            continue;
        if (errors.All(a => a == 0))
        {
            found = true;
            pressed = currentPressed;
            Console.WriteLine($"best = {pressed}");
            continue;
        }

        if (errors[joltIndex] == 0) // col is full next !
        {
            if (vectorIndex + 1 == vectors.Length)
                continue;
            vectorIndex++;
            while (vectorIndex + 1 < vectors.Length && vectors[vectorIndex][joltIndex] == 1)
                vectorIndex++;
            joltIndex++;
        }
        //if (joltIndex == errors.Length || errors[joltIndex] < 0)
        //    continue; // no solution
        if (vectorIndex + 1 < vectors.Length)
            stack.Push((currentPressed + 1, joltIndex, vectorIndex + 1, errors.Zip(vectors[vectorIndex+1], (a, b) => a - b).ToArray()));
        stack.Push((currentPressed + 1, joltIndex, vectorIndex, errors.Zip(vectors[vectorIndex], (a, b) => a - b).ToArray()));
    }
    if (!found)
        throw new Exception("No solution");
    watch.Stop();
    Console.WriteLine($"pressed = {pressed} elapsed {watch.ElapsedMilliseconds}");
    return pressed;
}