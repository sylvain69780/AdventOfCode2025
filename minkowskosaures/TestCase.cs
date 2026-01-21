using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minkowskosaures
{
    class TestCase
    {
        public int N { get; }
        public double[] Sums { get; }

        public TestCase(int n, double[] sums)
        {
            N = n;
            Sums = sums;
        }
    }
}
