using System.Runtime.CompilerServices;

var inputFile = "test1.txt";
inputFile = "input.txt";

var input = File.ReadAllLines(inputFile).Select(x => x.ToCharArray()).ToArray();
var lines = input.Length;
var rows = input[0].Length;
var count = 0;

for (var line = 0; line<lines-1;line++)
{
    for (var row = 0; row<rows; row++)
    {
        var c = input[line][row];
        if ( c == '|' || c == 'S')
        {
            var down = input[line + 1][row];
            if (down == '^')
            {
                count++;
                if (row - 1 >= 0)
                    input[line + 1][row - 1] = '|';
                if (row + 1 < rows)
                    input[line + 1][row + 1] = '|';
            }
            else
                input[line + 1][row] = '|';
        }
    }
}

Console.WriteLine(count);