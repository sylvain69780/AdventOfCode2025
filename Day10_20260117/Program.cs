using System.Diagnostics;

var inputFile = "test1.txt";
inputFile = "input.txt";
var input = File.ReadAllLines(inputFile)
    .Select(line => line.Split(' '))
    .Select(line => (diagrams: line[0][1..^1], wiring: line[1..^1].Select(x => x[1..^1].Split(',').Select(x => int.Parse(x)).ToArray()).ToArray(), joltage: line[^1][1..^1].Split(',').Select(x => int.Parse(x)).ToArray()))
    .ToArray();

var count = 0;
foreach (var (_, wiring, joltage) in input)
{
    var watch = Stopwatch.StartNew();
    Console.WriteLine(string.Join(',', joltage));
    int pressed = Solve(wiring, joltage);
    count += pressed;
    watch.Stop();
    Console.WriteLine($"Elapsed {watch.ElapsedMilliseconds}ms");
}

Console.WriteLine(count);

static int Solve(int[][] wiring, int[] joltage)
{
    wiring = wiring
    .Select((slots, idx) => (slots, idx))
    .OrderByDescending(pair => pair.slots.Length)
    .Select(pair => pair.slots)
    .ToArray();

    foreach (var slot in wiring)
    {
        Console.WriteLine(string.Join (",", slot)); 
    }

    int n = wiring.Length; // buttons
    int m = joltage.Length; // slots

    // Build matrix: slot x button
    int[,] A = new int[m, n];
    for (int i = 0; i < n; i++)
        foreach (var slot in wiring[i])
            A[slot, i] = 1;

    int[]? best = null;
    int minPresses = int.MaxValue;

    // Try all combinations (brute-force, only for small n)
    int maxPress = joltage.Max();
    int[] x = new int[n];

    void Try(int idx, int[] slots)
    {
        if (idx == n)
        {
            if (slots.SequenceEqual(joltage))
            {
                int total = x.Sum();
                if (total < minPresses)
                {
                    minPresses = total;
                    best = x.ToArray();
                }
            }
            return;
        }

        // Compute minV and maxV for this button
        int minV = 0;
        int maxV = int.MaxValue;
        foreach (var slot in wiring[idx])
        {
            int needed = joltage[slot] - slots[slot];

            // Check if this is the last button that can increment this slot
            bool isLast = true;
            for (int j = idx + 1; j < n; j++)
                if (wiring[j].Contains(slot))
                {
                    isLast = false;
                    break;
                }

            if (isLast)
            {
                // Must exactly reach the target for this slot
                minV = Math.Max(minV, needed);
                maxV = Math.Min(maxV, needed);
            }
            else
            {
                // Otherwise, can't go below 0, can't go above needed
                minV = Math.Max(minV, 0);
                maxV = Math.Min(maxV, needed);
            }
        }
        minV = Math.Max(0, minV);
        maxV = Math.Max(0, maxV);

        for (int v = minV; v <= maxV; v++)
        {
            x[idx] = v;
            // Apply v presses to slots
            foreach (var slot in wiring[idx])
                slots[slot] += v;

            // Prune if current sum already exceeds best found
            int currentSum = x.Take(idx + 1).Sum();
            if (currentSum < minPresses)
                Try(idx + 1, slots);

            // Backtrack
            foreach (var slot in wiring[idx])
                slots[slot] -= v;
        }

    }


    int[] slots = new int[m];
    Try(0, slots);

    if (best != null)
    {
        Console.WriteLine("Button presses: " + string.Join(",", best));
        return minPresses;
    }
    else
    {
        Console.WriteLine("No solution found.");
        return 0;
    }
}


