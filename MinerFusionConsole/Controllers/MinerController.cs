using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MinerFusionConsole.Models;
using MinerFusionConsole.Models.Miners;
using MinerFusionConsole.Services;
using MinerFusionConsole.Services.MinerServices;
using MinerFusionConsole.Views;

namespace MinerFusionConsole.Controllers
{
    public class MinerController
    {
        private readonly IEnumerable<IMinerService> _minerSvc;

        private readonly MinersView _consoleView;

        private readonly INetworkService _networkSvc;

        private const int UpdateInterval = 5;

        public MinerController(Tuple<string, IImmutableList<MinersConfigModel>> data)
        {
            _minerSvc = CreateTasks(data);
            _consoleView = new MinersView();
            _networkSvc = new NetworkService();

            _networkSvc.Setup();
        }

        public async Task Work()
        {
            var tasksToRun = new List<Task<BaseMinerModel>>();

            while (true)
            {
                _consoleView.DisplayInfo();
                tasksToRun.AddRange(_minerSvc.Select(item => item.GetMinerStatus()));

                var results = await Task.WhenAll(tasksToRun);

                foreach (var item in results)
                {
                    _consoleView.PrintMinerInfo(item.MinerName, item.MinerAlive, item.TotalHashRate.ToString(CultureInfo.InvariantCulture));
                    if (!await _networkSvc.SendMinerData(item))
                        _consoleView.ServerCommunicationError();
                }

                tasksToRun.Clear();

                Thread.Sleep(TimeSpan.FromSeconds(UpdateInterval));
            }
        }

        private IEnumerable<IMinerService> CreateTasks(Tuple<string, IImmutableList<MinersConfigModel>> data)
        {
            if (data == null) return new List<IMinerService>();

            var activeMiners = new List<IMinerService>();

            var accessKey = data.Item1;
            var configList = data.Item2;
            
            foreach (var item in configList)
            {
                activeMiners.Add(GetService(item.MinerType, item.MinerId, accessKey, item.MinerName,
                    item.MinerIpAddress, item.MinerPassword, item.MinerPort));
            }

            return activeMiners;
        }

        private IMinerService GetService(MinerTypes minerType, string minerId, string userId, string minerName,
            string minerIpAddress, string minerPassword, int minerPort=3333)
        {
            return minerType switch
            {
                MinerTypes.Claymore => new ClaymoreService(minerId, userId, minerName, minerIpAddress, minerPort),
                MinerTypes.Phoenix => new PhoenixService(minerId, userId, minerName, minerIpAddress, minerPort,
                    minerPassword),
                MinerTypes.NBMiner => new NbMinerService(minerId, userId, minerName, minerIpAddress, minerPort),
                MinerTypes.LolMiner => new LolMinerService(minerId, userId, minerName, minerIpAddress, minerPort),
                MinerTypes.TRex => new TRexMinerService(minerId, userId, minerName, minerIpAddress, minerPort),
                _ => throw new ArgumentOutOfRangeException(nameof(minerType), minerType, null)
            };
        }
    }
}
