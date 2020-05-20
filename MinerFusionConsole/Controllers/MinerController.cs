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
        private IEnumerable<IMinerService> _minerSvc;

        private readonly ConsoleView _consoleView;

        private readonly INetworkService _networkSvc;

        private const int UpdateInterval = 5;

        public MinerController(Tuple<string, IImmutableList<MinersConfigModel>> data)
        {
            _minerSvc = CreateTasks(data);
            _consoleView = new ConsoleView();
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
                    await _networkSvc.SendMinerData(item);
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
            switch (minerType)
            {
                case MinerTypes.Claymore:
                    return new ClaymoreService(minerId, userId, minerName, minerIpAddress, minerPort);
                case MinerTypes.Phoenix:
                    return new PhoenixService(minerId, userId, minerName, minerIpAddress, minerPort, minerPassword);
                default:
                    throw new ArgumentOutOfRangeException(nameof(minerType), minerType, null);
            }
        }

        private IEnumerable<IMinerService> CreateDummyMiners()
        {
            List<IMinerService> dummyMiners = new List<IMinerService>();

            for (int i = 0; i < 10; i++)
            {
                //dummyMiners.Add(new ClaymoreService(i.ToString(), Guid.NewGuid().ToString(), "miner: " + i, "192.168.1.44"));
                dummyMiners.Add(new PhoenixService(i.ToString(), Guid.NewGuid().ToString(), "miner: " + i, "192.168.1.44"));
            }

            return dummyMiners;
        }
    }
}
