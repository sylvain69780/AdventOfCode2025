var inputFile = "test1.txt";
inputFile = "input.txt";
var nodes = File.ReadAllLines(inputFile)
    .Select(x => x.Split(','))
    .Select(a => (x: long.Parse(a[0]),y: long.Parse(a[1]), z: long.Parse(a[2])))
    .ToArray();

var size = nodes.Length;
var iterations = 1000; // 10 for example
var distances = new List<((long x, long y, long z) n1, (long x, long y, long z) n2,double distance)>();

for (var  i = 0; i < size-1; i++)
    for (var j = i+1; j < size; j++)
    {
        var n1 = nodes[i];
        var n2 = nodes[j];
        distances.Add((n1,n2,EDist(n1,n2)));
    }
distances.Sort((x,y) => x.distance.CompareTo(y.distance));

static double EDist((long x, long y, long z) value1, (long x, long y, long z) value2)
{
    var (dx, dy, dz) = (value1.x - value2.x, value1.y - value2.y, value1.z - value2.z);
    return Math.Sqrt(dx * dx + dy * dy + dz * dz);
}

var graphes = nodes.Select(x => new List<(long x, long y, long z)> { x }).ToList();

for (var i = 0;i< iterations;i++)
{
    var (n1, n2, distance) = distances[i];
    var graph1 = graphes.Single(x => x.Contains(n1));
    if (graph1.Contains(n2))
        continue;
    var graph2 = graphes.Single(x => x.Contains(n2));
    graphes.Remove(graph2);
    var index = graphes.IndexOf(graph1);
    graphes[index].AddRange(graph2);
}

Console.WriteLine(graphes.Select(x => x.Count).OrderByDescending(x => x).Take(3).Aggregate(1,(a,b) => a*b));
