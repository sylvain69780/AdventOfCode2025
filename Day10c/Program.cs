using MathNet.Numerics.LinearAlgebra;

var inputFile = "test1.txt";
inputFile = "input.txt";
var input = File.ReadAllLines(inputFile)
    .Select(line => line.Split(' '))
    .Select(line => (diagrams: line[0][1..^1], wiring: line[1..^1].Select(x => x[1..^1].Split(',').Select(x => double.Parse(x)).ToArray()).ToArray(), joltage: line[^1][1..^1].Split(',').Select(x => double.Parse(x)).ToArray()))
    .ToArray();

var count = 0;
foreach (var (_, wiring, joltage) in input)
{
    Console.WriteLine(string.Join(',', joltage));
    var range = Enumerable.Range(0, joltage.Length);
    var vectors = wiring.Select(v => range.Select(i => v.Contains(i) ? 1.0 : 0.0).ToArray())
        .OrderByDescending(v => v.Sum())
        .ToArray();
    foreach (var v in vectors)
        Console.WriteLine(string.Join('-', v));
    var pressed = 0;

    var matrix = Matrix<double>.Build.DenseOfRowArrays(vectors);
    var target = Vector<double>.Build.Dense(joltage);

    var coefficients = matrix.Solve(target);
    Console.Write(coefficients.ToString()); 
    if (coefficients.All(c => !double.IsNaN(c) && !double.IsInfinity(c)))
        pressed = (int)(coefficients.Sum());
    else
        throw new Exception("bobo");

    Console.WriteLine($"pressed = {pressed}");
    count += pressed;
}

Console.WriteLine(count);