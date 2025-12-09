var inputFile = "test1.txt";
inputFile = "input.txt";
var nodes = File.ReadAllLines(inputFile)
    .Select(x => x.Split(','))
    .Select(a => (x: long.Parse(a[0]), y: long.Parse(a[1])))
    .ToArray();

var size = nodes.Length;

var lines = nodes[0..^1].Zip(nodes[1..], (n1, n2) => (n1, n2)).Append((n1: nodes[^1], n2: nodes[0])).ToArray();
var vlines = lines
    .Where(a => a.n1.x == a.n2.x)
    .Select(a => a.n1.y > a.n2.y ? (n1:a.n2,n2:a.n1) : (a.n1,a.n2))
    .ToArray();
var hlines = lines
    .Where(a => a.n1.y == a.n2.y)
    .Select(a => a.n1.x > a.n2.x ? (n1: a.n2, n2: a.n1) : (a.n1, a.n2))
    .ToArray();

var surfaces = new List<((long x, long y) n1, (long x, long y) n2, long surface)>();
for (var i = 0; i < size - 1; i++)
    for (var j = i + 1; j < size; j++)
    {
        var n1 = nodes[i];
        var n2 = nodes[j];
        if ( n1.x > n2.x )
            (n1,n2) = ((n2.x,n1.y),(n1.x,n2.y));
        if (n1.y > n2.y)
            (n1, n2) = ((n1.x, n2.y), (n2.x, n1.y));
        var diff = (n2.x - n1.x + 1) * (n2.y - n1.y + 1);
        surfaces.Add((n1, n2, diff));
    }
surfaces.Sort((x, y) => y.surface.CompareTo(x.surface));

var res = 0L;
for (var i = 0; i < surfaces.Count; i++)
{
    var (n1, n2, diff) = surfaces[i];
    if (vlines.Any(v => (v.n1.x > n1.x && v.n1.x < n2.x) && v.n1.y < n2.y && v.n2.y > n1.y))
        continue;
    if (hlines.Any(v => (v.n1.y > n1.y && v.n1.y < n2.y) && v.n1.x < n2.x && v.n2.x > n1.x))
        continue;
    res = diff;
    break;
}

Console.WriteLine(res);
