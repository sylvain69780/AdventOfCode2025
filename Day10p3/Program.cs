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
    var watch = Stopwatch.StartNew();
    Console.WriteLine(string.Join(',', joltage));
    int pressed = Solve(wiring, joltage);
    count += pressed;
    watch.Stop();
    Console.WriteLine($"Elapsed {watch.ElapsedMilliseconds}ms");
}

Console.WriteLine(count);

static int Solve(int[][] wiring, int[] joltage)
{
    foreach (var w in wiring)
        Console.WriteLine(string.Join(',', w));
    var count = 0;
    var stack = new Stack<(int depth, int[] errors)>();
    stack.Push((0, joltage));
    var error = int.MaxValue;
    while (true)
    {
        var (depth, errors) = stack.Pop();
        var newError = errors.Max();
        if (newError < error)
        {
            error = newError;
            Console.WriteLine(string.Join(",", errors));
        }
        depth++;
        var options = new List<int[]>();
        foreach (var w in wiring)
        {
            var newValue = errors.Select((v, col) => v - (w.Contains(col) ? 1 : 0)).ToArray();
            if (newValue.All(x => x == 0))
                return depth;
            if (newValue.All(v => v >= 0))
                options.Add(newValue);
        }
//        foreach (var o in options.OrderBy(x => x.Sum(y => (y - x.Min())*(y - x.Min()))))
        foreach (var o in options.OrderByDescending(x => x.Sum(y => (y+100)*(y+100))))
        {
            stack.Push((depth, o));
        }
    }
    Console.WriteLine($"Solution = {count}");
    return count;
}

