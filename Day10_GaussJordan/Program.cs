using System.Diagnostics;

var inputFile = "test1.txt";
inputFile = "input.txt";
var input = File.ReadAllLines(inputFile)
    .Select(line => line.Split(' '))
    .Select(line => (
        diagrams: line[0][1..^1], 
        wiring: line[1..^1].Select(x => x[1..^1].Split(',').Select(x => int.Parse(x)).ToArray()).ToArray(), 
        joltage: line[^1][1..^1].Split(',').Select(x => int.Parse(x)).ToArray()
    ))
    .ToArray();

Console.WriteLine($"Processing {input.Length} configurations using Gauss-Jordan elimination...\n");

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
    int pressed = SolveGaussJordan(wiring, joltage);
    count += pressed;
    watch.Stop();
    
    Console.WriteLine($"✓ Result: {pressed} presses ({watch.ElapsedMilliseconds}ms)");
    Console.WriteLine($"Running total: {count} presses");
}

totalWatch.Stop();
Console.WriteLine($"\n{new string('=', 60)}");
Console.WriteLine($"=== FINAL TOTAL: {count} presses in {totalWatch.ElapsedMilliseconds}ms ===");
Console.WriteLine($"{new string('=', 60)}");

static int SolveGaussJordan(int[][] wiring, int[] joltage)
{
    int numButtons = wiring.Length;
    int numSlots = joltage.Length;
    
    Console.WriteLine($"  [STEP 1] Building coefficient matrix ({numSlots}x{numButtons})...");
    
    // Build the coefficient matrix A where A[slot][button] = 1 if button affects slot
    double[,] A = new double[numSlots, numButtons];
    for (int button = 0; button < numButtons; button++)
    {
        foreach (int slot in wiring[button])
        {
            A[slot, button] = 1;
        }
    }
    
    // Target vector b (joltage requirements)
    double[] b = joltage.Select(x => (double)x).ToArray();
    
    Console.WriteLine($"  [STEP 2] Performing Gauss-Jordan elimination...");
    
    // Create augmented matrix [A|b]
    double[,] augmented = new double[numSlots, numButtons + 1];
    for (int i = 0; i < numSlots; i++)
    {
        for (int j = 0; j < numButtons; j++)
        {
            augmented[i, j] = A[i, j];
        }
        augmented[i, numButtons] = b[i];
    }
    
    // Gauss-Jordan elimination to reduced row echelon form (RREF)
    int[] pivotCols = GaussJordanElimination(augmented, numSlots, numButtons);
    
    // Identify pivot and free variables
    HashSet<int> pivotVars = new HashSet<int>(pivotCols.Where(x => x >= 0));
    List<int> freeVars = new List<int>();
    for (int i = 0; i < numButtons; i++)
    {
        if (!pivotVars.Contains(i))
        {
            freeVars.Add(i);
        }
    }
    
    Console.WriteLine($"  • Pivot variables: {pivotVars.Count}");
    Console.WriteLine($"  • Free variables: {freeVars.Count}");
    
    if (freeVars.Count == 0)
    {
        // System is fully determined - check if solution is valid
        Console.WriteLine($"  [STEP 3] System is fully determined, checking solution...");
        double[] solution = new double[numButtons];
        bool isValid = true;
        
        for (int row = 0; row < pivotCols.Length && row < numSlots; row++)
        {
            if (pivotCols[row] >= 0)
            {
                solution[pivotCols[row]] = augmented[row, numButtons];
                if (solution[pivotCols[row]] < 0 || Math.Abs(solution[pivotCols[row]] - Math.Round(solution[pivotCols[row]])) > 1e-9)
                {
                    isValid = false;
                    break;
                }
            }
        }
        
        if (isValid)
        {
            int sum = solution.Select(x => (int)Math.Round(x)).Sum();
            Console.WriteLine($"  ✓ Valid solution found: {sum}");
            return sum;
        }
        else
        {
            Console.WriteLine($"  ✗ No valid integer solution exists");
            return 0;
        }
    }
    
    Console.WriteLine($"  [STEP 3] Determining bounds for free variables...");
    
    // Determine upper bounds for free variables
    int[] upperBounds = new int[freeVars.Count];
    for (int i = 0; i < freeVars.Count; i++)
    {
        int freeVar = freeVars[i];
        int maxValue = joltage.Max(); // Conservative upper bound
        
        // Better bound: check which slots this button affects
        foreach (int slot in wiring[freeVar])
        {
            maxValue = Math.Min(maxValue, joltage[slot]);
        }
        
        upperBounds[i] = maxValue;
    }
    
    Console.WriteLine($"  • Upper bounds: [{string.Join(", ", upperBounds)}]");
    
    Console.WriteLine($"  [STEP 4] Enumerating free variable combinations...");
    
    int bestSum = int.MaxValue;
    long combinations = 1;
    foreach (var bound in upperBounds)
    {
        combinations *= (bound + 1);
    }
    Console.WriteLine($"  • Total combinations to check: {combinations:N0}");
    
    // Generate all combinations of free variables
    int[] freeVarValues = new int[freeVars.Count];
    long testedCount = 0;
    long valid = 0;
    
    void EnumerateCombinations(int idx)
    {
        if (idx == freeVars.Count)
        {
            testedCount++;
            if (testedCount % 100000 == 0)
            {
                string bestStr = bestSum == int.MaxValue ? "none" : bestSum.ToString();
                Console.Write($"\r    • Checked: {testedCount:N0}/{combinations:N0}, Valid: {valid:N0}, Best: {bestStr}        ");
            }
            
            // Calculate pivot variable values based on free variables
            double[] solution = new double[numButtons];
            
            // Set free variables
            for (int i = 0; i < freeVars.Count; i++)
            {
                solution[freeVars[i]] = freeVarValues[i];
            }
            
            // Calculate pivot variables from RREF
            bool isValid = true;
            for (int row = 0; row < numSlots; row++)
            {
                if (pivotCols[row] >= 0)
                {
                    double value = augmented[row, numButtons];
                    for (int col = 0; col < numButtons; col++)
                    {
                        if (!pivotVars.Contains(col))
                        {
                            value -= augmented[row, col] * solution[col];
                        }
                    }
                    solution[pivotCols[row]] = value;
                    
                    // Check if valid (non-negative integer)
                    if (value < -1e-9 || Math.Abs(value - Math.Round(value)) > 1e-9)
                    {
                        isValid = false;
                        break;
                    }
                }
            }
            
            if (isValid)
            {
                // Verify solution satisfies original equations
                int[] intSolution = solution.Select(x => (int)Math.Round(x)).ToArray();
                bool correctSolution = true;
                
                for (int slot = 0; slot < numSlots; slot++)
                {
                    int sum = 0;
                    for (int button = 0; button < numButtons; button++)
                    {
                        if (A[slot, button] > 0.5)
                        {
                            sum += intSolution[button];
                        }
                    }
                    if (sum != joltage[slot])
                    {
                        correctSolution = false;
                        break;
                    }
                }
                
                if (correctSolution)
                {
                    valid++;
                    int totalPresses = intSolution.Sum();
                    if (totalPresses < bestSum)
                    {
                        bestSum = totalPresses;
                        Console.Write("\r" + new string(' ', 120) + "\r");
                        Console.WriteLine($"    ★ New best: {bestSum} presses (checked {testedCount:N0})");
                    }
                }
            }
            
            return;
        }
        
        // Try all values for current free variable
        for (int val = 0; val <= upperBounds[idx]; val++)
        {
            freeVarValues[idx] = val;
            EnumerateCombinations(idx + 1);
        }
    }
    
    EnumerateCombinations(0);
    
    Console.Write("\r" + new string(' ', 120) + "\r");
    Console.WriteLine($"  • Enumeration complete: {testedCount:N0} combinations, {valid:N0} valid");
    
    return bestSum == int.MaxValue ? 0 : bestSum;
}

