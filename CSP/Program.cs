using System;
using CSP.EinsteinRiddle;
using CSP.MapColouring;

namespace CSP
{
    class Program
    {
        static void Main(string[] args)
        {
            TestEinstein();
            //TestMCP();
        }

        static void TestEinstein()
        {
            int solutionCount = 0;
            EinsteinRiddleProblem riddle = new EinsteinRiddleProblem();
            riddle.OnSolutionFound += (xd) =>
            {
                solutionCount++;

                EinsteinRiddleProblem.DisplaySolution(xd);
            };
            riddle.RunBacktracking();
            Console.WriteLine("Solutions: " + solutionCount);
        }

        static void TestMCP()
        {
            int solutionCount = 0;
            MapColouringNaPale riddle = new MapColouringNaPale();
            riddle.GenerateInstance(3,3);
            riddle.OnSolutionFound += (xd) =>
            {
                solutionCount++;
            };
            riddle.RunAlgo();
            Console.WriteLine("Solutions: " + solutionCount);
        }

        // TODO WIZUALIZACJA MCP
        // Generowanie kozackiej bitmapki

        // TODO NAPRAWIĆ GENEROWANIE
        // 1. wygeneruj wszystkie możliwe connections między cords
        // 2. wybierz losowy punkt z puli punktów we wszystkich conn
        // 3. wybierz losowy najkrótszy conn który zawiera wylosowany punkt i dodaj conn do listy conn
        // 4. wyrzuć wszystkie conn które krzyżują się z wybranym conn
        // 5. Rób 2,3,4 dopóki lista wszystkich conn nie jest pusta

        // TODO DOPASOWAĆ WSZYSTKO DO ABSTRAKCJI
        // 1. Zmienić return type (callback) w Einstein Riddle
        // 2. Opakować constraint w obiekt z referencjami na zmienne
        // 3. Zmienić abstrakcyjny framework i zaimplementować w Einstein i MCP
    }
}
