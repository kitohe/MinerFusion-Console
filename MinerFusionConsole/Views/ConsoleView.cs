using System;

namespace MinerFusionConsole.Views
{
    public class ConsoleView
    {
        public void DisplayInfo()
        {
            Console.Clear();

            Console.WriteLine("MinerFusion Client v1");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Last updated {DateTime.Now}\n");

            Console.ResetColor();
        }

        public void PrintMinerInfo(string minerName, bool minerAlive, string minerHashRate)
        {
            if (minerAlive)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"ONLINE - Miner: {minerName} running at: {minerHashRate}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"OFFLINE - Miner: {minerName} running at: {minerHashRate}");
            }

            Console.ResetColor();
        }
    }
}
