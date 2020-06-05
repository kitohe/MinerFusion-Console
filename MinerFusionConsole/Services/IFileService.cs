using System.Threading.Tasks;
using MinerFusionConsole.BuildingBlocks;

namespace MinerFusionConsole.Services
{
    public interface IFileService
    {
        Task<FileResponse> Load();
    }
}
