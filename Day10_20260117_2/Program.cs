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

    // Precompute which buttons can affect each slot (for faster lookups)
    bool[][] buttonAffectsSlot = new bool[n][];
    for (int i = 0; i < n; i++)
    {
        buttonAffectsSlot[i] = new bool[m];
        foreach (var slot in wiring[i])
            buttonAffectsSlot[i][slot] = true;
    }

    // Precompute if button is last to affect each slot
    bool[][] isLastForSlot = new bool[n][];
    for (int i = 0; i < n; i++)
    {
        isLastForSlot[i] = new bool[m];
        foreach (var slot in wiring[i])
        {
            bool isLast = true;
            for (int j = i + 1; j < n; j++)
            {
                if (buttonAffectsSlot[j][slot])
                {
                    isLast = false;
                    break;
                }
            }
            isLastForSlot[i][slot] = isLast;
        }
    }

    int minPresses = int.MaxValue;
    int[] x = new int[n];
    int[] slots = new int[m];

    void Try(int idx, int currentSum)
    {
        // Early termination if already worse
        if (currentSum >= minPresses)
            return;

        if (idx == n)
        {
            bool match = true;
            for (int i = 0; i < m; i++)
            {
                if (slots[i] != joltage[i])
                {
                    match = false;
                    break;
                }
            }
            if (match && currentSum < minPresses)
            {
                minPresses = currentSum;
            }
            return;
        }

        // Compute minV and maxV for this button
        int minV = 0;
        int maxV = int.MaxValue;
        
        foreach (var slot in wiring[idx])
        {
            int needed = joltage[slot] - slots[slot];

            if (isLastForSlot[idx][slot])
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

        for (int v = maxV; v >= minV; v--)
        {
            // Apply v presses to slots
            foreach (var slot in wiring[idx])
                slots[slot] += v;

            // Test d'impossibilité sur les slots
            bool impossible = false;
            for (int s = 0; s < m; s++)
            {
                if (slots[s] > joltage[s])
                {
                    impossible = true;
                    break;
                }
                if (slots[s] < joltage[s])
                {
                    // Y a-t-il un bouton restant qui agit sur ce slot ?
                    bool canStillPress = false;
                    for (int j = idx + 1; j < n; j++)
                    {
                        if (buttonAffectsSlot[j][s])
                        {
                            canStillPress = true;
                            break;
                        }
                    }
                    if (!canStillPress)
                    {
                        impossible = true;
                        break;
                    }
                }
            }
            
            if (!impossible)
            {
                x[idx] = v;
                Try(idx + 1, currentSum + v);
            }

            // Backtrack
            foreach (var slot in wiring[idx])
                slots[slot] -= v;
        }
    }

    Try(0, 0);

    return minPresses == int.MaxValue ? 0 : minPresses;
}


