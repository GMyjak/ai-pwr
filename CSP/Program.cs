using System;
using System.Collections.Generic;
using CSP.EinsteinRiddle;

namespace CSP
{
    class Program
    {
        static void Main(string[] args)
        {
            var xd = Permutation.GetAllPermutations();
            foreach (var perm in xd)
            {
                foreach (var i in perm.Values)
                {
                    Console.Write(i + " ");
                }   
                Console.WriteLine();
            }
        }
    }
}
