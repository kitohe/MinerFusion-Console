using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using MinerFusionConsole.Models;
using MinerFusionConsole.Services;
using MinerFusionConsole.Views;

namespace MinerFusionConsole.Controllers
{
    public class FileController
    {
        private readonly IFileService _fileSvc;

        private readonly FileView _fileView;

        public FileController()
        {
            _fileSvc = new FileService();
            _fileView = new FileView();
        }

        public async Task<Tuple<string, IImmutableList<MinersConfigModel>>> Setup()
        {
            var fileResponse = await _fileSvc.Load();

            if (fileResponse.Errors.Any())
            {
                _fileView.DisplayErrors(fileResponse.Errors);
                Environment.Exit(-1);
            }
            
            return new Tuple<string, IImmutableList<MinersConfigModel>>
                (fileResponse.AccessKey, fileResponse.MinersConfig);
        }
    }
}
