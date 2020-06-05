using System;
using System.Net.Http;
using System.Threading.Tasks;
using MinerFusionConsole.BuildingBlocks;
using MinerFusionConsole.Models.Miners;
using MinerFusionConsole.Models.Miners.TRexMiner;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MinerFusionConsole.Services.MinerServices
{
    public class TRexMinerService : IMinerService
    {
        private readonly HttpClient _httpClient;

        private readonly string _uri;

        private readonly BaseMinerModel _model;

        public TRexMinerService(string minerId, string userId, string minerName, string minerIpAddress,
            int minerPort = 4067)
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
                jsonObject = (JObject) minerStatus;
            }
            catch (InvalidCastException)
            {
                _model.MinerAlive = false;
                return;
            }

            var data = JsonConvert.DeserializeObject<TRexMinerModel>(jsonObject.ToString());

            _model.MinerAlive = true;
            _model.TotalHashRate = data.TotalHashRate;
            _model.AcceptedShares = data.AcceptedShares;

            foreach (var gpu in data.Devices)
            {
                _model.PerGpuHashRate.Add(gpu.HashRate);
                _model.PerGpuFanSpeed.Add(gpu.FanSpeed);
                _model.PerGpuTemperatures.Add(gpu.Temperature);
                _model.PerGpuShares.Add(0); // per gpu shares are not supported in TRex
            }
        }
    }
}
