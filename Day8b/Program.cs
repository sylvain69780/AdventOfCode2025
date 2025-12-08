var inputFile = "test1.txt";
inputFile = "input.txt";
var nodes = File.ReadAllLines(inputFile)
    .Select(x => x.Split(','))
    .Select(a => (x: long.Parse(a[0]), y: long.Parse(a[1]), z: long.Parse(a[2])))
    .ToArray();

var size = nodes.Length;
var distances = new List<((long x, long y, long z) n1, (long x, long y, long z) n2, double distance)>();

for (var i = 0; i < size - 1; i++)
    for (var j = i + 1; j < size; j++)
    {
        var n1 = nodes[i];
        var n2 = nodes[j];
        distances.Add((n1, n2, EDist(n1, n2)));
    }
distances.Sort((x, y) => x.distance.CompareTo(y.distance));

static double EDist((long x, long y, long z) value1, (long x, long y, long z) value2)
{
    var (dx, dy, dz) = (value1.x - value2.x, value1.y - value2.y, value1.z - value2.z);
    return Math.Sqrt(dx * dx + dy * dy + dz * dz);
}

var graphes = nodes.Select(x => new List<(long x, long y, long z)> { x }).ToList();

var k = 0;
while (graphes.Count > 1)
{
    var (n1, n2, distance) = distances[k++];
    var graph1 = graphes.Single(x => x.Contains(n1));
    if (graph1.Contains(n2))
        continue;
    var graph2 = graphes.Single(x => x.Contains(n2));
    graphes.Remove(graph2);
    var index = graphes.IndexOf(graph1);
    graphes[index].AddRange(graph2);
}

var (nr1, nr2, _) = distances[k-1];

Console.WriteLine(nr1.x*nr2.x);
