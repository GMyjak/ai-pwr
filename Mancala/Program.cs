namespace Mancala
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleUI ui = new ConsoleUI()
            {
                p1 = PlayerType.Computer,
                p2 = PlayerType.Computer,
                firstAiMoveRandom = true,
                //algorithm = Algo.MinMax,
                algorithm = Algo.AlphaBeta,
            };

            ui.Run();
        }
    }
}
