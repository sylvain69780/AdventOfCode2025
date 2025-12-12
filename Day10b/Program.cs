//var inputFile = "test1.txt";
//inputFile = "input.txt";
//var input = File.ReadAllLines(inputFile)
//    .Select(line => line.Split(' '))
//    .Select(line => (diagrams: line[0][1..^1], wiring: line[1..^1].Select(x => x[1..^1].Split(',').Select(x => int.Parse(x)).ToArray()).ToArray(), joltage: line[^1][1..^1].Split(',').Select(x => int.Parse(x)).ToArray()))
//    .ToArray();

//var count = 0;
//foreach (var (_, wiring, joltage) in input)
//{
//    Console.WriteLine(string.Join(',', joltage));
//    var pressed = 1;
//    var stack = new Stack<int[]>();
//    var backtrack = new Stack<int>();
//    stack.Push(new int[joltage.Length]);
//    backtrack.Push(1);
//    while (true)
//    {
//        var pos = stack.Pop();
//        var deep = backtrack.Pop();
//        if (Enumerable.Range(0, joltage.Length).All(a => pos[a] == joltage[a]))
//        {
//            pressed = deep-1;
//            break;
//        }
//        var list = new List<int[]>();
//        foreach (var b in wiring)
//        {
//            var pos2 = pos.ToArray();
//            foreach (var i in b)
//                pos2[i]++;
//            if (Enumerable.Range(0, joltage.Length).All(a => pos2[a] <= joltage[a]))
//                list.Add(pos2);
//        }
//        if (list.Count > 0)
//        {
//            foreach (var item in list.OrderByDescending(x => Enumerable.Range(0, joltage.Length).Sum(a => joltage[a] - x[a])))
//            {
//                stack.Push(item);
//                backtrack.Push(deep + 1);
//            }
//        }
//    }
//    Console.WriteLine($"pressed = {pressed}");
//    count += pressed;
//}

//Console.WriteLine(count);

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
    while (stack.Count>0)
    {
        var t = stack.Pop();
        pressed = t.Sum();

        var res = range.Select(i => t.Select((v,j) => vectors[j][i] * v).Sum()).ToArray();
        if (range.All(a => res[a] == joltage[a]))
        {
            break;
        }
        for (var i = 0; i<vectors.Length; i++)
        {
            var nt = t.ToArray();
            nt[i]++;
            var v = vectors[i];
            if (range.All(j => v[j] + res[j] <= joltage[j]))
                stack.Push(nt);
        }
    }

    Console.WriteLine($"pressed = {pressed}");
    count += pressed;
}

Console.WriteLine(count);