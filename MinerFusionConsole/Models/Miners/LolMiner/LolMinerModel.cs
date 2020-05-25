using System.Collections.Generic;
using Newtonsoft.Json;

namespace MinerFusionConsole.Models.Miners.LolMiner
{
    public class LolMinerModel
    {
        [JsonProperty(PropertyName = "GPUs")]
        public IEnumerable<LolMinerDevice> Devices { get; set; }

        public LolMinerSession Session { get; set; }
    }
}
