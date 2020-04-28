using System;
using System.Collections.Immutable;
using System.IO;
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
                return null;
            }

            using var file = new StreamReader(ConfigFilename, Encoding.UTF8);

            return JsonConvert.DeserializeObject<ImmutableList<MinersConfigModel>>(await file.ReadToEndAsync());
        }

        public async Task<string> LoadAccessKey()
        {
            if (!File.Exists(AccessKeyFileName))
            {
                Console.WriteLine("Could not locate file with access key. New file was created, please provide your access key, save and restart client.");
                return string.Empty;
            }

            using var file = new StreamReader(AccessKeyFileName, Encoding.UTF8);

            return await file.ReadToEndAsync();
        }
    }
}
