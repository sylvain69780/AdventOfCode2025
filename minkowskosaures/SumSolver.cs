using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minkowskosaures
{
    class SumSolver
    {
        private readonly double[] _sums;
        private readonly int _n;
        private readonly bool _verbose;

        public SumSolver(double[] sums, int n, bool verbose = false)
        {
            _sums = sums.OrderBy(x => x).ToArray();
            _n = n;
            _verbose = verbose;
        }

        public double[] Solve()
        {
            if (_verbose)
                Console.WriteLine($"Sommes triées : [{string.Join(", ", _sums)}]");

            // Cas particuliers
            if (_n == 2)
            {
                double a1 = _sums[0] / 2;
                return new[] { a1, a1 };
            }

            if (_n == 3)
            {
                return SolveN3();
            }

            // Cas général : n >= 4
            return SolveGeneral();
        }

        private double[] SolveN3()
        {
            // Pour n=3 : S1=a1+a2, S2=a1+a3, S3=a2+a3
            // a1 = (S1+S2-S3)/2
            double a1 = (_sums[0] + _sums[1] - _sums[2]) / 2;
            double a2 = _sums[0] - a1;
            double a3 = _sums[1] - a1;

            if (a1 > 0 && a1 <= a2 && a2 <= a3)
            {
                return new[] { a1, a2, a3 };
            }

            return null;
        }

        private double[] SolveGeneral()
        {
            double S1 = _sums[0];

            if (_verbose)
                Console.WriteLine($"S₁ = {S1} = a₁ + a₂");

            // Tester différentes hypothèses pour S2 (a1 + a3)
            for (int idx2 = 1; idx2 < Math.Min(5, _sums.Length); idx2++)
            {
                double S2 = _sums[idx2];

                if (_verbose)
                    Console.WriteLine($"Hypothèse : S₂ = {S2} = a₁ + a₃");

                // Tester différentes hypothèses pour S3 (a2 + a3)
                for (int idx3 = idx2 + 1; idx3 < Math.Min(idx2 + 5, _sums.Length); idx3++)
                {
                    double S3 = _sums[idx3];

                    // Formule : a1 = (S1 + S2 - S3) / 2
                    double a1 = (S1 + S2 - S3) / 2;

                    // Vérifier que a1 est valide
                    if (a1 <= 0 || Math.Abs(a1 - Math.Round(a1, 10)) > 1e-9)
                        continue;

                    double a2 = S1 - a1;
                    double a3 = S2 - a1;

                    // Vérifier l'ordre
                    if (a2 < a1 - 1e-9 || a3 < a2 - 1e-9)
                        continue;

                    if (_verbose)
                        Console.WriteLine($"  Test : a₁={a1}, a₂={a2}, a₃={a3}");

                    // Tenter de reconstruire tous les nombres
                    var result = TryReconstruct(a1, a2, a3);
                    if (result != null)
                        return result;
                }
            }

            return null;
        }

        private double[] TryReconstruct(double a1, double a2, double a3)
        {
            var numbers = new List<double> { a1, a2, a3 };
            var usedSums = new HashSet<double> { a1 + a2, a1 + a3, a2 + a3 };

            // Trouver les nombres restants
            for (int i = 4; i <= _n; i++)
            {
                bool found = false;

                foreach (var sum in _sums)
                {
                    if (usedSums.Contains(sum))
                        continue;

                    double candidate = sum - a1;

                    // Le candidat doit être plus grand que le dernier nombre
                    if (candidate < numbers[^1] )
                        continue;

                    // Vérifier que toutes les sommes avec ce candidat existent
                    bool allExist = true;
                    var newSums = new List<double>();

                    for (int j = 0; j < numbers.Count; j++)
                    {
                        double expectedSum = numbers[j] + candidate;

                        if (!_sums.Contains(expectedSum))
                        {
                            allExist = false;
                            break;
                        }

                        newSums.Add(expectedSum);
                    }

                    if (allExist)
                    {
                        numbers.Add(candidate);
                        foreach (var s in newSums)
                            usedSums.Add(s);

                        if (_verbose)
                            Console.WriteLine($"  Trouvé : a_{i}={candidate}");

                        found = true;
                        break;
                    }
                }

                if (!found)
                    return null;
            }

            // Vérification finale
            var allSums = new List<double>();
            for (int i = 0; i < numbers.Count; i++)
            {
                for (int j = i + 1; j < numbers.Count; j++)
                {
                    allSums.Add(numbers[i] + numbers[j]);
                }
            }

            allSums.Sort();
            if (allSums.SequenceEqual(_sums))
            {
                return numbers.ToArray();
            }

            return null;
        }
    }
}
