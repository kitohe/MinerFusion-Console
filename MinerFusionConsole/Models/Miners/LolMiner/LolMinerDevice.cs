using Newtonsoft.Json;

namespace MinerFusionConsole.Models.Miners.LolMiner
{
    public class LolMinerDevice
    {
        [JsonProperty(PropertyName = "index")]
        public int Index { get; set; }

        [JsonProperty(PropertyName = "Session_Accepted")]
        public int AcceptedShares { get; set; }

        [JsonProperty(PropertyName = "Performance")]
        public double HashRate { get; set; }
        
    }
}
