using System.Collections.Generic;
using Newtonsoft.Json;

namespace MinerFusionConsole.Models.Miners.PhoenixMiner
{
    public class PhoenixMinerModel
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; }

        [JsonProperty(PropertyName = "result")]
        public IEnumerable<string> Result { get; set; }
    }
}
