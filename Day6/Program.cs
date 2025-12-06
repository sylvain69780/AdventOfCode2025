//var fileName = "test1.txt";
var fileName = "input.txt";
var input = File.ReadAllLines(fileName).Select(x => x.Split(' ').Select(y => y.Trim()).Where(x => x != "").ToArray()).ToArray();
var nbCols = input[^1].Length;
var count = 0L;

for (var i = 0; i < nbCols; i++)
{
    var op = input[^1][i];
    if (op == "+")
        count += input[0..^1].Select(x => long.Parse(x[i])).Sum();
    if (op == "*")
        count += input[0..^1].Select(x => long.Parse(x[i])).Aggregate(1L,(a,b) => a * b);
}
Console.WriteLine(count);
