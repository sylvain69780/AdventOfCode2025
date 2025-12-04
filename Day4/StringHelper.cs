using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day4
{
    public static class StringHelper
    {
        public static int CountNeighbours(this string[] s, int line, int column)
        {
            char SafeChar(int line, int column)
            {
                if (line < 0 || column < 0)
                    return ' ';
                if (line >= s.Length || column >= s[0].Length)
                    return ' ';
                return s[line][column];
            }
            var count = 0;
            for (var x = -1; x <= 1; x++)
                for (var y = -1; y <= 1; y++)
                {
                    if ((x, y) == (0, 0))
                        continue;
                    var c = SafeChar(line + y, column + x);
                    if (c == '@')
                        count++;
                }
            return count;
        }
    }
}
