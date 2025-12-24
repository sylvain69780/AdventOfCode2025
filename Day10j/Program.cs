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
    var vectors = wiring.Select(w => joltage.Select((_, i) => w.Contains(i) ? 1 : 0).ToArray()).ToArray();
    Console.WriteLine(string.Join(',', joltage));
    foreach (var w in wiring)
        Console.WriteLine(string.Join(',', w));
    foreach (var v in vectors)
        Console.WriteLine(string.Join('-', v));

    var found = false;
    var stack = new Stack<(int pressed, int[] errors)>();
    stack.Push((0, joltage));
    var watch = Stopwatch.StartNew();
    var pressed = int.MaxValue;
    var dist = int.MaxValue;
    var dependsOn = joltage.
        Select((_, i) => (i, w: wiring.Where(w => w.Contains(i))))
        .ToDictionary(x => x.i, x => x.w);
    while (!found)
    {
        var (currentPressed, errors) = stack.Pop();
        if (currentPressed > pressed)
            continue;
        if (errors.All(a => a == 0))
        {
            found = true;
            pressed = currentPressed;
            Console.WriteLine($"best = {pressed}");
            break;
            //continue;
        }
        if (errors.Any(e => e<0) || errors.Where((e,i) => e > dependsOn[i].Select(w => w.Select(j => errors[j]).Min()).Max()).Any())
            continue;
        foreach (var w in wiring.OrderBy(w => w.Length))
        {

            // reformulation : on est bloqué si
            // on a besoin de remplir un slot mais que c'est plus possible car tout les autres slots 
            // qui sont nécessaire à poser la pièce nécessaire sont pleins.
            // on regarde les slots un par un on note la quantité Q à ajouter
            // on regarde les pièces nécessaires (dependsOn) possibles pour remplir ce slot
            // on doit avoir Q = n1 + n2 donc Q <= Max(n1) + Max(n2) 



                var newErrors = errors.Select((e, i) => w.Contains(i) ? e - 1 : e).ToArray();
                stack.Push((currentPressed + 1, newErrors));
        }

        //foreach (var newErrors in wiring
        //    .Select(w => errors.Select((e, i) => w.Contains(i) ? e - 1 : e).ToArray())
        //    .Where(e => e.All(v => v >= 0))
        //    .OrderByDescending(e => e.Select(x => x).Sum()))
        //    //.OrderBy(w => w.Max()-w.Min()))
        //    //.ThenByDescending(w => w.Sum()))
        //{
        //    // s'il y a un slot à zero et qu'on a une dependance avec d'autres slots encore à remplir on stop
        //    for (var i=0; i<errors.Length;i++)
        //    {
        //        var e = errors[i];
        //        var needed = wiring.Where(w => w.Contains(i));
        //        var sum = errors
        //            .Select((e, j) => j != i && wiring.Any(w => w.Contains(i) && w.Contains(j)) ? e : 0).Sum();
        //        if (e < sum)
        //            continue;
        //    }
        //    var newDistance = newErrors.Sum();
        //    if (newDistance<dist)
        //    {
        //        dist = newDistance;
        //        Console.WriteLine(newDistance);
        //    }
        //    stack.Push((currentPressed + 1, newErrors));
        //}
    }
    if (!found)
        throw new Exception("No solution");
    watch.Stop();
    Console.WriteLine($"pressed = {pressed} elapsed {watch.ElapsedMilliseconds}");
    return pressed;
}