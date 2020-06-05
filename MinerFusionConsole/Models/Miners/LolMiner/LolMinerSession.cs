using Newtonsoft.Json;

namespace MinerFusionConsole.Models.Miners.LolMiner
{
    public class LolMinerSession
    {
        [JsonProperty(PropertyName = "Uptime")]
        public int UpTime { get; set; }

        [JsonProperty(PropertyName = "Performance_Summary")]
        public double HashRate { get; set; }
    }
}
