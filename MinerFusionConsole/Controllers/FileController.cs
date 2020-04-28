using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using MinerFusionConsole.Models;
using MinerFusionConsole.Services;

namespace MinerFusionConsole.Controllers
{
    public class FileController
    {
        private readonly IFileService _fileSvc;

        public FileController()
        {
            _fileSvc = new FileService();
        }

        public async Task<Tuple<string, IImmutableList<MinersConfigModel>>> Setup()
        {
            return new Tuple<string, IImmutableList<MinersConfigModel>>
                (await _fileSvc.LoadAccessKey(), await _fileSvc.LoadMiners());
        }
    }
}
