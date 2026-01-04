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
    while (joltage.Any(x => x > 0))
    {
        count++;
        var options = new List<int[]>();
        foreach (var w in wiring)
        {
            var newValue = joltage.Select((v, col) => v - (w.Contains(col) ? 1 : 0)).ToArray();
            if (newValue.All(v => v >= 0))
                options.Add(newValue);
        }
        joltage = options
            .OrderBy(x => x.Sum(y => (y - x.Min())*(y - x.Min())))
            .First();
        Console.WriteLine(string.Join (",", joltage));
    }
    Console.WriteLine($"Solution = {count}");
    return count;
}

