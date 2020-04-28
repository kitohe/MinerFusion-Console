using System;
using System.Threading.Tasks;
using MinerFusionConsole.Models.Miners;

namespace MinerFusionConsole.Services
{
    public interface INetworkService : IDisposable
    {
        Task SendMinerData(BaseMinerModel data);
        Task Setup();
    }
}
