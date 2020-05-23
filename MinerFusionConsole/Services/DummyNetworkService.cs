using System;
using System.Threading;
using System.Threading.Tasks;
using MinerFusionConsole.Models.Miners;

namespace MinerFusionConsole.Services
{
    public class DummyNetworkService : INetworkService
    {
        public void Dispose()
        {
            Console.WriteLine("Simulating dispose...");
        }

        public async Task<bool> SendMinerData(BaseMinerModel data)
        {
            await Task.Yield();
            Thread.Sleep(10);
            return true;
        }

        public async Task Setup()
        {
            await Task.Yield();
            Console.WriteLine("Simulating Setup function...");
        }
    }
}
