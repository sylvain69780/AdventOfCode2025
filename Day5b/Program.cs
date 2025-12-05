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

static bool Overlaps((long start, long end) a, (long start, long end) b)
{
    return a.start <= b.end && a.end >= b.start;
    // 3 - 5
    // 10 - 14
    // 16 - 20
    // 12 - 18
}

var count = 0L;
var current = ranges.OrderBy(x => x.start).First();
do
{
    // soit il y a un autre interval qui se recoupe et je compte du debut de l'interval jusqu'au debut du suivant - 1
    // soit je compte tout les nombre de l'interval et je passe au suivant
    var nextOverlapping = ranges
        .Where(x => x.start <= current.end && x.end > current.end);
    if (nextOverlapping.Any())
    {
        var next = nextOverlapping.OrderByDescending(x => x.start).First();
        count += next.start - current.start;
        current = next;
    } else
    {
        count += current.end - current.start + 1;
        var next = ranges.Where(x => x.start > current.end);
        if (next.Any())
            current = next.OrderBy(x => x.start).First();
        else
            break;
    }
} while (true);
Console.WriteLine(count);


