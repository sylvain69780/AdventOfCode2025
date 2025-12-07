using System.Runtime.CompilerServices;

var inputFile = "test1.txt";
inputFile = "input.txt";

var input = File.ReadAllLines(inputFile).Select(x => x.ToCharArray()).ToArray();
var lines = input.Length;
var rows = input[0].Length;
var stack = new Stack<(int line, int row)>();
var start = (0, Array.IndexOf(input[0], 'S'));
stack.Push(start);
var cache = new Dictionary<(int line, int col), long>{};
var backtrack = new List<int>();
while (stack.TryPop(out var result))
{
    var (line,col) = result;
    if (cache.TryGetValue(result, out var value))
    {
        for (var i = 0; i < backtrack.Count; i++)
            cache[(i, backtrack[i])] += value;
        continue;
    } else
        cache[result] = 0;
    if (backtrack.Count-1 > line)
        backtrack = backtrack[0..line];
    backtrack.Add(col);
    //Thread.Sleep(50);
    //Console.Clear();
    //Console.WriteLine(String.Join('\n', input.Select((s, l) => new String(s.Select((a, c) => (l <= line && backtrack[l] == c) ? 'O' : a).ToArray()))));
    //Console.WriteLine();
    //Console.WriteLine(new String('O',stack.Count));
    //Console.WriteLine(cache[start]);
    //Console.WriteLine(backtrack.Count);
    if (line+1 == lines) continue;
    var below = input[line + 1][col];
    if (below == '^')
    {
        if (col-1 >= 0)
            stack.Push((line + 1, col - 1));
        if (col+1 < rows)
            stack.Push((line + 1, col + 1));
        for (var i = 0; i < backtrack.Count; i++)
            cache[(i, backtrack[i])]++;
    }
    else
        stack.Push((line + 1, col));
}
Console.WriteLine(cache[start]+1);