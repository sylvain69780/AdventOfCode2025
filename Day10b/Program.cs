/*
 * Marche en avant pour trouver un combinaison qui fonctionne.
 * On utilise les plus grands vecteurs d'abord
 * 
 * C'est le drame pourquoi ?
 * 
 * On met les vecteurs longs
 * Ca coince car il n'y a plus de place pour les vecteurs nécessaires sur certaines coordonnées
 * On peut essayer de minimiser le max des erreurs
 * 
 * Problème on va encore une fois utiliser des vecteurs qui vont bloquer d'autres
 * 
 * On peut utiliser les plus petits vecteurs d'abord mais eux aussi ca peut merder.
 * 
 * On peut cibler les colonnes avec le moins de cardinalité en premier.
 * 
 * C'est vraiement tout le problème de cette optimisation
 * 
 * Comment savoir quand ca va "bloquer ???"
 * 
 * Quand on voit qu'on est bloqué : on ne peux pas mettre une pièce qui réduirait l'erreur max
 * On regarde la colonne où il y a l'erreur max
 * Si on ne peux pas y mettre une pièce on est donc bloqué
 * On doit faire un graphe pour eviter ces interbloquages
 * 
 * 
 */

var inputFile = "test1.txt";
inputFile = "input.txt";
var input = File.ReadAllLines(inputFile)
    .Select(line => line.Split(' '))
    .Select(line => (diagrams: line[0][1..^1], wiring: line[1..^1].Select(x => x[1..^1].Split(',').Select(x => int.Parse(x)).ToArray()).ToArray(), joltage: line[^1][1..^1].Split(',').Select(x => int.Parse(x)).ToArray()))
    .ToArray();

var count = 0;
foreach (var (_, wiring, joltage) in input)
{
    Console.WriteLine(string.Join(',', joltage));
    //foreach (var w in wiring)
    //    Console.WriteLine(string.Join(',', w));
    int pressed = Solve(wiring, joltage);
    count += pressed;
}

Console.WriteLine(count);

static int Solve(int[][] wiring, int[] joltage)
{
    wiring = wiring.OrderBy(w => w.Length).ToArray();
    foreach (var w in wiring)
        Console.WriteLine(string.Join(',', w));

    var stack = new Stack<int[]>();
    stack.Push(new int[wiring.Length]);

    // colonnes et vecteurs connectés associés
    //var dependsOn = joltage.
    //Select((_, i) => (i, w: wiring.Where(w => w.Contains(i)).ToArray()))
    //.ToDictionary(x => x.i, x => x.w);

    while (stack.Count > 0)
    {
        var combinaison = stack.Pop();
        var errors = joltage.Select((v, col) => v - wiring.Select((w, c) => combinaison[c] * (w.Contains(col) ? 1 : 0)).Sum()).ToArray();
        if (errors.All(e => e == 0))
        {
            Console.WriteLine(new string('-', 30));
            Console.WriteLine(string.Join('-', combinaison));
            return combinaison.Sum();
        }
        var combinaisons = new List<(int[] c,int e)>();
        var i = 0;
        foreach (var w in wiring)
        {
            var combinaison2 = combinaison.ToArray();
            combinaison2[i++]++;
            var err = joltage.Select((v, col) => v - wiring.Select((w, c) => combinaison2[c] * (w.Contains(col) ? 1 : 0)).Sum()).ToArray();
            if (err.Any(e => e < 0))
                continue;
            combinaisons.Add((combinaison2, err.Max()));
        }
        foreach (var (c,e) in combinaisons.OrderByDescending(item => item.e))
        {
            stack.Push(c);
        }
    }
    throw new Exception("Not found");
}

