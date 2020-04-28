using System;
using System.Threading.Tasks;
using MinerFusionConsole.Models.Miners;

namespace MinerFusionConsole.Services.MinerServices
{
    public interface IMinerService : IDisposable
    { 
        Task<BaseMinerModel> GetMinerStatus();
    }
}
