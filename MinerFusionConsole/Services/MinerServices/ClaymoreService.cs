using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MinerFusionConsole.BuildingBlocks;
using MinerFusionConsole.Models.Miners;
using Newtonsoft.Json.Linq;

namespace MinerFusionConsole.Services.MinerServices
{
    public class ClaymoreService : IMinerService
    {
        private readonly HttpClient _httpClient;

        private readonly string _uri;

        private readonly BaseMinerModel _model;

        public ClaymoreService(string minerId, string userId, string minerName, string minerIpAddress,
            int minerPort = 3333)
        {
            _httpClient = new HttpClient();

            _uri = new UriBuilder("http", minerIpAddress, minerPort).Uri.ToString();
            _model = new BaseMinerModel(minerId, userId, minerName);
        }

        public async Task<BaseMinerModel> GetMinerStatus()
        {
            _model.FlushModelFields();

            UnpackMinerStatus(await DownloadMinerStatus());

            return _model;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        private async Task<object> DownloadMinerStatus()
        {
            try
            {
                var message = await _httpClient.GetStringAsync(_uri);
                return JsonUtils.ExtractJsonObject(message);
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        private void UnpackMinerStatus(object minerStatus)
        {
            if (minerStatus == null) return;

            JObject jsonObject;

            try
            {
                jsonObject = (JObject)minerStatus;
            }
            catch (InvalidCastException)
            {
                _model.MinerAlive = false;
                return;
            }

            var items = (from item in jsonObject["result"] select item.ToString()).ToList();
            var minerStats = items.ElementAt(2).Split(';');
            var serverResponseTimes = Array.ConvertAll(items.ElementAt(items.Count - 2).Split(';'), int.Parse).ToList();

            _model.MinerVersion = items.ElementAt(0);
            _model.UpTime = int.Parse(items.ElementAt(1));
            _model.TotalHashRate = double.Parse(minerStats.ElementAt(0)) / 1000;
            _model.AcceptedShares = int.Parse(minerStats.ElementAt(1));
            _model.RejectedShares = int.Parse(minerStats.ElementAt(2));
            _model.PerGpuHashRate = Array.ConvertAll(items.ElementAt(3).Split(';'), double.Parse).ToList().Select(d => d / 1000).ToList();
            _model.PerGpuShares = Array.ConvertAll(items.ElementAt(9).Split(';'), int.Parse).ToList();
            _model.RigWattage = int.Parse(items.ElementAt(items.Count - 1));
            _model.MinMineServerResponseTime = serverResponseTimes.ElementAt(0);
            _model.MaxMineServerResponseTime = serverResponseTimes.ElementAt(1);
            _model.AverageMineServerResponseTime = serverResponseTimes.ElementAt(2);
            _model.MineServer = items.ElementAt(7);
            _model.MinerAlive = true;

            var gpuTempsAndFans = items.ElementAt(6).Split(';');

            for (int i = 0; i < gpuTempsAndFans.Length; i++)
            {
                if (i % 2 == 0)
                    _model.PerGpuTemperatures.Add(int.Parse(gpuTempsAndFans[i]));
                else
                    _model.PerGpuFanSpeed.Add(int.Parse(gpuTempsAndFans[i]));
            }
        }
    }
}
