using System;

namespace MinerFusionConsole.Views
{
    public class ConsoleView
    {
        public void DisplayInfo()
        {
            Console.Clear();

            Console.WriteLine("MinerFusion Client v0.1");

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

        public void ServerCommunicationError()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("There was an error while trying to communicate with MinerFusion server.\nPlease check you miners.json file and/or validity of you access key.");
            Console.ResetColor();
        }
    }
}
