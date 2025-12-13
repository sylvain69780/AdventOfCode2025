
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
    var found = false;
    var pd = int.MaxValue;
    var cardinality = range.Select(i => wiring.Sum(w => w.Contains(i) ? 1 : 0)).ToArray();
    while (stack.Count > 0)
    {
        var t = stack.Pop();
        pressed = t.Sum();
        var rese = range.Select(i => t.Select((v, j) => (wiring[j].Contains(i) ? 1 : 0) * v).Sum());
        if (rese.Zip(joltage,(a,b) => b-a).Any(x => x < 0) )
            continue;
        var res = rese.ToArray();
        if (range.All(a => res[a] == joltage[a]))
        {
            found = true;
            break;
        }
        var rank = range.Where(i => joltage[i] - res[i] > 0)
            .OrderBy(i => cardinality[i])
            .ThenBy(i => joltage[i] - res[i])
            .First();

        var d = range.Select(i => joltage[i] - res[i]).Select(x => x).Sum();
        if (d < pd)
        {
            Console.WriteLine($"dist  {d}");
            Console.WriteLine("combi " + string.Join(',', t));
            Console.WriteLine("res   " + string.Join(',', res));
            Console.WriteLine("dist  " + string.Join(',', range.Select(i => joltage[i] - res[i])));
            pd = d;
        }
        var selection = wiring.Where(w => w.Contains(rank))
            .OrderBy(w => w.Length)
            .Select(w => Array.IndexOf(wiring, w)).ToArray();
        foreach (var s in selection)
        {
            var maxAmount = res.Select((v, i) => joltage[i] - v).Where((v, i) => wiring[s].Contains(i)).Min();
            for (var i = 0; i < maxAmount; i++)
            {
                var nt = t.ToArray();
                nt[s] += i + 1;
                stack.Push(nt);
            }
        }
        //foreach (var tuple in CombinationGenerator.GenerateCombinations(joltage[rank] - res[rank], selection.Length))
        //{
        //    var nt = t.ToArray();
        //    for (var i = 0; i < tuple.Length; i++)
        //    {
        //        var w = selection[i];
        //        nt[w] += tuple[i];
        //    }
        //    stack.Push(nt);
        //}
    }
    if (!found)
        throw new Exception("coucou");

    Console.WriteLine($"pressed = {pressed}");
    count += pressed;
}

Console.WriteLine(count);