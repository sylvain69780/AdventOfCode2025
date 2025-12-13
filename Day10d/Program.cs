
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
    var ranks = new Stack<int>();
    ranks.Push(0);
    var pd = int.MaxValue;
    var found = false;
    while (stack.Count>0)
    {
        var t = stack.Pop();
        var rank = ranks.Pop();
        pressed = t.Sum();
        var res = range.Select(i => t.Select((v, j) => (wiring[j].Contains(i) ? 1 : 0) * v).Sum())
            .ToArray();
        if (range.Any(i => res[i] > joltage[i]))
            continue;
        var d = range.Select(i => joltage[i] - res[i]).Select(x => x).Sum();
        if (d < pd)
        {
            Console.WriteLine($"dist  {d}");
            Console.WriteLine("combi " + string.Join(',', t));
            Console.WriteLine("res   " + string.Join(',', res));
            Console.WriteLine("dist  " + string.Join(',', range.Select(i => joltage[i] - res[i])));
            pd = d;
        }
        if (range.All(a => res[a] == joltage[a]))
        {
            found = true;
            break;
        }
        var selection = wiring.Where(w => w.Contains(rank))
            .OrderByDescending(w => w.Length)
            .Select(w => Array.IndexOf(wiring,w)).ToArray();
        if (res[rank]  == joltage[rank])
        {
            stack.Push(t);
            ranks.Push(rank + 1);
            continue;
        }
        foreach (var tuple in CombinationGenerator.GenerateCombinations(joltage[rank] - res[rank], selection.Length))
        {
            var nt = t.ToArray();
            for (var i = 0; i < tuple.Length; i++)
            {
                var w = selection[i];
                nt[w] += tuple[i];
            }
            stack.Push(nt);
            ranks.Push(rank + 1);
        }
    }
    if (!found)
        throw new Exception("coucou");

    Console.WriteLine($"pressed = {pressed}");
    count += pressed;
}

Console.WriteLine(count);