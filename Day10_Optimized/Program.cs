using System.Diagnostics;

var inputFile = "test1.txt";
inputFile = "input.txt";
var input = File.ReadAllLines(inputFile)
    .Select(line => line.Split(' '))
    .Select(line => (diagrams: line[0][1..^1], wiring: line[1..^1].Select(x => x[1..^1].Split(',').Select(x => int.Parse(x)).ToArray()).ToArray(), joltage: line[^1][1..^1].Split(',').Select(x => int.Parse(x)).ToArray()))
    .ToArray();

Console.WriteLine($"Processing {input.Length} configurations...\n");

var totalWatch = Stopwatch.StartNew();
var count = 0;
int configIndex = 0;

foreach (var (diagrams, wiring, joltage) in input)
{
    configIndex++;
    Console.WriteLine($"\n--- Configuration {configIndex}/{input.Length} ---");
    Console.WriteLine($"Diagram: [{diagrams}] Target: [{string.Join(',', joltage)}]");
    Console.WriteLine($"Buttons: {wiring.Length}, Slots: {joltage.Length}");
    
    var watch = Stopwatch.StartNew();
    int pressed = Solve(wiring, joltage, diagrams, configIndex);
    count += pressed;
    watch.Stop();
    
    Console.WriteLine($"✓ Result: {pressed} presses ({watch.ElapsedMilliseconds}ms)");
    Console.WriteLine($"Running total: {count} presses");
}

totalWatch.Stop();
Console.WriteLine($"\n{new string('=', 60)}");
Console.WriteLine($"=== FINAL TOTAL: {count} presses in {totalWatch.ElapsedMilliseconds}ms ===");
Console.WriteLine($"{new string('=', 60)}");

