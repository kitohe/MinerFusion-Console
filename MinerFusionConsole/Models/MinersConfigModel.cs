namespace MinerFusionConsole.Models
{
    public enum MinerTypes
    {
        Claymore,
        Phoenix,
        NBMiner
    }

    public class MinersConfigModel
    {
        public MinerTypes MinerType { get; set; }
        public string MinerName { get; set; }
        public string MinerIpAddress { get; set; }
        public string MinerPassword { get; set; }
        public string MinerId { get; set; }
        public int MinerPort { get; set; }
    }
}