static int[] GaussJordanElimination(double[,] matrix, int rows, int cols)
{
    int[] pivotCols = new int[rows];
    for (int i = 0; i < rows; i++) pivotCols[i] = -1;
    
    int currentRow = 0;
    
    for (int col = 0; col < cols && currentRow < rows; col++)
    {
        // Find pivot
        int pivotRow = -1;
        for (int row = currentRow; row < rows; row++)
        {
            if (Math.Abs(matrix[row, col]) > 1e-9)
            {
                pivotRow = row;
                break;
            }
        }
        
        if (pivotRow == -1) continue; // No pivot in this column
        
        // Swap rows
        if (pivotRow != currentRow)
        {
            for (int j = 0; j <= cols; j++)
            {
                (matrix[currentRow, j], matrix[pivotRow, j]) = (matrix[pivotRow, j], matrix[currentRow, j]);
            }
        }
        
        // Scale pivot row
        double pivot = matrix[currentRow, col];
        for (int j = 0; j <= cols; j++)
        {
            matrix[currentRow, j] /= pivot;
        }
        
        // Eliminate column in all other rows
        for (int row = 0; row < rows; row++)
        {
            if (row != currentRow && Math.Abs(matrix[row, col]) > 1e-9)
            {
                double factor = matrix[row, col];
                for (int j = 0; j <= cols; j++)
                {
                    matrix[row, j] -= factor * matrix[currentRow, j];
                }
            }
        }
        
        pivotCols[currentRow] = col;
        currentRow++;
    }
    
    return pivotCols;
}
