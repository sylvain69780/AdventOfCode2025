using System.Text;

//var fileName = "test1.txt";
var fileName = "input.txt";
var input = File.ReadAllLines(fileName);
var size = 12;
var sum = 0L;
foreach (var bank in input)
{
    var section = bank;
    var turnedOn = new StringBuilder(new String('0',size));
    for (var i = size; i >0 ; --i)
    {
        var top = section.Substring(0, section.Length - i + 1)
            .Select((c,p) => (c,p))
            .OrderByDescending(x => x.c)
            .First();
        turnedOn[size-i] = top.c;
        section = section[(top.p + 1)..];
    }
    sum += long.Parse(turnedOn.ToString());
}

Console.WriteLine(sum);
