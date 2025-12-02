
var fileName = "input.txt"; // "test1.txt";
//var fileName = "test1.txt";
var input = File.ReadAllLines(fileName);
var dial = 50;
var countZero = 0;
foreach (var move in input)
{
    var dir = move[0];
    var dist = int.Parse(move[1..]);
    for (var i = 0; i < dist; i++)
    {
        dial += dir == 'R' ? 1 : -1;
        if (dial >= 100)
            { dial -= 100; }
        if (dial < 0)
            { dial += 100; }
        if (dial == 0)
            countZero++;
    }
}
Console.WriteLine($"{countZero}");
