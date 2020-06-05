using System;
using System.Net.Http;
using System.Threading.Tasks;
using MinerFusionConsole.BuildingBlocks;
using MinerFusionConsole.Models.Miners;
using MinerFusionConsole.Models.Miners.LolMiner;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MinerFusionConsole.Services.MinerServices
{
    public class LolMinerService : IMinerService
    {
        private readonly HttpClient _httpClient;

        private readonly string _uri;

        private readonly BaseMinerModel _model;

        public LolMinerService(string minerId, string userId, string minerName, string minerIpAddress,
            int minerPort = 8080)
        {
            _httpClient = new HttpClient();

            _uri = new UriBuilder("http", minerIpAddress, minerPort, "summary").Uri.ToString();
            _model = new BaseMinerModel(minerId, userId, minerName);
        }

        public async Task<BaseMinerModel> GetMinerStatus()
        {
            _model.FlushModelFields();

            UnpackMinerStatus(await DownloadMinerStatus());

            return _model;
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
                jsonObject = (JObject) minerStatus;
            }
            catch (InvalidCastException)
            {
                _model.MinerAlive = false;
                return;
            }

            var data = JsonConvert.DeserializeObject<LolMinerModel>(jsonObject.ToString());

            _model.MinerAlive = true;
            _model.TotalHashRate = data.Session.HashRate;
            _model.UpTime = data.Session.UpTime;

            foreach (var gpu in data.Devices)
            {
                _model.PerGpuHashRate.Add(gpu.HashRate);
                _model.PerGpuShares.Add(gpu.AcceptedShares);
                _model.PerGpuTemperatures.Add(0); // not supported in lol miner
                _model.PerGpuFanSpeed.Add(0); // not supported in lol miner
            }
        }

        public void Dispose()
        {
            _httpClient.Dispose();           
        }

    }
}
