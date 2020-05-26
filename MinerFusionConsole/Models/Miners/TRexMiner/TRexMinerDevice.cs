using Newtonsoft.Json;

namespace MinerFusionConsole.Models.Miners.TRexMiner
{
    public class TRexMinerDevice
    {
        [JsonProperty(PropertyName = "fan_speed")]
        public int FanSpeed { get; set; }

        [JsonProperty(PropertyName = "hashrate")]
        public double HashRate { get; set; }

        [JsonProperty(PropertyName = "temperature")]
        public int Temperature { get; set; }
    }
}
