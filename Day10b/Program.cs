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
    var vectors = wiring.Select(v => range.Select(i => v.Contains(i) ? 1 : 0).ToArray())
        .OrderByDescending(v => v.Sum())
        .ToArray();
    foreach (var v in vectors)
        Console.WriteLine(string.Join('-', v));
    var pressed = 0;
    var combi = new int[vectors.Length]; // combination of vectors / quantity for each vectors
    var stack = new Stack<int[]>();
    stack.Push(combi);
    var backTrack = new Stack<int>();
    backTrack.Push(0);
    var pd = int.MaxValue;
    var cache = new Dictionary<int, int[]>();
    var rand = new Random();
    while (stack.Count>0)
    {
        var t = stack.Pop();
        var back = backTrack.Pop();
        pressed = t.Sum();
        var res = range.Select(i => t.Select((v,j) => vectors[j][i] * v).Sum()).ToArray();
        var d = range.Select(i => joltage[i] - res[i]).Select(x => x).Sum();
        if ( d < pd )
        {
            Console.WriteLine($"dist  {d}");
            Console.WriteLine("combi " + string.Join(',', t));
            Console.WriteLine("res   " + string.Join(',', res));
            Console.WriteLine("dist  " + string.Join(',', range.Select(i => joltage[i] - res[i])));
            pd = d;
        }
        if (range.All(a => res[a] == joltage[a]))
        {
            break;
        }
        for (var i = vectors.Length-1; i >= back; --i)
        {
            var nt = t.ToArray();
            nt[i]++;
            var v = vectors[i];
            if (range.All(j => v[j] + res[j] <= joltage[j]))
            {
                stack.Push(nt);
                backTrack.Push(i);
            }
        }
    }

    Console.WriteLine($"pressed = {pressed}");
    count += pressed;
}

Console.WriteLine(count);