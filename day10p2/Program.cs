/*
 * Marche en avant pour trouver un combinaison qui fonctionne.
 * On utilise les plus grands vecteurs d'abord
 */

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
    wiring = wiring.OrderBy(w => w.Length).ToArray();
    foreach (var w in wiring)
        Console.WriteLine(string.Join(',', w));

    var stack = new Stack<int[]>();
    stack.Push(new int[wiring.Length]);
    var distance = int.MaxValue;
    var depth = 0;
    while (stack.Count > 0)
    {
        var combi = stack.Pop();
        var errors = joltage.Select((v, col) => v - wiring.Select((w, c) => combi[c] * (w.Contains(col) ? 1 : 0)).Sum()).ToArray();
        if (errors.All(e => e == 0))
        {
            Console.WriteLine(new string('-', 30));
            Console.WriteLine(string.Join('-', combi));
            Console.WriteLine(new string('-', 30));
            return combi.Sum();
        }
        var currentDist = errors.Sum();
        if (currentDist < distance)
        {
            distance = currentDist;
            Console.WriteLine($"distance = {distance}");
        }
        var min = errors.Min();
        var max = errors.Max();
        /*
                on peut calculer la quantité max qu'on peut mettre de chaque vecteur.
                 si une colonne à remplir est superieure à la somme de ces max on skip
                 les colonnes a zero vont mettre à zero les max de certains vecteurs
                 ok done mais il ne faut compter les vecteurs dependants d'une colonne


                 il n'y a pas de risque d'utiliser le plus grand vecteur jusqu'à la valeur mini 
                 faux

                 je n'arrive pas à catégoriser ce problème
                 a priori juste un simple systeme d'equations linéaires à résoudre
                 sauf qu'on cherche un optiminum d'une part et des solutions entières d'autre part
                 si on explorer en utilisant les grands vecteurs d'abord on part vite en explosion combinatoire
                 il y a l'idée de maintenir "plate" la fonction d'erreur, en n'augmentant jamais la différence entre le min et le max.
                 on augemente cet ecart si on réduit le min sans reduire le max
                 mais on a pas toujours le vecteur qui a les 2 à dispo
                 réduire le min doit toujours être la dernière option.


                 Je ne suis pas satifait qu'est ce qui prouve que ca marcherait ?

                [.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}
                [...#.] (0,2,3,4) (2,3) (0,4) (0,1,2) (1,2,3,4) {7,5,12,7,2}
                [.###.#] (0,1,2,3,4) (0,3,4) (0,1,2,4,5) (1,2) {10,11,11,5,10,5}

                On voit des valeurs "rares" en pos 3 et 5, et justement 2 grands vecteur qui justement vont consommer l'un ou l'autre.
                ici il faut utiliser v0 jusqu'à plus de 3 et v2 jusqu'à plus de 5
                
                (0,1,2,3,4,5,6,7,9)
                (0,1,2,4,6,7,8,9)
                (0,1,2,3,4,5,7,9)
                (0,1,2,3,4,8,9)
                (0,4,5,6,7,9)
                (1,3,6,7,8) 
                (2,3,6,7,8)
                (0,6,8,9)
                (1,4,7)
                (2,7,9)
                (1,5)
                {69,70,57,68,59,60,70,84,46,75}

                on peut utiliser v0 (qui skip slot8) jusqu'à slot2=57 
                sauf qu'ensuite on pourra pas utiliser v1 et v2 qui ont besoin de slot2
                et donc utiliser les grands en premier n'est pas garantie d'avoir la solution la plus rapide en premier

                v1 manque slot3 et slot5
                v2 manque slot6 et slot8                
                
                si on pose la contrainte d'avoir le slot max dans le vecteur v0 va s'arrêter à 57-46 => 11
                on va toujours retenir la solution qui reduit le plus la difference entre le min et le max.
                ou bien réduit la somme des valeurs moins le min.

                ici on va utiliser v0 jusqu'à ce que slot2=46
                si on ajoute un nouveau v0, on aura un nouveau mini à 45, et on aura un +1 sur tout les autres slots
                si on utilise v1 ou v2 on change le mini slot2 ou slot8 c'est pas bon
                on va donc utiliser les v qui n'ont ni 2 ni 8

        */

        var maxPerVector = wiring.Select((v,i) => errors.Where((e,j) => v.Contains(j)).Min()).ToArray();
        var impossible = errors.Select((e, i) => (e, i)).Any(r => r.e > wiring.Select((w, j) => (w, m: maxPerVector[j])).Where(x => x.w.Contains(r.i)).Sum(x => x.m));
        if (impossible)
            continue;

        if (depth < combi.Sum())
        {
            depth = combi.Sum();
            Console.WriteLine($"depth = {depth}");
            Console.WriteLine(string.Join('-', combi));
            Console.WriteLine(string.Join('-', maxPerVector));
            Console.WriteLine(string.Join('-', errors));
            for (var i = 0; i < errors.Length; i++)
            {
                Console.WriteLine($"{errors[i]} = {string.Join('+',wiring.Select((w,j) => (w, m: maxPerVector[j])).Where(x => x.w.Contains(i)).Select(x => x.m))}");
            }
        }


        for (var index = 0; index < wiring.Length;index++)
        {
            var w = wiring[index]; 
            if (errors.Where((e,i) => e == 0 && w.Contains(i)).Any())
                continue;
            //if (indexMin != indexMax && (w.Contains(indexMin) ) )
            //    continue;
            // ne pas utiliser si on n'est pas sur les max.
            // on peut voir qu'on est bloqué lorsqu'une colonne a plus de points et qu'elle est nécessaire pour réduire le max.
            // c'est detecté par la présence de zero au debut
            // peut on aller plus loin ? on peut selectionner les erreurs qu'on va réduire
            // on ne veux pas arriver à zero sur uniquement 1 point, il faut tout mettre à zero si on met à zero.

            if (max > min && errors.Where((e,i) => w.Contains(i)).Max() != max)
                continue;
            var combinaison = combi.ToArray();
            combinaison[index++]++;
            stack.Push(combinaison);
            //if (errors.Any(e => e == 0))
            //{
            //    var indexNull = errors
            //        .Select((valeur, index) => new { valeur, index })
            //        .Where(x => x.valeur == 0)
            //        .Select(x => x.index)
            //        .ToList();
            //    var indexNotNull = errors
            //        .Select((valeur, index) => new { valeur, index })
            //        .Where(x => x.valeur > 0)
            //        .Select(x => x.index)
            //        .ToList();
            //    if (indexNotNull
            //        .Any(i => wiring.Where(w => w.Contains(i))
            //        .All(w => w.Any(j => indexNull.Contains(j)))))
            //        continue;
            //}
        }
    }
    throw new Exception("Not found");
}

