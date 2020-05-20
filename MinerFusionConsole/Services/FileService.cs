using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinerFusionConsole.Models;
using Newtonsoft.Json;

namespace MinerFusionConsole.Services
{
    public class FileService : IFileService
    {
        private const string ConfigFilename = "miners.json";

        private const string AccessKeyFileName = "access_key.txt";

        public async Task<IImmutableList<MinersConfigModel>> LoadMiners()
        {
            if (!File.Exists(ConfigFilename))
            {
                Console.WriteLine("No valid miners to load were found. Please add at least one mining rig to `miners.json` file and restart the client.");
                File.Create(ConfigFilename);
                Environment.Exit(-1);
            }

            using var file = new StreamReader(ConfigFilename, Encoding.UTF8);

            var miners = new List<MinersConfigModel>();

            try
            {
                miners = JsonConvert.DeserializeObject<List<MinersConfigModel>>(await file.ReadToEndAsync());
                file.Close();
            }
            catch (JsonSerializationException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Could not process miners config file: {e.Message}");
                Environment.Exit(-1);
            }

            if (miners == null || miners.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Miners file is empty or contains invalid entries. For instructions visit https://github.com/kitohe/MinerFusion-Console");
                Environment.Exit(-1);
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

            return miners.ToImmutableList();
        }

        public async Task<string> LoadAccessKey()
        {
            if (!File.Exists(AccessKeyFileName))
            {
                Console.WriteLine("Could not locate file with access key. New file was created, please provide your access key, save and restart client.");
                File.Create(AccessKeyFileName);
                Environment.Exit(-1);
            }

            using var file = new StreamReader(AccessKeyFileName, Encoding.UTF8);

            return await file.ReadToEndAsync();
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

        private async Task SaveMiners(IImmutableList<MinersConfigModel> miners)
        {
            await using var writer = new StreamWriter(ConfigFilename);
            await writer.WriteAsync(JsonConvert.SerializeObject(miners, Formatting.Indented));
        }
    }
}
