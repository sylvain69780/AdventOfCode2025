/*
 * Marche en avant pour trouver un combinaison qui fonctionne.
 * On utilise les plus grands vecteurs d'abord
 */

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
    wiring = wiring.OrderBy(w => w.Length).ToArray();
    foreach (var w in wiring)
        Console.WriteLine(string.Join(',', w));

    var stack = new Stack<int[]>();
    stack.Push(new int[wiring.Length]);
    var distance = int.MaxValue;
    while (stack.Count > 0)
    {
        var combi = stack.Pop();
        var errors = joltage.Select((v, col) => v - wiring.Select((w, c) => combi[c] * (w.Contains(col) ? 1 : 0)).Sum()).ToArray();
        if (errors.All(e => e == 0))
        {
            Console.WriteLine(new string('-', 30));
            Console.WriteLine(string.Join('-', combi));
            Console.WriteLine(new string('-', 30));
            return combi.Sum();
        }
        var currentDist = errors.Sum();
        if (currentDist < distance)
        {
            distance = currentDist;
            Console.WriteLine($"distance = {distance}");
        }
        var indexMin = errors.Select((e, i) => (e, i)).Where(r => r.e > 0).OrderBy(r => r.e).Select(r => r.i).First();
        var indexMax = errors.Select((e, i) => (e, i)).OrderByDescending(r => r.e).Select(r => r.i).First();
        for (var index = 0; index < wiring.Length;index++)
        {
            var w = wiring[index]; 
            if (errors.Where((e,i) => e == 0 && w.Contains(i)).Any())
                continue;
            if (indexMin != indexMax && (w.Contains(indexMin) ) )
                continue;
            if (indexMin != indexMax && (! w.Contains(indexMax)))
                continue;
            var combinaison = combi.ToArray();
            combinaison[index++]++;
            stack.Push(combinaison);
            //if (errors.Any(e => e == 0))
            //{
            //    var indexNull = errors
            //        .Select((valeur, index) => new { valeur, index })
            //        .Where(x => x.valeur == 0)
            //        .Select(x => x.index)
            //        .ToList();
            //    var indexNotNull = errors
            //        .Select((valeur, index) => new { valeur, index })
            //        .Where(x => x.valeur > 0)
            //        .Select(x => x.index)
            //        .ToList();
            //    if (indexNotNull
            //        .Any(i => wiring.Where(w => w.Contains(i))
            //        .All(w => w.Any(j => indexNull.Contains(j)))))
            //        continue;
            //}
        }
    }
    throw new Exception("Not found");
}

