var inputFile = "test1.txt";
inputFile = "input.txt";
var nodes = File.ReadAllLines(inputFile)
    .Select(x => x.Split(','))
    .Select(a => (x: long.Parse(a[0]), y: long.Parse(a[1])))
    .ToArray();

var size = nodes.Length;
var surfaces = new List<((long x, long y) n1, (long x, long y) n2, long surface)>();

for (var i = 0; i < size - 1; i++)
    for (var j = i + 1; j < size; j++)
    {
        var n1 = nodes[i];
        var n2 = nodes[j];
        if (n1.x > n2.x)
            (n1, n2) = ((n2.x, n1.y), (n1.x, n2.y));
        if (n1.y > n2.y)
            (n1, n2) = ((n1.x, n2.y), (n2.x, n1.y));
        var diff = (n2.x - n1.x + 1) * (n2.y - n1.y + 1);
        surfaces.Add((n1, n2, diff));
    }
surfaces.Sort((x, y) => x.surface.CompareTo(y.surface));
Console.WriteLine(surfaces[^1].surface);
