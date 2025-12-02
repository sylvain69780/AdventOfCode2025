static int Mod(int a, int n)
{
    int reste = a % n;
    return reste < 0 ? reste + n : reste;
}


var fileName = "input.txt"; // "test1.txt";
var input = File.ReadAllLines(fileName);
var dial = 50;
var countZero = 0;
foreach (var move in input)
{
    var dir = move[0];
    var dist = int.Parse(move[1..]);
    if (dir == 'L')
        dist = -dist;
    dial = Mod(dial + dist,100);
    if (dial == 0)
        countZero++;
}
Console.WriteLine($"{countZero}");
