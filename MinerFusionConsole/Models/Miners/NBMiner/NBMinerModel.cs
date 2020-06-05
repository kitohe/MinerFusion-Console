using System.Collections.Generic;
using Newtonsoft.Json;

namespace MinerFusionConsole.Models.Miners.NBMiner
{
    public sealed class NBMinerModel
    {
        public IEnumerable<NBMinerDevice> Devices { get; set; }

        [JsonProperty(PropertyName = "total_hashrate_raw")]
        public double TotalHashRateRaw { get; set; }

        [JsonProperty(PropertyName = "total_power_consume")]
        public int RigWattage { get; set; }
    }
}
