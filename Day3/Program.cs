using System.Text;

var fileName = "test1.txt";
//var fileName = "input.txt";
var input = File.ReadAllLines(fileName);
var size = 2; // 2 for part 1 , 12 for part 2
var sum = 0L;
foreach (var bank in input)
{
    var section = bank;
    var turnedOn = new StringBuilder(new String('0',size));
    for (var i = 0; i < size ; i++)
    {
        var remainingBatteries = size - 1 - i;
        var top = section.Substring(0, section.Length - remainingBatteries)
            .Select((c,p) => (c,p))
            .OrderByDescending(x => x.c)
            .First();
        turnedOn[i] = top.c;
        section = section[(top.p + 1)..];
    }
    sum += long.Parse(turnedOn.ToString());
}

Console.WriteLine(sum);
