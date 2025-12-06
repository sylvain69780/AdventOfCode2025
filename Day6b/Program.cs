using System.Text;

//var fileName = "test1.txt";
var fileName = "input.txt";
var input = File.ReadAllLines(fileName);
var cols = input.Select(x => x.Length).Max();
var count = 0L;
var i = 0;
while (i < cols)
{
    var op = input[^1][i];
    var n = string.Empty;
    var res = op == '*' ? 1L : 0L;
    do
    {
        var sb = new StringBuilder();
        foreach ( var c in input[0..^1]
            .Select(x => i < x.Length ? x[i] : ' ')
            .Where(x => x != ' '))
            sb.Append(c);
        n = sb.ToString();
        i++;
        if (n == string.Empty)
            break;
        if (op == '+')
            res += long.Parse(n);
        if (op == '*')
            res *= long.Parse(n);
    } while (true);
    count += res;
}

Console.WriteLine(count);
