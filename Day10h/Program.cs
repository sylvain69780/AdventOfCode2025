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

static int Solve(int[][] wiring, int[] joltage)
{
    var watch = Stopwatch.StartNew();
    Console.WriteLine(string.Join(',', joltage));
    var range = Enumerable.Range(0, joltage.Length);

    var backtrack = new Stack<int>();
    backtrack.Push(-1);
    var found = false;
    var vectors = wiring.Select(w => range.Select(i => w.Contains(i) ? 1 : 0).ToArray()).ToArray();
    foreach (var v in vectors)
        Console.WriteLine(string.Join('-', v));
    var pd = int.MaxValue;

    // max value - does not change anything
    var pressed = int.MaxValue;
    var maxValues = vectors.Select((v, i) => joltage.Select((jo, j) => v[j] * jo).Where(x => x != 0).Min()).ToArray();
    var stack = new Stack<int[]>();
//    stack.Push(new int[wiring.Length]);
    stack.Push(maxValues);
    var visitedCache = new HashSet<int>();

    while (stack.Count > 0)
    {
        var combination = stack.Pop();
        var currentPressed = combination.Sum();
        if (currentPressed >= pressed)
            continue;
        var hash = ComputeHash(combination);
        visitedCache.Add(hash);

        var rese = range.Select(i => combination.Select((v, j) => vectors[j][i] * v).Sum());
        var res = rese.ToArray();
        var delta = res.Zip(joltage,(a,b) => a-b).ToArray();
        if (delta.All(a => a == 0))
        {
            found = true;
            pressed = currentPressed;
            Console.WriteLine($"best = {pressed}");
            continue;
        }

        var d = range.Select(i => Math.Abs(joltage[i] - res[i])).Select(x => x).Sum();
        if (d < pd)
        {
            Console.WriteLine($"dist  {d}");
            Console.WriteLine("combi " + string.Join(',', combination));
            Console.WriteLine("res   " + string.Join(',', res));
            Console.WriteLine("dist  " + string.Join(',', range.Select(i => joltage[i] - res[i])));
            pd = d;
        }

        var candidates = Enumerable.Range(0, combination.Length)
            .Select(i => { var c = combination.ToArray(); c[i]++; return c; })
            .Concat(Enumerable.Range(0, combination.Length)
            .Select(i => { var c = combination.ToArray(); c[i]--; return c; })
            );
//           .Where(i => i == current || !backtrack.Contains(i))
 //           .Where(i => range.All(j => joltage[j] - res[j] - vectors[i][j] >= 0))
            //.OrderByDescending(i => range.Sum(j => joltage[j] - res[j] - vectors[i][j]))
            //.OrderBy(i => range.Where(j => vectors[i][j] == 1).Sum(j => Math.Abs(res[j] - joltage[j]))) // distance function
            ////            .OrderByDescending(i => selectivity[i])
            //.ToArray();

        foreach (var c in candidates.OrderByDescending(x => {
            var resx = range.Select(i => x.Select((v, j) => vectors[j][i] * v).Sum()).ToArray();
            return range.Select(i => Math.Abs(joltage[i] - resx[i])).Sum();
        }))
        {
            hash = ComputeHash(c);
            if (!visitedCache.Contains(hash))
            stack.Push(c);
        }
    }
    if (!found)
        throw new Exception("No solution");
    watch.Stop();
    Console.WriteLine($"pressed = {pressed} elapsed {watch.ElapsedMilliseconds}");
    return pressed;
}