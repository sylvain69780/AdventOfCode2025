//var input = File.ReadAllLines("test1.txt");
var input = File.ReadAllLines("input.txt");
var ranges = new List<(long start, long end)>();
var ids = new List<long>();
var row = 0;
while (row < input.Length)
{
    var line = input[row++];
    if (line == "")
        break;
    var rec = line.Split('-');
    ranges.Add((long.Parse(rec[0]), long.Parse(rec[1])));
}
while (row < input.Length)
{
    var line = input[row++];
    ids.Add(long.Parse(line));
}

var count = 0;
foreach (var id in ids)
{
    foreach( var range in ranges)
    {
        if ( id >= range.start && id <= range.end )
        {
            count++; 
            break;
        }
    }
}
Console.WriteLine(count);


