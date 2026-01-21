//https://prologin.org/train/2016/semifinal/reproduction_de_minkowskosaures
// solution de Claude

using minkowskosaures;

var input = File.ReadAllLines("input2.txt");
var n = int.Parse(input[0]);
var children = input[1].Split(' ').Select(x => double.Parse(x)).ToArray();

Console.WriteLine("{n}");

Console.WriteLine("=== Reconstruction de nombres depuis leurs sommes ===\n");

// Exemples de test
TestCase[] testCases = new[]
{
                new TestCase(4, [5.0, 7, 8, 9, 10, 12]),
                new TestCase(4, [2.0, 6, 6, 9, 9, 13]),
                new TestCase(5, [3.0, 4, 5, 5, 6, 6, 7, 7, 8, 9]),
                new TestCase(3, [3.0, 5, 8]),
                new TestCase(n,children),
            };

foreach (var test in testCases)
{
    Console.WriteLine($"Test : n={test.N}, sommes=[{string.Join(", ", test.Sums)}]");
    var solver = new SumSolver(test.Sums, test.N);
    var result = solver.Solve();

    if (result != null)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"✓ Solution : [{string.Join(", ", result)}]");
        Console.ResetColor();

        // Vérification
        if (VerifySolution(result, test.Sums))
        {
            Console.WriteLine("  Vérification : OK\n");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("  Vérification : ÉCHEC\n");
            Console.ResetColor();
        }
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("✗ Aucune solution trouvée\n");
        Console.ResetColor();
    }
}

static bool VerifySolution(double[] numbers, double[] expectedSums)
{
    var reconstructed = new List<double>();
    for (int i = 0; i < numbers.Length; i++)
    {
        for (int j = i + 1; j < numbers.Length; j++)
        {
            reconstructed.Add(numbers[i] + numbers[j]);
        }
    }

    reconstructed.Sort();
    var sorted = expectedSums.OrderBy(x => x).ToArray();

    return reconstructed.SequenceEqual(sorted);
}
