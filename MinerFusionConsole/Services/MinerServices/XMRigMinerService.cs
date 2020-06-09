using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MinerFusionConsole.BuildingBlocks;
using MinerFusionConsole.Models.Miners;
using MinerFusionConsole.Models.Miners.XMRig;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MinerFusionConsole.Services.MinerServices
{
    public class XMRigMinerService : IMinerService
    {
        private readonly HttpClient _httpClient;

        private readonly string _uri;

        private readonly BaseMinerModel _model;

        public XMRigMinerService(string minerId, string userId, string minerName, string minerIpAddress,
            int minerPort = 3333)
        {
            _httpClient = new HttpClient();

            _uri = new UriBuilder("http", minerIpAddress, minerPort, "/1/summary").Uri.ToString();
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

            var data = JsonConvert.DeserializeObject<XMRigMinerModel>(jsonObject.ToString());

            _model.MinerAlive = true;
            _model.MineServer = data.Connection.MineServer;
            _model.AcceptedShares = data.Connection.AcceptedShares;
            _model.RejectedShares = data.Connection.RejectedShares;

            _model.TotalHashRate = data.HashRate.TotalHashRate.ElementAt(0) ?? 0;

            foreach (var hashRate in data.HashRate.Threads)
            {
                _model.PerGpuHashRate.Add(hashRate.ElementAt(0) ?? 0);
                _model.PerGpuTemperatures.Add(0);
                _model.PerGpuShares.Add(0);
                _model.PerGpuFanSpeed.Add(0);
            }
        }
    }
}