static int Solve(int[][] wiring, int[] joltage, string diagrams = "", int configIndex = 0)
{
    int n = wiring.Length; // buttons
    int m = joltage.Length; // slots

    Console.WriteLine($"  [PHASE 1] Preprocessing...");
    
    // === PHASE 1: PREPROCESSING ===
    
    // Precompute which buttons can affect each slot
    bool[][] buttonAffectsSlot = new bool[n][];
    for (int i = 0; i < n; i++)
    {
        buttonAffectsSlot[i] = new bool[m];
        foreach (var slot in wiring[i])
            buttonAffectsSlot[i][slot] = true;
    }

    // Count how many buttons affect each slot
    int[] buttonsPerSlot = new int[m];
    for (int s = 0; s < m; s++)
    {
        for (int b = 0; b < n; b++)
        {
            if (buttonAffectsSlot[b][s])
                buttonsPerSlot[s]++;
        }
    }

    // Identify critical slots (slots with only 1 button) and calculate forced values
    int[] forcedButtonPresses = new int[n];
    bool[] buttonForced = new bool[n];
    
    for (int s = 0; s < m; s++)
    {
        if (buttonsPerSlot[s] == 1 && joltage[s] > 0)
        {
            // Find the only button that affects this slot
            for (int b = 0; b < n; b++)
            {
                if (buttonAffectsSlot[b][s])
                {
                    forcedButtonPresses[b] = Math.Max(forcedButtonPresses[b], joltage[s]);
                    buttonForced[b] = true;
                    break;
                }
            }
        }
    }

    // Verify forced values are consistent
    int[] testSlots = new int[m];
    for (int b = 0; b < n; b++)
    {
        if (buttonForced[b])
        {
            foreach (var slot in wiring[b])
            {
                testSlots[slot] += forcedButtonPresses[b];
            }
        }
    }
    
    // Check if forced values already exceed targets
    for (int s = 0; s < m; s++)
    {
        if (testSlots[s] > joltage[s])
        {
            Console.WriteLine($"  ✗ Impossible: Forced values exceed target for slot {s}");
            return int.MaxValue; // Impossible
        }
    }

    int forcedCount = buttonForced.Count(x => x);
    int totalForced = forcedButtonPresses.Sum();
    Console.WriteLine($"  • Critical slots detected: {buttonsPerSlot.Count(x => x == 1)}");
    Console.WriteLine($"  • Forced buttons: {forcedCount} (total: {totalForced} presses)");

    // === PHASE 2: OPTIMIZE BUTTON ORDER ===
    
    Console.WriteLine($"  [PHASE 2] Optimizing button order...");
    
    // Sort buttons by strategic importance:
    // 1. Forced buttons first
    // 2. Buttons affecting critical slots (few alternatives)
    // 3. Buttons affecting fewer slots (more specific)
    // 4. Buttons by descending wiring length (original heuristic)
    
    var buttonOrder = Enumerable.Range(0, n)
        .Select(idx => {
            int criticalScore = 0;
            int totalAlternatives = 0;
            
            foreach (var slot in wiring[idx])
            {
                if (buttonsPerSlot[slot] <= 2) criticalScore += 100;
                else if (buttonsPerSlot[slot] <= 3) criticalScore += 50;
                totalAlternatives += buttonsPerSlot[slot];
            }
            
            return new {
                Index = idx,
                IsForced = buttonForced[idx] ? 1 : 0,
                CriticalScore = criticalScore,
                InverseAlternatives = -totalAlternatives,
                WiringLength = wiring[idx].Length
            };
        })
        .OrderByDescending(x => x.IsForced)
        .ThenByDescending(x => x.CriticalScore)
        .ThenByDescending(x => x.InverseAlternatives)
        .ThenByDescending(x => x.WiringLength)
        .Select(x => x.Index)
        .ToArray();

    // Reorder wiring based on optimized order
    var orderedWiring = buttonOrder.Select(idx => wiring[idx]).ToArray();
    var orderedForced = buttonOrder.Select(idx => forcedButtonPresses[idx]).ToArray();
    
    // Rebuild buttonAffectsSlot for new order
    bool[][] orderedButtonAffectsSlot = new bool[n][];
    for (int i = 0; i < n; i++)
    {
        orderedButtonAffectsSlot[i] = new bool[m];
        foreach (var slot in orderedWiring[i])
            orderedButtonAffectsSlot[i][slot] = true;
    }

    // Precompute if button is last to affect each slot
    bool[][] isLastForSlot = new bool[n][];
    for (int i = 0; i < n; i++)
    {
        isLastForSlot[i] = new bool[m];
        foreach (var slot in orderedWiring[i])
        {
            bool isLast = true;
            for (int j = i + 1; j < n; j++)
            {
                if (orderedButtonAffectsSlot[j][slot])
                {
                    isLast = false;
                    break;
                }
            }
            isLastForSlot[i][slot] = isLast;
        }
    }

    // === PHASE 3: BRANCH AND BOUND ===
    
    Console.WriteLine($"  [PHASE 3] Starting search...");
    
    int minPresses = int.MaxValue;
    int[] slots = new int[m];
    long nodesExplored = 0;
    long nodesPruned = 0;
    int depthReached = 0;
    var searchWatch = Stopwatch.StartNew();

    void Try(int idx, int currentSum)
    {
        nodesExplored++;
        depthReached = Math.Max(depthReached, idx);
        
        // Progress update every 500k nodes
        if (nodesExplored % 500000 == 0)
        {
            Console.Write($"\r    • Nodes: {nodesExplored:N0}, Pruned: {nodesPruned:N0}, Best: {(minPresses == int.MaxValue ? "none" : minPresses.ToString())}, Depth: {depthReached}/{n}        ");
        }
        
        // Early termination if already worse
        if (currentSum >= minPresses)
        {
            nodesPruned++;
            return;
        }

        if (idx == n)
        {
            // Check if solution matches
            bool match = true;
            for (int i = 0; i < m; i++)
            {
                if (slots[i] != joltage[i])
                {
                    match = false;
                    break;
                }
            }
            if (match)
            {
                minPresses = currentSum;
                Console.Write("\r" + new string(' ', 120) + "\r");
                Console.WriteLine($"    ★ New best solution found: {minPresses} presses");
            }
            return;
        }

        // Compute minV and maxV for this button
        int minV = orderedForced[idx];
        int maxV = int.MaxValue;
        
        foreach (var slot in orderedWiring[idx])
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
                // Can't exceed what's needed
                maxV = Math.Min(maxV, needed);
            }
        }
        
        minV = Math.Max(0, minV);
        maxV = Math.Max(0, maxV);

        // Try values from maxV to minV (greedy: prefer larger values first)
        for (int v = maxV; v >= minV; v--)
        {
            // Skip if it would exceed current best
            if (currentSum + v >= minPresses)
                continue;

            // Apply v presses to slots
            foreach (var slot in orderedWiring[idx])
                slots[slot] += v;

            // Fast impossibility check
            bool impossible = false;
            for (int s = 0; s < m; s++)
            {
                if (slots[s] > joltage[s])
                {
                    impossible = true;
                    break;
                }
                
                // Check if unreachable slots exist
                if (slots[s] < joltage[s])
                {
                    bool canStillPress = false;
                    for (int j = idx + 1; j < n; j++)
                    {
                        if (orderedButtonAffectsSlot[j][s])
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
                Try(idx + 1, currentSum + v);
            }
            else
            {
                nodesPruned++;
            }

            // Backtrack
            foreach (var slot in orderedWiring[idx])
                slots[slot] -= v;
        }
    }

    Try(0, 0);
    
    searchWatch.Stop();
    Console.Write("\r" + new string(' ', 120) + "\r"); // Efface la dernière ligne de progression
    Console.WriteLine($"  • Search completed in {searchWatch.ElapsedMilliseconds}ms");
    Console.WriteLine($"  • Total nodes explored: {nodesExplored:N0}");
    Console.WriteLine($"  • Nodes pruned: {nodesPruned:N0} ({(nodesPruned * 100.0 / Math.Max(1, nodesExplored)):F1}%)");
    Console.WriteLine($"  • Max depth reached: {depthReached}/{n}");

    return minPresses == int.MaxValue ? 0 : minPresses;
}
