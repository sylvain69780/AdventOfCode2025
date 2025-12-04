using Day4;

//var input = File.ReadAllLines("test1.txt");
var input = File.ReadAllLines("input.txt");
var count = int.MaxValue;
var lastCount = 0;
var removed = 0;
do
{
    lastCount = count;
    count = 0;
    for (var line = 0; line < input.Length; line++)
    {
        for (var col = 0; col < input[0].Length; col++)
        {
            var cell = input[line][col];
            if (cell != '@')
                continue;
            count++;
            var n = input.CountNeighbours(line, col);
            if (n < 4)
            {
                var ca = input[line].ToCharArray();
                ca[col] = ' ';
                input[line] = new String(ca);
                removed++;
            }
        }
    }
} while (count < lastCount);

Console.WriteLine(removed);
