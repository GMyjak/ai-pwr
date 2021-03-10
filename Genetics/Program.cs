using System;

namespace Genetics
{
    class Program
    {
        static void Main(string[] args)
        {
            Problem p = ProblemLoader.LoadProblemFromFile("../../../TestData/zad0.txt");
            Console.WriteLine(p.Paths[1].A.Y);
        }
    }
}
