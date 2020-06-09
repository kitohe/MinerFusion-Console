using System.Collections.Generic;
using Newtonsoft.Json;

namespace MinerFusionConsole.Models.Miners.XMRig
{
    public class XMRigHashRateModel
    {
        [JsonProperty(PropertyName = "total")]
        public IEnumerable<double?> TotalHashRate { get; set; }

        [JsonProperty(PropertyName = "threads")]
        public IEnumerable<IEnumerable<double?>> Threads { get; set; }
    }
}
