using System.Text.RegularExpressions;

//var fileName = @"test1.txt";
var fileName = @"input.txt";
var input = File.ReadAllText(fileName);
var data = input.Split(',')
    .Select(x => x.Split('-'))
    .Select(x => (firstId: long.Parse(x[0]), lastId: long.Parse(x[1])))
    .ToArray();

var sumInvalid = 0L;
var regex = new Regex(@"^(?<pattern>.+?)(\k<pattern>)+$");

foreach (var (firstId, lastId) in data)
{
    for (var i = firstId; i <= lastId; i++)
    {
        var s = i.ToString();
        if (regex.IsMatch(s))
            sumInvalid += i;
    }
}
Console.WriteLine($"{sumInvalid}");
