using System;
using CSP.EinsteinRiddle;

namespace CSP
{
    class Program
    {
        static void Main(string[] args)
        {
            EinsteinRiddleNaPale riddle = new EinsteinRiddleNaPale();
            var xd = riddle.RunAlgo();

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
