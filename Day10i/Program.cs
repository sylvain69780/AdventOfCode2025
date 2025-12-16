using System.ComponentModel;
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
        //.OrderByDescending(v => v[0])
        // .ThenByDescending(v => v.Length)
        .OrderByDescending(v => v.Length)
        .ToArray();
    var vectors = wiring.Select(w => range.Select(i => w.Contains(i) ? 1 : 0).ToArray()).ToArray();
    Console.WriteLine(string.Join(',', joltage));
    foreach (var w in wiring)
        Console.WriteLine(string.Join(',', w));
    foreach (var v in vectors)
        Console.WriteLine(string.Join('-', v));

    var found = false;
    var c = new int[vectors.Length];
    var stack = new Stack<(int index, int[] c)>();
    stack.Push((0, c));
    while (!found)
    {
        var (index, combination) = stack.Pop();
        var errors = joltage.Select((jol, ijol) => jol - vectors.Select((ve, ive) => ve[ijol] * combination[ive]).Sum()).ToArray();
        var currentPressed = combination.Sum();
        if (errors.All(a => a == 0))
        {
            found = true;
            pressed = currentPressed;
            Console.WriteLine($"best = {pressed}");
            break;
            // continue;
        }
        if (index + 1 > vectors.Length)
            continue;
        else
            stack.Push((index + 1, combination.ToArray()));
        while (true)
        {
            combination[index]++;
            errors = joltage.Select((jol, ijol) => jol - vectors.Select((ve, ive) => ve[ijol] * combination[ive]).Sum()).ToArray();
            if (!errors.Any(x => x < 0))
                stack.Push((index + 1, combination.ToArray()));
            else
                break;
        }
    }
    if (!found)
        throw new Exception("No solution");
    watch.Stop();
    Console.WriteLine($"pressed = {pressed} elapsed {watch.ElapsedMilliseconds}");
    return pressed;
}