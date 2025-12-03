using System.Text;

var fileName = "test1.txt";
//var fileName = "input.txt";
var input = File.ReadAllLines(fileName);
var size = 2; // 2 for part 1 , 12 for part 2
var sum = 0L;
foreach (var bank in input)
{
    var bankSection = bank;
    var turnedOn = new StringBuilder(new String('0',size));
    for (var i = 0; i < size ; i++)
    {
        var remainingBatteries = size - i - 1;
        var top = bankSection
            .Take(bankSection.Length - remainingBatteries)
            .Max();
        turnedOn[i] = top;
        var pos = bankSection.IndexOf(top);
        bankSection = bankSection[(pos + 1)..];
    }
    sum += long.Parse(turnedOn.ToString());
}

Console.WriteLine(sum);
