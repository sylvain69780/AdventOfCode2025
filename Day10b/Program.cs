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
    var vectors = wiring.Select(v => Enumerable.Range(0, joltage.Length).Select(i => v.Contains(i) ? 1 : 0).ToArray()).ToArray();
    foreach (var v in vectors)
        Console.WriteLine(string.Join('-', v));
    var pressed = 0;
    var queue = new Queue<int[]>();
    queue.Enqueue(new int[joltage.Length]);
    var found = false;
    while (!found)
    {
        pressed++;
        var queue2 = new Queue<int[]>();
        var visited = new Dictionary<int, int[]>();

        while (queue.Count > 0)
        {
            var pos = queue.Dequeue();
            if (Enumerable.Range(0, joltage.Length).All(a => pos[a] == joltage[a]))
            {
                pressed--;
                found = true; break;
            }
            foreach (var b in wiring)
            {
                var pos2 = pos.ToArray();
                foreach (var i in b)
                    pos2[i]++;
                var hash = ComputeHash(pos2);
                if (visited.TryGetValue(hash, out var oldpos))
                {
                    if (Enumerable.Range(0, joltage.Length).All(a => pos2[a] == oldpos[a]))
                        continue;
                }
                else
                    visited.Add(hash, pos2);
                if (Enumerable.Range(0, joltage.Length).All(a => pos2[a] <= joltage[a]))
                    queue2.Enqueue(pos2);

            }
        }
        if (!found && queue2.Count == 0)
            throw new IndexOutOfRangeException();
        foreach (var e in queue2.OrderBy(x => Enumerable.Range(0, joltage.Length).Sum(a => (joltage[a] - x[a]) * (joltage[a] - x[a]))).Take(200000))
            queue.Enqueue(e);
        //queue = queue2;
    }
    Console.WriteLine($"pressed = {pressed}");
    count += pressed;
}

Console.WriteLine(count);