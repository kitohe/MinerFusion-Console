using Newtonsoft.Json;

namespace MinerFusionConsole.Models.Miners.XMRig
{
    public class XMRigMinerModel
    {
        [JsonProperty(PropertyName = "hashrate")]
        public XMRigHashRateModel HashRate { get; set; }

        [JsonProperty(PropertyName = "connection")]
        public XMRigConnectionModel Connection { get; set; }
    }
}
