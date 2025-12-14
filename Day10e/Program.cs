
using System.Collections.Immutable;
using System.Security.Cryptography;

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
    var range = Enumerable.Range(0, joltage.Length);
    var pressed = 0;

    var stack = new Stack<int[]>();
    var combi = new int[wiring.Length]; // combination of vectors / quantity for each vectors
    stack.Push(combi);
    var currentWires = new Stack<int>();
    currentWires.Push(0);
    var found = false;
    var pd = int.MaxValue;
    var cardinality = range.Select(i => wiring.Sum(w => w.Contains(i) ? 1 : 0)).ToArray();
    var wiring2 = wiring.OrderBy(w => cardinality.Where((c, i) => w.Contains(i)).Min()).ToArray();
    wiring2 = wiring
        .OrderBy(w => cardinality.Where((c, i) => w.Contains(i)).Min() == 1 ? 1 : 2)
        .ThenByDescending(w => w.Length)
        .ThenBy(w => cardinality.Where((c, i) => w.Contains(i)).Min())
        .ToArray();
    var vectors = wiring2.Select(w => range.Select(i => w.Contains(i) ? 1 : 0).ToArray()).ToArray();
    foreach (var v in vectors)
        Console.WriteLine(string.Join('-', v));
    while (stack.Count > 0) 
    {
        var t = stack.Pop();
        var currentWire = currentWires.Pop();
        pressed = t.Sum();
        var rese = range.Select(i => t.Select((v, j) => vectors[j][i] * v).Sum());
        var res = rese.ToArray();
        //if (res.Zip(joltage, (a, b) => b - a).Any(x => x < 0))
        //    continue;
        if (range.All(a => res[a] == joltage[a]))
        {
            found = true;
            break;
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
        if (currentWire >= wiring2.Length)
            continue;
        var maxAmount = res.Select((v, i) => joltage[i] - v)
            .Where((v,i) => vectors[currentWire][i] == 1 )
            .Min();
        // fin de combinaison
        if (currentWire == wiring2.Length-1 && maxAmount > 0)
        {
            var nt = t.ToArray();
            nt[currentWire] += maxAmount;
            stack.Push(nt);
            currentWires.Push(currentWire + 1);
        }
        //        for (var i = maxAmount; i >= 0; --i)
        foreach (var i in Enumerable.Range(0,maxAmount+1) /*.OrderBy(a => Math.Abs(a - a/2))*/)
        {
            var nt = t.ToArray();
            nt[currentWire] += i;
            stack.Push(nt);
            currentWires.Push(currentWire + 1);
        }
    }
    if (!found)
        throw new Exception("No solution");

    Console.WriteLine($"pressed = {pressed}");
    count += pressed;
}

Console.WriteLine(count);