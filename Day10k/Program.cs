using System;
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
    var equations = joltage.Select((jo, row) => wiring.Select((w, col) => wiring[col].Contains(row) ? 1 : 0).Append(joltage[row]).ToArray())
        //        .OrderByDescending(e => string.Join('-',e))
        .ToArray();


    Console.WriteLine(string.Join(',', joltage));
    foreach (var w in wiring)
        Console.WriteLine(string.Join(',', w));
    Print(equations);

    var found = true;
    while (found)
    {
        found = false;
        foreach (var equ in equations.Where(e => !e.All(x => x == 0)))
        {
            var helperEquation = equations.Where(e => !e.All(x => x == 0)).Where(e => e != equ && !e.SkipLast(1).Where((x, i) => equ[i] == 0 && x != 0).Any()).FirstOrDefault();
            if (helperEquation is not null)
            {
                found = true;
                var (v, index) = equ.SkipLast(1).Select((v, i) => (v, i)).Where(x => x.v != 0 && helperEquation[x.i] != 0).First();
                for (var i = 0; i <= wiring.Length; i++)
                    if (i != index)
                        equ[i] = equ[i] * helperEquation[index] - helperEquation[i] * equ[index];
                equ[index] = 0;
                Print(equations);
            }
        }

    }

    foreach (var equ in equations.Where(e => e.SkipLast(1).Where(x => x != 0).Count() == 1))
    {
        var v = equ[..^1].Where(x => x != 0).First();
        var i = Array.IndexOf(equ[..^1], v);
        if (v != 1)
        {
            equ[^1] /= equ[i];
            equ[i] = 1;
        }
    }

    equations = [.. equations.OrderByDescending(e => e[..^1].Where(x => x == 0).Count())];

    Print(equations);


    return equations.Select(e => e[^1]).Sum();

    static void Print(int[][] equations)
    {
        Console.WriteLine();
        for (var ii = 0; ii < equations.Length; ii++)
            if (!equations[ii].All(x => x == 0))
                Console.WriteLine($"{string.Join('+', equations[ii][..^1].Select((e, i) => (e, i)).Where(x => x.e != 0).Select(x => $" {x.e}*x{x.i}"))} = {equations[ii][^1]}");
    }
}


//for (var i = 0; i < joltage.Length; i++)
//{
//    // (x1 * v11) + (x2 * v21) + (x3 * v31) ... (xk * vk1) = n1  
//    // v11 = 1
//    // x1 = n1 - ((x2 * v21) + (x3 * v31) ... (xk * vk1))

//    // (x1 * v12) + (x2 * v22) + (x3 * v32) ... (xk * vk2) = n2  
//    // v12 = 1 et v22 = 1
//    // (x1 * v12) + (x2 * v22) + (x3 * v32) ... (xk * vk2) = n2  
//}


//var stack = new Stack<(int pressed, int[] errors)>();
//stack.Push((0, joltage));
//var watch = Stopwatch.StartNew();
//var pressed = int.MaxValue;
//while (!found)
//{
//    var (currentPressed, errors) = stack.Pop();
//    if (currentPressed > pressed)
//        continue;
//    if (errors.All(a => a == 0))
//    {
//        found = true;
//        pressed = currentPressed;
//        Console.WriteLine($"best = {pressed}");
//        // break;
//        continue;
//    }
//    foreach (var newErrors in wiring
//        .Select(w => errors.Select((e, i) => w.Contains(i) ? e - 1 : e).ToArray())
//        .Where(w => !w.Any(v => v < 0))
//        .OrderByDescending(w => Math.Sqrt(w.Select(x => x * x).Sum())))
//    //.OrderBy(w => w.Max()-w.Min()))
//    //.ThenByDescending(w => w.Sum()))
//    {

//        stack.Push((currentPressed + 1, newErrors));
//    }
//}
//if (!found)
//    throw new Exception("No solution");
//watch.Stop();
//Console.WriteLine($"pressed = {pressed} elapsed {watch.ElapsedMilliseconds}");
//return pressed;
//}