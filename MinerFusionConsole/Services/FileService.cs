using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MinerFusionConsole.BuildingBlocks;
using MinerFusionConsole.Models;
using Newtonsoft.Json;

namespace MinerFusionConsole.Services
{
    public class FileService : IFileService
    {
        private const string ConfigFilename = "miners.json";

        private const string AccessKeyFileName = "access_key.txt";

        private FileResponse _fileResponse;

        public FileService()
        {
            _fileResponse = new FileResponse();
        }

        public async Task<FileResponse> Load()
        {
            await LoadMiners();
            await LoadAccessKey();

            return _fileResponse;
        }

        private async Task LoadMiners()
        {
            if (!File.Exists(ConfigFilename))
            {
                _fileResponse.AddError("No valid miners to load were found. Please add at least one mining rig to `miners.json` file and restart the client.");
                File.Create(ConfigFilename);
                return;
            }

            using var file = new StreamReader(ConfigFilename, Encoding.UTF8);

            List<MinersConfigModel> miners;

            try
            {
                miners = JsonConvert.DeserializeObject<List<MinersConfigModel>>(await file.ReadToEndAsync());
                file.Close();
            }
            catch (JsonSerializationException e)
            {
                _fileResponse.AddError($"Could not process miners config file: {e.Message}");
                return;
            }
            catch (JsonReaderException e)
            {
                _fileResponse.AddError($"Could not process miners config file: {e.Message}");
                return;
            }

            if (miners == null || miners.Count == 0)
            {
                _fileResponse.AddError("Miners file is empty or contains invalid entries. For instructions visit https://github.com/kitohe/MinerFusion-Console");
                return;
            }

            if (miners.Any(miner => miner.MinerPort < 1 || miner.MinerPort > 65535))
            {
                FixMinerPorts(miners);
                await SaveMiners(miners.ToImmutableList());
            }
                
            if (miners.Any(miner => miner.MinerId == null || !Guid.TryParse(miner.MinerId, out _)))
            {
                GenerateValidGuids(miners);
                await SaveMiners(miners.ToImmutableList());
            }

            _fileResponse.SetMinersConfig(miners.ToImmutableList());
        }

        private async Task LoadAccessKey()
        {
            if (!File.Exists(AccessKeyFileName))
            {
                _fileResponse.AddError("Could not locate file with access key. New file was created, please provide your access key, save and restart client.");
                File.Create(AccessKeyFileName);
            }

            using var file = new StreamReader(AccessKeyFileName, Encoding.UTF8);

            _fileResponse.SetAccessKey(await file.ReadToEndAsync());
        }

        private void GenerateValidGuids(IEnumerable<MinersConfigModel> miners)
        {
            foreach (var miner in miners)
                if (miner.MinerId == null || !Guid.TryParse(miner.MinerId, out _))
                    miner.MinerId = Guid.NewGuid().ToString();
        }

        private void FixMinerPorts(IEnumerable<MinersConfigModel> miners)
        {
            foreach (var miner in miners)
                if (miner.MinerPort < 1 || miner.MinerPort > 65535)
                    miner.MinerPort = 3333; // Default miner monitoring port
        }

        private void CheckIfValidIp(IEnumerable<MinersConfigModel> miners)
        {
            foreach (var miner in miners)
            {
                if(!IPAddress.TryParse(miner.MinerIpAddress, out _))
                    _fileResponse.AddError($"Invalid IP address detected for miner: {miner.MinerName}");
            }
        }

        private async Task SaveMiners(IImmutableList<MinersConfigModel> miners)
        {
            await using var writer = new StreamWriter(ConfigFilename);
            await writer.WriteAsync(JsonConvert.SerializeObject(miners, Formatting.Indented));
        }
    }
}
