using System.Collections.Immutable;
using System.Threading.Tasks;
using MinerFusionConsole.Models;

namespace MinerFusionConsole.Services
{
    public interface IFileService
    { 
        Task<IImmutableList<MinersConfigModel>> LoadMiners();
        Task<string> LoadAccessKey();
    }
}
