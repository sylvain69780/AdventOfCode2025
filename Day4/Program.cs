using Day4;

//var input = File.ReadAllLines("test1.txt");
var input = File.ReadAllLines("input.txt");
var count = 0;
for (var line = 0; line < input.Length; line++)
{
    for (var col = 0; col < input[0].Length;col++)
    {
        var cell = input[line][col];
        if (cell != '@') 
            continue;
        var n = input.CountNeighbours(line,col);
        if (n < 4) count++;
    }
}
Console.WriteLine(count);