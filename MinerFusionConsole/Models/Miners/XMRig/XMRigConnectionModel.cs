using Newtonsoft.Json;

namespace MinerFusionConsole.Models.Miners.XMRig
{
    public class XMRigConnectionModel
    {
        [JsonProperty(PropertyName = "accepted")]
        public int AcceptedShares { get; set; }

        [JsonProperty(PropertyName = "rejected")]
        public int RejectedShares { get; set; }

        [JsonProperty(PropertyName = "uptime")]
        public int UpTime { get; set; }

        [JsonProperty(PropertyName = "pool")]
        public string MineServer { get; set; }
    }
}
