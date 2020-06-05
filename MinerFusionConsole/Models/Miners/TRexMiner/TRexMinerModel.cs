using System.Collections.Generic;
using Newtonsoft.Json;

namespace MinerFusionConsole.Models.Miners.TRexMiner
{
    public class TRexMinerModel
    {
        [JsonProperty(PropertyName = "hashrate")]
        public double TotalHashRate { get; set; }

        [JsonProperty(PropertyName = "solved_count")]
        public int AcceptedShares { get; set; }

        [JsonProperty(PropertyName = "uptime")]
        public int UpTime { get; set; }

        [JsonProperty(PropertyName = "gpus")]
        public IEnumerable<TRexMinerDevice> Devices { get; set; }
    }
}
