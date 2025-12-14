using System.Diagnostics;
 static int ComputeHash(int[] array)
{
    if (array == null)
        return 0;

    var hashCode = new HashCode();
    foreach (var item in array)
    {
        hashCode.Add(item);
    }
    return hashCode.ToHashCode();
}

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
    var cardinality = range.Select(i => wiring.Sum(w => w.Contains(i) ? 1 : 0)).ToArray();
    var selectivity = vectors.Select(v => range.Select(i => cardinality[i]).Where((c,i) => v[i] == 1).Min())
        .ToArray();
    var pd = int.MaxValue;
    var cache = new Dictionary<int, int>();
    var explored = new HashSet<int>();
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
        var delta = range.Select(i => joltage[i] - res[i]).ToArray();
        var hash2 = ComputeHash(delta);
        if (cache.TryGetValue(hash2, out var result))
        {
            if (result + currentPressed < pressed)
                pressed = result + currentPressed;
            found = true;
            Console.WriteLine($"best = {pressed}");
            continue;
        }
        if (explored.Contains(hash2))
            continue;
        else
            explored.Add(hash2);
        if (pressed < 10000)
        {
            var hash = ComputeHash(res);
            if (!cache.TryAdd(hash, currentPressed))
            {
                continue;
            }
        }

        var d = range.Select(i => joltage[i] - res[i]).Select(x => x).Sum();
        if (d < pd)
        {
            Console.WriteLine($"dist  {d}");
            Console.WriteLine("combi " + string.Join(',', t));
            Console.WriteLine("res   " + string.Join(',', res));
            Console.WriteLine("dist  " + string.Join(',', range.Select(i => joltage[i] - res[i])));
            pd = d;
        }
        var candidates = Enumerable.Range(0, vectors.Length)
            //.Where(i => i == current || !backtrack.Contains(i))
            .Where(i => range.All(j => joltage[j] - res[j] - vectors[i][j] >= 0))
            //.OrderByDescending(i => range.Sum(j => joltage[j] - res[j] - vectors[i][j]))
            .OrderBy(i => range.Where(j => vectors[i][j] == 1).Sum(j => joltage[j] - res[j]))
            //            .OrderByDescending(i => selectivity[i])
            .ToArray();
        if (candidates.Length ==1)
        {
            var v = candidates[0];
            var maxAmount = res.Select((a, i) => joltage[i] - a)
                .Where((a, i) => vectors[v][i] == 1)
                .Min();
                var nt = t.ToArray();
                nt[v]+=maxAmount;
                stack.Push(nt);
                backtrack.Push(v);
            continue;
        }

        foreach (var v in candidates)
        {
            var nt = t.ToArray();
            nt[v]++;
            stack.Push(nt);
            backtrack.Push(v);
        }
    }
    if (!found)
        throw new Exception("No solution");
    watch.Stop();
    Console.WriteLine($"pressed = {pressed} elapsed {watch.ElapsedMilliseconds}");
    count += pressed;
}

Console.WriteLine(count);