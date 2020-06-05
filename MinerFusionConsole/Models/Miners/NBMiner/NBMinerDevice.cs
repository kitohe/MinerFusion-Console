using Newtonsoft.Json;

namespace MinerFusionConsole.Models.Miners.NBMiner
{
    public sealed class NBMinerDevice
    {
        [JsonProperty(PropertyName = "hashrate_raw")]
        public double HashRateRaw { get; set; }

        [JsonProperty(PropertyName = "accepted_shares")]
        public int AcceptedShares { get; set; }

        [JsonProperty(PropertyName = "fan")]
        public int Fan { get; set; }

        [JsonProperty(PropertyName = "temperature")]
        public int Temperature { get; set; }

        [JsonProperty(PropertyName = "rejected_shares")]
        public int RejectedShares { get; set; }

        [JsonProperty(PropertyName = "power")]
        public int Power { get; set; }
    }
}
