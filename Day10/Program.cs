var inputFile = "test1.txt";
inputFile = "input.txt";
var input = File.ReadAllLines(inputFile)
    .Select(line => line.Split(' '))
    .Select(line => (diagrams: line[0][1..^1], wiring: line[1..^1].Select(x => x[1..^1].Split(',').Select(x => int.Parse(x)).ToArray()).ToArray(), joltage: line[^1][1..^1].Split(',')))
    .ToArray();

var count = 0;
foreach (var (diagrams, wiring, joltage) in input)
{
    var pressed = 1;
    var combi = new List<int>() { 0 };
    while (true)
    {
        var t = new String('.',diagrams.Length).ToCharArray();
        foreach (var b in combi)
        {
            foreach (var i in wiring[b])
            {
                if (t[i] == '.')
                    t[i] = '#';
                else
                    t[i] = '.';
            }
        }
        var test = new string(t);
        if (test == diagrams)
        {
            pressed = combi.Count;
            break; 
        }
        var idx = 0;
        do
        {
            if (combi[idx]+1 < wiring.Length)
            {
                combi[idx]++;
                break;
            }
            else
            {
                combi[idx++] = 0;
                if (idx >= combi.Count)
                {
                    combi.Add(0);
                    break;
                }
            }
        } while (true);        
    }
    count += pressed;
}

Console.WriteLine(count);