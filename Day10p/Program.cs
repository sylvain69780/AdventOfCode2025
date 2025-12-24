using System;
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
    Console.WriteLine(string.Join(',', joltage));
    foreach (var w in wiring)
        Console.WriteLine(string.Join(',', w));
    int pressed = Solve(wiring, joltage);
    count += pressed;
}

Console.WriteLine(count);

static int Solve(int[][] wiring, int[] joltage)
{
    var equations = BuildEquations(wiring, joltage);
    PrintEquations(equations);
    Simplify(equations);

    var found = false;
    var stack = new Stack<(int w, int[] n)>();
    stack.Push((0, new int[wiring.Length]));
    var watch = Stopwatch.StartNew();
    var pressed = int.MaxValue;
    var maxValues = wiring.Select((_, i) => equations.Where(x => x.w.Contains(i)).Min(x => x.j)).ToArray();
    while (stack.Count > 0)
    {
        var (index, n) = stack.Pop();
        var currentPressed = n.Sum();
        if (currentPressed > pressed)
            continue;
        var errors = equations.Select(e => e.j - e.w.Select(w => n[w]).Sum() );
        if (errors.All(x => x == 0))
        {
            found = true;
            pressed = currentPressed;
            Console.WriteLine($"{String.Join('-',n)}");
            Console.WriteLine($"best = {pressed}");
            continue;
        }
        if (index >= n.Length)
            continue;
        if (equations.Where(x => x.w.Count == 1 && x.w[0] == index).Any())
        {
            var e = equations.Where(x => x.w.Count == 1 && x.w[0] == index).First();
            n[index] = e.j;
            stack.Push((index + 1, n.ToArray()));
            continue;
        }
        if (equations.Where(x => x.w.Contains(index) && x.w.All(y => y <= index)).Any())
        {
            var e = equations.Where(x => x.w.Contains(index) && x.w.All(y => y <= index)).First();
            var v = e.j - e.w.SkipLast(1).Select(i => n[i]).Sum();
            if (v < 0)
                continue;
            n[index] = v;
            stack.Push((index + 1, n.ToArray()));
            continue;
        }

        stack.Push((index + 1, n.ToArray()));
        if (equations.All(e => !e.w.Contains(index)))
            continue;
        while (true)


        {
            n[index]++;
            errors = equations.Select(e => e.j - e.w.Select(w => n[w]).Sum());
            if (errors.Min() < 0)
                break;
            stack.Push((index + 1, n.ToArray()));
        }
    }
    if (!found)
        throw new Exception("No solution");
    watch.Stop();
    Console.WriteLine($"pressed = {pressed} elapsed {watch.ElapsedMilliseconds}");
    return pressed;

}
static void Simplify(List<(int j, List<int> w)> equations)
{
    var found = true;
    while (found)
    {
        found = false;
        for (var i = 0; i < equations.Count; i++)
        {
            var equation = equations[i];
            var included = equations
                .Where(e => e != equation)
                .Where(e => e.w.All(n => equation.w.Contains(n)));
            if (included.Any())
            {
                found = true;
                var (j, w) = included.First();
                foreach (var n in w)
                    equation.w.Remove(n);
                equation.j -= j;
                if (equation.w.Count == 0)
                    equations.RemoveAt(i);
                else
                    equations[i] = equation;
                PrintEquations(equations);
                break;
            }
        }
    }
}

static List<(int j, List<int> w)> BuildEquations(int[][] wiring, int[] joltage)
{
    return joltage.Select((j, row) => (j, w: wiring.Select((col, i) => (col, i)).Where(x => x.col.Contains(row)).Select(x => x.i).ToList())).ToList();
}

static void PrintEquations(List<(int j, List<int> n)> equations)
{
    Console.WriteLine("Equations to solve:");
    for (var i = 0; i < equations.Count; i++)
        if (equations[i].n.Count != 0)
            Console.WriteLine($"{string.Join('+', equations[i].n.Select(n => $"n{n}"))} = {equations[i].j}");
    Console.WriteLine(new String('-',30));

    Console.WriteLine(new String('-', 30));
}
