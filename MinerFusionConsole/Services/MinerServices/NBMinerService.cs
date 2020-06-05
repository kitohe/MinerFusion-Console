using System;
using System.Net.Http;
using System.Threading.Tasks;
using MinerFusionConsole.BuildingBlocks;
using MinerFusionConsole.Models.Miners;
using MinerFusionConsole.Models.Miners.NBMiner;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MinerFusionConsole.Services.MinerServices
{
    public class NbMinerService : IMinerService
    {
        private readonly HttpClient _httpClient;

        private readonly string _uri;

        private readonly BaseMinerModel _model;

        public NbMinerService(string minerId, string userId, string minerName, string minerIpAddress,
            int minerPort = 22333)
        {
            _httpClient = new HttpClient();

            _uri = new UriBuilder("http", minerIpAddress, minerPort, "api/v1/status").Uri.ToString();
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
            
            var data = JsonConvert.DeserializeObject<NBMinerModel>(jsonObject["miner"].ToString());

            _model.MinerAlive = true;
            _model.RigWattage = data.RigWattage;
            _model.TotalHashRate = Math.Round(data.TotalHashRateRaw / 10e5, 3); // to MH/s

            foreach (var gpu in data.Devices)
            {
                _model.PerGpuHashRate.Add(Math.Round(gpu.HashRateRaw / 10e5, 3));
                _model.PerGpuFanSpeed.Add(gpu.Fan);
                _model.PerGpuShares.Add(gpu.AcceptedShares);
                _model.PerGpuTemperatures.Add(gpu.Temperature);
                _model.AcceptedShares += gpu.AcceptedShares;
            }
        }
    }
}
