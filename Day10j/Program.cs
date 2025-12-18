using System.ComponentModel;
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
    var vectors = wiring.Select(w => joltage.Select((_,i) => w.Contains(i) ? 1 : 0).ToArray()).ToArray();
    Console.WriteLine(string.Join(',', joltage));
    foreach (var w in wiring)
        Console.WriteLine(string.Join(',', w));
    foreach (var v in vectors)
        Console.WriteLine(string.Join('-', v));

    var found = false;
    var stack = new Stack<(int pressed,int[] errors)>();
    stack.Push((0,joltage));
    var watch = Stopwatch.StartNew();
    var pressed = int.MaxValue;
    while (!found)
    {
        var (currentPressed,errors) = stack.Pop();
        if (currentPressed > pressed)
            continue;
        if (errors.All(a => a == 0))
        {
            found = true;
            pressed = currentPressed;
            Console.WriteLine($"best = {pressed}");
            // break;
            continue;
        }
        foreach (var newErrors in wiring
            .Select(w => errors.Select((e, i) => w.Contains(i) ? e - 1 : e).ToArray())
            .Where(w => !w.Any(v => v < 0))
            .OrderByDescending(w => Math.Sqrt(w.Select(x => x*x).Sum())))
            //.OrderBy(w => w.Max()-w.Min()))
            //.ThenByDescending(w => w.Sum()))
        {

            stack.Push((currentPressed + 1, newErrors));
        }
    }
    if (!found)
        throw new Exception("No solution");
    watch.Stop();
    Console.WriteLine($"pressed = {pressed} elapsed {watch.ElapsedMilliseconds}");
    return pressed;
}