using System.Collections.Generic;
using System.Collections.Immutable;
using MinerFusionConsole.Models;

namespace MinerFusionConsole.BuildingBlocks
{
    public class FileResponse
    {
        public IImmutableList<MinersConfigModel> MinersConfig { get; private set; }
        public string AccessKey { get; private set; }
        public List<string> Errors { get; }

        public FileResponse()
        {
            Errors = new List<string>();
        }

        public void AddError(string message)
        {
            Errors.Add(message);
        }

        public void SetMinersConfig(IImmutableList<MinersConfigModel> minersConfig)
        {
            MinersConfig ??= minersConfig;
        }

        public void SetAccessKey(string accessKey)
        {
            AccessKey ??= accessKey;
        }
    }
}
