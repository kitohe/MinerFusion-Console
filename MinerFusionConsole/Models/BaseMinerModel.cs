using System.Collections.Generic;

namespace MinerFusionConsole.Models.Miners
{
    public class BaseMinerModel
    {
        public bool MinerAlive { get; set; }
        public string MinerId { get; set; }
        public string MinerName { get; set; }
        public string UserId { get; set; }
        public string MinerVersion { get; set; }
        public int UpTime { get; set; }
        public double TotalHashRate { get; set; }
        public int AcceptedShares { get; set; }
        public int RejectedShares { get; set; }
        public string MineServer { get; set; }
        public int MinMineServerResponseTime { get; set; }
        public int MaxMineServerResponseTime { get; set; }
        public int AverageMineServerResponseTime { get; set; }
        public int RigWattage { get; set; }
        public List<int> PerGpuTemperatures { get; set; }
        public List<double> PerGpuHashRate { get; set; }
        public List<int> PerGpuFanSpeed { get; set; }
        public List<int> PerGpuShares { get; set; }

        public BaseMinerModel(string minerId, string userId, string minerName)
        {
            MinerName = minerName;
            MinerId = minerId;
            UserId = userId;

            PerGpuTemperatures = new List<int>();
            PerGpuHashRate = new List<double>();
            PerGpuFanSpeed = new List<int>();
            PerGpuShares = new List<int>();
        }
    }
}
